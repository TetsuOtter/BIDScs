using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BIDScs
{
  /// <summary>
  /// Pipeをクライアントとして利用するクラス
  /// </summary>
  public class Pipe : IDisposable
  {
    /// <summary>
    /// 指定のサーバー
    /// </summary>
    /// <param name="SVPCAddr">サーバーマシンのアドレス</param>
    public Pipe(string SVPCAddr)
    {
      SVPCName = SVPCAddr;
    }

    //1.新しいスレッドを建てる
    //2.パイプ通信を開始する
    //3.値に変化があればイベント実行
    //4.ループ
    //5.パイプを閉じる
    private string PipeName { get; } = "BIDSPipe";
    private string SVPCName { get; } = ".";
    private bool IsLocal { get; } = false;
    private static string SRAMName = "BIDSSharedMem";
    //SECTION_ALL_ACCESS=983071
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr CreateFileMapping(UIntPtr hFile, IntPtr lpAttributes, uint flProtect, uint dwMaximumSizeHigh, uint dwMaximumSizeLow, string lpName);
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, uint dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, uint dwNumberOfBytesToMap);
    //[DllImport("kernel32.dll")]
    //static extern void CopyMemory(IntPtr Destination, IntPtr Source, uint Length);//予約
    [DllImport("kernel32.dll")]
    static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);
    [DllImport("kernel32.dll")]
    static extern bool CloseHandle(IntPtr hObject);
    /// <summary>
    /// スペック情報
    /// </summary>
    private struct Spec
    {
      /// <summary>
      /// Bノッチ段数
      /// </summary>
      public int B;
      /// <summary>
      /// Pノッチ段数
      /// </summary>
      public int P;
      /// <summary>
      /// ATS確認段数
      /// </summary>
      public int A;
      /// <summary>
      /// B67に相当する段数
      /// </summary>
      public int J;
      /// <summary>
      /// 編成車両数
      /// </summary>
      public int C;
    };
    /// <summary>
    /// 車両状態情報
    /// </summary>
    private struct State
    {
      /// <summary>
      /// 列車位置情報[m]
      /// </summary>
      public double Z;
      /// <summary>
      /// 列車速度[km/h]
      /// </summary>
      public float V;
      /// <summary>
      /// 0時からの経過時間[ms]
      /// </summary>
      public int T;
      /// <summary>
      /// BC圧力[kPa]
      /// </summary>
      public float BC;
      /// <summary>
      /// MR圧力[kPa]
      /// </summary>
      public float MR;
      /// <summary>
      /// ER圧力[kPa]
      /// </summary>
      public float ER;
      /// <summary>
      /// BP圧力[kPa]
      /// </summary>
      public float BP;
      /// <summary>
      /// SAP圧力[kPa]
      /// </summary>
      public float SAP;
      /// <summary>
      /// 電流[A]
      /// </summary>
      public float I;
    };
    /// <summary>
    /// ハンドル位置情報
    /// </summary>
    private struct Hand
    {
      /// <summary>
      /// Bノッチ位置
      /// </summary>
      public int B;
      /// <summary>
      /// Pノッチ位置
      /// </summary>
      public int P;
      /// <summary>
      /// レバーサーハンドル位置
      /// </summary>
      public int R;
      /// <summary>
      /// 定速制御情報
      /// </summary>
      public int C;
    };
    /// <summary>
    /// 地上子情報
    /// </summary>
    private struct Beacon
    {
      /// <summary>
      /// Beaconの番号
      /// </summary>
      public int Num;
      /// <summary>
      /// 対応する閉塞の現示番号
      /// </summary>
      public int Sig;
      /// <summary>
      /// 対応する閉塞までの距離[m]
      /// </summary>
      public float Z;
      /// <summary>
      /// Beaconの第三引数
      /// </summary>
      public int Data;
    };
    //Version 200ではBeaconData,IsKeyPushed,SignalSetIntはDIsabled
    /// <summary>
    /// 共有メモリに格納するための構造体
    /// </summary>
    private struct BIDSSharedMemoryData
    {
      /// <summary>
      /// BIDSpp.dllが有効かどうか
      /// </summary>
      public bool IsEnabled;
      /// <summary>
      /// BIDSpp.dllのバージョン
      /// </summary>
      public int VersionNum;
      /// <summary>
      /// スペック情報
      /// </summary>
      public Spec SpecData;
      /// <summary>
      /// 車両状態情報
      /// </summary>
      public State StateData;
      /// <summary>
      /// ハンドル位置情報
      /// </summary>
      public Hand HandleData;
      /// <summary>
      /// ドア閉扉情報
      /// </summary>
      public bool IsDoorClosed;
      /// <summary>
      /// パネル表示情報
      /// </summary>
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
      public int[] Panel;
      /// <summary>
      /// サウンド状態情報
      /// </summary>
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
      public int[] Sound;


      //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
      //public Beacon[] BeaconData;
      //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
      //public bool[] IsKeysPushed;
      //public int SignalSetInt;
    };

    static readonly uint size = (uint)Marshal.SizeOf(typeof(BIDSSharedMemoryData));
    static IntPtr hSharedMemory;
    static IntPtr pMemory;
    private NamedPipeClientStream NPCS;

    bool IsDisposing = false;

    /// <summary>
    /// 通信を開始する。
    /// </summary>
    public void Open()
    {
      if (!IsLocal)
      {
        NPCS = new NamedPipeClientStream(SVPCName, PipeName, PipeDirection.InOut);
        PipeStart();
      }
      else
      {
        SMemStart();
      }
    }

    /// <summary>
    /// 通信を終了する
    /// </summary>
    public void Dispose()
    {
      if (!IsLocal)
      {
        try
        {
          NPCS.Close();
        }
        catch (Exception e)
        {
          DialogResult DR = MessageBox.Show(e.Message, "BIDScs.dll", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
        }
        NPCS.Dispose();
      }
      else
      {
        UnmapViewOfFile(pMemory);
        CloseHandle(hSharedMemory);
      }
    }


    //https://docs.microsoft.com/ja-jp/dotnet/standard/io/how-to-use-named-pipes-for-network-interprocess-communication#robust-programming

    private void PipeStart()
    {
      if (NPCS.IsConnected == false)
      {
        while (true)
        {
          try
          {
            NPCS.Connect(500);
            //await NPCS.ConnectAsync(5000);
          }
          catch (Exception e)
          {
            DialogResult DR = MessageBox.Show(e.Message, "BIDScs.dll", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
            if (DR == DialogResult.Cancel){ break; }
          }
          if (NPCS.IsConnected == true) { break; }
        }
      }
      else
      {
        MessageBox.Show("既に接続されています。", "BIDScs.dll", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void SMemStart()
    {
      hSharedMemory = CreateFileMapping(UIntPtr.Zero, IntPtr.Zero, 4, 0, size, SRAMName);
      pMemory = MapViewOfFile(hSharedMemory, 983071, 0, 0, size);
    }

    /// <summary>
    /// 車両のスペック情報
    /// </summary>
    public struct SpecData
    {
      /// <summary>
      /// Bノッチ段数
      /// </summary>
      public int Brake;
      /// <summary>
      /// Pノッチ段数
      /// </summary>
      public int Power;
      /// <summary>
      /// ATS確認段数
      /// </summary>
      public int ATSCheck;
      /// <summary>
      /// B67に相当する段数
      /// </summary>
      public int B67;
      /// <summary>
      /// 編成車両数
      /// </summary>
      public int CarNum;
    }
    /// <summary>
    /// 車両状態(ハンドル位置を含む)
    /// </summary>
    public struct StateData
    {
      /// <summary>
      /// 列車位置[m]
      /// </summary>
      double Location;
      /// <summary>
      /// 列車速度[km/h]
      /// </summary>
      float Speed;
      /// <summary>
      /// 電流[A]
      /// </summary>
      float Current;
      /// <summary>
      /// (準備中)架線電圧[V]
      /// </summary>
      float Voltage;
      /// <summary>
      /// Bノッチ位置
      /// </summary>
      int BrakeNotch;
      /// <summary>
      /// Pノッチ位置
      /// </summary>
      int PowerNotch;
      /// <summary>
      /// レバーサー位置
      /// </summary>
      int Reverser;
      /// <summary>
      /// (準備中)定速情報
      /// </summary>
      int ConstSpeed;
    }
    /// <summary>
    /// 車両状態(ドア情報含む)
    /// </summary>
    public struct State2Data
    {
      /// <summary>
      /// BC圧[kPa]
      /// </summary>
      float BC;
      /// <summary>
      /// MR圧[kPa]
      /// </summary>
      float MR;
      /// <summary>
      /// ER圧[kPa]
      /// </summary>
      float ER;
      /// <summary>
      /// BP圧[kPa]
      /// </summary>
      float BP;
      /// <summary>
      /// SAP圧[kPa]
      /// </summary>
      float SAP;
      /// <summary>
      /// ドア閉扉情報
      /// </summary>
      bool IsDoorClosed;
      /// <summary>
      /// (準備中)信号番号
      /// </summary>
      int SignalNum;
      /// <summary>
      /// 時刻[時]
      /// </summary>
      int Hour;
      /// <summary>
      /// 時刻[分]
      /// </summary>
      int Minute;
      /// <summary>
      /// 時刻[秒]
      /// </summary>
      int Second;
      /// <summary>
      /// 時刻[ミリ秒]
      /// </summary>
      int Millisecond;
    }


    /// <summary>
    /// 現在の車両スペック情報
    /// </summary>
    public SpecData CarSpec;
    /// <summary>
    /// 現在の車両状態(ハンドル位置含む)
    /// </summary>
    public StateData NowState;
    /// <summary>
    /// 車両状態(ドア情報含む)
    /// </summary>
    public State2Data NowState2;
    /// <summary>
    /// 現在のパネル表示情報
    /// </summary>
    public int[] NowPanel = new int[256];
    /// <summary>
    /// 現在のサウンド情報
    /// </summary>
    public int[] NowSound = new int[256];


    /// <summary>
    /// スペック情報を取得する。
    /// </summary>
    /// <returns>車両スペック情報</returns>
    public SpecData GetSpec()
    {
      SpecData rd = new SpecData();
      return rd;
    }
    /// <summary>
    /// ハンドル位置情報を含む車両状態を取得する
    /// </summary>
    /// <returns>取得した車両状態</returns>
    public StateData GetState()
    {
      StateData rd = new StateData();
      return rd;
    }
    /// <summary>
    /// ドア情報を含む車両状態を取得する
    /// </summary>
    /// <returns>取得した車両状態</returns>
    public State2Data GetState2()
    {
      State2Data rd = new State2Data();
      return rd;
    }
    /// <summary>
    /// 指定のPanelの表示情報を取得する
    /// </summary>
    /// <param name="Indexes">取得するPanelのインデックス</param>
    /// <returns>取得したPanel情報</returns>
    public List<int> GetPanel(List<int> Indexes)
    {
      List<int> GetList = new List<int>();
      return GetList;
    }
    /// <summary>
    /// 指定のSoundの表示情報を取得する。
    /// </summary>
    /// <param name="Indexes">取得するSoundのインデックス</param>
    /// <returns>取得したSound情報</returns>
    public List<int> GetSound(List<int> Indexes)
    {
      List<int> GetList = new List<int>();
      return GetList;
    }
  }
}

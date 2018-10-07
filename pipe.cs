using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace TR.BIDScs
{
  /// <summary>
  /// BVEと、クライアントとして通信するクラス
  /// </summary>
  public class Pipe : IDisposable
  {
    /// <summary>
    /// 自マシンとの通信を準備する
    /// </summary>
    public Pipe()
    {
      SVPCName = ".";
      IsLocal = true;
    }

    /// <summary>
    /// 指定のサーバーとの通信を準備する
    /// </summary>
    /// <param name="SVPCAddr">サーバーマシンのアドレス</param>
    public Pipe(string SVPCAddr)
    {
      //自マシンへの指定は共有メモリを読む設定になる。

      SVPCName = SVPCAddr;
      if (SVPCAddr == "." || SVPCAddr == "localhost")
      {
        IsLocal = true;
        return;
      }
      string[] AddrSplit = SVPCAddr.Split('.');
      if (AddrSplit[0] == "127")
      {
        IsLocal = true;
        return;
      }
      AddrSplit = SVPCAddr.Split(':');
      if (AddrSplit.Last() == "1")
      {
        IsLocal = true;
        return;
      }
      IPHostEntry iphe = Dns.GetHostEntry(Dns.GetHostName());
      foreach (IPAddress ip in iphe.AddressList)
      {
        if (ip.ToString() == SVPCAddr)
        {
          IsLocal = true;
          return;
        }
      }
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
    static private NamedPipeClientStream NPCS;

    static bool IsDisposing = false;
    bool IsOpen = false;
    /// <summary>
    /// 通信を開始する。
    /// </summary>
    public void Open()
    {
      if (!IsLocal)
      {
        NPCS = new NamedPipeClientStream(SVPCName, PipeName, PipeDirection.InOut);
        try
        {
          PipeStart();
        }
        catch (Exception)
        {
          throw;
        }
      }
      else
      {
        SMemStart();
      }
      IsOpen = true;
    }

    /// <summary>
    /// 通信を終了する
    /// </summary>
    public void Dispose()
    {
      IsDisposing = true;
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
      IsOpen = false;
    }


    Thread thr = new Thread(PipeReadWork);
    private void PipeStart()
    {
      if (!NPCS.IsConnected)
      {
        while (true)
        {
          try
          {
            NPCS.Connect(500);
            //await NPCS.ConnectAsync(5000);
          }
          catch (Exception)
          {
            throw;
          }
          if (NPCS.IsConnected == true)  break;
        }
      }
      else
      {
        throw new InvalidOperationException("既にパイプは開かれています。");
      }

    }

    static private void PipeReadWork()
    {
      while (NPCS.IsConnected && !IsDisposing)
      {
        byte[] ReadByte = new byte[32];
        try
        {
          NPCS.Read(ReadByte, 0, 32);
        }
        catch (ObjectDisposedException) { }
        catch (InvalidOperationException) { }
        catch (Exception)
        {
          throw;
        }
        if(ReadByte.Skip(30).Take(2).ToArray()==new byte[2] { 0xFE, 0xFE })
        {
          short hed = 0;
          try
          {
            hed = Convert.ToInt16(ReadByte.Take(2).ToArray());
          }
          catch(Exception)
          {
            throw;
          }
          //値を代入するまとまりごとにカウントアップをする=>NowXXXでそれを読んで更新されたかどうかを判別する
          switch (hed)
          {
            case 14://Spec
              break;
            case 15://State1
              break;
            case 16://State2
              break;
            case 20://Sound
              break;
            case 21://Panel
              break;
          }
        }
      }
    }

    private void SMemStart()
    {
      hSharedMemory = CreateFileMapping(UIntPtr.Zero, IntPtr.Zero, 4, 0, size, SRAMName);
      pMemory = MapViewOfFile(hSharedMemory, 983071, 0, 0, size);
    }

    private byte[] PipeGetByte(byte data)
    {
      byte[] ReturnArray = new byte[32];
      return ReturnArray;
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
      public double Location;
      /// <summary>
      /// 列車速度[km/h]
      /// </summary>
      public float Speed;
      /// <summary>
      /// 電流[A]
      /// </summary>
      public float Current;
      /// <summary>
      /// (準備中)架線電圧[V]
      /// </summary>
      public float Voltage;
      /// <summary>
      /// Bノッチ位置
      /// </summary>
      public int BrakeNotch;
      /// <summary>
      /// Pノッチ位置
      /// </summary>
      public int PowerNotch;
      /// <summary>
      /// レバーサー位置
      /// </summary>
      public int Reverser;
      /// <summary>
      /// (準備中)定速情報
      /// </summary>
      public int ConstSpeed;
    }
    /// <summary>
    /// 車両状態(ドア情報含む)
    /// </summary>
    public struct State2Data
    {
      /// <summary>
      /// BC圧[kPa]
      /// </summary>
      public float BC;
      /// <summary>
      /// MR圧[kPa]
      /// </summary>
      public float MR;
      /// <summary>
      /// ER圧[kPa]
      /// </summary>
      public float ER;
      /// <summary>
      /// BP圧[kPa]
      /// </summary>
      public float BP;
      /// <summary>
      /// SAP圧[kPa]
      /// </summary>
      public float SAP;
      /// <summary>
      /// ドア閉扉情報
      /// </summary>
      public bool IsDoorClosed;
      /// <summary>
      /// (準備中)信号番号
      /// </summary>
      public int SignalNum;
      /// <summary>
      /// 時刻[時]
      /// </summary>
      public int Hour;
      /// <summary>
      /// 時刻[分]
      /// </summary>
      public int Minute;
      /// <summary>
      /// 時刻[秒]
      /// </summary>
      public int Second;
      /// <summary>
      /// 時刻[ミリ秒]
      /// </summary>
      public int Millisecond;
    }

    private SpecData _CarSpec;
    private StateData _NowState;
    private State2Data _NowState2;
    private int[] _NowPanel = new int[256];
    private int[] _NowSound = new int[256];
    /// <summary>
    /// 現在の車両スペック情報
    /// </summary>
    public SpecData CarSpec
    {
      private set
      {
        _CarSpec = value;
      }
      get
      {
        if (!IsLocal) return _CarSpec;
        else return SMemGetSpec();
      }
    }
    /// <summary>
    /// 現在の車両状態(ハンドル位置含む)
    /// </summary>
    public StateData NowState
    {
      get
      {
        if (!IsLocal) return _NowState;
        else return SMemGetState();
      }
      private set
      {
        _NowState = value;
      }
    }
    /// <summary>
    /// 車両状態(ドア情報含む)
    /// </summary>
    public State2Data NowState2
    {
      get
      {
        if (!IsLocal) return _NowState2;
        else return SMemGetState2();
      }
      private set
      {
        _NowState2 = value;
      }
    }
    /// <summary>
    /// 現在のパネル表示情報
    /// </summary>
    public int[] NowPanel
    {
      get
      {
        if(!IsLocal) return _NowPanel;
        else return ((BIDSSharedMemoryData)Marshal.PtrToStructure(pMemory, typeof(BIDSSharedMemoryData))).Panel;
      }
      private set
      {
        _NowPanel = value;
      }
    }
    /// <summary>
    /// 現在のサウンド情報
    /// </summary>
    public int[] NowSound
    {
      get
      {
        if (!IsLocal) return _NowSound;
        else return ((BIDSSharedMemoryData)Marshal.PtrToStructure(pMemory, typeof(BIDSSharedMemoryData))).Sound;
      }
      private set
      {
        _NowSound = value;
      }
    }


    /// <summary>
    /// スペック情報を取得する。
    /// </summary>
    /// <returns>車両スペック情報</returns>
    /// <exception cref="InvalidOperationException">通信未開始</exception>
    public SpecData GetSpec()
    {
      if (!IsOpen)
      {
        throw new InvalidOperationException("通信が開始していません。");
      }
      if (IsLocal) return SMemGetSpec();
      else return PipeGetSpec();
    }
    /// <summary>
    /// ハンドル位置情報を含む車両状態を取得する
    /// </summary>
    /// <returns>取得した車両状態</returns>
    /// <exception cref="InvalidOperationException">通信未開始</exception>
    public StateData GetState()
    {
      if (!IsOpen)
      {
        throw new InvalidOperationException("通信が開始していません。");
      }
      if (IsLocal) return SMemGetState();
      else return PipeGetState();
    }
    /// <summary>
    /// ドア情報を含む車両状態を取得する
    /// </summary>
    /// <returns>取得した車両状態</returns>
    /// <exception cref="InvalidOperationException">通信未開始</exception>
    public State2Data GetState2()
    {
      if (!IsOpen)
      {
        throw new InvalidOperationException("通信が開始していません。");
      }
      if (IsLocal) return SMemGetState2();
      else return PipeGetState2();
    }
    /// <summary>
    /// 指定のPanelの表示情報を取得する
    /// </summary>
    /// <param name="Indexes">取得するPanelのインデックス</param>
    /// <returns>取得したPanel情報</returns>
    /// <exception cref="InvalidOperationException">通信未開始</exception>
    public List<int> GetPanel(List<int> Indexes)
    {
      if (!IsOpen)
      {
        throw new InvalidOperationException("通信が開始していません。");
      }
      if (IsLocal) return SMemGetPanel(Indexes);
      else return PipeGetPanel(Indexes);
    }
    /// <summary>
    /// 指定のSoundの表示情報を取得する。
    /// </summary>
    /// <param name="Indexes">取得するSoundのインデックス</param>
    /// <returns>取得したSound情報</returns>
    /// <exception cref="InvalidOperationException">通信未開始</exception>
    public List<int> GetSound(List<int> Indexes)
    {
      if (!IsOpen)
      {
        throw new InvalidOperationException("通信が開始していません。");
      }
      if (IsLocal) return SMemGetSound(Indexes);
      else return PipeGetSound(Indexes);
    }


    private SpecData PipeGetSpec()
    {
      SpecData rd = new SpecData();
      return rd;
    }
    private StateData PipeGetState()
    {
      StateData rd = new StateData();
      return rd;
    }
    private State2Data PipeGetState2()
    {
      State2Data rd = new State2Data();
      return rd;
    }
    private List<int> PipeGetPanel(List<int> Indexes)
    {
      List<int> GetList = new List<int>();
      return GetList;
    }
    private List<int> PipeGetSound(List<int> Indexes)
    {
      List<int> GetList = new List<int>();
      return GetList;
    }



    private SpecData SMemGetSpec()
    {
      SpecData s = new SpecData();
      Spec sp = ((BIDSSharedMemoryData)Marshal.PtrToStructure(pMemory, typeof(BIDSSharedMemoryData))).SpecData;
      s.ATSCheck = sp.A;
      s.B67 = sp.J;
      s.Brake = sp.B;
      s.CarNum = sp.C;
      s.Power=sp.P;
      return s;

    }
    private StateData SMemGetState()
    {
      State s = ((BIDSSharedMemoryData)Marshal.PtrToStructure(pMemory, typeof(BIDSSharedMemoryData))).StateData;
      Hand h = ((BIDSSharedMemoryData)Marshal.PtrToStructure(pMemory, typeof(BIDSSharedMemoryData))).HandleData;
      return new StateData()
      {
        BrakeNotch = h.B,
        ConstSpeed = h.C,
        Current = s.I,
        Location = s.Z,
        PowerNotch = h.P,
        Reverser = h.R,
        Speed = s.V,
        Voltage = 0
      };
    }
    private State2Data SMemGetState2()
    {
      State s = ((BIDSSharedMemoryData)Marshal.PtrToStructure(pMemory, typeof(BIDSSharedMemoryData))).StateData;
      TimeSpan t = new TimeSpan();
      t = TimeSpan.FromMilliseconds(s.T);
      return new State2Data()
      {
        BC = s.BC,
        BP = s.BP,
        ER = s.ER,
        Hour = t.Hours,
        IsDoorClosed = ((BIDSSharedMemoryData)Marshal.PtrToStructure(pMemory, typeof(BIDSSharedMemoryData))).IsDoorClosed,
        Millisecond = t.Milliseconds,
        Minute = t.Minutes,
        MR = s.MR,
        SAP = s.SAP,
        Second = t.Seconds,
        SignalNum = 0
      };
    }
    private List<int> SMemGetPanel(List<int> Indexes)
    {
      int[] p = ((BIDSSharedMemoryData)Marshal.PtrToStructure(pMemory, typeof(BIDSSharedMemoryData))).Panel;
      List<int> GetList = new List<int>();
      for(int i = 0; i < Indexes.Count; i++)
      {
        if (Indexes[i] >= 0 && Indexes[i] < 256) GetList.Add(p[Indexes[i]]);
        else GetList.Add(-1);
      }
      return GetList;
    }
    private List<int> SMemGetSound(List<int> Indexes)
    {
      int[] p = ((BIDSSharedMemoryData)Marshal.PtrToStructure(pMemory, typeof(BIDSSharedMemoryData))).Sound;
      List<int> GetList = new List<int>();
      for (int i = 0; i < Indexes.Count; i++)
      {
        if (Indexes[i] >= 0 && Indexes[i] < 256) GetList.Add(p[Indexes[i]]);
        else GetList.Add(-1);
      }
      return GetList;
    }
  }
}

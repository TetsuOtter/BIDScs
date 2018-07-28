using System;
using System.IO.Pipes;
using System.Windows.Forms;

namespace BIDScs
{
  /// <summary>
  /// Pipeをサーバーとして使用するクラス
  /// </summary>
  static public class PipeSV
  {
    static private string PipeName = "BIDSPipe";
    static private int MaxCl = 8;
    static private int PipeSize = 256;// PipeData.Length * sizeof(Byte);
    static private Byte[] PipeData = new Byte[32];
    static private Byte[] PipeRData = new Byte[32];
    /// <summary>
    /// "BIDSPipe"というパイプを開く
    /// </summary>
    static public async void Start()
    {
      PipeName = "BIDSPipe";

      try
      {
        NPSS = new NamedPipeServerStream(PipeName, PipeDirection.InOut, MaxCl,
        PipeTransmissionMode.Byte, PipeOptions.Asynchronous, PipeSize, PipeSize);
      }catch(Exception e)
      {
        MessageBox.Show(e.Message);
      }
      try
      {
        await NPSS.WaitForConnectionAsync();
      }catch(Exception e)
      {
        MessageBox.Show(e.Message);
      }
    }
    /// <summary>
    /// パイプを開く
    /// </summary>
    /// <param name="pn">開くパイプの名称</param>
    static public async void Start(string pn)
    {
      if (pn == null || pn == "")
      {
        PipeName = "BIDSPipe";
      }
      else
      {
        PipeName = pn;
      }
      try
      {
        NPSS = new NamedPipeServerStream(PipeName, PipeDirection.InOut, MaxCl,
        PipeTransmissionMode.Byte, PipeOptions.Asynchronous, PipeSize, PipeSize);
      }
      catch (Exception e)
      {
        MessageBox.Show(e.Message);
      }
      try
      {
        await NPSS.WaitForConnectionAsync();
      }
      catch (Exception e)
      {
        MessageBox.Show(e.Message);
      }
    }
    /// <summary>
    /// パイプを閉じる
    /// </summary>
    static public void Stop()
    {
      NPSS.Close();
    }

    static private NamedPipeServerStream NPSS;

    /// <summary>
    /// clientに情報を送信する
    /// </summary>
    /// <param name="b">送信する情報</param>
    static public void PipeSend(Byte[] b)
    {
      if (NPSS.CanWrite == true && NPSS.IsConnected == true)
      {
        try
        {
          NPSS.BeginWrite(b, 0, PipeSize, PipeWriteAsyncCallBack, null);
        }catch(Exception e)
        {
          MessageBox.Show(e.Message);
        }
      }
    }

    private static void PipeWriteAsyncCallBack(IAsyncResult ar)
    {
      NPSS.EndWrite(ar);
    }

    /// <summary>
    /// clientからの情報を読み取る
    /// </summary>
    /// <returns>clientが送信した情報</returns>
    static public Byte[] PipeGet()
    {
      if (NPSS.CanRead == true && NPSS.IsConnected == true)
      {
        NPSS.BeginRead(PipeRData, 0, PipeSize, PipeReadAsyncCallBack, null);
      }
      else
      {
        return null;
      }
      return PipeRData;
    }

    private static void PipeReadAsyncCallBack(IAsyncResult ar)
    {
      NPSS.EndRead(ar);
    }

    /// <summary>
    /// Spec情報を送信する
    /// </summary>
    /// <param name="B">ブレーキハンドル段数</param>
    /// <param name="P">ノッチハンドル段数</param>
    /// <param name="A">ATS確認のできる段数</param>
    /// <param name="J">常用最大段数</param>
    /// <param name="C">編成両数</param>
    static public void SpecSend(short B, short P, short A, short J, byte C)
    {
      int sn = 0;
      int tn = 2;
      PipeData = BitConverter.GetBytes((short)14);
      Array.Copy(BitConverter.GetBytes(B), 0, PipeData, sn += tn, tn = 2);
      Array.Copy(BitConverter.GetBytes(P), 0, PipeData, sn += tn, tn = 2);
      Array.Copy(BitConverter.GetBytes(A), 0, PipeData, sn += tn, tn = 2);
      Array.Copy(BitConverter.GetBytes(J), 0, PipeData, sn += tn, tn = 2);
      PipeData[10] = C;
      PipeSend(PipeData);
    }

    /// <summary>
    /// State情報1を送信する。
    /// </summary>
    /// <param name="L">列車位置[m]</param>
    /// <param name="S">列車速度[km/h]</param>
    /// <param name="C">電流[A]</param>
    /// <param name="V">電圧[V]</param>
    /// <param name="B">ブレーキハンドル位置</param>
    /// <param name="P">力行ハンドル位置</param>
    /// <param name="R">レバーサーハンドル位置</param>
    /// <param name="Co">定速スイッチ押下情報</param>
    /// <param name="H">時刻の「時」情報</param>
    /// <param name="M">時刻の「分」情報</param>
    /// <param name="Se">時刻の「秒」情報</param>
    /// <param name="MS">時刻の「ミリ秒」情報</param>
    static public void StateSend(double L, float S, float C, float V, short B, short P, byte R, byte Co, byte H, byte M, byte Se, byte MS)
    {
      int sn = 0;
      int tn = 2;
      PipeData = BitConverter.GetBytes((short)15);
      Array.Copy(BitConverter.GetBytes(L), 0, PipeData, sn += tn, tn = 8);
      Array.Copy(BitConverter.GetBytes(S), 0, PipeData, sn += tn, tn = 4);
      Array.Copy(BitConverter.GetBytes(C), 0, PipeData, sn += tn, tn = 4);
      Array.Copy(BitConverter.GetBytes(V), 0, PipeData, sn += tn, tn = 4);
      Array.Copy(BitConverter.GetBytes(B), 0, PipeData, sn += tn, tn = 2);
      Array.Copy(BitConverter.GetBytes(P), 0, PipeData, sn += tn, tn = 2);
      PipeData[26] = R;
      PipeData[27] = Co;
      PipeData[28] = H;
      PipeData[29] = M;
      PipeData[30] = Se;
      PipeData[31] = MS;
      PipeSend(PipeData);
    }

    /// <summary>
    /// State情報2を送信する。
    /// </summary>
    /// <param name="C">BC圧[kPa]</param>
    /// <param name="M">MR圧[kPa]</param>
    /// <param name="E">ER圧[kPa]</param>
    /// <param name="P">BP圧[kPa]</param>
    /// <param name="A">SAP圧[kPa]</param>
    /// <param name="D">ドア状態(trueで閉扉)</param>
    /// <param name="S">現在の閉塞の信号現示番号</param>
    static public void State2Send(float C, float M, float E, float P, float A, byte D, short S)
    {
      int sn = 0;
      int tn = 2;
      PipeData = BitConverter.GetBytes((short)16);
      Array.Copy(BitConverter.GetBytes(C), 0, PipeData, sn += tn, tn = 4);
      Array.Copy(BitConverter.GetBytes(M), 0, PipeData, sn += tn, tn = 4);
      Array.Copy(BitConverter.GetBytes(E), 0, PipeData, sn += tn, tn = 4);
      Array.Copy(BitConverter.GetBytes(P), 0, PipeData, sn += tn, tn = 4);
      Array.Copy(BitConverter.GetBytes(A), 0, PipeData, sn += tn, tn = 4);
      sn += tn;
      tn = 1;
      PipeData[22] = D;
      Array.Copy(BitConverter.GetBytes(S), 0, PipeData, sn += tn, tn = 2);
      PipeSend(PipeData);
    }

    /// <summary>
    /// 変化したサウンド番号について送信する
    /// </summary>
    /// <param name="I">変化したSoundインデックス(最大で15個) 不要部は「-1」</param>
    /// <param name="N">変化したSoundインデックスの変化後の値(最大で15個)</param>
    static public void SoundSend(byte[] I, byte[] N)
    {
      PipeData = BitConverter.GetBytes((short)20);
      for (int i = 0; i < 15; i++)
      {
        int p = i + 1;
        PipeData[p * 2] = I[i];
        PipeData[(p * 2) + 1] = N[i];
      }
      PipeSend(PipeData);
    }

    /// <summary>
    /// 変化したPanel番号について送信する
    /// </summary>
    /// <param name="I">変化したPanelのインデックス番号(最大10個) 不要部は「-1」</param>
    /// <param name="N">変化した後のPanelの値(最大10個)</param>
    static public void PanelSend(byte[] I,short[] N)
    {
      PipeData = BitConverter.GetBytes((short)21);
      for(int i = 0; i < 10; i++)
      {
        PipeData[2 + (i * 3)] = I[i];
        Array.Copy(BitConverter.GetBytes(N[i]), 0, PipeData, 3 + (i * 3), 2);
      }
      PipeSend(PipeData);
    }
    /// <summary>
    /// 各ボタン(キー)の情報を送信する
    /// </summary>
    /// <param name="k">押下情報</param>
    static public void KeyInfoSend(byte[] k)
    {
      PipeData = BitConverter.GetBytes((short)21);
      for(int i = 0; i < 20; i++)
      {
        PipeData[i + 2] = k[i];
      }
      PipeSend(PipeData);
    }
  }
}

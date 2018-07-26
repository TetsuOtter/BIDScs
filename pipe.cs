using System;
using System.Net;
using System.Windows;
using System.Windows.Threading;
using System.IO.Pipes;
using System.Windows.Forms;
using System.Collections.Generic;

namespace BIDScs
{
  static public class Pipe
  {
    //1.新しいスレッドを建てる
    //2.パイプ通信を開始する
    //3.値に変化があればイベント実行
    //4.ループ
    //5.パイプを閉じる
    static private string PipeName = "BIDSPipe";
    static private string SVPCName = ".";

    static private bool[] keys = new bool[16];
    static private int[] horn = new int[3];
    static private int[] hornblow = new int[3];
    static private int[,] beacon = new int[10, 6];

    /// <summary>
    /// サーバーとの通信を開始する。
    /// </summary>
    static public void Start()
    {
      PipeStart();
    }
    /// <summary>
    /// サーバーとの通信を開始する。
    /// </summary>
    /// <param name="PCName">サーバーのコンピュータ名</param>
    static public void Start(string PCName)
    {
      if (PCName != null || PCName == "")
      {
        SVPCName = PCName;
      }
      else
      {
        SVPCName = ".";
      }
      PipeStart();
    }
    /// <summary>
    /// サーバーとの通信を開始する。
    /// </summary>
    /// <param name="PCName">サーバーのコンピュータ名</param>
    /// <param name="PpName">使用するパイプの名前</param>
    static public void Start(string PCName, string PpName)
    {
      if (PpName != null || PpName == "")
      {
        PipeName = PpName;
      }
      else
      {
        PipeName = "BIDSPipe";
      }
      if (PCName != null || PCName == "")
      {
        SVPCName = PCName;
      }
      else
      {
        SVPCName = ".";
      }
      PipeStart();
    }

    /// <summary>
    /// サーバーとの通信を終了する。
    /// </summary>
    static public void Stop()
    {
      PipeEnd();
    }

    //https://docs.microsoft.com/ja-jp/dotnet/standard/io/how-to-use-named-pipes-for-network-interprocess-communication#robust-programming
    
    static private NamedPipeClientStream NPCS =
      new NamedPipeClientStream(SVPCName, PipeName,
                        PipeDirection.InOut, PipeOptions.None);
    static private void PipeStart()
    {
      if (NPCS.IsConnected == false)
      {
        while (true)
        {
          try
          {
            NPCS.ConnectAsync(5000);
          }
          catch (Exception e)
          {
            DialogResult DR = MessageBox.Show(e.Message, "BIDSシステム", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
            if (DR == DialogResult.Cancel)
            {
              break;
            }
          }
        }
      }
      else
      {
        MessageBox.Show("既に接続されています。", "BIDSシステム", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }
    static private Byte[] rcvBytes = new Byte[2128];

    static private void PipeGet()
    {
      try
      {
        NPCS.BeginRead(rcvBytes, 0, 2128, ByteTo, null);
      }catch(Exception e)
      {
        MessageBox.Show(e.Message, "BIDSシステム", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }
    //static private string udpRcvText;
    static private void ByteTo(IAsyncResult ar)
    {
      int[] DataInt = new int[532];

      NPCS.EndRead(ar);
      //Array.Reverse(rcvBytes);
      string[] data = { "" };
      //udpRcvText = BitConverter.ToString(rcvBytes);
      string rcvText = BitConverter.ToString(rcvBytes);
      foreach (var n in SubstringAtCount(rcvText))
      {
        rcvText = rcvText.Replace("-", "");
        data = SubstringAtCount(rcvText);
      }
      //Array.Reverse(data);
      for (int i = 0; i < 532; i++)
      {
        DataInt[i] = Convert.ToInt32(data[i], 16);
      }
      DataInt = Data.DataArray;
      if (DataInt[0] < 8)
      {
        OldHedDataSet(DataInt);
      }
      HedSW(DataInt);
    }

    static private void PipeSend(Byte[] sd)
    {

    }
    static private void PipeEnd()
    {
      while (true) {
        try
        {
          NPCS.Close();
        }
        catch(Exception e)
        {
          DialogResult DR = MessageBox.Show(e.Message, "BIDSシステム", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
          if (DR == DialogResult.Cancel)
          {
            break;
          }
        }
      }
    }

    static private void HedSW(int[] DataInt)
    {
      switch (DataInt[0])
      {
        case 0:
          break;
        case 1:
          //Dispose
          break;
        case 2:
          //Elapse
          int[] Panel = new int[256];
          int[] Sound = new int[256];
          for (int i = 0; i < 256; i++)
          {
            Panel[i] = DataInt[i + 19];
            Sound[i] = DataInt[i + 19 + 256];
          }
          for (int i = 0; i < 3; i++)
          {
            if (horn[i] != 0 && hornblow[i] < 5)
            {
              hornblow[i]++;
            }
            else
            {
              horn[i] = 0;
              hornblow[i] = 0;
            }
          }
          horn = Data.Horn;
          Panel = Data.Panel;
          Sound = Data.Sound;
          if (DataInt[531] != 1)
          {
            Data.State.Door = false;
          }
          else
          {
            Data.State.Door = true;
          }
          break;
        case 3:
          //KeyDown
          keys[DataInt[19]] = true;
          break;
        case 4:
          //KeyUp
          keys[DataInt[19]] = false;
          break;
        case 5:
          //HornBlow
          horn[DataInt[19]]++;
          break;
        case 6:
          //SetSignal
          Data.SetSig = DataInt[19];
          break;
        case 7:
          //SetBeaconData
          for (int i = 9; i >= 0; i--)
          {
            for (int b = 0; b < 6; b++)
            {
              if (i > 0)
              {
                beacon[i - 1, b] = beacon[i, b];
              }
              else
              {
                beacon[0, b] = DataInt[b + 19];
              }
            }
          }
          beacon[0, 5] = DataInt[12];//Time
          beacon[0, 6] = DataInt[10];//Distance
          beacon = Data.Beacon;
          break;
      }
    }

    static private void OldHedDataSet(int[] d)
    {
      Data.Spec.Brake = d[1];
      Data.Spec.B67 = d[2];
      Data.Spec.Power = d[3];
      Data.Spec.ATS = d[4];
      Data.Spec.Car = d[5];
      Data.Handle.Brake = d[6];
      Data.Handle.Power = d[7];
      Data.Handle.Lever = d[8];
      Data.Handle.Const = d[9];
      Data.State.Location = (double)d[10] / 1000;
      Data.State.Speed = (float)d[11] / 1000;
      Data.State.Time = d[12];
      Data.State.Current = (float)d[18] / 100000;
      Data.Pres.BC = (float)d[13] / 1000000;
      Data.Pres.MR = (float)d[14] / 1000000;
      Data.Pres.ER = (float)d[15] / 1000000;
      Data.Pres.BP = (float)d[16] / 1000000;
      Data.Pres.SAP = (float)d[17] / 1000000;
    }
    static private string[] SubstringAtCount(string self)
    {
      //参考 http://baba-s.hatenablog.com/entry/2015/03/19/140748
      var result = new List<string>();
      var length = (int)Math.Ceiling((double)self.Length / 8);

      for (int i = 0; i < length; i++)
      {
        int start = 8 * i;
        if (self.Length <= start)
        {
          break;
        }
        if (self.Length < start + 8)
        {
          result.Add(self.Substring(start));
        }
        else
        {
          result.Add(self.Substring(start, 8));
        }
      }
      return result.ToArray();
    }

  }
}

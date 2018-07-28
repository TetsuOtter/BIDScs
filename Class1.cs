using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace BIDScs
{
  /// <summary>
  /// 互換性確保のために残しています。Pipeの利用を推奨します。
  /// </summary>
  static public class BIDS
  {
    static private bool[] keys = new bool[16];
    static private int[] horn = new int[3];
    static private int[] hornblow = new int[3];
    static private int[,] beacon = new int[10, 6];



    /// <summary>
    /// ポート番号8931を開いて情報を受信する。
    /// </summary>
    /// <returns>受信成功か否か</returns>
    static public bool Get()
    {
      return UdpGet(IPAddress.Any, 8931);
    }

    /// <summary>
    /// ポート番号8931を開いて、指定したIPアドレスからの情報を受信する。
    /// </summary>
    /// <param name="IP">送信元のIPアドレス</param>
    /// <returns>受信成功か否か</returns>
    static public bool Get(IPAddress IP)
    {
      return UdpGet(IP, 8931);
    }

    /// <summary>
    /// 指定したポートを開いて、任意のIPアドレスからの情報を受信する。
    /// </summary>
    /// <param name="Port">受信するために開けるポート</param>
    /// <returns>受信成功か否か</returns>
    static public bool Get(int Port)
    {
      return UdpGet(IPAddress.Any, Port);
    }

    /// <summary>
    /// 指定したポートを開いて、指定したIPアドレスからの情報を受信する。
    /// </summary>
    /// <param name="IP">送信元のIPアドレス</param>
    /// <param name="Port">受信するために開けるポート</param>
    /// <returns></returns>
    static public bool Get(IPAddress IP, int Port)
    {
      return UdpGet(IP, Port);
    }

    /// <summary>
    /// Exceptionをgetできる
    /// </summary>
    static public Exception Exception { get; private set; }



    static private bool UdpGet(IPAddress IP,int Port)
    {
      string udpRcvText;
      int[] intdata = new int[532];
      string[] data = new string[532];

      Exception = null;
      Byte[] rcvBytes;
      try
      {
        IPEndPoint localEP = new IPEndPoint(IP, Port);
        UdpClient udp = new UdpClient(localEP);
        IPEndPoint remoteEP = null;
        rcvBytes = udp.Receive(ref remoteEP);
        udp.Close();
      }
      catch (Exception ex)
      {
        Exception = ex;
        return false;
      }
      Array.Reverse(rcvBytes);
      udpRcvText = BitConverter.ToString(rcvBytes);
      string rcvText = BitConverter.ToString(rcvBytes);
      foreach (var n in rcvText.SubstringAtCount())
      {
        rcvText = rcvText.Replace("-", "");
        data = rcvText.SubstringAtCount();
      }
      Array.Reverse(data);
      for (int i = 0; i < 532; i++)
      {
        intdata[i] = Convert.ToInt32(data[i], 16);
      }
      intdata = Data.DataArray;
      switch (intdata[0])
      {
        case 0:
          break;
        case 1:
          //Dispose
          break;
        case 2:
          //Elapse
          int[] PanelData = new int[256];
          int[] SoundData = new int[256];
          for (int i = 0; i < 256; i++)
          {
            PanelData[i] = intdata[i + 19];
            SoundData[i] = intdata[i + 19 + 256];
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
          PanelData = Data.Panel;
          SoundData = Data.Sound;
          horn = Data.Horn;
          break;
        case 3:
          //KeyDown
          keys[intdata[19]] = true;
          keys = Data.Keys;
          break;
        case 4:
          //KeyUp
          keys[intdata[19]] = false;
          keys = Data.Keys;
          break;
        case 5:
          //HornBlow
          horn[intdata[19]]++;
          horn = Data.Horn;
          break;
        case 6:
          //SetSignal
          Data.SetSig = intdata[19];
          break;
        case 7:
          //SetBeaconData
          for(int i = 9; i >= 0; i--)
          {
            for(int b = 0; b < 6; b++)
            {
              if (i > 0)
              {
                beacon[i - 1, b] = beacon[i, b];
              }
              else
              {
                beacon[0, b] = intdata[b + 19];
              }
            }
          }
          beacon[0, 5] = intdata[12];//Time
          beacon[0, 6] = intdata[10];//Distance
          beacon = Data.Beacon;
          break;
      }
      return true;

    }

    static private string[] SubstringAtCount(this string self)
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

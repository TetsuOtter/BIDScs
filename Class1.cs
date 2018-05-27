using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BIDScs
{
  static public class BIDS
  {
    static private Exception exc;
    static private string udpRcvText;
    static private int[] intdata = new int[532];
    static private string[] data = new string[532];
    static private int[] PanelData = new int[256];
    static private int[] SoundData = new int[256];
    static private bool[] keys = new bool[16];
    static private int[] horn = new int[3];
    static private int[] hornblow = new int[3];
    static private int[,] beacon = new int[10, 6];
    static private int setSig = new int();



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
    /// 受信したデータをint形式で返す
    /// </summary>
    static public int[] Data
    {
      get
      {
        return intdata;
      }
    }


    /// <summary>
    /// 受信したデータのヘッダ情報
    /// </summary>
    static public int Headder
    {
      get
      {
        return intdata[0];
      }
    }

    /// <summary>
    /// 車両のスペックに関する情報
    /// </summary>
    static public class Spec
    {
      /// <summary>
      /// ブレーキ段数
      /// </summary>
      static public int Brake
      {
        get
        {
          return intdata[1];
        }
      }

      /// <summary>
      /// 67度にあたるブレーキ位置(段数)
      /// </summary>
      static public int B67
      {
        get
        {
          return intdata[2];
        }
      }

      /// <summary>
      /// 力行段数
      /// </summary>
      static public int Power
      {
        get
        {
          return intdata[3];
        }
      }
      
      /// <summary>
      /// ATS確認ができる最低の段数
      /// </summary>
      static public int ATS
      {
        get
        {
          return intdata[4];
        }
      }

      /// <summary>
      /// 車両両数
      /// </summary>
      static public int Car
      {
        get
        {
          return intdata[5];
        }
      }
    }

    /// <summary>
    /// 各種ハンドルの位置情報
    /// </summary>
    static public class Handle
    {
      /// <summary>
      /// ブレーキハンドル位置
      /// </summary>
      static public int Brake
      {
        get
        {
          return intdata[6];
        }
      }

      /// <summary>
      /// マスコンハンドル位置
      /// </summary>
      static public int Power
      {
        get
        {
          return intdata[7];
        }
      }

      /// <summary>
      /// レバーサーハンドル位置
      /// </summary>
      static public int Lever
      {
        get
        {
          return intdata[8];
        }
      }

      /// <summary>
      /// 定速スイッチの状態(2018年4月1日現在未実装)
      /// </summary>
      static public int Const
      {
        get
        {
          return intdata[9];
        }
      }
    }

    /// <summary>
    /// 各種圧力を除く車両の各種状態
    /// </summary>
    static public class State
    {
      /// <summary>
      /// 現在の列車位置
      /// </summary>
      static public double Location
      {
        get
        {
          return ((double)intdata[10]) / 1000;
        }
      }

      /// <summary>
      /// 現在の列車速度
      /// </summary>
      static public float Speed
      {
        get
        {
          return ((float)intdata[11]) / 1000;
        }
      }

      /// <summary>
      /// 現在時刻
      /// </summary>
      static public int Time
      {
        get
        {
          return intdata[12];
        }
      }

      /// <summary>
      /// 電動機電流
      /// </summary>
      static public float Current
      {
        get
        {
          return ((float)intdata[18]) / 100000;
        }
      }

      /// <summary>
      /// ドア状態(閉扉=true)
      /// </summary>
      static public bool Door
      {
        get
        {
          if (intdata[531] != 1)
          {
            return false;
          }
          else
          {
            return true;
          }
        }
      }
    }

    /// <summary>
    /// 車両の各種圧力状態
    /// </summary>
    static public class Pres
    {
      /// <summary>
      /// BC圧(ブレーキシリンダ圧力)
      /// </summary>
      static public float BC
      {
        get
        {
          return ((float)intdata[13]) / 1000000;
        }
      }

      /// <summary>
      /// MR圧(元空気ダメ圧力)
      /// </summary>
      static public float MR
      {
        get
        {
          return ((float)intdata[14]) / 1000000;
        }
      }

      /// <summary>
      /// ER圧(釣り合い空気ダメ圧力)
      /// </summary>
      static public float ER
      {
        get
        {
          return ((float)intdata[15]) / 1000000;
        }
      }

      /// <summary>
      /// BP圧(ブレーキ管圧力
      /// </summary>
      static public float BP
      {
        get
        {
          return ((float)intdata[16]) / 1000000;
        }
      }

      /// <summary>
      /// SAP圧(直通管圧力)
      /// </summary>
      static public float SAP
      {
        get
        {
          return ((float)intdata[17]) / 1000000;
        }
      }
    }

    /// <summary>
    /// Panelの状態
    /// </summary>
    static public int[] Panel
    {
      get
      {
        return PanelData;
      }
    }

    /// <summary>
    /// Soundの状態
    /// </summary>
    static public int[] Sound
    {
      get
      {
        return SoundData;
      }
    }

    /// <summary>
    /// ボタンの押下状態(押下時true)
    /// </summary>
    static public bool[] Keys
    {
      get
      {
        return keys;
      }
    }

    /// <summary>
    /// 吹鳴されている警笛の種類
    /// </summary>
    static public int[] Horn
    {
      get
      {
        return horn;
      }
    }

    /// <summary>
    /// Beaconの情報([履歴番号(0~9),情報番号]の形で格納)
    /// </summary>
    static public int[,] Beacon
    {
      get
      {
        return beacon;
      }
    }

    /// <summary>
    /// 現在の閉塞が現示している信号番号
    /// </summary>
    static public int SetSig
    {
      get
      {
        return setSig;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    static public Exception Exception
    {
      get
      {
        return exc;
      }
    }



    static private bool UdpGet(IPAddress IP,int Port)
    {
      exc = null;
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
        exc = ex;
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

      switch (Headder)
      {
        case 0:
          break;
        case 1:
          //Dispose
          break;
        case 2:
          //Elapse
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
          break;
        case 3:
          //KeyDown
          keys[intdata[19]] = true;
          break;
        case 4:
          //KeyUp
          keys[intdata[19]] = false;
          break;
        case 5:
          //HornBlow
          horn[intdata[19]]++;
          break;
        case 6:
          //SetSignal
          setSig = intdata[19];
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

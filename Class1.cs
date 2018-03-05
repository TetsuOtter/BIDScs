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
        static public bool Get()
        {
            //ポート番号8931を開いて情報を受信する。
            return UdpGet(IPAddress.Any, 8931);
        }
        static public bool Get(IPAddress IP)
        {
            //ポート番号8931を開いて、指定したIPアドレスからの情報を受信する。
            return UdpGet(IP, 8931);
        }
        static public bool Get(int Port)
        {
            //指定したポートを開いて、すべてのIPアドレスからの情報を受信する。
            return UdpGet(IPAddress.Any, Port);
        }
        static public bool Get(IPAddress IP, int Port)
        {
            //指定したポートを開いて、指定したIPアドレスからの情報を受信する。
            return UdpGet(IP, Port);
        }

        static public int[] Data
        {
            get
            {
                return intdata;
            }
        }

        static public int Headder
        {
            //ヘッダ情報を取得
            get
            {
                return intdata[0];
            }
        }

        static public class Spec
        {
            //車両のスペックを取得
            static public int Brake
            {
                //ブレーキ段数を取得
                get
                {
                    return intdata[1];
                }
            }
            static public int B67
            {
                //67度にあたるブレーキ位置を取得
                get
                {
                    return intdata[2];
                }
            }
            static public int Power
            {
                //力行段数を取得
                get
                {
                    return intdata[3];
                }
            }
            static public int ATS
            {
                //ATS確認段数を取得
                get
                {
                    return intdata[4];
                }
            }
            static public int Car
            {
                //車両数を取得
                get
                {
                    return intdata[5];
                }
            }
        }
        static public class Handle
        {
            //各種ハンドルの位置を取得できる。
            static public int Brake
            {
                get
                {
                    return intdata[6];
                }
            }
            static public int Power
            {
                get
                {
                    return intdata[7];
                }
            }
            static public int Lever
            {
                get
                {
                    return intdata[8];
                }
            }
            static public int Const
            {
                //(ハンドルじゃないけど)定速SWの状態を取得できる。
                get
                {
                    return intdata[9];
                }
            }
        }
        static public class State
        {
            //車両の各種状態を取得できる。圧力系は別。
            static public double Location
            {
                get
                {
                    return ((double)intdata[10]) / 1000;
                }
            }
            static public float Speed
            {
                get
                {
                    return ((float)intdata[11]) / 1000;
                }
            }
            static public int Time
            {
                get
                {
                    return intdata[12];
                }
            }
            static public float Current
            {
                get
                {
                    return ((float)intdata[18]) / 100000;
                }
            }
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
        static public class Pres
        {
            //車両の各種圧力を取得
            static public float BC
            {
                get
                {
                    return ((float)intdata[13]) / 1000000;
                }
            }
            static public float MR
            {
                get
                {
                    return ((float)intdata[14]) / 1000000;
                }
            }
            static public float ER
            {
                get
                {
                    return ((float)intdata[15]) / 1000000;
                }
            }
            static public float BP
            {
                get
                {
                    return ((float)intdata[16]) / 1000000;
                }
            }
            static public float SAP
            {
                get
                {
                    return ((float)intdata[17]) / 1000000;
                }
            }
        }

        static public int[] Panel
        {
            get
            {
                return PanelData;
            }
        }
        static public int[] Sound
        {
            get
            {
                return SoundData;
            }
        }
        static public bool[] Keys
        {
            //押下時true
            get
            {
                return keys;
            }
        }
        static public int[] Horn
        {
            get
            {
                return horn;
            }
        }
        static public int[,] Beacon
        {
            //Beaconの情報を新しいものから10件格納している。
            //[履歴番号(0~9),情報]の形。
            get
            {
                return beacon;
            }
        }
        static public int SetSig
        {
            get
            {
                return setSig;
            }
        }
        static public Exception Exception
        {
            get
            {
                return exc;
            }
        }

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

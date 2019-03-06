using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TR.BIDSSMemLib;

namespace TR.BIDScs
{
  /// <summary>Server API</summary>
  public partial class Server : IDisposable
  {
    /// <summary>Program Version</summary>
    public readonly int ProgramVersion = 201;

    /// <summary>Connection Setting</summary>
    public Connection Cn { get; }
    /// <summary>Communication Mode Setting</summary>
    public Mode Md { get; }

    /// <summary>Initialize as Server</summary>
    /// <param name="c"></param>
    /// <param name="m"></param>
    public Server(in Connection c = Connection.Local, in Mode m = Mode.Full)
    {
      Cn = c;
      Md = m;

      Md = Mode.Old;

      switch (c)
      {
        case Connection.Local:
          SML = new SMemLib(false, (byte)Md);
          break;
        case Connection.TCP:
          break;
        case Connection.UDP:
          break;
      }
    }
    /// <summary>Connection Type</summary>
    public enum Connection
    {
      /// <summary>SharedMemory Connection</summary>
      Local,
      /// <summary>TCP Connection</summary>
      TCP,
      /// <summary>UDP Connection</summary>
      UDP
    }
    /// <summary>Communication Mode</summary>
    public enum Mode
    {
      /// <summary>FullMode (Basic, Panel, Sound, and Security Data)</summary>
      Full,
      /// <summary>Old structure (Some of Basic, Panel, and Sound Data)</summary>
      Old,
      /// <summary>Lite+ Mode (Basic and Sound Data)</summary>
      LitePlus,
      /// <summary>Lite Mode (Basic Data only)</summary>
      Lite
    }


    private readonly SMemLib SML = null;
    private readonly Tcp TCPCn = null;
    private readonly Udp UDPCn = null;

    /// <summary>Do Connect Process</summary>
    /// <param name="IPAddr"></param>
    public void Connect(in string IPAddr = null)
    {
      switch (Cn)
      {
        case Connection.Local:
          SML.Read<BIDSSharedMemoryData>();
          if (Md == Mode.Old && !SML.BIDSSMemData.IsEnabled) throw new ApplicationException("SharedMemoryに接続できていません。");
          break;
        case Connection.TCP:
          throw new NotSupportedException("TCP接続はまだサポートされていません。 TCP connection method is not yet supported.");
        case Connection.UDP:
          throw new NotSupportedException("UDP接続はまだサポートされていません。 UDP connection method is not yet supported.");
        default:
          throw new NotSupportedException("設定された接続方法はサポートされていません。 The set connection method is not supported.");
      }
    }

    /// <summary>Dispose All</summary>
    public void Dispose()
    {
      SML?.Dispose();
      TCPCn?.Dispose();
      UDPCn?.Dispose();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Command"></param>
    public string OpeReq(in string Command)
    {
      switch (Cn)
      {
        case Connection.Local:
          return DataSelectTR(Command);
        case Connection.TCP:
          break;
        case Connection.UDP:
          break;
        default:
          throw new NotSupportedException("設定された接続方法はサポートされていません。 The set connection method is not supported.");
      }
      return null;
    }

    /// <summary>レバーサーの番号をセットする</summary>
    private int ReverserNum { set => InputEventIgniter(0, value); }

    /// <summary>レバーサーの番号をセットする</summary>
    private int PowerNotchNum { set => InputEventIgniter(1, value); }
    private int BrakeNotchNum { set => InputEventIgniter(2, value); }
    private int SHandleNum { set => InputEventIgniter(3, value); }
    private int BtDown
    {
      set
      {
        if (value < 4) InputEventIgniter(-1, value);
        else InputEventIgniter(-2, value - 4);
      }
    }
    private int BtUp
    {
      set
      {
        if (value < 4) InputEventIgniter(-1, value);
        else InputEventIgniter(-2, value - 4);
      }
    }

    private void InputEventIgniter(int Type, int Value)
    {

    }

    private string DataSelectTR(in string GetString)
    {
      string ReturnString = string.Empty;
      ReturnString = GetString + "X";
      //0 1 2 3
      //T R X X
      switch (GetString.Substring(2, 1))
      {
        case "V":
          return GetString + "X" + ProgramVersion.ToString();

        case "R"://レバーサー
          switch (GetString.Substring(3))
          {
            case "R":
              ReverserNum = -1;
              break;
            case "N":
              ReverserNum = 0;
              break;
            case "F":
              ReverserNum = 1;
              break;
            case "-1":
              ReverserNum = -1;
              break;
            case "0":
              ReverserNum = 0;
              break;
            case "1":
              ReverserNum = 1;
              break;
            default:
              return "TRE7";//要求情報コードが不正
          }
          return ReturnString + "0";
        case "S"://ワンハンドル
          int sers = 0;
          try
          {
            sers = Convert.ToInt32(GetString.Substring(3));
          }
          catch (FormatException)
          {
            return "TRE6";//要求情報コード 文字混入
          }
          catch (OverflowException)
          {
            return "TRE5";//要求情報コード 変換オーバーフロー
          }
          SHandleNum = sers;
          return ReturnString + "0";
        case "P"://Pノッチ操作
          int serp = 0;
          try
          {
            serp = Convert.ToInt32(GetString.Substring(3));
          }
          catch (FormatException)
          {
            return "TRE6";//要求情報コード 文字混入
          }
          catch (OverflowException)
          {
            return "TRE5";//要求情報コード 変換オーバーフロー
          }
          PowerNotchNum = serp;
          return ReturnString + "0";
        case "B"://Bノッチ操作
          int serb = 0;
          try
          {
            serb = Convert.ToInt32(GetString.Substring(3));
          }
          catch (FormatException)
          {
            return "TRE6";//要求情報コード 文字混入
          }
          catch (OverflowException)
          {
            return "TRE5";//要求情報コード 変換オーバーフロー
          }
          BrakeNotchNum = serb;
          return ReturnString + "0";
        case "K"://キー操作
          int serk = 0;
          try
          {
            serk = Convert.ToInt32(GetString.Substring(4));
          }
          catch (FormatException)
          {
            return "TRE6";//要求情報コード 文字混入
          }
          catch (OverflowException)
          {
            return "TRE5";//要求情報コード 変換オーバーフロー
          }
          switch (GetString.Substring(3, 1))
          {
            //udpr
            case "U":
              //if (KyUp(serk)) return ReturnString + "0";
              //else
                return "TRE8";
            case "D":
              //if (KyDown(serk)) return ReturnString + "0";
              //else
                return "TRE8";
            case "P":
              if (serk < 20)
              {
                BtDown = serk;
                return ReturnString + "0";
              }
              else
              {
                return "TRE2";
              }
            case "R":
              if (serk < 20)
              {
                BtUp = serk;
                return ReturnString + "0";
              }
              else
              {
                return "TRE2";
              }
            default:
              return "TRE3";//記号部不正
          }
        case "I"://情報取得
          if (!SML.BIDSSMemData.IsEnabled) return "TRE1";
          int seri = 0;
          try
          {
            seri = Convert.ToInt32(GetString.Substring(4));
          }
          catch (FormatException)
          {
            return "TRE6";//要求情報コード 文字混入
          }
          catch (OverflowException)
          {
            return "TRE5";//要求情報コード 変換オーバーフロー
          }
          switch (GetString.Substring(3, 1))
          {
            case "C":
              switch (seri)
              {
                case 0:
                  return ReturnString + SML.BIDSSMemData.SpecData.B.ToString();
                case 1:
                  return ReturnString + SML.BIDSSMemData.SpecData.P.ToString();
                case 2:
                  return ReturnString + SML.BIDSSMemData.SpecData.A.ToString();
                case 3:
                  return ReturnString + SML.BIDSSMemData.SpecData.J.ToString();
                case 4:
                  return ReturnString + SML.BIDSSMemData.SpecData.C.ToString();
                default: return "TRE2";
              }
            case "E":
              switch (seri)
              {
                case 0: return ReturnString + SML.BIDSSMemData.StateData.Z;
                case 1: return ReturnString + SML.BIDSSMemData.StateData.V;
                case 2: return ReturnString + SML.BIDSSMemData.StateData.T;
                case 3: return ReturnString + SML.BIDSSMemData.StateData.BC;
                case 4: return ReturnString + SML.BIDSSMemData.StateData.MR;
                case 5: return ReturnString + SML.BIDSSMemData.StateData.ER;
                case 6: return ReturnString + SML.BIDSSMemData.StateData.BP;
                case 7: return ReturnString + SML.BIDSSMemData.StateData.SAP;
                case 8: return ReturnString + SML.BIDSSMemData.StateData.I;
                //case 9: return ReturnString + SML.BIDSSMemData.StateData.Volt;//予約 電圧
                case 10: return ReturnString + TimeSpan.FromMilliseconds(SML.BIDSSMemData.StateData.T).Hours.ToString();
                case 11: return ReturnString + TimeSpan.FromMilliseconds(SML.BIDSSMemData.StateData.T).Minutes.ToString();
                case 12: return ReturnString + TimeSpan.FromMilliseconds(SML.BIDSSMemData.StateData.T).Seconds.ToString();
                case 13: return ReturnString + TimeSpan.FromMilliseconds(SML.BIDSSMemData.StateData.T).Milliseconds.ToString();
                default: return "TRE2";
              }
            case "H":
              switch (seri)
              {
                case 0: return ReturnString + SML.BIDSSMemData.HandleData.B;
                case 1: return ReturnString + SML.BIDSSMemData.HandleData.P;
                case 2: return ReturnString + SML.BIDSSMemData.HandleData.R;
                //定速状態は予約
                default: return "TRE2";
              }
            case "P":
              if (seri > 255 || seri < 0) return "TRE2";
              return ReturnString + SML.BIDSSMemData.Panel[seri];
            case "S":
              if (seri > 255 || seri < 0) return "TRE2";
              return ReturnString + SML.BIDSSMemData.Sound[seri];
            case "D":
              if (SML.BIDSSMemData.IsDoorClosed) return ReturnString + "0";
              else return ReturnString + "1";
            default: return "TRE3";//記号部不正
          }
        default:
          return "TRE4";//識別子不正
      }
    }
  }
}

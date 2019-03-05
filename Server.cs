using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TR.BIDScs
{
  /// <summary>Server API</summary>
  public partial class Server : IDisposable
  {
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
    /// <summary>Do Connect Process</summary>
    /// <param name="IPAddr"></param>
    public void Connect(in string IPAddr = null)
    {

    }

    /// <summary>Dispose All</summary>
    public void Dispose()
    {

    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Command"></param>
    public object OpeReq(in string Command)
    {
      return null;
    }
  }
}

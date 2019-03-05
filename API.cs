using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TR.BIDScs
{
  /// <summary>
  /// Provide the interface of BIDS (as the Client Mode)
  /// </summary>
  public class API : IDisposable
  {
    /// <summary>RunMode Setting</summary>
    public RunMode Mode { get; }
    /// <summary>
    /// Initialize the Class
    /// </summary>
    /// <param name="r">Mode which you want to use</param>
    public API(RunMode r = RunMode.Local)
    {
      Mode = r;
    }
    /// <summary>BIDScs Running Mode</summary>
    public enum RunMode
    {
      /// <summary>Connect to Local (SharedMemory Connection)</summary>
      Local,
      /// <summary>Connect to Other Computer (TCP Connection)</summary>
      TCP,
      /// <summary>Connect to Other Computer (TCP Connection)</summary>
      UDP
    }
    /// <summary>Operation Request(To Get the Data or To Request the Operation)</summary>
    /// <param name="cmd">Request Command</param>
    /// <returns>Operation Result</returns>
    public object OpeReq(string cmd)
    {

      return null;
    }

    /// <summary>Dispose All</summary>
    public void Dispose()
    {

    }
  }
}

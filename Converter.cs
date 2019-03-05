using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TR.BIDSSMemLib;

namespace TR.BIDScs
{
  /// <summary>Convert Any Byte Array following the BIDS Specification</summary>
  static public class Converter
  {
    /// <summary>Convert to the struct you want</summary>
    /// <typeparam name="T">The Struct Name you want to convert to</typeparam>
    /// <param name="Array">The byte Array you received</param>
    /// <returns>The Convert Result</returns>
    static public T Conv<T>(in byte[] Array) where T : struct
    {
      return default;
    }

    static public ElapD GetElapD(in byte[] Array)
    {
      return default;
    }
  }
}

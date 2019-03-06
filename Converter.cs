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

    /// <summary>The Header Array of ElapD Array</summary>
    static public readonly byte[] ElapDHeader = new byte[4]{ 0x54, 0x52, 0x62, 0x02 };

    /// <summary>The Tail Array of all array structure</summary>
    static public readonly byte[] TailArray = new byte[4] { 0xFE, 0xFE, 0xFE, 0xFE };

    /// <summary>
    /// Convert to ElapD from byte Array
    /// </summary>
    /// <param name="Array">Received Array</param>
    /// <returns>Converted data</returns>
    static public ElapD GetElapD(in byte[] Array)
    {
      if (Array.Take(4) != ElapDHeader) throw new FormatException("The Header Value is not valid.  接頭辞の形式が不正です。");
      if (Array.Reverse().Take(4) != TailArray) throw new FormatException("The Tail Value is not valid.  接尾辞の形式が不正です。");
      if (Array.Count() < 60) throw new ArgumentOutOfRangeException("The Array length is not valid.  入力された配列の長さが不正です。");
      try
      {
        return new ElapD()
        {
          IsEnabled = true,
          Location = Array.GetDouble(1),
          Speed = Array.GetSingle(3),
          Current = Array.GetSingle(4),
          Voltage = 0,//NotSupported
          Time = Array.GetUInt(6),
          BCPres = Array.GetSingle(7),
          MRPres = Array.GetSingle(8),
          ERPres = Array.GetSingle(9),
          BPPres = Array.GetSingle(10),
          SAPPres = Array.GetSingle(11),
          SignalNum = Array.GetInt(12),
          DoorState = Array.GetUInt(13)
        };
      }
      catch { throw; }
    }



    static private double GetDouble(this byte[] b, int DataNum) => Convert.ToDouble(b.Skip(4 * DataNum).Take(8).ToArray());
    static private int GetInt(this byte[] b, int DataNum) => Convert.ToInt32(b.Skip(4 * DataNum).Take(4).ToArray());
    static private float GetSingle(this byte[] b, int DataNum) => Convert.ToSingle(b.Skip(4 * DataNum).Take(4).ToArray());
    static private uint GetUInt(this byte[] b, int DataNum) => Convert.ToUInt32(b.Skip(4 * DataNum).Take(4).ToArray());
  }
}

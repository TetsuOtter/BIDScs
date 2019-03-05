using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Event Describing File

namespace TR.BIDScs
{
  public partial class Server
  {
    /// <summary>Ignit when Received Basic Data</summary>
    public event EventHandler<BasicDataGroupReceivedEventArgs> BasicReceived;
    /// <summary>Ignit when Received Panel Data Array</summary>
    public event EventHandler<ArrayDataReceivedEventArgs> PanelReceived;
    /// <summary>Ignit when Received Sound Data Array</summary>
    public event EventHandler<ArrayDataReceivedEventArgs> SoundReceived;
    /// <summary>
    /// Please note : It does not work now.
    /// Ignit when Received Security Device Data
    /// </summary>
    public event EventHandler<SecurityDeviceDataReceivedEventArgs> SecurityReceived;


    /// <summary>Class used when Received ArrayData</summary>
    public class ArrayDataReceivedEventArgs : EventArgs
    {
      /// <summary>The Array Number</summary>
      public byte Num { get; }
      /// <summary>Array Data (int[128])</summary>
      public int[] Data { get; }
    }
    /// <summary>Class used when Received Basic Data</summary>
    public class BasicDataGroupReceivedEventArgs : EventArgs
    {
      /// <summary>Value of Header</summary>
      public enum Header
      {
        /// <summary>The Constant Data Group</summary>
        Constant,
        /// <summary>The Data, used in BVE5 and OpenBVE, Group</summary>
        Shared,
        /// <summary>The Data, used in BVE5 only, Group</summary>
        BVE5,
        /// <summary>The Data, used in OpenBVE only, Group</summary>
        OpenBVE
      }
      /// <summary>The Header Number</summary>
      public Header HNum;
      /// <summary>All of Received Data (including Header and Tail Sign)</summary>
      public byte[] AllData;
    }
    /// <summary>Class used when Received Security Device Data</summary>
    public class SecurityDeviceDataReceivedEventArgs : EventArgs
    {

    }

  }
}

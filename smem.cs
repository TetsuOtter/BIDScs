using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using TR.BIDSSMemLib;

namespace TR.BIDScs
{
  internal class smem : IBIDSCom
  {
    public bool IsLocal => true;
    public event EventHandler<SMemLib.BSMDChangedEArgs> BIDSSMemChanged
    {
      add => SMemLib.BIDSSMemChanged += value;
      remove => SMemLib.BIDSSMemChanged -= value;
    }
    public event EventHandler<SMemLib.OpenDChangedEArgs> OpenDChanged
    {
      add => SMemLib.OpenDChanged += value;
      remove => SMemLib.OpenDChanged -= value;
    }
    public event EventHandler<SMemLib.ArrayDChangedEArgs> PanelDChanged
    {
      add => SMemLib.PanelDChanged += value;
      remove => SMemLib.PanelDChanged -= value;
    }
    public event EventHandler<SMemLib.ArrayDChangedEArgs> SoundDChanged
    {
      add => SMemLib.SoundDChanged += value;
      remove => SMemLib.SoundDChanged -= value;
    }
    SMemLib sml = null;
    public bool Close()
    {
      try
      {
        sml.ReadStop();
        return true;
      }
      catch(Exception e)
      {
        Console.WriteLine(e);
        return false;
      }
    }

    public void Dispose() => sml?.Dispose();

    public bool Open()
    {
      try
      {
        sml = new SMemLib(true, 0);
        if (sml == null) return false;
        sml.ReadStart();
        return true;
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        return false;
      }
    }

    public bool Open(int Port) => Open();
    public bool Open(IPAddress addr) => Open();
    public bool Open(int Port, IPAddress addr) => Open();

    public object Read(InfoType type)
    {
      switch (type)
      {
        case InfoType.BIDSSharedMemoryData:
          return sml?.Read<BIDSSharedMemoryData>();
        case InfoType.OpenD:
          return sml?.Read<OpenD>();
        case InfoType.Panel:
          return sml?.Read<PanelD>();
        case InfoType.Sound:
          return sml?.Read<SoundD>();
        case InfoType.Spec:
          return ((BIDSSharedMemoryData)(sml?.Read<BIDSSharedMemoryData>())).SpecData;
        case InfoType.State:
          return ((BIDSSharedMemoryData)(sml?.Read<BIDSSharedMemoryData>())).StateData;
        case InfoType.Handle:
          return ((BIDSSharedMemoryData)(sml?.Read<BIDSSharedMemoryData>())).HandleData;
        default: return null;
      }
    }

    public byte[] ReadByte(InfoType type)
    {
      switch (type)
      {
        case InfoType.BIDSSharedMemoryData:
          return UsefulFunc.StructToByteA(sml?.Read<BIDSSharedMemoryData>());
          //var bsmd = sml?.Read<BIDSSharedMemoryData>();
          //var bsmdsize = Marshal.SizeOf(bsmd);
          //byte[] bsmdba = new byte[bsmdsize];
          //IntPtr ipb = Marshal.AllocHGlobal(bsmdsize);
          //Marshal.StructureToPtr(bsmd, ipb, true);
          //Marshal.Copy(ipb, bsmdba, 0, bsmdsize);
          //Marshal.FreeHGlobal(ipb);
          //return null;
        case InfoType.OpenD:
          return UsefulFunc.StructToByteA(sml?.Read<OpenD>());
        case InfoType.Panel:
          /*var panel = (PanelD)sml?.Read<PanelD>();
          byte[] pdba = new byte[panel.Length + sizeof(int)];
          //https://stackoverflow.com/questions/3278827/how-to-convert-a-structure-to-a-byte-array-in-c
          IntPtr ipp = Marshal.AllocHGlobal(pdba.Length);
          Marshal.StructureToPtr(panel, ipp, true);
          Marshal.Copy(ipp, pdba, 0, pdba.Length);
          Marshal.FreeHGlobal(ipp);
          return pdba;*/
          return UsefulFunc.StructToByteA(sml?.Read<PanelD>());
        case InfoType.Sound:
          /*var sound = (SoundD)sml?.Read<SoundD>();
          byte[] sdba = new byte[sound.Length + sizeof(int)];
          IntPtr ips = Marshal.AllocHGlobal(sdba.Length);
          Marshal.StructureToPtr(panel, ips, true);
          Marshal.Copy(ips, sdba, 0, sdba.Length);
          Marshal.FreeHGlobal(ips);
          return sdba;*/
          return UsefulFunc.StructToByteA(sml?.Read<SoundD>());
        case InfoType.Spec:
          return UsefulFunc.StructToByteA(((BIDSSharedMemoryData)(sml?.Read<BIDSSharedMemoryData>())).SpecData);
        case InfoType.State:
          return UsefulFunc.StructToByteA(((BIDSSharedMemoryData)(sml?.Read<BIDSSharedMemoryData>())).StateData);
        case InfoType.Handle:
          return UsefulFunc.StructToByteA(((BIDSSharedMemoryData)(sml?.Read<BIDSSharedMemoryData>())).HandleData);
        default: return null;
      }
    }

    public bool Write(InfoType type, object data)
    {
      switch (type)
      {
        case InfoType.Control:
          //http://hensa40.cutegirl.jp/archives/3938
          //名前付きイベントを発火させる。
          if (data.GetType() != typeof(string)) return false;
          try
          {
            using (EventWaitHandle e = EventWaitHandle.OpenExisting("BIDSNamedEv_" + (string)data))
            {
              e.Set();
              return true;
            }
          }catch(Exception e)
          {
            Console.WriteLine(e);
            return false;
          }
        default: return false;
      }
    }

    public bool Write<T>(T data) where T : struct
    {
      if (new T() is Hand)//Control
      {
        string[] Hdrs = new string[] { "TRR", "TRP", "TRB", "TRK" };
        return true;
      }
      else return false;
    }
  }
}

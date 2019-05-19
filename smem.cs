using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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

    public void Dispose() => sml.Dispose();

    public bool Open()
    {
      try
      {
        sml = new SMemLib(0);
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

        default: return null;
      }
    }

    public byte[] ReadByte(InfoType type)
    {
      throw new NotImplementedException();
    }

    public bool Write(InfoType type, object data)
    {
      throw new NotImplementedException();
    }

    public bool Write<T>(T data) where T : struct
    {
      throw new NotImplementedException();
    }
  }
}

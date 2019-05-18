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
    public bool IsLocal { get; } = true;
    public event EventHandler<object> DataUpdated;
    public event EventHandler<SMemLib.BSMDChangedEArgs> BIDSSMemChanged;
    public event EventHandler<SMemLib.OpenDChangedEArgs> OpenDChanged;
    public event EventHandler<SMemLib.ArrayDChangedEArgs> PanelDChanged;
    public event EventHandler<SMemLib.ArrayDChangedEArgs> SoundDChanged;
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
      throw new NotImplementedException();
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

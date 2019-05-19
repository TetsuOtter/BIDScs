using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TR.BIDSSMemLib;

namespace TR.BIDScs
{
  internal class tcp : IBIDSCom
  {
    public bool IsLocal => false;

    public event EventHandler<SMemLib.BSMDChangedEArgs> BIDSSMemChanged;
    public event EventHandler<SMemLib.OpenDChangedEArgs> OpenDChanged;
    public event EventHandler<SMemLib.ArrayDChangedEArgs> PanelDChanged;
    public event EventHandler<SMemLib.ArrayDChangedEArgs> SoundDChanged;

    public bool Close()
    {
      throw new NotImplementedException();
    }

    public void Dispose()
    {
      throw new NotImplementedException();
    }

    public bool Open()
    {
      throw new NotImplementedException();
    }

    public bool Open(int Port)
    {
      throw new NotImplementedException();
    }

    public bool Open(IPAddress addr)
    {
      throw new NotImplementedException();
    }

    public bool Open(int Port, IPAddress addr)
    {
      throw new NotImplementedException();
    }

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TR.BIDScs
{
  internal class com : IBIDSCom
  {
    public event EventHandler<object> DataUpdated;

    public bool Close()
    {
      throw new NotImplementedException();
    }

    public bool Open()
    {
      throw new NotImplementedException();
    }

    public object Read(InfoType type)
    {
      throw new NotImplementedException();
    }

    public bool Write(InfoType type, object data)
    {
      throw new NotImplementedException();
    }
  }
}

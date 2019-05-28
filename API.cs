using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TR.BIDScs
{
  /// <summary>The type of Request</summary>
  public enum ReqType
  {
    None
  }

  class API
  {
    
    public API(ModeType mt)
    {

    }

    public int GetInt(in string cmd) => (int?)ReqAnalizer(in cmd) ?? 0;
    public float GetSingle(in string cmd) => (float?)ReqAnalizer(in cmd) ?? 0f;
    public double GetDouble(in string cmd) => (double?)ReqAnalizer(in cmd) ?? 0d;
    public bool GetBool(in string cmd) => (bool?)ReqAnalizer(in cmd) ?? false;

    public string SendReq(in string cmd) => (ReqAnalizer(in cmd) ?? string.Empty).ToString();

    public string SendReq(ReqType rt, int infoNum)
    {
      return string.Empty;
    }


    public object ReqAnalizer(in string cmd)
    {
      if ((string)cmd.Take(2) == "TR")
      {
        switch ((string)cmd.Skip(2).Take(1))
        {

          default: return null;
        }
      }
      if ((string)cmd.Take(2) == "TO")
      {
        switch ((string)cmd.Skip(2).Take(1))
        {

          default: return null;
        }
      }
      return null;
    }
  }
}

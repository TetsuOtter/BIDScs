using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Pipes;
using System.Windows.Forms;

namespace BIDScs
{
  static public class PipeSV
  {
    static private string PipeName = "BIDSPipe";

    static private string SVPCName = ".";
    /// <summary>
    /// パイプを開く
    /// </summary>
    static public void Start()
    {
      PipeStart();
    }
    /// <summary>
    /// パイプを開く
    /// </summary>
    /// <param name="PCName">通信相手のコンピュータ名</param>
    static public void Start(string PCName)
    {
      if (PCName != null || PCName == "")
      {
        SVPCName = PCName;
      }
      PipeStart();
    }
    /// <summary>
    /// パイプを開く
    /// </summary>
    /// <param name="PCName">クライアントのコンピュータ名</param>
    /// <param name="PpName">使用するパイプの名前</param>
    static public void Start(string PCName, string PpName)
    {
      if (PpName != null || PpName == "")
      {
        PipeName = PpName;
      }
      else
      {
        PipeName = "BIDSPipe";
      }
      if (PCName != null || PCName == "")
      {
        SVPCName = PCName;
      }
      else
      {
        SVPCName = ".";
      }
      PipeStart();
    }
    /// <summary>
    /// パイプを閉じる
    /// </summary>
    static public void Stop()
    {
      PipeEnd();
    }

    static private void PipeStart()
    {

    }
    static private void PipeEnd()
    {

    }
  }
}

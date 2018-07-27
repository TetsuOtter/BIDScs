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
    static private int MaxCl = 1;
    static private int PipeSize = PipeData.Length * sizeof(Byte);
    static private Byte[] PipeData = new Byte[32];
    static private Byte[] PipeRData = new Byte[32];
    static private string ClPCName = ".";
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
        ClPCName = PCName;
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
        ClPCName = PCName;
      }
      else
      {
        ClPCName = ".";
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

    static private NamedPipeServerStream NPSS
      = new NamedPipeServerStream(PipeName, PipeDirection.InOut, MaxCl,
        PipeTransmissionMode.Byte, PipeOptions.Asynchronous, PipeSize, PipeSize);
    private static IAsyncResult ar;

    /// <summary>
    /// サーバーとしてPipeを開く
    /// </summary>
    static private void PipeStart()
    {
      NPSS.ReadTimeout = 5000;
    }
    static private void PipeStart(string pn)
    {
      if (pn == null || pn == "")
      {
        PipeName = "BIDSPipe";
      }
      NPSS.ReadTimeout = 5000;
      NPSS.WaitForConnectionAsync();
    }
    static private void PipeEnd()
    {
      if (NPSS.IsConnected == true)
      {
        try
        {
          NPSS.EndWaitForConnection(ar);
        }
        catch (Exception e)
        {
          MessageBox.Show("BIDScs.dll", e.Message);
        }
      }
      else
      {
        NPSS.Close();
      }
    }
    static private void PipeSend(Byte[] b)
    {
      if (NPSS.CanWrite == true)
      {
        NPSS.BeginWrite(b, 0, PipeSize, PipeWriteAsyncCallBack, null);
      }
    }

    private static void PipeWriteAsyncCallBack(IAsyncResult ar)
    {
      NPSS.EndWrite(ar);
    }

    static private Byte[] PipeGet()
    {
      if (NPSS.CanRead == true)
      {
        NPSS.BeginRead(PipeRData, 0, PipeSize, PipeReadAsyncCallBack, null);
      }
      else
      {
        return null;
      }
      return PipeRData;
    }

    private static void PipeReadAsyncCallBack(IAsyncResult ar)
    {
      NPSS.EndRead(ar);
    }
  }
}

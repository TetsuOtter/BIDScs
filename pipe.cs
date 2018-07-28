using System;
using System.IO.Pipes;
using System.Windows.Forms;

namespace BIDScs
{
  /// <summary>
  /// Pipeをクライアントとして利用するクラス
  /// </summary>
  static public class Pipe
  {
    //1.新しいスレッドを建てる
    //2.パイプ通信を開始する
    //3.値に変化があればイベント実行
    //4.ループ
    //5.パイプを閉じる
    static private string PipeName = "BIDSPipe";
    static private string SVPCName = ".";

    /// <summary>
    /// サーバーとの通信を開始する。
    /// </summary>
    static public void Start()
    {
      PipeStart();
    }
    /// <summary>
    /// サーバーとの通信を開始する。
    /// </summary>
    /// <param name="PCName">サーバーのコンピュータ名</param>
    static public void Start(string PCName)
    {
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
    /// サーバーとの通信を開始する。
    /// </summary>
    /// <param name="PCName">サーバーのコンピュータ名</param>
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
    /// サーバーとの通信を終了する。
    /// </summary>
    static public void Stop()
    {
      PipeEnd();
    }

    //https://docs.microsoft.com/ja-jp/dotnet/standard/io/how-to-use-named-pipes-for-network-interprocess-communication#robust-programming
    
    static private NamedPipeClientStream NPCS =
      new NamedPipeClientStream(SVPCName, PipeName,
                        PipeDirection.InOut, PipeOptions.None);
    static async private void PipeStart()
    {
      if (NPCS.IsConnected == false)
      {
        while (true)
        {
          try
          {
            await NPCS.ConnectAsync(5000);
          }
          catch (Exception e)
          {
            DialogResult DR = MessageBox.Show(e.Message, "BIDScs.dll", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
            if (DR == DialogResult.Cancel){ break; }
          }
          if (NPCS.IsConnected == true) { break; }
        }
      }
      else
      {
        MessageBox.Show("既に接続されています。", "BIDScs.dll", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }
    static private Byte[] ReadByte = new Byte[32];
    /// <summary>
    /// サーバーからのデータを読み取る
    /// </summary>
    static public void PipeGet()
    {
      try
      {
        NPCS.BeginRead(ReadByte, 0, sizeof(Byte) * 32, ByteTo, null);
      }catch(Exception e)
      {
        MessageBox.Show(e.Message, "BIDScs.dll", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }
    static private void ByteTo(IAsyncResult ar)
    {
      NPCS.EndRead(ar);
      Data.Sort(ReadByte);
    }
    /// <summary>
    /// サーバーにByteデータを送信する
    /// </summary>
    /// <param name="sd">送信するデータ</param>
    static public void PipeSend(Byte[] sd)
    {
      NPCS.BeginWrite(sd, 0, sizeof(Byte) * 32, WriteCB, null);
    }

    private static void WriteCB(IAsyncResult ar)
    {
      NPCS.EndWrite(ar);
    }

    static private void PipeEnd()
    {
      try
      {
        NPCS.Close();
      }
      catch (Exception e)
      {
        DialogResult DR = MessageBox.Show(e.Message, "BIDScs.dll", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
      }
    }
  }
}

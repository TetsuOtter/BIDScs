using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TR.BIDSSMemLib;

namespace TR.BIDScs
{
  /// <summary>BIDScsに実装された通信手段の列挙</summary>
  public enum ModeType
  {
    /// <summary>SharedMemoryを使用した通信</summary>
    smem,
    /// <summary>TCPを用いた通信</summary>
    tcp,
    /// <summary>UDPを用いた通信</summary>
    udp,
    /// <summary>COMポートを用いた通信</summary>
    com,
    /// <summary>名前付きパイプを用いた通信</summary>
    pipe
  }
  /// <summary>BIDSの情報種類</summary>
  public enum InfoType
  {
    /// <summary>デフォルト指定用 何もしません</summary>
    None,
    /// <summary>基本的な情報すべてを格納する構造体</summary>
    BIDSSharedMemoryData,
    /// <summary>OpenBVEでのみ取得できる情報を格納する構造体</summary>
    OpenD,
    /// <summary>Panel配列を格納する構造体</summary>
    Panel,
    /// <summary>Sound配列を格納する構造体</summary>
    Sound,
    /// <summary>車両のスペックを格納する構造体</summary>
    Spec,
    /// <summary>車両状態を格納する構造体</summary>
    State,
    /// <summary>ハンドル位置情報を格納する構造体</summary>
    Handle,
    /// <summary>操作要求を出すときに使用する。</summary>
    Control
  }

  /// <summary>BIDScsがWrapする通信手段のインターフェース</summary>
  public interface IBIDSCom
  {
    /// <summary>対外向け通信手段でない(T)か, 否(F)か</summary>
    bool IsLocal { get; }
    /// <summary>データが更新されたことを通知するのに用います。</summary>
    event EventHandler<object> DataUpdated;
    /// <summary> BIDSSMemDataが更新された際に呼ばれるイベント </summary>
    event EventHandler<SMemLib.BSMDChangedEArgs> BIDSSMemChanged;
    /// <summary> OpenDが更新された際に呼ばれるイベント </summary>
    event EventHandler<SMemLib.OpenDChangedEArgs> OpenDChanged;
    /// <summary> Panelが更新された際に呼ばれるイベント </summary>
    event EventHandler<SMemLib.ArrayDChangedEArgs> PanelDChanged;
    /// <summary> Soundが更新された際に呼ばれるイベント </summary>
    event EventHandler<SMemLib.ArrayDChangedEArgs> SoundDChanged;

    /// <summary>自動で接続先を探索し、接続する処理</summary>
    /// <returns>接続に成功したかどうか</returns>
    bool Open();
    /// <summary>接続先ポート番号を指定し、接続する処理</summary>
    /// <param name="Port">接続先ポート番号</param>
    /// <returns>接続に成功したかどうか</returns>
    bool Open(int Port);
    /// <summary>指定の接続先アドレスに接続する処理</summary>
    /// <param name="addr">接続先アドレス</param>
    /// <returns>接続に成功したかどうか</returns>
    bool Open(IPAddress addr);
    /// <summary>指定のアドレス/ポートに対して接続を試行する処理</summary>
    /// <param name="Port">接続先ポート番号</param>
    /// <param name="addr">接続先アドレス</param>
    /// <returns>接続に成功したかどうか</returns>
    bool Open(int Port, IPAddress addr);

    /// <summary>指定の情報を取得する</summary>
    /// <param name="type">情報種類</param>
    /// <returns>取得した情報</returns>
    object Read(InfoType type);
    /// <summary>指定の情報をbyte配列で取得する</summary>
    /// <param name="type">情報種類</param>
    /// <returns>受信した情報配列</returns>
    byte[] ReadByte(InfoType type);

    /// <summary>指定の情報を送信する</summary>
    /// <param name="type">情報種類</param>
    /// <param name="data">送信する情報</param>
    /// <returns>成功したかどうか</returns>
    bool Write(InfoType type, object data);
    /// <summary>指定の構造体データを書き込む</summary>
    /// <typeparam name="T">書き込む構造体の型</typeparam>
    /// <param name="data">書き込む構造体</param>
    /// <returns>成功したかどうか</returns>
    bool Write<T>(T data) where T : struct;

    /// <summary>通信を終了します。</summary>
    /// <returns>終了できたかどうか</returns>
    bool Close();
    /// <summary>すべてのリソースを解放します。通信中である場合、通信は終了されます。</summary>
    void Dispose();
  }
}

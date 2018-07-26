using System;

namespace BIDScs
{
  public class Data
  {
    /// <summary>
    /// 受信したデータをint形式で返す
    /// </summary>
    static public int[] DataArray { get; set; } = new int[532];


    /// <summary>
    /// 受信したデータのヘッダ情報
    /// </summary>
    static public int Headder
    {
      get
      {
        return DataArray[0];
      }
      set
      {
        DataArray[0] = value;
      }
    }

    /// <summary>
    /// 車両のスペックに関する情報
    /// </summary>
    static public class Spec
    {
      /// <summary>
      /// ブレーキ段数
      /// </summary>
      static public int Brake { get; set; } = new int();

      /// <summary>
      /// 67度にあたるブレーキ位置(段数)
      /// </summary>
      static public int B67 { get; set; } = new int();

      /// <summary>
      /// 力行段数
      /// </summary>
      static public int Power { get; set; } = new int();

      /// <summary>
      /// ATS確認ができる最低の段数
      /// </summary>
      static public int ATS { get; set; } = new int();

      /// <summary>
      /// 車両両数
      /// </summary>
      static public int Car { get; set; } = new int();
    }

    /// <summary>
    /// 各種ハンドルの位置情報
    /// </summary>
    static public class Handle
    {
      /// <summary>
      /// ブレーキハンドル位置
      /// </summary>
      static public int Brake { get; set; } = new int();

      /// <summary>
      /// マスコンハンドル位置
      /// </summary>
      static public int Power { get; set; } = new int();

      /// <summary>
      /// レバーサーハンドル位置
      /// </summary>
      static public int Lever { get; set; } = new int();

      /// <summary>
      /// 定速スイッチの状態(2018年4月1日現在未実装)
      /// </summary>
      static public int Const { get; set; } = new int();
    }

    /// <summary>
    /// 各種圧力を除く車両の各種状態
    /// </summary>
    static public class State
    {
      /// <summary>
      /// 現在の列車位置
      /// </summary>
      static public double Location { get; set; } = new double();

      /// <summary>
      /// 現在の列車速度
      /// </summary>
      static public float Speed { get; set; } = new float();

      /// <summary>
      /// 現在時刻
      /// </summary>
      static public int Time { get; set; } = new int();

      /// <summary>
      /// 電動機電流
      /// </summary>
      static public float Current { get; set; } = new int();

      /// <summary>
      /// ドア状態(閉扉=true)
      /// </summary>
      static public bool Door { get; set; } = new bool();
    }

    /// <summary>
    /// 車両の各種圧力状態
    /// </summary>
    static public class Pres
    {
      /// <summary>
      /// BC圧(ブレーキシリンダ圧力)
      /// </summary>
      static public float BC { get; set; } = new float();

      /// <summary>
      /// MR圧(元空気ダメ圧力)
      /// </summary>
      static public float MR { get; set; } = new float();

      /// <summary>
      /// ER圧(釣り合い空気ダメ圧力)
      /// </summary>
      static public float ER { get; set; } = new float();

      /// <summary>
      /// BP圧(ブレーキ管圧力
      /// </summary>
      static public float BP { get; set; } = new float();

      /// <summary>
      /// SAP圧(直通管圧力)
      /// </summary>
      static public float SAP { get; set; } = new float();
    }

    /// <summary>
    /// Panelの状態
    /// </summary>
    static public int[] Panel { get; set; } = new int[256];

    /// <summary>
    /// Soundの状態
    /// </summary>
    static public int[] Sound { get; set; } = new int[256];

    /// <summary>
    /// ボタンの押下状態(押下時true)
    /// </summary>
    static public bool[] Keys { get; set; } = new bool[16];

    /// <summary>
    /// 吹鳴されている警笛の種類
    /// </summary>
    static public int[] Horn { get; set; } = new int[3];

    /// <summary>
    /// Beaconの情報([履歴番号(0~9),情報番号]の形で格納)
    /// </summary>
    static public int[,] Beacon { get; set; } = new int[10, 6];

    /// <summary>
    /// 現在の閉塞が現示している信号番号
    /// </summary>
    static public int SetSig { get; set; } = new int();

    /// <summary>
    /// 予約 ポインタ用
    /// </summary>
    static public Byte[,] Pointer { get; set; }

    /// <summary>
    /// 保安機器についての情報(準備中)
    /// 点滅でも点灯でもtrue 消灯でfalse
    /// </summary>
    static public class Hoan
    {
      /// <summary>
      /// 動作中の保安装置
      /// </summary>
      static public byte Working { get; set; } = new byte();
      /// <summary>
      /// ATS-Aに関する情報(準備中) 番号:0
      /// </summary>
      static public class ATS_A
      {

      }
      /// <summary>
      /// ATS-Bに関する情報 番号:1
      /// </summary>
      static public class ATS_B
      {
        /// <summary>
        /// 保安装置が使用できるかどうか
        /// </summary>
        static public byte TF { get; set; } = new byte();
        /// <summary>
        /// 電源表示TF
        /// </summary>
        static public byte Power { get; set; } = new byte();
        /// <summary>
        /// 動作表示TF
        /// </summary>
        static public byte Working { get; set; } = new byte();


      }
      /// <summary>
      /// ATS-Sに関する情報 番号:2
      /// </summary>
      static public class ATS_S
      {
        /// <summary>
        /// 保安装置が使用できるかどうか
        /// </summary>
        static public byte TF { get; set; } = new byte();
        /// <summary>
        /// 電源表示TF
        /// </summary>
        static public byte Power { get; set; } = new byte();
        /// <summary>
        /// 動作表示TF
        /// </summary>
        static public byte Working { get; set; } = new byte();

      }

      //番号3は予備
      /// <summary>
      /// ATS-Snに関する情報 番号:4
      /// </summary>
      static public class ATS_Sn
      {
        /// <summary>
        /// 保安装置が使用できるかどうか
        /// </summary>
        static public byte TF { get; set; } = new byte();
        /// <summary>
        /// 保安装置電源TF
        /// </summary>
        static public byte Power { get; set; } = new byte();
        /// <summary>
        /// ATS動作表示TF
        /// </summary>
        static public byte Working { get; set; } = new byte();

      }
      /// <summary>
      /// ATS-STに関する情報 番号:5
      /// </summary>
      static public class ATS_St
      {
        /// <summary>
        /// 保安装置が使用できるかどうか
        /// </summary>
        static public byte TF { get; set; } = new byte();
        /// <summary>
        /// 電源表示TF
        /// </summary>
        static public byte Power { get; set; } = new byte();
        /// <summary>
        /// 動作表示TF
        /// </summary>
        static public byte Working { get; set; } = new byte();

      }
      /// <summary>
      /// ATS-Swに関する情報 番号:6
      /// </summary>
      static public class ATS_Sw
      {
        /// <summary>
        /// 保安装置が使用できるかどうか
        /// </summary>
        static public byte TF { get; set; } = new byte();
        /// <summary>
        /// 電源表示TF
        /// </summary>
        static public byte Power { get; set; } = new byte();
        /// <summary>
        /// 動作表示TF
        /// </summary>
        static public byte Working { get; set; } = new byte();

      }
      /// <summary>
      /// ATS-SSに関する情報 番号:7
      /// </summary>
      static public class ATS_SS
      {
        /// <summary>
        /// 保安装置が使用できるかどうか
        /// </summary>
        static public byte TF { get; set; } = new byte();
        /// <summary>
        /// 電源表示TF
        /// </summary>
        static public byte Power { get; set; } = new byte();
        /// <summary>
        /// 動作表示TF
        /// </summary>
        static public byte Working { get; set; } = new byte();

      }
      /// <summary>
      /// ATS-SKに関する情報 番号:8
      /// </summary>
      static public class ATS_SK
      {
        /// <summary>
        /// 保安装置が使用できるかどうか
        /// </summary>
        static public byte TF { get; set; } = new byte();
        /// <summary>
        /// 電源表示TF
        /// </summary>
        static public byte Power { get; set; } = new byte();
        /// <summary>
        /// 動作表示TF
        /// </summary>
        static public byte Working { get; set; } = new byte();

      }
      /// <summary>
      /// ATS-SFに関する情報 番号:9
      /// </summary>
      static public class ATS_SF
      {
        /// <summary>
        /// 保安装置が使用できるかどうか
        /// </summary>
        static public byte TF { get; set; } = new byte();
        /// <summary>
        /// 電源表示TF
        /// </summary>
        static public byte Power { get; set; } = new byte();
        /// <summary>
        /// 動作表示TF
        /// </summary>
        static public byte Working { get; set; } = new byte();

      }
      //番号10は予備

      /// <summary>
      /// ATS-Pに関する情報 番号:11
      /// </summary>
      static public class ATS_P
      {
        /// <summary>
        /// 保安装置が使用できるかどうか
        /// </summary>
        static public byte TF { get; set; } = new byte();
        /// <summary>
        /// 保安装置の電源投入状況
        /// </summary>
        static public byte Power { get; set; } = new byte();
        /// <summary>
        /// パターン接近TF
        /// </summary>
        static public byte Pattern_Approaching { get; set; } = new byte();
        /// <summary>
        /// ブレーキ動作TF
        /// </summary>
        static public byte Brake_Working { get; set; } = new byte();
        /// <summary>
        /// ブレーキ開放TF
        /// </summary>
        static public byte Brake_Release { get; set; } = new byte();
        /// <summary>
        /// 「ATS-P」表示TF
        /// </summary>
        static public byte ATS_P_Lump { get; set; } = new byte();
        /// <summary>
        /// 故障TF
        /// </summary>
        static public byte Broken { get; set; } = new byte();

        /// <summary>
        /// 通停判別
        /// 0で無効 1なら停車 2なら通過
        /// </summary>
        static public byte Pass_Stop { get; set; } = new byte();
        /// <summary>
        /// パターン速度情報
        /// </summary>
        static public float Pattern_Speed { get; set; } = new float();
      }
      /// <summary>
      /// ATS-Psに関する情報 番号:12
      /// </summary>
      static public class ATS_Ps
      {
        /// <summary>
        /// 保安装置が使用できるかどうか
        /// </summary>
        static public byte TF { get; set; } = new byte();
        /// <summary>
        /// 保安装置の電源投入状況
        /// </summary>
        static public byte Power { get; set; } = new byte();
        /// <summary>
        /// パターン発生TF
        /// </summary>
        static public byte Pattern_Created { get; set; } = new byte();
        /// <summary>
        /// パターン接近TF
        /// </summary>
        static public byte Pattern_Approaching { get; set; } = new byte();
        /// <summary>
        /// ブレーキ動作TF
        /// </summary>
        static public byte Brake_Working { get; set; } = new byte();
        /// <summary>
        /// ブレーキ開放TF
        /// </summary>
        static public byte Brake_Release { get; set; } = new byte();
        /// <summary>
        /// 「ATS-Ps」表示TF
        /// </summary>
        static public byte ATS_Ps_Lump { get; set; } = new byte();
        /// <summary>
        /// 故障TF
        /// </summary>
        static public byte Broken { get; set; } = new byte();

        /// <summary>
        /// 通停判別
        /// 0で無効 1なら停車 2なら通過
        /// </summary>
        static public byte Pass_Stop { get; set; } = new byte();
        /// <summary>
        /// パターン速度情報
        /// </summary>
        static public float Pattern_Speed { get; set; } = new float();
      }
      /// <summary>
      /// ATS-PTに関する情報 番号:13
      /// </summary>
      static public class ATS_PT
      {
        /// <summary>
        /// 保安装置が使用できるかどうか
        /// </summary>
        static public byte TF { get; set; } = new byte();
        /// <summary>
        /// 保安装置の電源投入状況
        /// </summary>
        static public byte Power { get; set; } = new byte();
        /// <summary>
        /// パターン接近TF
        /// </summary>
        static public byte Pattern_Approaching { get; set; } = new byte();
        /// <summary>
        /// ブレーキ動作TF
        /// </summary>
        static public byte Brake_Working { get; set; } = new byte();
        /// <summary>
        /// ブレーキ開放TF
        /// </summary>
        static public byte Brake_Release { get; set; } = new byte();
        /// <summary>
        /// 「ATS-PT」表示TF
        /// </summary>
        static public byte ATS_PT_Lump { get; set; } = new byte();
        /// <summary>
        /// 故障TF
        /// </summary>
        static public byte Broken { get; set; } = new byte();

        /// <summary>
        /// 通停判別
        /// 0で無効 1なら停車 2なら通過
        /// </summary>
        static public byte Pass_Stop { get; set; } = new byte();
        /// <summary>
        /// パターン速度情報
        /// </summary>
        static public float Pattern_Speed { get; set; } = new float();
      }
      /// <summary>
      /// ATS-PFに関する情報 番号:14
      /// </summary>
      static public class ATS_PF
      {
        /// <summary>
        /// 保安装置が使用できるかどうか
        /// </summary>
        static public byte TF { get; set; } = new byte();
        /// <summary>
        /// 保安装置の電源投入状況
        /// </summary>
        static public byte Power { get; set; } = new byte();
        /// <summary>
        /// パターン発生TF
        /// </summary>
        static public byte Pattern_Created { get; set; } = new byte();
        /// <summary>
        /// パターン接近TF
        /// </summary>
        static public byte Pattern_Approaching { get; set; } = new byte();
        /// <summary>
        /// ブレーキ動作TF
        /// </summary>
        static public byte Brake_Working { get; set; } = new byte();
        /// <summary>
        /// ブレーキ開放TF
        /// </summary>
        static public byte Brake_Release { get; set; } = new byte();
        /// <summary>
        /// 「ATS-PF」表示TF
        /// </summary>
        static public byte ATS_PF_Lump { get; set; } = new byte();
        /// <summary>
        /// 故障TF
        /// </summary>
        static public byte Broken { get; set; } = new byte();

        /// <summary>
        /// 通停判別
        /// 0で無効 1なら停車 2なら通過
        /// </summary>
        static public byte Pass_Stop { get; set; } = new byte();
        /// <summary>
        /// パターン速度情報
        /// </summary>
        static public float Pattern_Speed { get; set; } = new float();
      }
      //番号15は予備

      /// <summary>
      /// ATS-Dnに関する情報(準備中) 番号:16
      /// </summary>
      static public class ATS_Dn
      {

      }
      /// <summary>
      /// ATS-DKに関する情報(準備中) 番号:17
      /// </summary>
      static public class ATS_DK
      {

      }
      /// <summary>
      /// ATS-DFに関する情報(準備中) 番号:18
      /// </summary>
      static public class ATS_DF
      {

      }
      /// <summary>
      /// ATS-DWs(D-TAS)に関する情報(準備中) 番号:19
      /// </summary>
      static public class ATS_DWs
      {

      }
      //番号20は予備
      /// <summary>
      /// 1型ATCに関する情報(準備中) 番号:21
      /// </summary>
      static public class ATC_1
      {

      }
      /// <summary>
      /// 2型ATCに関する情報(準備中) 番号:22
      /// </summary>
      static public class ATC_2
      {

      }
      /// <summary>
      /// 3型ATCに関する情報(準備中) 番号:23
      /// </summary>
      static public class ATC_3
      {

      }
      /// <summary>
      /// 4型ATCに関する情報 番号:24
      /// </summary>
      static public class ATC_4
      {
        /// <summary>
        /// 保安装置が使用できるかどうか
        /// </summary>
        static public byte TF { get; set; } = new byte();
      }
      /// <summary>
      /// 5型ATCに関する情報(準備中) 番号:25
      /// </summary>
      static public class ATC_5
      {

      }
      /// <summary>
      /// 6型ATCに関する情報 番号:26
      /// </summary>
      static public class ATC_6
      {
        /// <summary>
        /// 保安装置が使用できるかどうか
        /// </summary>
        static public byte TF { get; set; } = new byte();
      }
      /// <summary>
      /// 9型ATCに関する情報(準備中) 番号:27
      /// </summary>
      static public class ATC_9
      {

      }
      /// <summary>
      /// 10型ATCに関する情報(準備中) 番号:28
      /// </summary>
      static public class ATC_10
      {

      }
      //番号29は予備
      /// <summary>
      /// ATC-Lに関する情報(準備中) 番号:30
      /// </summary>
      static public class ATC_L
      {

      }
      /// <summary>
      /// DS-ATCに関する情報(準備中) 番号;31
      /// </summary>
      static public class DS_ATC
      {

      }
      /// <summary>
      /// RS-ATCに関する情報(準備中) 番号:32
      /// </summary>
      static public class RS_ATC
      {

      }
      /// <summary>
      /// D-ATCに関する情報 番号:33
      /// </summary>
      static public class D_ATC
      {

      }
      /// <summary>
      /// ATC-NSに関する情報(準備中) 番号:34
      /// </summary>
      static public class ATC_NS
      {

      }
      /// <summary>
      /// KS_ATCに関する情報(準備中) 番号:35
      /// </summary>
      static public class KS_ATC
      {

      }
      //番号36-39は予備

      /// <summary>
      /// 名鉄ATSに関する情報(準備中) 番号:40
      /// </summary>
      static public class M_ATS
      {

      }
      /// <summary>
      /// 京王ATSに関する情報(準備中) 番号:41
      /// </summary>
      static public class KO_ATS
      {

      }
      /// <summary>
      /// OM-ATSに関する情報(準備中) 番号:42
      /// </summary>
      static public class OM_ATS
      {

      }
      /// <summary>
      /// 相鉄ATSに関する情報(準備中) 番号:43
      /// </summary>
      static public class SO_ATS
      {

      }
      /// <summary>
      /// T形ATS(東武TSP式)に関する情報(準備中) 番号:44
      /// </summary>
      static public class T_ATS
      {

      }
      /// <summary>
      /// 阪急ATSに関する情報(準備中) 番号:45
      /// </summary>
      static public class HQ_ATS
      {

      }
      /// <summary>
      /// 1号型ATSに関する情報(準備中) 番号:46
      /// </summary>
      static public class ATS_1
      {

      }
      /// <summary>
      /// C-ATSに関する情報(準備中) 番号:47
      /// </summary>
      static public class C_ATS
      {

      }
      /// <summary>
      /// i-ATSに関する情報(準備中) 番号:48
      /// </summary>
      static public class I_ATS
      {

      }
      /// <summary>
      /// K-ATSに関する情報(準備中) 番号:49
      /// </summary>
      static public class K_ATS
      {

      }
      /// <summary>
      /// D-ATS-Pに関する情報(準備中) 番号:50
      /// </summary>
      static public class D_ATS_P
      {

      }
      /// <summary>
      /// ATS-SPに関する情報(準備中) 番号:51
      /// </summary>
      static public class ATS_SP
      {

      }
      /// <summary>
      /// ATS-PNに関する情報(準備中) 番号:52
      /// </summary>
      static public class ATS_PN
      {

      }
      //53-59は予備

      /// <summary>
      /// (新)CS-ATCに関する情報(準備中) 番号:60
      /// </summary>
      static public class CS_ATC
      {

      }
      /// <summary>
      /// ATC-Pに関する情報(準備中) 番号:61
      /// </summary>
      static public class ATC_P
      {

      }
      /// <summary>
      /// 京王ATCに関する情報(準備中) 番号:62
      /// </summary>
      static public class KO_ATC
      {

      }
      /// <summary>
      /// T-DATCに関する情報(準備中) 番号:63
      /// </summary>
      static public class T_DATC
      {

      }
      /// <summary>
      /// WS-ATCに関する情報(準備中) 番号:64
      /// </summary>
      static public class WS_ATC
      {

      }
      //番号65-69は予備

      /// <summary>
      /// ATACSに関する情報(準備中) 番号:70
      /// </summary>
      static public class ATACS
      {

      }
      /// <summary>
      /// JRのTASCに関する情報(準備中) 番号:71
      /// </summary>
      static public class J_TASC
      {

      }
      //以降255まで予備
    }
  }
}

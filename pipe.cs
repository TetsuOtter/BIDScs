using System;
using System.Net;
using System.Windows;
using System.Windows.Threading;
using System.IO.Pipes;
using System.Windows.Forms;
using System.Collections.Generic;

namespace BIDScs
{
  public class Pipe
  {
    //1.新しいスレッドを建てる
    //2.パイプ通信を開始する
    //3.値に変化があればイベント実行
    //4.ループ
    //5.パイプを閉じる
    const string PipeName = "BIDSPipe";

    public delegate void DataChange(object sender, int[] e);
    public delegate void SpecChange(object sender, Spec e);

    public delegate void BrakeHandleChange(object sender, int e);
    public delegate void NotchHandleChange(object sender, int e);
    public delegate void LeverHandleChange(object sender, int e);
    public delegate void ConstSwitchChange(object sender, int e);

    public delegate void LocationChange(object sender, double e);
    public delegate void SpeedChange(object sender, float e);
    public delegate void TimeChange(object sender, int e);
    public delegate void CurrentChange(object sender, float e);
    public delegate void DoorChange(object sender, bool e);

    public delegate void BCPresChange(object sender, float e);
    public delegate void MRPresChange(object sender, float e);
    public delegate void ERPresChange(object sender, float e);
    public delegate void BPPresChange(object sender, float e);
    public delegate void SAPPresChange(object sender, float e);

    public delegate void PanelChange(object sender, int[] e);
    public delegate void SoundChange(object sender, int[] e);

    public delegate void KeysChange(object sender, bool[] e);
    public delegate void HornChange(object sender, int[] e);
    public delegate void BeaconChange(object sender, double[,] e);
    public delegate void SignalChange(object sender, int e);

    public event DataChange DataChangeEvent;
    public event SpecChange SpecChangeEvent;

    public event BrakeHandleChange BrakeHandleChangeEvent;
    public event NotchHandleChange NotchHandleChangeEvent;
    public event LeverHandleChange LeverHandleChangeEvent;
    public event ConstSwitchChange ConstSwitchChangeEvent;

    public event LocationChange LocationChangeEvent;
    public event SpeedChange SpeedChangeEvent;
    public event TimeChange TimeChangeEvent;
    public event CurrentChange CurrentChangeEvent;
    public event DoorChange DoorChangeEvent;

    public event BCPresChange BCPresChangeEvent;
    public event MRPresChange MRPresChangeEvent;
    public event ERPresChange ERPresChangeEvent;
    public event BPPresChange BPPresChangeEvent;
    public event SAPPresChange SAPPresChangeEvent;

    public event PanelChange PanelChangeEvent;
    public event SoundChange SoundChangeEvent;

    public event KeysChange KeysChangeEvent;
    public event HornChange HornChangeEvent;
    public event BeaconChange BeaconChangeEvent;
    public event SignalChange SignalChangeEvent;

    private DispatcherTimer timer = new DispatcherTimer();

    private int[] OldData = { 0 };
    private Spec OldSpec = new Spec();
    private int OldBrakeHandle = 0;
    private int OldNotchHandle = 0;
    private int OldLeverHandle = 0;
    private int OldConstSwitch = 0;
    private double OldLocation = 0;
    private float OldSpeed = 0;
    private int OldTime = 0;
    private float OldCurrent = 0;
    private bool OldDoor = false;
    private float OldBCPres = 0;
    private float OldMRPres = 0;
    private float OldERPres = 0;
    private float OldBPPres = 0;
    private float OldSAPPres = 0;
    private int[] OldPanel = { 0 };
    private int[] OldSound = { 0 };
    private bool[] OldKeys = { false };
    private int[] OldHorn = { 0 };
    private double[,] OldBeacon = new double[10,7];
    private int OldSignal = 0;

    private int[] Data = { 0 };
    private Spec Spec = new Spec();
    private int BrakeHandle = 0;
    private int NotchHandle = 0;
    private int LeverHandle = 0;
    private int ConstSwitch = 0;
    private double Location = 0;
    private float Speed = 0;
    private int Time = 0;
    private float Current = 0;
    private bool Door = false;
    private float BCPres = 0;
    private float MRPres = 0;
    private float ERPres = 0;
    private float BPPres = 0;
    private float SAPPres = 0;
    private int[] Panel = { 0 };
    private int[] Sound = { 0 };
    private bool[] Keys = { false };
    private int[] Horn = { 0 };
    private double[,] Beacon = new double[10, 7];
    private int Signal = 0;
    private static string SVPCName = ".";
    /// <summary>
    /// BVEとの通信を開始する。
    /// </summary>
    public void Start()
    {
      NumClear();
      timer.Tick += Timer_Tick;
      timer.Interval = new TimeSpan(0, 0, 0, 0, 5);
      timer.Start();
    }
    /// <summary>
    /// BVEとの通信を開始する。
    /// </summary>
    /// <param name="PCName">BVEを起動しているパソコンの名前</param>
    public void Start(string PCName)
    {
      NumClear();
      if (PCName != null || PCName == "")
      {
        SVPCName = PCName;
      }
      timer.Tick += Timer_Tick;
      timer.Interval = new TimeSpan(0, 0, 0, 0, 5);
      timer.Start();
    }
    /// <summary>
    /// BVEとの通信を終了する。
    /// </summary>
    public void Stop()
    {
      timer.Stop();
      timer.Tick -= Timer_Tick;
      NumClear();
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
      EventTF();
    }
    //https://docs.microsoft.com/ja-jp/dotnet/standard/io/how-to-use-named-pipes-for-network-interprocess-communication#robust-programming
    
    NamedPipeClientStream NPCS =
      new NamedPipeClientStream(SVPCName, PipeName,
                        PipeDirection.InOut, PipeOptions.None);
    private void PipeStart()
    {
      if (NPCS.IsConnected == false)
      {
        while (true)
        {
          try
          {
            NPCS.ConnectAsync(5000);
          }
          catch (Exception e)
          {
            DialogResult DR = MessageBox.Show(e.Message, "BIDSシステム", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
            if (DR == DialogResult.Cancel)
            {
              break;
            }
          }
        }
      }
      else
      {
        MessageBox.Show("既に接続されています。", "BIDSシステム", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }
    private Byte[] rcvBytes = new Byte[2128];

    private void PipeGet()
    {
      try
      {
        NPCS.BeginRead(rcvBytes, 0, 2128, ByteTo, null);
      }catch(Exception e)
      {
        MessageBox.Show(e.Message, "BIDSシステム", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }
    private int[] hornblow = new int[3];
    private string udpRcvText;
    private void ByteTo(IAsyncResult ar)
    {
      NPCS.EndRead(ar);
      Array.Reverse(rcvBytes);
      string[] data = { "" };
      udpRcvText = BitConverter.ToString(rcvBytes);
      string rcvText = BitConverter.ToString(rcvBytes);
      foreach (var n in SubstringAtCount(rcvText))
      {
        rcvText = rcvText.Replace("-", "");
        data = SubstringAtCount(rcvText);
      }
      Array.Reverse(data);
      for (int i = 0; i < 532; i++)
      {
        Data[i] = Convert.ToInt32(data[i], 16);
      }

      switch (Data[0])
      {
        case 0:
          break;
        case 1:
          //Dispose
          break;
        case 2:
          //Elapse
          for (int i = 0; i < 256; i++)
          {
            Panel[i] = Data[i + 19];
            Sound[i] = Data[i + 19 + 256];
          }
          for (int i = 0; i < 3; i++)
          {
            if (Horn[i] != 0 && hornblow[i] < 5)
            {
              hornblow[i]++;
            }
            else
            {
              Horn[i] = 0;
              hornblow[i] = 0;
            }
          }
          break;
        case 3:
          //KeyDown
          Keys[Data[19]] = true;
          break;
        case 4:
          //KeyUp
          Keys[Data[19]] = false;
          break;
        case 5:
          //HornBlow
          Horn[Data[19]]++;
          break;
        case 6:
          //SetSignal
          Signal = Data[19];
          break;
        case 7:
          //SetBeaconData
          for (int i = 9; i >= 0; i--)
          {
            for (int b = 0; b < 6; b++)
            {
              if (i > 0)
              {
                Beacon[i - 1, b] = Beacon[i, b];
              }
              else
              {
                Beacon[0, b] = Data[b + 19];
              }
            }
          }
          Beacon[0, 5] = Data[12];//Time
          Beacon[0, 6] = Data[10];//Distance
          break;
      }
      NumSet();
    }

    private void PipeSend()
    {

    }
    private void PipeEnd()
    {
      while (true) {
        try
        {
          NPCS.Close();
        }
        catch(Exception e)
        {
          DialogResult DR = MessageBox.Show(e.Message, "BIDSシステム", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
          if (DR == DialogResult.Cancel)
          {
            break;
          }
        }
      }
    }

    private void NumSet()
    {
      Spec.ATS = Data[4];
      Spec.B67 = Data[2];
      Spec.Brake = Data[1];
      Spec.Car = Data[5];
      Spec.Power = Data[3];
      BrakeHandle = Data[6];
      NotchHandle = Data[7];
      LeverHandle = Data[8];
      ConstSwitch = Data[9];
      Location = Data[10] / 1000;
      Speed = Data[11] / 1000;
      Time = Data[12];
      Current = Data[18] / 100000;
      if (Data[531] != 1)
      {
        Door = false;
      }
      else
      {
        Door = true;
      }
      BCPres = Data[13] / 1000000;
      MRPres = Data[14] / 1000000;
      ERPres = Data[15] / 1000000;
      BPPres = Data[16] / 1000000;
      SAPPres = Data[17] / 1000000;
    }
    private void NumClear()
    {
      OldSpec.ATS = 0;
      OldSpec.B67 = 0;
      OldSpec.Brake = 0;
      OldSpec.Car = 0;
      OldSpec.Power = 0;
      OldBrakeHandle = 0;
      OldNotchHandle = 0;
      OldLeverHandle = 0;
      OldConstSwitch = 0;
      OldLocation = 0;
      OldSpeed = 0;
      OldTime = 0;
      OldCurrent = 0;
      OldDoor = false;
      OldBCPres = 0;
      OldMRPres = 0;
      OldERPres = 0;
      OldBPPres = 0;
      OldSAPPres = 0;
      OldSignal = 0;
      for (int i = 0; i < 256; i++) {
        OldPanel[i] = 0;
        OldSound[i] = 0;
        Panel[i] = 0;
        Sound[i] = 0;
        if (i < 16) {
          OldKeys[i] = false;
          Keys[i] = false;
        }
        if (i < 3) {
          OldHorn[i] = 0;
          Horn[i] = 0;
        }
        if (i < 10)
        {
          OldBeacon[i, 0] = 0;
          OldBeacon[i, 1] = 0;
          OldBeacon[i, 2] = 0;
          OldBeacon[i, 3] = 0;
          OldBeacon[i, 4] = 0;
          OldBeacon[i, 5] = 0;
          OldBeacon[i, 6] = 0;
          Beacon[i, 0] = 0;
          Beacon[i, 1] = 0;
          Beacon[i, 2] = 0;
          Beacon[i, 3] = 0;
          Beacon[i, 4] = 0;
          Beacon[i, 5] = 0;
          Beacon[i, 6] = 0;
        }
      }
      SVPCName = ".";
      Spec.ATS = 0;
      Spec.B67 = 0;
      Spec.Brake = 0;
      Spec.Car = 0;
      Spec.Power = 0;
      BrakeHandle = 0;
      NotchHandle = 0;
      LeverHandle = 0;
      ConstSwitch = 0;
      Location = 0;
      Speed = 0;
      Time = 0;
      Current = 0;
      Door = false;
      BCPres = 0;
      MRPres = 0;
      ERPres = 0;
      BPPres = 0;
      SAPPres = 0;
      Signal = 0;
    }
    private void EventTF()
    {
      if (Data != OldData)
      {
        DataChangeEvent?.Invoke(this, Data);
        OldData = Data;
      }
      if (Location != OldLocation)
      {
        LocationChangeEvent?.Invoke(this, Location);
        OldLocation = Location;
      }
      if (Spec != OldSpec)
      {
        SpecChangeEvent?.Invoke(this, Spec);
        OldSpec = Spec;
      }
      if (BrakeHandle != OldBrakeHandle)
      {
        BrakeHandleChangeEvent?.Invoke(this, BrakeHandle);
        OldBrakeHandle = BrakeHandle;
      }
      if (NotchHandle != OldNotchHandle)
      {
        NotchHandleChangeEvent?.Invoke(this, NotchHandle);
        OldNotchHandle = NotchHandle;
      }
      if (LeverHandle != OldLeverHandle)
      {
        LeverHandleChangeEvent?.Invoke(this, LeverHandle);
        OldLeverHandle = LeverHandle;
      }
      if (ConstSwitch != OldConstSwitch)
      {
        ConstSwitchChangeEvent?.Invoke(this, ConstSwitch);
        OldConstSwitch = ConstSwitch;
      }
      if (Speed != OldSpeed)
      {
        SpeedChangeEvent?.Invoke(this, Speed);
        OldSpeed = Speed;
      }
      if (Time != OldTime)
      {
        TimeChangeEvent?.Invoke(this, Time);
        OldTime = Time;
      }
      if (Current != OldCurrent)
      {
        CurrentChangeEvent?.Invoke(this, Current);
        OldCurrent = Current;
      }
      if (Door != OldDoor)
      {
        DoorChangeEvent?.Invoke(this, Door);
        OldDoor = Door;
      }
      if (BCPres != OldBCPres)
      {
        BCPresChangeEvent?.Invoke(this, BCPres);
        OldBCPres = BCPres;
      }
      if (MRPres != OldMRPres)
      {
        MRPresChangeEvent?.Invoke(this, MRPres);
        OldMRPres = MRPres;
      }
      if (ERPres != OldERPres)
      {
        ERPresChangeEvent?.Invoke(this, ERPres);
        OldERPres = ERPres;
      }
      if (BPPres != OldBPPres)
      {
        BPPresChangeEvent?.Invoke(this, BPPres);
        OldBPPres = BPPres;
      }
      if (SAPPres != OldSAPPres)
      {
        SAPPresChangeEvent?.Invoke(this, SAPPres);
        OldSAPPres = SAPPres;
      }
      if (Panel != OldPanel)
      {
        PanelChangeEvent?.Invoke(this, Panel);
        OldPanel = Panel;
      }
      if (Sound != OldSound)
      {
        SoundChangeEvent?.Invoke(this, Sound);
        OldSound = Sound;
      }
      if (Keys != OldKeys)
      {
        KeysChangeEvent?.Invoke(this, Keys);
        OldKeys = Keys;
      }
      if (Horn != OldHorn)
      {
        HornChangeEvent?.Invoke(this, Horn);
        OldHorn = Horn;
      }
      if (Beacon != OldBeacon)
      {
        BeaconChangeEvent?.Invoke(this, Beacon);
        OldBeacon = Beacon;
      }
      if (Signal != OldSignal)
      {
        SignalChangeEvent?.Invoke(this, Signal);
        OldSignal = Signal;
      }
    }

    private string[] SubstringAtCount(string self)
    {
      //参考 http://baba-s.hatenablog.com/entry/2015/03/19/140748
      var result = new List<string>();
      var length = (int)Math.Ceiling((double)self.Length / 8);

      for (int i = 0; i < length; i++)
      {
        int start = 8 * i;
        if (self.Length <= start)
        {
          break;
        }
        if (self.Length < start + 8)
        {
          result.Add(self.Substring(start));
        }
        else
        {
          result.Add(self.Substring(start, 8));
        }
      }
      return result.ToArray();
    }

  }




  /// <summary>
  /// 車両のスペックに関する情報
  /// </summary>
  public class Spec : EventArgs
  {
    /// <summary>
    /// ブレーキ段数
    /// </summary>
    public int Brake;

    /// <summary>
    /// 67度にあたるブレーキ位置(段数)
    /// </summary>
    public int B67;

    /// <summary>
    /// 力行段数
    /// </summary>
    public int Power;

    /// <summary>
    /// ATS確認ができる最低の段数
    /// </summary>
    public int ATS;

    /// <summary>
    /// 車両両数
    /// </summary>
    public int Car;
  }
}

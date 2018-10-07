using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TR.BIDScs;
namespace BIDScsConsoleTest
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("**********************************");
      Console.WriteLine("BIDScs テストアプリ by Tetsu Otter");
      Console.WriteLine("Version : 100");
      Console.WriteLine("つかいかた");
      Console.WriteLine("1. 14,15,16 のいずれかの数字を入力");
      Console.WriteLine("2. 情報が表示される");
      Console.WriteLine("3. 上記を繰り返す");
      Console.WriteLine("4. exitを入力して終了する");
      Console.WriteLine("**********************************");
      using (Pipe p = new Pipe())
      {
        p.Open();
        while (true)
        {
          switch (Console.ReadLine())
          {
            case "14":
              Console.WriteLine("Spec Info");
              Pipe.SpecData s = new Pipe.SpecData();
              s = p.GetSpec();
              string ws =
                "B:" + s.Brake.ToString() +
                ", P:" + s.Power.ToString() +
                ", ATS:" + s.ATSCheck.ToString() +
                ", B67:" + s.B67.ToString() +
                ", Cars:" + s.CarNum.ToString();
              Console.WriteLine(ws);
              break;
            case "15":
              Console.WriteLine("State1 Info");
              Pipe.StateData t1 = new Pipe.StateData();
              t1 = p.GetState();
              string ws1 =
                "Z:" + t1.Location.ToString("F") +
                ", V" + t1.Speed.ToString("F") +
                ", I:" + t1.Current.ToString("F") +
                ", B" + t1.BrakeNotch.ToString() +
                ", P:" + t1.PowerNotch.ToString() +
                ", R:" + t1.Reverser.ToString();
              Console.WriteLine(ws1);
              break;
            case "16":
              Console.WriteLine("State2 Info");
              Pipe.State2Data t2 = new Pipe.State2Data();
              t2 = p.GetState2();
              string ws2 =
                "BC:" + t2.BC.ToString("F") +
                ", MR:" + t2.MR.ToString("F") +
                ", ER:" + t2.ER.ToString("F") +
                ", BP" + t2.BP.ToString("F") +
                ", SAP:" + t2.SAP.ToString("F") +
                ", 閉扉:" + t2.IsDoorClosed.ToString() +
                ", 時刻=" + t2.Hour.ToString() + ":" + t2.Minute.ToString() + ":" + t2.Second.ToString() + "." + t2.Millisecond.ToString();
              Console.WriteLine(ws2);
              break;
            case "exit":
              return;
            default:
              Console.WriteLine("14,15,16 のいずれかの数字を入力してください。終了時はexitを入力してください。");
              break;
          }
        }
      }
    }
  }
}

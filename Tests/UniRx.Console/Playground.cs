using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace UniRx
{
    public class Playground
    {
        public void Run()
        {



            new Aiueo.Kakikukeko.Sasisuseso.HogeHoge.HugaHuga.TakoTako().Hoge();


        }
    }
}


namespace Aiueo.Kakikukeko.Sasisuseso
{
    public class HogeHoge
    {
        public class HugaHuga
        {
            public class TakoTako
            {
                public void Hoge()
                {
                    Console.WriteLine(new StackTrace(true).ToString());
                    // Debug.Log("hogehoge");
                }
            }
        }
    }
}

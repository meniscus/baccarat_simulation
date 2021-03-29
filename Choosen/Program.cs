using System;
using System.Collections.Generic;

namespace Choosen
{
    class Program
    {
        static void Main(string[] args)
        {
            var c = new Choosen();
            //Console.WriteLine($"{10}ドルベット");
            //for(var i = 0; i < 5; i++) { 
            //    var betPrice = new Choosen.BetPrice()
            //    {
            //        Banker = 6,
            //        Player = 0,
            //        Tie = 2,
            //        BankerPair = 1,
            //        PlayerPair = 1,
            //    };
            //    c.Execute(betPrice);
            //}

            //Console.WriteLine($"{20}ドルベット");
            //for (var i = 0; i < 5; i++)
            //{
            //    var betPrice = new Choosen.BetPrice()
            //    {
            //        Banker = 13,
            //        Player = 0,
            //        Tie = 3,
            //        BankerPair = 2,
            //        PlayerPair = 2,
            //    };
            //    c.Execute(betPrice);
            //}

            Console.WriteLine($"{30}ドルベット");
            for (var i = 0; i < 5; i++)
            {
                var betPrice = new Choosen.BetPrice()
                {
                    Banker = 19,
                    Player = 0,
                    Tie = 5,
                    BankerPair = 3,
                    PlayerPair = 3,
                };
                c.Execute(betPrice);
            }
        }
    }
}

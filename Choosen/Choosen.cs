using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace Choosen
{
    

    class Choosen
    {
        class Probabilities
        {
            public int p_hit { get; set; }
            public int p_lose
            {
                get { return p_total - p_hit; }
            }

            public int p_total { get; set; }

            private string type { get; set; }

            public Probabilities()
            {

            }
        }

        public class BetPrice
        {
            public double Banker { get; set; }
            public double Player { get; set; }
            public double Tie { get; set; }
            public double BankerPair { get; set; }
            public double PlayerPair { get; set; }
            public double TotalBet
            {
                get
                {
                    return Banker + Player + Tie + BankerPair + PlayerPair;
                }
            }

            public override string ToString()
            {
                return $"ベット額:{TotalBet}  Banker:{Banker},Player:{Player},Tie:{Tie},BankerPair:{BankerPair},PlayerPair:{PlayerPair}";
            }

        }

        enum ResultType
        {
            BankerWin,
            PlayerWin,
            Tie
        }

        enum OptionalResultType
        {
            NoPair,
            BankerPair,
            PlayerPair,
            EitherPair
        }

        Probabilities p_banker = new Probabilities()
        {
            p_hit = 458597,
            p_total = 1000000
        };
        Probabilities p_player = new Probabilities()
        {
            p_hit = 446247,
            p_total = 1000000
        };
        Probabilities p_tie = new Probabilities()
        {
            p_hit = 95156,
            p_total = 1000000
        };

        Probabilities p_banker_pair = new Probabilities()
        {
            p_hit = 588,
            p_total = 10000
        };
        Probabilities p_player_pair = new Probabilities()
        {
            p_hit = 588,
            p_total = 10000
        };

        

        List<ResultType> BattleBox = new List<ResultType>();
        List<OptionalResultType> OptionalBox = new List<OptionalResultType>();

        public static readonly int FINISH_CONSECUTIVE_WINS_COUNT = 5;


        public Choosen()
        {
            var battleBox = new List<ResultType>();
            
            for (var i = 0; i < this.p_banker.p_total;i++)
            {
                // 厳密じゃないけどまあいいか
                if (i < this.p_banker.p_hit)
                {
                    battleBox.Add(ResultType.BankerWin);
                }

                if (i < this.p_player.p_hit)
                {
                    battleBox.Add(ResultType.PlayerWin);
                }

                if (i < this.p_tie.p_hit)
                {
                    battleBox.Add(ResultType.Tie);
                }
            }

            var optionalBox = new List<OptionalResultType>();
            for (var i = 0; i < p_banker_pair.p_total; i++)
            {
                // 一旦共通で使うことにする。2回抽選すればよろし
                if (i < this.p_banker_pair.p_hit)
                {
                    optionalBox.Add(OptionalResultType.BankerPair);
                }
                else
                {
                    optionalBox.Add(OptionalResultType.NoPair);
                }
            }


            this.BattleBox = battleBox;
            this.OptionalBox = optionalBox;
        }

        public void Execute(BetPrice betPrice)
        {
            var totalEventCost = 0d;
            var tryCount = 1000000;
            var gameCountDic = new Dictionary<int, int>();

            for (var i = 0; i < tryCount; i++)
            {
                var consecutiveWinsCount = 0;
                var cost = 0d;
                var gameCount = 0;

                while (true)
                {
                    cost -= betPrice.TotalBet;
                    var reward = 0.0;
                    gameCount++;

                    var index = RandomNumberGenerator.GetInt32(BattleBox.Count);
                    
                    var result = this.BattleBox[index];

                    if (result == ResultType.BankerWin)
                    {
                        reward += betPrice.Banker * Constants.GAME_ODDS.BANKER;
                    }

                    if (result == ResultType.PlayerWin)
                    {
                        reward += betPrice.Player * Constants.GAME_ODDS.PLAYER;
                    }

                    if (result == ResultType.Tie)
                    {
                        reward += betPrice.Banker + betPrice.Player + betPrice.Tie * Constants.GAME_ODDS.TIE;
                    }

                    index = RandomNumberGenerator.GetInt32(OptionalBox.Count);
                    var opResult1 = this.OptionalBox[index];
                    index = RandomNumberGenerator.GetInt32(OptionalBox.Count);
                    var opResult2 = this.OptionalBox[index];

                    if (opResult1 == OptionalResultType.BankerPair)
                    {
                        reward += betPrice.BankerPair * Constants.GAME_ODDS.BANKER_PAIR;
                    }

                    if (opResult2 == OptionalResultType.BankerPair)
                    {
                        reward += betPrice.PlayerPair * Constants.GAME_ODDS.BANKER_PAIR;
                    }

                    if (betPrice.TotalBet < reward)
                    {
                        // 勝利
                        consecutiveWinsCount += 1;
                    }

                    cost += reward;

                    var logStr = $"{result} {opResult1} {opResult2} 連勝数:{consecutiveWinsCount} 収支:{cost}";
                    //Console.WriteLine(logStr);

                    if (consecutiveWinsCount >= FINISH_CONSECUTIVE_WINS_COUNT)
                    {
                        break;
                    }
                }

                var resultStr = $"★{FINISH_CONSECUTIVE_WINS_COUNT}連勝達成 ゲーム数:{gameCount} プレイ収支:{cost} イベント収支:{cost + betPrice.TotalBet * 5}";
                //Console.WriteLine(resultStr);
                totalEventCost += cost + betPrice.TotalBet * 5;

                if (!gameCountDic.ContainsKey(gameCount))
                {
                    gameCountDic.Add(gameCount, 0);
                }

                gameCountDic[gameCount]++;
                
            }

            var s = $"{tryCount}回試行 1回あたり収支:{totalEventCost / tryCount * 1.0}";
            Console.WriteLine(s);

            Console.WriteLine("ゲーム数分布");
            foreach(KeyValuePair<int,int> kvp in gameCountDic.OrderBy(x => x.Key))
            {
                Console.WriteLine($"{kvp.Key}ゲーム : {kvp.Value}");
            }
        }

    }
}

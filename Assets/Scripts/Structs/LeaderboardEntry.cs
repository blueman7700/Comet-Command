using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Structs
{
    class LeaderboardEntry
    {
        public LeaderboardEntry(string name, int score)
        {
            Name = name;
            Score = score;
        }

        public string Name { get; private set; }

        public int Score { get; private set; }
    }
}

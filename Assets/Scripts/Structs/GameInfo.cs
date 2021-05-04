using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Structs
{
    [Serializable]
    [CreateAssetMenu(fileName = "Data", menuName = "Structs/GameInfo", order = 1)]
    public class GameInfo : ScriptableObject
    {

        public string Username { get; set; }
        public int RoundSeed { get; set; }
        public int Score { get; set; }

        public LeaderboardEntry GetStats() 
        {
            return new LeaderboardEntry(Username, Score);
        }
    }
}

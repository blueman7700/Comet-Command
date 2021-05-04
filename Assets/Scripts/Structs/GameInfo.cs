using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Structs
{
    [Serializable]
    public class GameInfo
    {
        private static GameInfo instance;

        private string username;
        private int roundSeed;
        private int score;
        private int roundNumber;
        private int saveSlot;
        private int[] cityPositions;

        public GameInfo() 
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        public GameInfo(string username, int slot, bool overwrite)
        {
            SetToDefault(username, slot);
            if (instance == null || overwrite)
            {
                instance = this;
            }
        }   


        private void SetToDefault(string username, int slot)
        {
            this.username = username;
            roundNumber = 0;
            saveSlot = slot;
            score = 0;
            cityPositions = new int[] { 0, 1, 2, 3, 4, 5};
            roundSeed = UnityEngine.Random.Range(1000, 10000);
        }

        public string Username()
        {
            if (instance == this)
            {
                return username;
            }
            else
            {
                return instance.username;
            }
        }

        public int RoundNumber() 
        {
            if (instance == this)
            {
                return roundNumber;
            }
            else
            {
                return instance.roundNumber;
            }
        }

        public int RoundSeed()
        {
            if (instance == this)
            {
                return roundSeed;
            }
            else
            {
                return instance.roundSeed;
            }
        }

        public int Score() 
        {
            if (instance == this)
            {
                return score;
            }
            else
            {
                return instance.score;
            }
        }

        public int SaveSlot()
        {
            if (instance == this)
            {
                return saveSlot;
            }
            else
            {
                return instance.saveSlot;
            }
        }

        public int[] CityPositions()
        {
            if (instance == this)
            {
                return cityPositions;
            }
            else
            {
                return instance.cityPositions;
            }
        }

        public void setSeed(int seed)
        {
            if (instance == this)
            {
                roundSeed = seed;
            }
            else
            {
                instance.roundSeed = seed;
            }
        }

        public void setScore(int score)
        {
            if (instance == this)
            {
                this.score = score;
            }
            else
            {
                instance.score = score;
            }
        }

        public void setCities(int[] positions) 
        {
            if (instance == this)
            {
                cityPositions = positions;
            }
            else
            {
                instance.cityPositions = positions;
            }
        }

        public void setRoundNumber(int num)
        {
            if (instance == this)
            {
                roundNumber = num;
            }
            else
            {
                instance.roundNumber = num;
            }
        }

        public LeaderboardEntry GetStats() 
        {
            return new LeaderboardEntry(instance.username, instance.score);
        }

        public void UpdateActiveInfo()
        {
            instance.username = this.username;
            instance.roundSeed = this.roundSeed;
            instance.score = this.score;
            instance.roundNumber = this.roundNumber;
            instance.saveSlot = this.saveSlot;
            instance.cityPositions = this.cityPositions;
        }

        public GameInfo GetInstance()
        {
            return instance;
        }
    }
}

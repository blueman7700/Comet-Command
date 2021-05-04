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
        private int money;

        private int missileSpeed;
        private int missileCount;
        private float reloadSpeed;
        private bool hasTargeting = false;

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
            money = 0;
            cityPositions = new int[] { 0, 1, 2, 3, 4, 5};
            roundSeed = UnityEngine.Random.Range(1000, 10000);

            missileSpeed = 7;
            missileCount = 30;
            reloadSpeed = 2;
            hasTargeting = false;
        }

        public string Username()
        {
            return instance.username;
        }

        public int RoundNumber() 
        {
            return instance.roundNumber;
            
        }

        public int RoundSeed()
        {
            return instance.roundSeed;
        }

        public int Score() 
        {
            return instance.score;
        }

        public int SaveSlot()
        {
            return instance.saveSlot;
        }

        public int[] CityPositions()
        {
            return instance.cityPositions;
        }

        public int Money()
        {
            return instance.money;
        }

        public int MissileSpeed()
        {
            return instance.missileSpeed;
        }

        public int MissileCount()
        {
            return instance.missileCount;
        }

        public float ReloadSpeed()
        {
            return instance.reloadSpeed;
        }

        public bool HasTargeting()
        {
            return instance.hasTargeting;
        }

        public void setSeed(int seed)
        {
            instance.roundSeed = seed;
        }

        public void setMoney(int m)
        {
            instance.money = m;
        }

        public void setScore(int score)
        {
            instance.score = score;
        }

        public void setCities(int[] positions) 
        {
            instance.cityPositions = positions;
        }

        public void setRoundNumber(int num)
        {
            instance.roundNumber = num;
        }

        public void enableTargeting()
        {
            instance.hasTargeting = true;
        }

        public void setNumMissiles(int num)
        {
            instance.missileCount = num;
        }

        public void setMissileSpeed(int speed)
        {
            instance.missileSpeed = speed;
        }

        public void setReloadSpeed(float speed)
        {
            instance.reloadSpeed = speed;
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

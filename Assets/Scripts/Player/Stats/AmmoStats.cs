using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.Player
{
    // Represents stats for one ammo type
    class AmmoStats
    {
        private string name;
        private int cost;
        private int amount;
        private int maxAmount;

        public AmmoStats(string name, int cost, int amount, int maxAmount)
        {
            this.name = name;
            this.cost = cost;
            this.amount = amount;
            this.maxAmount = maxAmount;
        }

        #region getters
        public string Name => name; 
        /// <summary>
        /// Cost of ammo for some amount ("Count")
        /// </summary>
        public int Cost => cost; 
        /// <summary>
        /// Amount of ammo which can be bought for some cost ("Cost")
        /// </summary>
        public int Amount => amount; 
        public int MaxAmount => maxAmount;
        #endregion
    }
}

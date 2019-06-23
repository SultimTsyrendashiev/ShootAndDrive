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

        public AmmoStats(string name, int cost, int amount)
        {
            this.name = name;
            this.cost = cost;
            this.amount = amount;
        }

        #region getters
        public string Name { get { return name; } }
        /// <summary>
        /// Cost of ammo for some amount ("Count")
        /// </summary>
        public int Cost { get { return cost; } }
        /// <summary>
        /// Amount of ammo which can be bought for some cost ("Cost")
        /// </summary>
        public int Amount { get { return amount; } }
        #endregion
    }
}

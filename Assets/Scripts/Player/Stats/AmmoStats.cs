//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace SD.PlayerLogic
//{
//    // Represents stats for one ammo type
//    class AmmoStats
//    {
//        private string name;
//        private int price;
//        private int amount;
//        private int maxAmount;

//        public AmmoStats(string name, int cost, int amount, int maxAmount)
//        {
//            this.name = name;
//            this.price = cost;
//            this.amount = amount;
//            this.maxAmount = maxAmount;
//        }

//        #region getters
//        public string Name => name;
//        /// <summary>
//        /// Price for specified amount of ammo
//        /// </summary>
//        public int Price => price; 
//        /// <summary>
//        /// How many ammo can be bought for specified price
//        /// </summary>
//        public int AmountToBuy => amount; 
//        public int MaxAmount => maxAmount;
//        #endregion
//    }
//}

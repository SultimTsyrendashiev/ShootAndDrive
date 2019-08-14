using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.Utils
{
    /// <summary>
    /// Default event but with null check
    /// </summary>
    class SDEvent 
    {
        public int SubscribersAmount;
        public event Void Action;

        public SDEvent()
        {
            SubscribersAmount = 0;
        }

        public void Add(Void action)
        {
            SubscribersAmount++;
            Action += action;
        }

        public void Remove(Void action)
        {
            SubscribersAmount--;
            Action -= action;
        }

        public void Invoke()
        {
            if (SubscribersAmount > 0)
            {
                Action();
            }
        }
    }
}

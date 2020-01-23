using System;
using System.Collections.Generic;
using System.Linq;

namespace SD.Utils.Collections
{
    class UniqueQueue<T> : LinkedList<T>
    {
        public int MaxCapacity { get; set; }

        public UniqueQueue(int maxCapacity)
        {
            MaxCapacity = maxCapacity;
        }

        public void Push(T item)
        {
            if (!Contains(item))
            {
                AddLast(item);
            }
            else
            {
                // remove and add to the end of list
                Remove(item);
                AddLast(item);
            }

            while (Count > MaxCapacity)
            {
                // remove beginning
                Remove(First.Value);
            }
        }
    }
}

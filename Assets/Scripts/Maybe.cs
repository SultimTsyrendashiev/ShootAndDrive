using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD
{
    class Maybe<T>
    {
        public bool Exist;
        public T    Value;

        /// <summary>
        /// Default constructor.
        /// Note: 'Exist' will be 'false'
        /// </summary>
        public Maybe()
        {
            this.Exist = false;
        }

        /// <summary>
        /// Value constructor.
        /// Note: 'Exist' will be 'true'
        /// </summary>
        public Maybe(T value)
        {
            this.Exist = true;
            this.Value = value;
        }

        public Maybe(bool exist, T value)
        {
            this.Exist = exist;
            this.Value = value;
        }
    }
}

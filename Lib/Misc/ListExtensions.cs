using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Big3.Hitbase.Miscellaneous
{
    public static class ListExtensions
    {
       /* public static void Shuffle<T>(this IList<T> list)
        {
            var provider = new RNGCryptoServiceProvider();
            int n = list.Count;
            while (n > 1)
            {
                var box = new byte[1];
                do provider.GetBytes(box);
                while (!(box[0] < n * (Byte.MaxValue / n)));
                var k = (box[0] % n);
                n--;
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }*/

        /// <summary>
        /// method for shuffling a generic list of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">the list we wish to shuffle</param>
        public static void Shuffle<T>(this IList<T> list)
        {
            //generate a Random instance
            Random rnd = new Random();
            //get the count of items in the list
            int i = list.Count();
            //do we have a reference type or a value type
            T val = default(T);

            //we will loop through the list backwards
            while (i >= 1)
            {
                //decrement our counter
                i--;
                //grab the next random item from the list
                var nextIndex = rnd.Next(i, list.Count());
                val = list[nextIndex];
                //start swapping values
                list[nextIndex] = list[i];
                list[i] = val;
            }
        }
    }
}

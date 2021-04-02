using System;
using System.Collections.Generic;
using System.Text;

namespace GameUtilities.Cards
{
    public static class CardUtils
    {
        public static int GetRandomIndex<T>(this ICollection<T> col, Random rand)
        {
            return rand.Next(0, col.Count);
        }
    }
}

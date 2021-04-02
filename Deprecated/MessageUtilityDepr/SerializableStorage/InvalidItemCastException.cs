using System;

namespace MessageUtilities
{
    public class InvalidItemCastException : Exception
    {
        public string ItemName { get; }
        public InvalidItemCastException(string itemName, string msg)
            : base(msg)
        {
            ItemName = itemName;
        }
    }
}

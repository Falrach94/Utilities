using System;

namespace MessageUtilities
{
    public class ItemNotFoundException : Exception
    {
        public string Name { get; }
        public ItemNotFoundException(string name)
            : base("Item '" + name + "' not found!")
        {
            Name = name;
        }
    }
}

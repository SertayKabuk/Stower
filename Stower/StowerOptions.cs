using System;
using System.Collections.Generic;

namespace Stower
{
    public class StowerOptions
    {
        private readonly List<StackOptions> stacks = new List<StackOptions>();

        public class StackOptions
        {
            public int MaxStackLenght { get; set; }
            public int MaxWaitInSecond { get; set; }
            public Type Type { get; set; }
        }

        public void AddStack<T>(int maxStackLenght, int maxWaitInSecond)
        {
            if (maxStackLenght <= 0)
                throw new ArgumentException("MaxStackLenght more than zero!");
            if (maxWaitInSecond <= 0 || maxWaitInSecond > 50000)
                throw new ArgumentException("MaxWait must between 0 and 090909");

            stacks.Add(new StackOptions { MaxStackLenght = maxStackLenght, MaxWaitInSecond = maxWaitInSecond, Type = typeof(T) });
        }

        internal List<StackOptions> Stacks => stacks;
    }
}
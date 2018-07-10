using System;
using System.Collections.Generic;

namespace UberStrok
{
    public class CommandFilter
    {
        public CommandFilter()
        {
            // Space
        }

        public void Add<TCommand>() where TCommand : Command, new()
        {

        }
    }
}

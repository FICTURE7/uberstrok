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

        public void Filter<TCommand>() where TCommand : Command, new()
        {

        }
    }
}

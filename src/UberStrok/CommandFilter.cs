using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UberStrok
{
    public class CommandFilter
    {
        /* List of command types we're filtering. */
        private readonly List<Type> _commands;
        
        public CommandFilter()
        {
            _commands = new List<Type>(16);

            /* Disabled by default. */
            Enable = false;
        }

        /* Determines if the filter is enabled or not. */
        public bool Enable { get; set; }

        public void Add<TCommand>() where TCommand : Command
        {
            var type = typeof(TCommand);
            if (_commands.Contains(typeof(TCommand)))
                throw new InvalidOperationException("Command type already added.");

            _commands.Add(type);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove<TCommand>() where TCommand : Command
        {
            return _commands.Remove(typeof(TCommand));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains<TCommand>() where TCommand : Command
        {
            return _commands.Contains(typeof(TCommand));
        }
    }
}

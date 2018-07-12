using System;
using System.Collections.Generic;

namespace UberStrok
{
    public class CommandFilter
    {
        private readonly List<Type> _commands;
        
        public CommandFilter()
        {
            _commands = new List<Type>();
        }

        public bool Filtering { get; set; }

        public void Add<TCommand>() where TCommand : Command
        {
            var type = typeof(TCommand);
            if (_commands.Contains(typeof(TCommand)))
                throw new InvalidOperationException("Command type already added.");

            _commands.Add(type);
        }

        public bool Remove<TCommand>() where TCommand : Command
        {
            return _commands.Remove(typeof(TCommand));
        }

        public bool Contains<TCommand>() where TCommand : Command
        {
            return _commands.Contains(typeof(TCommand));
        }
    }
}

using System;
using System.Collections.Generic;

namespace UberStrok.Realtime
{
    public class StateMachine<T> where T : struct, IConvertible
    {
        private T _currentId;
        private State _current;
        private readonly Dictionary<T, State> _states;

        public StateMachine()
        {
            _states = new Dictionary<T, State>();
        }

        public T Current => _currentId;

        public void Register(T stateId, State state)
        {
            if (_states.ContainsKey(stateId))
                throw new Exception("Already contains a state handler for the specified state ID.");

            _states.Add(stateId, state);
        }

        public void Set(T stateId)
        {
            var state = default(State);
            if (!_states.TryGetValue(stateId, out state))
                throw new Exception("No state handler registered for the specified state ID.");

            _current?.OnExit();

            _current = state;
            _currentId = stateId;

            _current?.OnEnter();
        }

        public void Update()
        {
            _current?.OnUpdate();
        }
    }
}

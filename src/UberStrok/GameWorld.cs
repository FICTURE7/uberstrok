using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace UberStrok
{
    public class GameWorld : IStateMachine<GameWorldState>, ICommandDispatcher, IEventDispatcher
    {
        /* Current tick. */
        private int _tick;
        /* Current game state. */
        private GameWorldState _state;

        /* List of game objects in the game instance. */
        internal readonly GameObjectCollection _objects;
        /* Recorder to record commands received. */
        private readonly CommandRecorder _recorder;
        /* Queue of commands to be dispatched. */
        private readonly ConcurrentQueue<Command> _queue;
        /* Dictionary of type of game states to game state instances. */
        private readonly Dictionary<Type, GameWorldState> _states;

        public GameWorld()
        {
            _tick = 0;
            _state = null;
            _states = new Dictionary<Type, GameWorldState>();
            _recorder = new CommandRecorder();
            _queue = new ConcurrentQueue<Command>();
            _objects = new GameObjectCollection(this);
        }

        public int Tick => _tick;
        public CommandRecorder Recorder => _recorder;
        public GameObjectCollection Objects => _objects;

        public void ResetState()
        {
            /* TODO: Call OnEnter & OnExit and stuff. */
            _state = null;
        }

        public TGameState RegisterState<TGameState>() where TGameState : GameWorldState, new()
        {
            var type = typeof(TGameState);
            if (_states.ContainsKey(type))
                throw new InvalidOperationException("State already registered.");

            var state = new TGameState { _game = this };
            _states.Add(type, state);

            return state;
        }

        public void SetState<TGameState>() where TGameState : GameWorldState, new()
        {
            var type = typeof(TGameState);
            var state = default(GameWorldState);
            if (!_states.TryGetValue(type, out state))
                throw new InvalidOperationException("State was not registered.");

            /* TODO: Call OnEnter & OnExit and stuff. */
            _state = state;
        }

        public GameWorldState GetState() => _state;

        public void OnCommand(Command command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            command._game = this;
            command._tick = _tick;
            /* Add the command in the dispatch queue. */
            _queue.Enqueue(command);
            /* Record command, incase we want to replay it. */
            _recorder.Record(command);
        }

        public void OnEvent<TEvent>(TEvent @event) where TEvent : Event
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            /* If we are not in any state, we return early. */
            if (_state == null)
                return;

            /* TODO: Optimize this shit, cause its slow af. */
            var type = typeof(TEvent);
            var method = default(MethodInfo);
            if (!_state._onEventMethods.TryGetValue(type, out method))
                return;

            method.Invoke(_state, new object[] { @event });
        }

        public void DoTick()
        {
            /* 
                Dispatches the commands in the queue and updates all objects in the game 
                instance and the current state.
             */
            DoDispatch();
            DoUpdate();
            _tick++;
        }

        private void DoDispatch()
        {
            /* Execute each command in the command queue until queue is empty. */
            while (!_queue.IsEmpty)
            {
                var command = default(Command);
                if (_queue.TryDequeue(out command) && _state._filter.Contains(command.GetType()))
                    command.DoExecute();
            }
        }

        private void DoUpdate()
        {
            /* TODO: Avoid foreach here. */
            /* Update each game object in the game object list. */
            foreach (var obj in _objects)
            {
                if (obj.Enable)
                    obj.DoUpdate();
            }

            /* Update the current GameState as well if have any. */
            _state?.OnUpdate();
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace UberStrok
{
    public class Game
    {
        /* Current tick. */
        private int _tick;
        /* Current game state. */
        private GameState _state;

        /* List of game objects in the game instance. */
        internal readonly List<GameObject> _gameObjects;
        /* Recorder to record commands received. */
        private readonly CommandRecorder _recorder;
        /* Queue of commands to be dispatched. */
        private readonly ConcurrentQueue<Command> _queue;
        /* Dictionary of type of game states to game state instances. */
        private readonly Dictionary<Type, GameState> _states;

        public Game() : this(GameState.Empty)
        {
            // Space
        }

        public Game(GameState defaultState)
        {
            if (defaultState == null)
                throw new ArgumentNullException(nameof(defaultState));

            _tick = 0;
            _state = null;
            _states = new Dictionary<Type, GameState>();
            _recorder = new CommandRecorder();
            _queue = new ConcurrentQueue<Command>();
            _gameObjects = new List<GameObject>();
        }

        public int Tick => _tick;
        public CommandRecorder Recorder => _recorder;
        public IReadOnlyList<GameObject> Objects => _gameObjects.AsReadOnly();

        public void DoTick()
        {
            /* 
                Dispatches the commands in the queue and
                updates all objects in the game instance.
             */
            DoDispatch();
            DoUpdate();
            _tick++;
        }

        public void RegisterState<TGameState>() where TGameState : GameState, new()
        {
            var type = typeof(TGameState);
            if (_states.ContainsKey(type))
                throw new InvalidOperationException("State already registered.");

            var state = new TGameState();
            state._game = this;
            _states.Add(type, state);
        }
        
        public void SetState<TGameState>() where TGameState : GameState, new()
        {
            var type = typeof(TGameState);
            var state = default(GameState);
            if (!_states.TryGetValue(type, out state))
                throw new InvalidOperationException("State was not registered.");

            _state = state;
        }

        public GameState GetState()
        {
            return _state;
        }

        public void OnCommand(Command command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            command._tick = Tick;
            /* Add the command in the dispatch queue. */
            _queue.Enqueue(command);
            /* Record command, incase we want to replay it. */
            _recorder.Record(command);
        }

        private void DoDispatch()
        {
            /* Execute each command in the command queue until queue is empty. */
            while (!_queue.IsEmpty)
            {
                var command = default(Command);
                if (_queue.TryDequeue(out command))
                    command.DoExecute(this, null);
            }
        }

        private void DoUpdate()
        {
            /* Update each game object in the game object list. */
            for (int i = 0; i < _gameObjects.Count; i++)
                _gameObjects[i].DoUpdate();

            /* Update the current GameState as well if have any. */
            _state?.DoUpdate();
        }
    }
}

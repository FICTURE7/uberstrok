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
        private readonly GameState _state;
        /* Recorder to record commands received. */
        private readonly CommandRecorder _recorder;
        /* Queue of commands to be dispatched. */
        private readonly ConcurrentQueue<Command> _queue;
        /* List of game objects in the game instance. */
        private readonly List<GameObject> _gameObjects;

        public Game()
        {
            _tick = 0;
            _queue = new ConcurrentQueue<Command>();
            _gameObjects = new List<GameObject>();
        }

        public int Tick => _tick;
        public GameState State => _state;
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

        public void Register<TState>() 
        {

        }

        public void OnCommand(Command command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            command._tick = Tick;
            /* Add the command in the dispatch queue. */
            _queue.Enqueue(command);
            _recorder.Record(command);
        }

        private void DoDispatch()
        {
            /* Execute each command in the command queue. */
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
        }
    }
}

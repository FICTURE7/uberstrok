using NUnit.Framework;
using UberStrok.Tests.Mocks;

namespace UberStrok.Tests
{
    [TestFixture]
    public class GameWorldTests
    {
        [Test]
        public void DoTick_Tick_Increases()
        {
            var game = new GameWorld();
            Assert.That(game.Tick, Is.EqualTo(0));
            game.DoTick();
            Assert.That(game.Tick, Is.EqualTo(1));
        }

        [Test]
        public void RegisterState()
        {
            var game = new GameWorld();
            Assert.That(game.GetState(), Is.Null);

            var state = game.RegisterState<MockGameWorldState>();

            /* State not set, should return null. */
            Assert.That(game.GetState(), Is.Null);
            Assert.That(state, Is.Not.Null);
            Assert.That(state, Is.TypeOf<MockGameWorldState>());
        }

        [Test]
        public void RegisterState_State_Already_Registered_Exception()
        {
            var game = new GameWorld();
            game.RegisterState<MockGameWorldState>();
            Assert.That(() => game.RegisterState<MockGameWorldState>(), Throws.InvalidOperationException);
        }

        [Test]
        public void SetState_GetState()
        {
            var game = new GameWorld();
            game.RegisterState<MockGameWorldState>();

            Assert.That(game.GetState(), Is.Null);

            game.SetState<MockGameWorldState>();

            var state = game.GetState();
            Assert.That(state, Is.TypeOf<MockGameWorldState>());
            Assert.That(state._game, Is.EqualTo(game));
        }

        [Test]
        public void ResetState()
        {
            var game = new GameWorld();
            game.RegisterState<MockGameWorldState>();
            game.SetState<MockGameWorldState>();

            Assert.That(game.GetState(), Is.Not.Null);
            Assert.That(game.GetState(), Is.TypeOf<MockGameWorldState>());

            game.ResetState();

            Assert.That(game.GetState(), Is.Null);
        }

        [Test]
        public void OnCommand()
        {
            var game = new GameWorld();
            var command = new MockCommand();

            game.DoTick();
            game.OnCommand(command);

            Assert.That(command.Tick, Is.EqualTo(game.Tick));
        }

        [Test]
        public void OnCommand_Null_Args_Exception()
        {
            var game = new GameWorld();
            Assert.That(() => game.OnCommand(null), Throws.ArgumentNullException);
        }

        [Test]
        public void OnEvent()
        {
            var called = false;
            var game = new GameWorld();
            var mockEvent = new MockEvent();
            var state = game.RegisterState<MockGameWorldState>();
            state.OnMockEventCallback = (@event) =>
            {
                called = true;
                Assert.That(@event, Is.EqualTo(mockEvent));
            };
           
            game.SetState<MockGameWorldState>();
            game.OnEvent(mockEvent);

            Assert.That(called, Is.True);
        }

        [Test]
        public void OnEvent_StateNotSet_OnEvent_NotCalled()
        {
            var called = false;
            var game = new GameWorld();
            var mockEvent = new MockEvent();
            var state = game.RegisterState<MockGameWorldState>();

            state.OnMockEventCallback = (@event) => called = true;
            game.OnEvent(mockEvent);

            Assert.That(called, Is.False);
        }

        [Test]
        public void OnEvent_Null_Args_Exception()
        {
            var game = new GameWorld();
            Assert.That(() => game.OnEvent<MockEvent>(null), Throws.ArgumentNullException);
        }

        [Test]
        public void State_()
        {
            var game = new GameWorld();
            game.RegisterState<MockGameWorldState>();
            game.SetState<MockGameWorldState>();

            var state = game.GetState();
        }
    }
}

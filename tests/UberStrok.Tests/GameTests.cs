using NUnit.Framework;
using UberStrok.Tests.Mocks;

namespace UberStrok.Tests
{
    [TestFixture]
    public class GameTests
    {
        [Test]
        public void DoTick_Tick_Increases()
        {
            var game = new Game();
            Assert.That(game.Tick, Is.EqualTo(0));
            game.DoTick();
            Assert.That(game.Tick, Is.EqualTo(1));
        }

        [Test]
        public void RegisterState()
        {
            var game = new Game();
            Assert.That(game.GetState(), Is.Null);

            var state = game.RegisterState<MockGameState>();

            /* State not set, should return null. */
            Assert.That(game.GetState(), Is.Null);
            Assert.That(state, Is.Not.Null);
            Assert.That(state, Is.TypeOf<MockGameState>());
        }

        [Test]
        public void RegisterState_State_Already_Registered_Exception()
        {
            var game = new Game();
            game.RegisterState<MockGameState>();
            Assert.That(() => game.RegisterState<MockGameState>(), Throws.InvalidOperationException);
        }

        [Test]
        public void SetState_GetState()
        {
            var game = new Game();
            game.RegisterState<MockGameState>();

            Assert.That(game.GetState(), Is.Null);

            game.SetState<MockGameState>();

            var state = game.GetState();
            Assert.That(state, Is.TypeOf<MockGameState>());
            Assert.That(state._game, Is.EqualTo(game));
        }

        [Test]
        public void ResetState()
        {
            var game = new Game();
            game.RegisterState<MockGameState>();
            game.SetState<MockGameState>();

            Assert.That(game.GetState(), Is.Not.Null);
            Assert.That(game.GetState(), Is.TypeOf<MockGameState>());

            game.ResetState();

            Assert.That(game.GetState(), Is.Null);
        }

        [Test]
        public void OnCommand()
        {
            var game = new Game();
            var command = new MockCommand();

            game.DoTick();
            game.OnCommand(command);

            Assert.That(command.Tick, Is.EqualTo(game.Tick));
        }

        [Test]
        public void OnCommand_Null_Args_Exception()
        {
            var game = new Game();
            Assert.That(() => game.OnCommand(null), Throws.ArgumentNullException);
        }

        [Test]
        public void OnEvent()
        {
            var called = false;
            var game = new Game();
            var mockEvent = new MockEvent();
            var state = game.RegisterState<MockGameState>();
            state.OnMockEventCallback = (@event) =>
            {
                called = true;
                Assert.That(@event, Is.EqualTo(mockEvent));
            };
           
            game.SetState<MockGameState>();
            game.OnEvent(mockEvent);

            Assert.That(called, Is.True);
        }

        [Test]
        public void OnEvent_Null_Args_Exception()
        {
            var game = new Game();
            Assert.That(() => game.OnEvent<MockEvent>(null), Throws.ArgumentNullException);
        }

        [Test]
        public void State_()
        {
            var game = new Game();
            game.RegisterState<MockGameState>();
            game.SetState<MockGameState>();

            var state = game.GetState();
        }
    }
}

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

            game.RegisterState<MockGameState>();

            /* State not set, should return null. */
            Assert.That(game.GetState(), Is.Null);

            /*
            var state = game.GetState();
            Assert.That(state, Is.Not.Null);
            Assert.That(state, Is.TypeOf<MockGameState>());
            */
        }

        [Test]
        public void RegisterState_State_Already_Registered_Exception()
        {
            var game = new Game();
            game.RegisterState<MockGameState>();
            Assert.That(() => game.RegisterState<MockGameState>(), Throws.InvalidOperationException);
        }

        [Test]
        public void SetState()
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
        public void State_()
        {
            var game = new Game();
            game.RegisterState<MockGameState>();
            game.SetState<MockGameState>();

            var state = game.GetState();
        }
    }
}

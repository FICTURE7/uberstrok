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
        public void State_()
        {
            var game = new Game();
            game.Register<MockState>();
            var something = game.Recorder.Recording;
        }
    }
}

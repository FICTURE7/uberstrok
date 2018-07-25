using NUnit.Framework;
using System;

namespace UberStrok.Tests
{
    [TestFixture]
    public class GameTickerTests
    {
        [Test]
        public void Ctor_tps_Less_Than_0_Exception()
        {
            Assert.That(() => new GameTicker(-1), Throws.InstanceOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void Sandbox()
        {
            var game = new Game();
            var ticker = new GameTicker(10);
        }
    }
}

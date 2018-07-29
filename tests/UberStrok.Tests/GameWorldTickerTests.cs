using NUnit.Framework;
using System;

namespace UberStrok.Tests
{
    [TestFixture]
    public class GameWorldTickerTests
    {
        [Test]
        public void Ctor_tps_Less_Than_0_Exception()
        {
            Assert.That(() => new GameWorldTicker(-1), Throws.InstanceOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void Sandbox()
        {
            var game = new GameWorld();
            var ticker = new GameWorldTicker(10);
        }
    }
}

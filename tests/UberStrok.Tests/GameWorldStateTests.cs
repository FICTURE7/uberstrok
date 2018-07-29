using NUnit.Framework;
using System.Reflection;
using UberStrok.Tests.Mocks;

namespace UberStrok.Tests
{
    [TestFixture]
    public class GameWorldStateTests
    {
        [Test]
        public void Find_OnEvent_Methods()
        {
            var state = new MockGameWorldState();

            Assert.That(state._onEventMethods, Contains.Key(typeof(MockEvent)));

            var methodInfo = state._onEventMethods[typeof(MockEvent)];
            Assert.That(methodInfo, Is.EqualTo(state.GetType().GetMethod("OnEvent", BindingFlags.Instance | BindingFlags.NonPublic)));
        }
    }
}

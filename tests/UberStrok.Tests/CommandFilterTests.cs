using NUnit.Framework;
using UberStrok.Tests.Mocks;

namespace UberStrok.Tests
{
    [TestFixture]
    public class CommandFilterTests
    {
        [Test]
        public void Add()
        {
            var filter = new CommandFilter();
            filter.Add<MockCommand>();
        }

        [Test]
        public void Add_Already_Added_Exception()
        {
            var filter = new CommandFilter();
            filter.Add<MockCommand>();

            Assert.That(() => filter.Add<MockCommand>(), Throws.InvalidOperationException);
        }

        [Test]
        public void Remove()
        {
            var filter = new CommandFilter();

            Assert.That(filter.Remove<MockCommand>(), Is.False);

            filter.Add<MockCommand>();

            Assert.That(filter.Remove<MockCommand>(), Is.True);
        }

        [Test]
        public void Contains()
        {
            var filter = new CommandFilter();

            Assert.That(filter.Contains<MockCommand>(), Is.False);

            filter.Add<MockCommand>();

            Assert.That(filter.Contains<MockCommand>(), Is.True);
        }

        [Test]
        public void Filtering_Get_Set()
        {
            var filter = new CommandFilter();

            Assert.That(filter.Filtering, Is.False);

            filter.Filtering = true;

            Assert.That(filter.Filtering, Is.True);
        }
    }
}

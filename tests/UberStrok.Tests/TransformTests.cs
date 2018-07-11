using NUnit.Framework;

namespace UberStrok.Tests
{
    [TestFixture]
    public class TransformTests
    {
        private Game _game;
        private GameObject _go;

        [SetUp]
        public void SetUp()
        {
            _game = new Game();
            _go = new GameObject(_game);
        }

        [Test]
        public void Ctor()
        {
            var transform = _go.AddComponent<Transform>();
        }

        [Test]
        public void Position_Get_Set()
        {
            var transform = _go.AddComponent<Transform>();
            Assert.That(transform.Position, Is.EqualTo(new Vector3(0, 0, 0)));

            transform.Position = new Vector3(10, 10, 10);

            Assert.That(transform.Position, Is.EqualTo(new Vector3(10,10,10)));
        }

        [Test]
        public void Rotation_Get_Set()
        {
            var transform = _go.AddComponent<Transform>();
            Assert.That(transform.Rotation, Is.EqualTo(new Vector3(0, 0, 0)));

            transform.Rotation = new Vector3(10, 10, 10);

            Assert.That(transform.Rotation, Is.EqualTo(new Vector3(10,10,10)));
        }
    }
}

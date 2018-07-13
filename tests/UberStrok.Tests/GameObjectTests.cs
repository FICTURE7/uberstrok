using NUnit.Framework;
using UberStrok.Tests.Mocks;

namespace UberStrok.Tests
{
    [TestFixture]
    public class GameObjectTests
    {
        private Game _game;

        [SetUp]
        public void SetUp()
        {
            _game = new Game();
        }

        [Test]
        public void AddComponent()
        {
            var go = _game.Objects.Create();
            var component = go.AddComponent<MockComponent>();
            Assert.That(component, Is.Not.Null);
            Assert.That(component, Is.TypeOf<MockComponent>());
            Assert.That(component.GameObject, Is.EqualTo(go));
        }

        [Test]
        public void AddComponent_ComponentAlreadyExists_ThrowException()
        {
            var go = _game.Objects.Create();
            var component = go.AddComponent<MockComponent>();
            Assert.That(() => go.AddComponent<MockComponent>(), Throws.InvalidOperationException);
        }

        [Test]
        public void RemoveComponent()
        {
            var go = _game.Objects.Create();
            var component = go.AddComponent<MockComponent>();
            Assert.That(component.GameObject, Is.EqualTo(go));
            Assert.That(go.RemoveComponent<MockComponent>(), Is.True);
            Assert.That(component.GameObject, Is.Null);
        }

        [Test]
        public void RemoveComponent_Not_Exists_Returns_False()
        {
            var go = _game.Objects.Create();
            Assert.That(go.RemoveComponent<MockComponent>(), Is.False);
        }

        [Test]
        public void GetComponent()
        {
            var go = _game.Objects.Create();
            var component1 = go.AddComponent<MockComponent>();
            var component2 = go.GetComponent<MockComponent>();

            Assert.That(component1, Is.EqualTo(component2));
        }

        [Test]
        public void ToString_Returns_String_Representation()
        {
            var go = _game.Objects.Create();
            go.Name = "testObject";
            go.AddComponent<MockComponent>();

            Assert.That(go.ToString, Is.EqualTo("{testObject:1}"));
        }

        [Test]
        public void Name_Set_Get()
        {
            var go = _game.Objects.Create();
            go.Name = "xD";

            Assert.That(go.Name, Is.EqualTo("xD"));
        }

        [Test]
        public void Enable_Set_Get()
        {
            var go = _game.Objects.Create();
            Assert.That(go.Enable, Is.EqualTo(true));
            go.Enable = false;
            Assert.That(go.Enable, Is.EqualTo(false));
        }

        [Test]
        public void Game_Get()
        {
            var go = _game.Objects.Create();
            Assert.That(go.Game, Is.EqualTo(_game));
        }

        [Test]
        public void Ctor_Registered_In_GameObject_List()
        {
            var go = _game.Objects.Create();
            Assert.That(_game.Objects, Contains.Item(go));
        }
    }
}

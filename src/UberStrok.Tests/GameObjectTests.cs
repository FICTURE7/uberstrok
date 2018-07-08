using NUnit.Framework;
using UberStrok.Tests.Mocks;

namespace UberStrok.Tests
{
    [TestFixture]
    public class GameObjectTests
    {
        [Test]
        public void AddComponent()
        {
            var go = new GameObject();
            var component = go.AddComponent<MockComponent>();
            Assert.That(component.GameObject, Is.EqualTo(go));
            Assert.That(component, Is.Not.Null);
            Assert.That(component, Is.TypeOf<MockComponent>());            
        }

        [Test]
        public void AddComponent_ComponentAlreadyExists_ThrowException()
        {
            var go = new GameObject();
            var component = go.AddComponent<MockComponent>();
            Assert.That(() => go.AddComponent<MockComponent>(), Throws.InvalidOperationException);
        }

        [Test]
        public void RemoveComponent()
        {
            var go = new GameObject();
            var component = go.AddComponent<MockComponent>();
            Assert.That(component.GameObject, Is.Not.Null);
            Assert.That(go.RemoveComponent<MockComponent>(), Is.True);
            Assert.That(component.GameObject, Is.Null);
        }

        [Test]
        public void GetComponent()
        {
            var go = new GameObject();
            var component1 = go.AddComponent<MockComponent>();
            var component2 = go.GetComponent<MockComponent>();

            Assert.That(component1, Is.EqualTo(component2));
        }

        [Test]
        public void Name_Set_Get()
        {
            var go = new GameObject();
            go.Name = "xD";

            Assert.That(go.Name, Is.EqualTo("xD"));
        }

        [Test]
        public void Enabled_Set_Get()
        {
            var go = new GameObject();
            Assert.That(go.Enabled, Is.EqualTo(true));
            go.Enabled = false;
            Assert.That(go.Enabled, Is.EqualTo(false));
        }
    }
}

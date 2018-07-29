namespace UberStrok.Tests.Mocks
{
    public class MockGameWorldState : GameWorldState
    {
        /* To test if the OnEvent methods are getting called. */
        public delegate void OnMockEvent(MockEvent @event);
        public OnMockEvent OnMockEventCallback;

        private void OnEvent(MockEvent @event)
        {
            OnMockEventCallback?.Invoke(@event);
        }

        public override void OnEnter()
        {

        }

        public override void OnExit()
        {

        }

        public override void OnResume()
        {

        }

        public override void OnUpdate()
        {

        }
    }
}

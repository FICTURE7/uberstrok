namespace UberStrok
{
    public interface IState
    {
        void OnEnter();
        void OnResume();
        void OnUpdate();
        void OnExit();
    }
}

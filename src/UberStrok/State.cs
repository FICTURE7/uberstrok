namespace UberStrok
{
    public abstract class State : IState
    {
        public abstract void OnUpdate();

        public virtual void OnEnter()
        {
            /* Space */
        }

        public virtual void OnResume()
        {
            /* Space */
        }

        public virtual void OnExit()
        {
            /* Space */
        }
    }
}

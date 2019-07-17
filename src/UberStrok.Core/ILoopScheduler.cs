namespace UberStrok.Core
{
    public interface ILoopScheduler
    {
        void Schedule(ILoop loop);
        bool Unschedule(ILoop loop);
        float GetLoad();
    }
}

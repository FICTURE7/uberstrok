namespace UberStrok
{
    public abstract class Command
    {
        private readonly int _tick;

        public int Tick => _tick;
    }
}

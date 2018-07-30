namespace UberStrok.Realtime.Server.Game
{
    public enum GameActorState
    {
        None,
        Connected,
        Authenticated,
    }

    public class GameActor : Actor
    {
        public GameActorState State { get; set; }

        public string AuthToken { get; set; }
        public int Cmid { get; set; }
    }
}

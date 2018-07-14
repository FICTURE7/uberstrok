using UberStrok.Core.Common;

namespace UberStrok.Realtime.Server.Game.Commands
{
    public class PowerUpPickedCommand : Command
    {
        /* Pick up ID. */
        public int Id { get; set; }
        /* Pick up type. */
        public PickupItemType Type { get; set; }
        /* Pick up value. E.g Armor points, Health points etc... */
        public byte Value { get; set; }

        protected override void OnExecute()
        {
            
        }
    }
}

namespace UberStrok.Core.Views
{
    public class RoomDataView
    {
        /* Original `Number`. */
        public int RoomId { get; set; }
        public string Guid { get; set; }
        public string Name { get; set; }
        public ConnectionAddressView Server { get; set; }
        public bool IsPasswordProtected { get; set; }
    }
}

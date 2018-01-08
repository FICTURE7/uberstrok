namespace UberStrok.Core.Views
{
    public class RoomDataView
    {
        public string Guid { get; set; }
        public string Name { get; set; }
        public ConnectionAddressView Server { get; set; }
        public int Number { get; set; }
        public bool IsPasswordProtected { get; set; }
    }
}

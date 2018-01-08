using System.IO;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class PhotonServerLoadViewProxy
    {
        public static void Serialize(Stream stream, PhotonServerLoadView instance)
        {
            using (var bytes = new MemoryStream())
            {
                SingleProxy.Serialize(bytes, instance.MaxPlayerCount);
                Int32Proxy.Serialize(bytes, instance.PeersConnected);
                Int32Proxy.Serialize(bytes, instance.PlayersConnected);
                Int32Proxy.Serialize(bytes, instance.RoomsCreated);
                bytes.WriteTo(stream);
            }
        }

        public static PhotonServerLoadView Deserialize(Stream bytes)
        {
            return new PhotonServerLoadView
            {
                MaxPlayerCount = SingleProxy.Deserialize(bytes),
                PeersConnected = Int32Proxy.Deserialize(bytes),
                PlayersConnected = Int32Proxy.Deserialize(bytes),
                RoomsCreated = Int32Proxy.Deserialize(bytes)
            };
        }
    }
}

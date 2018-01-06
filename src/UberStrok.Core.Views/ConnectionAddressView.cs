using System;

namespace UberStrok.Core.Views
{
    [Serializable]
    public class ConnectionAddressView
    {
        public ConnectionAddressView()
        {
            // Space
        }

        public ConnectionAddressView(string connection)
        {
            var ipPort = connection.Split(':');

            Ipv4 = ToInteger(ipPort[0]);
            Port = ushort.Parse(ipPort[1]);
        }

        public ConnectionAddressView(string ipAddress, ushort port)
        {
            Ipv4 = ToInteger(ipAddress);
            Port = port;
        }

        public static int ToInteger(string ipAddress)
        {
            int ipV4 = 0;
            var segments = ipAddress.Split('.');
            if (segments.Length == 4)
            {
                for (int i = 0; i < segments.Length; i++)
                    ipV4 |= int.Parse(segments[i]) << (3 - i) * 8;
            }

            return ipV4;
        }

        public static string ToString(int ipv4)
        {
            return string.Format("{0}.{1}.{2}.{3}", new object[]
            {
                ipv4 >> 24 & 255,
                ipv4 >> 16 & 255,
                ipv4 >> 8 & 255,
                ipv4 & 255
            });
        }

        public string ConnectionString => string.Format("{0}:{1}", ToString(this.Ipv4), this.Port);
        public string IpAddress => ToString(Ipv4);

        public int Ipv4 { get; set; }
        public ushort Port { get; set; }
    }
}

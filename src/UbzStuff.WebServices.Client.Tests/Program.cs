namespace UbzStuff.WebServices.Client.Tests
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            //NOTE: You should fetch the authToken manually from the web service logs.

            /*76561198117697849:aHR0cDovL2xvY2FsaG9zdC8yLjAjIyMjIzkvNC8yMDE3IDE6MDE6MTMgUE0=*/

            var client = new UserWebServiceClient("http://localhost/2.0");
            var member = client.GetMember("aHR0cDovL2xvY2FsaG9zdC8yLjAjIyMjIzkvNC8yMDE3IDE6MDE6MTMgUE0=");
        }
    }
}

using System;

namespace UbzStuff.WebServices.Client.Tests
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var userServiceClient = new UserWebServiceClient("http://localhost/2.0");
            var authenticationServiceClient = new AuthenticationWebServiceClient("http://localhost/2.0");

            var loginResult = authenticationServiceClient.LoginSteam("test", "", "");
            var member = userServiceClient.GetMember(loginResult.AuthToken);

            Console.WriteLine(member.CmuneMemberView);
            Console.WriteLine(member.UberstrikeMemberView);
            Console.ReadLine();
        }
    }
}

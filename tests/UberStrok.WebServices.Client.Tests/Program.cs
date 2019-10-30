﻿using System;

namespace UberStrok.WebServices.Client.Tests
{
    public static class Program
    {
        //TODO: Write some stress test code or some stuff.

        public static void Main(string[] args)
        {
            var userServiceClient = new UserWebServiceClient("http://localhost:5000/2.0");
            var authenticationServiceClient = new AuthenticationWebServiceClient("http://localhost:5000/2.0");

            var loginResult = authenticationServiceClient.LoginSteam("test", "", "");
            var member = userServiceClient.GetMember(loginResult.AuthToken);

            Console.WriteLine(member.CmuneMemberView);
            Console.WriteLine(member.UberstrikeMemberView);
            Console.ReadLine();
        }
    }
}

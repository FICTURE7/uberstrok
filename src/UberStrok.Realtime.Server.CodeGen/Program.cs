using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;
using System;
using System.Reflection;

namespace UberStrok.Realtime.Server.CodeGen
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var assemblyCSharpFirstpassPath = "../../../../libs/uberstrike/Assembly-CSharp-firstpass.dll";
            var lobbyRoomOperationSender = "UberStrike.Realtime.Client.LobbyRoomOperations";

            try
            {
                /* Load the Assembly-CSharp-firstpass.dll file. */
                var assembly = Assembly.LoadFrom(assemblyCSharpFirstpassPath);
                var type = assembly.GetType(lobbyRoomOperationSender);


                var workspace = new AdhocWorkspace();
                var generator = SyntaxGenerator.GetGenerator(workspace, LanguageNames.CSharp);

                var codeGen = new OperationHandlerCodeGen(generator, type, "CommPeer");
                var output = codeGen.GenerateCode();

                Console.WriteLine(output);
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("ERROR: " + ex);
                Console.ReadLine();
                return 1;
            }

            return 0;
        }

        public static string GetTypeAlias(Type type)
        {
            var name = type.Name;
            if (type == typeof(int))
                name = "int";
            else if (type == typeof(uint))
                name = "uint";
            else if (type == typeof(short))
                name = "short";
            else if (type == typeof(ushort))
                name = "ushort";
            else if (type == typeof(byte))
                name = "byte";
            else if (type == typeof(sbyte))
                name = "sbyte";
            else if (type == typeof(long))
                name = "long";
            else if (type == typeof(ulong))
                name = "ulong";
            else if (type == typeof(float))
                name = "float";
            else if (type == typeof(bool))
                name = "bool";

            return name;
        }
    }
}

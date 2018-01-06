using System;
using System.Reflection;
using System.Text;

namespace UberStrok.CodeGen.Realtime
{
    public class Program
    {
        public static int Main(string[] args)
        {
#if !DEBUG
            if (args.Length < 2)
            {
                Console.Error.WriteLine("ERROR: Specify file name & operation sender.");
                return 1 ;
            }

            string fileName = args[0];
            string operationSender = args[1];
#else
            string fileName = "../../../../lib/uberstrike/Assembly-CSharp-firstpass.dll";
            string operationSender = "UberStrike.Realtime.Client.LobbyRoomOperations";

#endif

            var output = new StringBuilder();

            try
            {
                var assembly = Assembly.LoadFrom(fileName);
                var type = assembly.GetType(operationSender);
                var methods = type.GetMethods();

                foreach (var method in methods)
                {
                    if (method.Name.StartsWith("Send"))
                    {
                        string outputMethodName = method.Name.Remove(0, "Send".Length);
                        output.AppendLine($"private void {outputMethodName}(MemoryStream bytes)");
                        output.AppendLine("{");

                        string parameterList = string.Empty;
                        var parameters = method.GetParameters();
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            var parameter = parameters[i];
                            string proxyClass = parameter.ParameterType.Name + "Proxy";
                            if (parameter.ParameterType.IsGenericType)
                            {
                                if (parameter.ParameterType.GetGenericArguments().Length == 1)
                                {
                                    string genericParameter = parameter.ParameterType.GetGenericArguments()[0].Name;
                                    string genericClassName = parameter.ParameterType.Name;
                                    genericClassName = genericClassName.Remove(genericClassName.IndexOf('`'));

                                    proxyClass = genericClassName + "Proxy<" + genericParameter + ">";
                                }
                            }

                            output.AppendLine($"    var {parameter.Name} = {proxyClass}.Deserialize(bytes);");
                            parameterList += parameter.Name;
                            if (i < parameters.Length - 1)
                                parameterList += ", ";
                        }

                        output.AppendLine($"    On{outputMethodName}({parameterList});");
                        output.AppendLine("}");
                        output.AppendLine($"public abstract void On{outputMethodName}({parameterList});");
                        output.AppendLine();
                    }
                }

                Console.WriteLine(output);
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("ERROR: " + ex);
                return 1;
            }

            return 0;
        }
    }
}

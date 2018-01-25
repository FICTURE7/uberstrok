using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Editing;
using System;
using System.Collections.Generic;
using System.Reflection;

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
            string fileName = "../../../../libs/uberstrike/Assembly-CSharp-firstpass.dll";
            string operationSender = "UberStrike.Realtime.Client.LobbyRoomOperations";
#endif

            try
            {
                var assembly = Assembly.LoadFrom(fileName);
                var type = assembly.GetType(operationSender);
                var methods = type.GetMethods();

                var sendMethods = new List<MethodInfo>();
                var handlerMethods = new List<SyntaxNode>();

                var workspace = new AdhocWorkspace();
                var generator = SyntaxGenerator.GetGenerator(workspace, LanguageNames.CSharp);

                foreach (var method in methods)
                {
                    if (method.Name.StartsWith("Send"))
                        sendMethods.Add(method);
                }

                foreach (var sendMethod in sendMethods)
                {
                    var abstractMethodParameters = new List<SyntaxNode>();
                    abstractMethodParameters.Add(generator.ParameterDeclaration(
                        name: "peer",
                        type: SyntaxFactory.ParseTypeName("CommPeer")
                    ));

                    foreach (var parameter in sendMethod.GetParameters())
                    {
                        var parameterType = parameter.ParameterType;
                        var parameterTypeName = GetTypeAlias(parameterType);

                        var parameterDecl = generator.ParameterDeclaration(
                            parameter.Name,
                            type: SyntaxFactory.ParseTypeName(parameterTypeName)
                        );

                        abstractMethodParameters.Add(parameterDecl);
                    }

                    var abstractMethodName = "On" + sendMethod.Name.Remove(0, "Send".Length);
                    var abstractMethodDecl = generator.MethodDeclaration(
                        name: abstractMethodName,
                        modifiers: DeclarationModifiers.Abstract,
                        accessibility: Accessibility.Public,
                        parameters: abstractMethodParameters
                    );

                    handlerMethods.Add(abstractMethodDecl);
                }

                var operationRequestMethodBody = new List<SyntaxNode>();
                var operationRequestOpCodeCast = generator.LocalDeclarationStatement(
                    name: "operation",
                    initializer: generator.CastExpression(
                        SyntaxFactory.ParseTypeName("ILobbyRoomOperations"),
                        generator.IdentifierName("opCode")
                    )
                );

                var operationRequestSwitchSections = new List<SyntaxNode>();
                foreach (var sendMethod in sendMethods)
                {
                    var name = sendMethod.Name.Remove(0, "Send".Length); 
                    var section = generator.SwitchSection(
                        SyntaxFactory.ParseTypeName("ILobbyRoomOperations." + name),
                        new SyntaxNode[] { generator.InvocationExpression(generator.IdentifierName(name)) }
                    );

                    operationRequestSwitchSections.Add(section);
                }

                var operationRequestSwitch = generator.SwitchStatement(generator.IdentifierName("operation"), operationRequestSwitchSections);

                operationRequestMethodBody.Add(operationRequestOpCodeCast);
                operationRequestMethodBody.Add(operationRequestSwitch);

                var operationRequestMethod = generator.MethodDeclaration(
                    name: "OnOperationRequest",
                    parameters: new SyntaxNode[] {
                        generator.ParameterDeclaration(
                            name: "peer",
                            type: SyntaxFactory.ParseTypeName("CommPeer")
                        ),
                        generator.ParameterDeclaration(
                            name: "opCode",
                            type: SyntaxFactory.ParseTypeName("byte")
                        ),
                        generator.ParameterDeclaration(
                            name: "bytes",
                            type: SyntaxFactory.ParseTypeName("MemoryStream")
                        ),
                    },
                    accessibility: Accessibility.Public,
                    modifiers: DeclarationModifiers.Override,
                    statements: operationRequestMethodBody
                );

                handlerMethods.Add(operationRequestMethod);

                var deserializerMethodParameter = new List<SyntaxNode>
                {
                    generator.ParameterDeclaration(
                        "peer",
                        type: SyntaxFactory.ParseTypeName("CommPeer")
                    ),
                    generator.ParameterDeclaration(
                        "bytes",
                        type: SyntaxFactory.ParseTypeName("MemoryStream")
                    ),
                };

                foreach (var sendMethod in sendMethods)
                {
                    var deserializerMethodBody = new List<SyntaxNode>();
                    var variables = new List<SyntaxNode>();

                    variables.Add(generator.IdentifierName("peer"));

                    foreach (var parameter in sendMethod.GetParameters())
                    {
                        var proxyClass = parameter.ParameterType.Name + "Proxy";

                        var variableDecl = generator.LocalDeclarationStatement(
                            name: parameter.Name,
                            initializer: generator.InvocationExpression(generator.IdentifierName(proxyClass + ".Deserialize"), generator.IdentifierName("bytes"))
                        );

                        deserializerMethodBody.Add(variableDecl);
                        variables.Add(generator.IdentifierName(parameter.Name));
                    }

                    var abstractMethodName = "On" + sendMethod.Name.Remove(0, "Send".Length);
                    var abstractMethodCall = generator.InvocationExpression(
                        generator.IdentifierName(abstractMethodName),
                        variables
                    );

                    deserializerMethodBody.Add(abstractMethodCall);

                    var deserializerMethodName = sendMethod.Name.Remove(0, "Send".Length);
                    var deserializerMethodDecl = generator.MethodDeclaration(
                        deserializerMethodName,
                        parameters: deserializerMethodParameter,
                        accessibility: Accessibility.Private,
                        statements: deserializerMethodBody
                    );

                    handlerMethods.Add(deserializerMethodDecl);
                }

                var className = "Base" + type.Name.Replace("Operations", "OperationHandler");
                var classDefinition = generator.ClassDeclaration(
                    className,
                    accessibility: Accessibility.Public,
                    modifiers: DeclarationModifiers.Abstract,
                    members: handlerMethods,
                    baseType: generator.GenericName("BaseOperationHandler", SyntaxFactory.ParseTypeName("CommPeer"))
                );

                var usingDirectives = generator.NamespaceImportDeclaration("System.IO");

                var namespaceDecl = generator.NamespaceDeclaration("UberStrok.Realtime.Server", classDefinition);
                var compilationUnit = generator.CompilationUnit(usingDirectives, namespaceDecl).NormalizeWhitespace();
                var output = compilationUnit.ToString();

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

            return name;
        }
    }
}

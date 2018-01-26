using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Editing;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace UberStrok.Realtime.Server.CodeGen
{
    public class OperationHandlerCodeGen
    {
        private readonly SyntaxGenerator _generator;
        private readonly Type _operationSenderType;
        private readonly string _peerTypeName;

        private readonly List<MethodInfo> _senderMethods;
        private readonly List<string> _callbackMethodNames;
        private readonly List<string> _deserializerMethodNames;

        public OperationHandlerCodeGen(SyntaxGenerator generator, Type operationSenderType, string peerTypeName)
        {
            if (generator == null)
                throw new ArgumentNullException(nameof(generator));
            if (operationSenderType == null)
                throw new ArgumentNullException(nameof(operationSenderType));
            if (peerTypeName == null)
                throw new ArgumentNullException(nameof(peerTypeName));

            _generator = generator;
            _operationSenderType = operationSenderType;
            _peerTypeName = peerTypeName;

            _senderMethods = new List<MethodInfo>();
            _callbackMethodNames = new List<string>();
            _deserializerMethodNames = new List<string>();
            foreach (var method in _operationSenderType.GetMethods())
            {
                var methodName = method.Name;
                if (methodName.StartsWith("Send"))
                {
                    _senderMethods.Add(method);

                    var deserializerMethodName = methodName.Remove(0, "Send".Length);
                    var callbackMethodName = "On" + deserializerMethodName;

                    _deserializerMethodNames.Add(deserializerMethodName);
                    _callbackMethodNames.Add(callbackMethodName);
                }
            }
        }

        public string GenerateCode()
        {
            var callbacksDecl = GenerateAbstractCallbacks();
            var deserializersDecl = GenerateDeserializers();
            var onOperationRequestDecl = GenerateOnOperationRequestMethod();

            var members = new List<SyntaxNode>();
            members.AddRange(callbacksDecl);
            members.Add(onOperationRequestDecl);
            members.AddRange(deserializersDecl);

            var className = "Base" + _operationSenderType.Name.Replace("Operations", "OperationHandler");
            var classDefinition = _generator.ClassDeclaration(
                className,
                accessibility: Accessibility.Public,
                modifiers: DeclarationModifiers.Abstract,
                members: members,
                baseType: _generator.GenericName("BaseOperationHandler", SyntaxFactory.ParseTypeName(_peerTypeName))
            );

            var namespaceDecl = _generator.NamespaceDeclaration("UberStrok.Realtime.Server", classDefinition);
            return namespaceDecl.NormalizeWhitespace().ToString();
        }

        private SyntaxNode GenerateOnOperationRequestMethod()
        {
            var enumType = _generator.IdentifierName("I" + _operationSenderType.Name + "Type");
            var statements = new List<SyntaxNode>
            {
                _generator.LocalDeclarationStatement(
                    name: "operation",
                    initializer: _generator.CastExpression(
                        enumType,
                        _generator.IdentifierName("opCode")
                    )
                )
            };

            var sections = new List<SyntaxNode>();
            for (int i = 0; i < _senderMethods.Count; i++)
            {
                var senderMethod = _senderMethods[i];
                var name = _deserializerMethodNames[i];
                var section = _generator.SwitchSection(
                    caseExpression: _generator.IdentifierName(enumType.ToString() + "." + name),
                    statements: new SyntaxNode[] {
                            _generator.InvocationExpression(_generator.IdentifierName(name), _generator.IdentifierName("peer"), _generator.IdentifierName("bytes")),
                            _generator.ExitSwitchStatement()
                    }
                );

                sections.Add(section);
            }

            sections.Add(_generator.DefaultSwitchSection(
                new SyntaxNode[] {
                    _generator.ThrowStatement(
                        _generator.ObjectCreationExpression(SyntaxFactory.ParseTypeName("NotSupportedException"))
                    )
                }
            ));

            statements.Add(_generator.SwitchStatement(
                expression: _generator.IdentifierName("operation"),
                sections: sections
            ));

            var methodDecl = _generator.MethodDeclaration(
                name: "OnOperationRequest",
                parameters: new SyntaxNode[] {
                    _generator.ParameterDeclaration(
                        name: "peer",
                        type: SyntaxFactory.ParseTypeName(_peerTypeName)
                    ),
                    _generator.ParameterDeclaration(
                        name: "opCode",
                        type:  SyntaxFactory.ParseTypeName("byte")
                    ),
                    _generator.ParameterDeclaration(
                        name: "bytes",
                        type: SyntaxFactory.ParseTypeName("MemoryStream")
                    ),
                },
                accessibility: Accessibility.Public,
                modifiers: DeclarationModifiers.Override,
                statements: statements
            );

            return methodDecl;
        }

        private List<SyntaxNode> GenerateDeserializers()
        {
            var deserializerMethodsDecl = new List<SyntaxNode>(_senderMethods.Count);

            for (int i = 0; i < _senderMethods.Count; i++)
            {
                var senderMethod = _senderMethods[i];
                var statements = new List<SyntaxNode>();
                var parameters = new List<SyntaxNode>
                {
                    _generator.ParameterDeclaration(
                        name: "peer",
                        type: SyntaxFactory.ParseTypeName(_peerTypeName)
                    ),
                    _generator.ParameterDeclaration(
                        name: "bytes",
                        type: SyntaxFactory.ParseTypeName("MemoryStream")
                    )
                };

                var callbackInvocationArgs = new List<SyntaxNode>
                {
                    _generator.IdentifierName("peer")
                };

                foreach (var parameter in senderMethod.GetParameters())
                {
                    var parameterType = parameter.ParameterType;
                    var variableDecl = GetProxyDeserializeInvocation(parameter);

                    statements.Add(variableDecl);
                    callbackInvocationArgs.Add(_generator.IdentifierName(parameter.Name));
                }

                var callbackInvocation = _generator.InvocationExpression(
                    expression: _generator.IdentifierName(_callbackMethodNames[i]),
                    arguments: callbackInvocationArgs
                );

                statements.Add(callbackInvocation);

                var methodDecl = _generator.MethodDeclaration(
                    name: _deserializerMethodNames[i],
                    parameters: parameters,
                    accessibility: Accessibility.Private,
                    statements: statements
                );

                deserializerMethodsDecl.Add(methodDecl);
            }

            return deserializerMethodsDecl;
        }

        private List<SyntaxNode> GenerateAbstractCallbacks()
        {
            var callbackMethodsDecl = new List<SyntaxNode>(_senderMethods.Count);

            for (int i = 0; i < _senderMethods.Count; i++)
            {
                var senderMethod = _senderMethods[i];
                var parameters = new List<SyntaxNode>
                {
                    _generator.ParameterDeclaration(
                        name: "peer",
                        type: SyntaxFactory.ParseTypeName(_peerTypeName)
                    )
                };

                foreach (var parameter in senderMethod.GetParameters())
                {
                    var parameterType = parameter.ParameterType;
                    var parameterTypeName = GetTypeName(parameterType);
                    var parameterName = parameter.Name;

                    var parameterDecl = _generator.ParameterDeclaration(
                        name: parameterName,
                        type: SyntaxFactory.ParseTypeName(parameterTypeName)
                    );

                    parameters.Add(parameterDecl);
                }

                var methodDecl = _generator.MethodDeclaration(
                    name: _callbackMethodNames[i],
                    parameters: parameters,
                    accessibility: Accessibility.Protected,
                    modifiers: DeclarationModifiers.Abstract
                );

                callbackMethodsDecl.Add(methodDecl);
            }

            return callbackMethodsDecl;
        }

        private static string GetTypeName(Type type)
        {
            if (type.IsGenericType)
            {
                var genericTypeDefinition = type.GetGenericTypeDefinition();
                var genericTypeDefinitionName = genericTypeDefinition.Name;
                var genericArguments = type.GetGenericArguments();

                var name = genericTypeDefinitionName.Remove(genericTypeDefinitionName.IndexOf('`')) + "<";

                for (int i = 0; i < genericArguments.Length; i++)
                {
                    var argument = genericArguments[i];
                    var argumentName = GetTypeName(argument);

                    name += argumentName;
                    if (i < genericArguments.Length - 1)
                        name += ", ";
                }

                name += ">";

                return name;
            }

            return GetTypeNameAlias(type);
        }

        private static string GetTypeNameAlias(Type type)
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
            else if (type == typeof(string))
                name = "string";
            return name;
        }

        private SyntaxNode GetProxyDeserializeInvocation(ParameterInfo parameter)
        {
            var proxyClass = GetProxyClassName(parameter.ParameterType);
            var initializer = default(SyntaxNode);

            var wtf = parameter.ParameterType;
            if (wtf.IsGenericType && wtf.GetGenericTypeDefinition() == typeof(List<>))
            {
                initializer = _generator.InvocationExpression(
                    _generator.IdentifierName(proxyClass + ".Deserialize"),
                    _generator.IdentifierName("bytes"),
                    _generator.IdentifierName(GetProxyClassName(parameter.ParameterType.GetGenericArguments()[0]) + ".Deserialize")
                );
            }
            else
            {
                initializer = _generator.InvocationExpression(
                    _generator.IdentifierName(proxyClass + ".Deserialize"),
                    _generator.IdentifierName("bytes")
                );
            }

            return _generator.LocalDeclarationStatement(
                name: parameter.Name,
                initializer: initializer
            );
        }

        private static string GetProxyClassName(Type type)
        {
            if (type.IsPrimitive || type == typeof(string))
                return type.Name + "Proxy";
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                var genericArgumentsString = "<";
                var genericArguments = type.GetGenericArguments();
                for (int i = 0; i < genericArguments.Length; i++)
                {
                    var argument = genericArguments[i];
                    var argumentName = GetTypeName(argument);

                    genericArgumentsString += argumentName;
                    if (i < genericArguments.Length - 1)
                        genericArgumentsString += ", ";
                }

                genericArgumentsString += ">";

                return "ListProxy" + genericArgumentsString;
            }

            return type.Name + "ViewProxy";
        }
    }
}

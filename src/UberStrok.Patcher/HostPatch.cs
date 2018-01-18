using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;

namespace UberStrok.Patcher
{
    public class HostPatch : Patch
    {
        public override void Apply(UberStrike uberStrike)
        {
            var ApplicationDataManager_Type = uberStrike.AssemblyCSharp.Find("ApplicationDataManager", true);
            var ApplicationDataManager_CCtor = ApplicationDataManager_Type.FindStaticConstructor();
            var ilBody = ApplicationDataManager_CCtor.Body;

            if (ilBody.Instructions.Count != 15)
                throw new Exception("I think it has been patched or altered.");

            var endOfPatch = ilBody.Instructions[4];

            /* UnityEngine.Application */
            var Application_TypeRef = new TypeRefUser(uberStrike.AssemblyCSharp, "UnityEngine", "Application", uberStrike.UnityEngine.Assembly.ToAssemblyRef());
            /* UnityEngine.Application.get_dataPath */
            var Application_get_dataPath = new MemberRefUser(
                uberStrike.AssemblyCSharp,
                "get_dataPath",
                MethodSig.CreateStatic(uberStrike.AssemblyCSharp.CorLibTypes.String),
                Application_TypeRef
            );

            /* Calls Application.dataPath */
            ilBody.Instructions.Insert(0, OpCodes.Call.ToInstruction(Application_get_dataPath));
            /* Loads ".uberstrok" onto the stack. */
            ilBody.Instructions.Insert(1, OpCodes.Ldstr.ToInstruction(".uberstrok"));

            /* System.IO.Path */
            var Path_TypeRef = new TypeRefUser(uberStrike.AssemblyCSharp, "System.IO", "Path", uberStrike.AssemblyCSharp.CorLibTypes.AssemblyRef);
            /* System.IO.Path.Combine(string, string) */
            var Path_Combine_MethodRef = new MemberRefUser(
                uberStrike.AssemblyCSharp,
                "Combine",
                MethodSig.CreateStatic(
                    uberStrike.AssemblyCSharp.CorLibTypes.String,
                    uberStrike.AssemblyCSharp.CorLibTypes.String,
                    uberStrike.AssemblyCSharp.CorLibTypes.String
                ),
                Path_TypeRef
            );

            /* Calls Path.Combine(Application.dataPath, ".uberstrok") */
            ilBody.Instructions.Insert(2, OpCodes.Call.ToInstruction(Path_Combine_MethodRef));

            /* System.IO.File */
            var File_TypeRef = new TypeRefUser(uberStrike.AssemblyCSharp, "System.IO", "File", uberStrike.AssemblyCSharp.CorLibTypes.AssemblyRef);
            /* System.IO.Path.ReadAllText(string) */
            var File_ReadAllText_MethodRef = new MemberRefUser(
                uberStrike.AssemblyCSharp,
                "ReadAllText",
                MethodSig.CreateStatic(
                    uberStrike.AssemblyCSharp.CorLibTypes.String,
                    uberStrike.AssemblyCSharp.CorLibTypes.String
                ),
                File_TypeRef
            );

            /* Calls File.ReadAllText(Path.Combine(Application.dataPath, ".uberstrok")) */
            ilBody.Instructions.Insert(3, OpCodes.Call.ToInstruction(File_ReadAllText_MethodRef));

            /* Duplicate the value on the stack again to set both the WebServiceBaseUrl & ImagePath */
            ilBody.Instructions.Insert(4, OpCodes.Dup.ToInstruction());

            var WebServiceBaseUrl_Field = ApplicationDataManager_Type.FindField("WebServiceBaseUrl");
            var ImagePath_Field = ApplicationDataManager_Type.FindField("ImagePath");

            /*
                ApplicationDataManager.WebServiceBaseUrl = pop() stack
                ApplicationDataManager.ImagePath = pop() stack
             */
            ilBody.Instructions.Insert(5, OpCodes.Stsfld.ToInstruction(WebServiceBaseUrl_Field));
            ilBody.Instructions.Insert(6, OpCodes.Stsfld.ToInstruction(ImagePath_Field));

            /* Jump out of try-catch. */
            ilBody.Instructions.Insert(7, OpCodes.Leave_S.ToInstruction(endOfPatch));

            /* Clean stack. */
            ilBody.Instructions.Insert(8, OpCodes.Pop.ToInstruction());

            /* Loads "Failed to load '.uberstrok' host config." onto the stack. */
            ilBody.Instructions.Insert(9, OpCodes.Ldstr.ToInstruction("Failed to load '.uberstrok' host config."));

            /* ApplicationDataManager.LockApplication(string) */
            var ApplicationDataManager_LockApplication_Method = ApplicationDataManager_Type.FindMethod("LockApplication");

            ilBody.Instructions.Insert(10, OpCodes.Call.ToInstruction(ApplicationDataManager_LockApplication_Method));

            /* Jump out of try-catch. */
            ilBody.Instructions.Insert(11, OpCodes.Leave_S.ToInstruction(endOfPatch));

            /* Remove old instructions. */
            ilBody.Instructions.RemoveAt(12);
            ilBody.Instructions.RemoveAt(12);
            ilBody.Instructions.RemoveAt(12);
            ilBody.Instructions.RemoveAt(12);

            /* Register the exception handler. */
            ilBody.ExceptionHandlers.Add(new ExceptionHandler
            {
                CatchType = uberStrike.AssemblyCSharp.CorLibTypes.Object.ToTypeDefOrRef(),
                HandlerType = ExceptionHandlerType.Catch,
                HandlerStart = ilBody.Instructions[8],
                HandlerEnd = endOfPatch,
                TryStart = ilBody.Instructions[0],
                TryEnd = ilBody.Instructions[8]
            });
        }
    }
}

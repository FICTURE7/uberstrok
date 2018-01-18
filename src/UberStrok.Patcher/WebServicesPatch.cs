using dnlib.DotNet.Emit;

namespace UberStrok.Patcher
{
    public class WebServicesPatch : Patch
    {
        public override void Apply(UberStrike uberStrike)
        {
            var types = uberStrike.AssemblyCSharpFirstpass.GetTypes();
            foreach (var type in types)
            {
                if (!type.Name.EndsWith("WebServiceClient"))
                    continue;

                foreach (var method in type.Methods)
                {
                    foreach (var il in method.Body.Instructions)
                    {
                        if (il.OpCode == OpCodes.Ldstr)
                        {
                            var str = (string)il.Operand;
                            if (str.StartsWith("UberStrike.DataCenter.WebService.CWS.") && str.EndsWith("Contract.svc"))
                            {
                                str = str.Replace("UberStrike.DataCenter.WebService.CWS.", string.Empty).Replace("Contract.svc", string.Empty);
                                il.Operand = str;
                            }
                        }
                    }
                }
            }
        }
    }
}

using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;

namespace UberStrok.Patcher
{
    public class QuickSwitchPatch : Patch
    {
        public override void Apply(UberStrike uberStrike)
        {
            var GameState_Type = uberStrike.AssemblyCSharp.Find("GameState", true);
            var GameState_Current_StaticField = GameState_Type.GetField("Current");
            var GaneState_RoomData_Property = GameState_Type.FindProperty("RoomData");

            var WeaponController_Type = uberStrike.AssemblyCSharp.Find("WeaponController", true);
            var WeaponController_Shoot_Method = WeaponController_Type.FindMethod("Shoot");
            var ilBody = WeaponController_Shoot_Method.Body;

            if (ilBody.Instructions.Count != 75)
                throw new Exception("I think it has been patched or altered.");

            /* Loads GameFlags.QuickSwitch onto the stack. */
            ilBody.Instructions.Insert(13, OpCodes.Ldc_I4_4.ToInstruction());
            /* Loads GameState.Current onto the stack. */
            ilBody.Instructions.Insert(14, OpCodes.Ldsfld.ToInstruction(GameState_Current_StaticField));

            var GameRoomData_TypeRef = new TypeRefUser(uberStrike.AssemblyCSharp, "UberStrike.Core.Models", "GameRoomData", uberStrike.AssemblyCSharpFirstpass.Assembly.ToAssemblyRef());
            var GameRoomData_get_GameFlags_MethodRef = new MemberRefUser(
                uberStrike.AssemblyCSharp,
                "get_GameFlags",
                MethodSig.CreateInstance(uberStrike.AssemblyCSharpFirstpass.CorLibTypes.Int32),
                GameRoomData_TypeRef
            );

            /* Calls GameState.Current.get_RoomData().get_GameFlags() */
            ilBody.Instructions.Insert(15, OpCodes.Callvirt.ToInstruction(GaneState_RoomData_Property.GetMethod));
            ilBody.Instructions.Insert(16, OpCodes.Callvirt.ToInstruction(GameRoomData_get_GameFlags_MethodRef));

            var GameFlags_TypeRef = new TypeRefUser(uberStrike.AssemblyCSharp, "UberStrike.Realtime.UnitySdk", "GameFlags", uberStrike.AssemblyCSharpFirstpass.Assembly.ToAssemblyRef());
            var GAME_FLAGS_TypeRef = new TypeRefUser(uberStrike.AssemblyCSharp, string.Empty, "GAME_FLAGS", GameFlags_TypeRef);

            var GameFlags_IsFlagSet_MethodRef = new MemberRefUser(
                uberStrike.AssemblyCSharpFirstpass,
                "IsFlagSet",
                MethodSig.CreateStatic(
                    uberStrike.AssemblyCSharpFirstpass.CorLibTypes.Boolean,
                    GAME_FLAGS_TypeRef.ToTypeSig(),
                    uberStrike.AssemblyCSharpFirstpass.CorLibTypes.Int32
                ),
                GameFlags_TypeRef
            );

            /* Calls GameFlags.IsFlagSet(,) */
            ilBody.Instructions.Insert(17, OpCodes.Call.ToInstruction(GameFlags_IsFlagSet_MethodRef));

            /* Branching out if GameFlags.IsFlagSet return true;. */
            ilBody.Instructions.Insert(18, OpCodes.Brtrue_S.ToInstruction(ilBody.Instructions[24]));
        }
    }
}

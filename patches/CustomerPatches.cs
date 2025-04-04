using ApClient.mapping;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace ApClient.patches;

public class CustomerPatches
{

    [HarmonyPatch(typeof(Customer), "OnItemScanned")]
    public static class OnScan
    {
        [HarmonyPostfix]
        public static void Postfix(Item item)
        {
            CPlayerData.m_StockSoldList[(int)item.GetItemType()]++;
            Plugin.Log($"{item} has sold {CPlayerData.m_StockSoldList[(int)item.GetItemType()]} times");
            foreach(var Loc in LicenseMapping.GetKeyValueFromType(item.GetItemType()))
            {
                //Plugin.Log(Loc.Value.count)
                if (CPlayerData.m_StockSoldList[(int)item.GetItemType()] >= Loc.Value.count)
                {
                    Plugin.session.Locations.CompleteLocationChecks(Loc.Value.locid);
                }
            }
            
        }
    }

    [HarmonyPatch(typeof(CustomerTradeCardScreen), "SetCustomer")]
    public static class CustomerTradeScreen_SetCustomer_Transpiler
    {
        // We assume that the original call to GetCardData is a call instruction we can intercept.
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            // Get a reference to the private field we're interested in.
            FieldInfo field_CardDataL = AccessTools.Field(typeof(CustomerTradeCardScreen), "m_CardData_L");
            // Get the original method that creates random card data.
            MethodInfo originalGetCardData = AccessTools.Method(typeof(CPlayerData), "GetCardData", new[] { typeof(int), typeof(ECardExpansionType), typeof(bool) });
            // Get our custom builder method.
            MethodInfo myCustomMethod = AccessTools.Method(typeof(Plugin), nameof(Plugin.RandomNewCard));
            // Get the Plugin.ControlTrades() method.
            MethodInfo controlTradesMethod = AccessTools.Method(typeof(Plugin), nameof(Plugin.ControlTrades));

            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            Label skipCustom = il.DefineLabel();
            Label endIf = il.DefineLabel();
            bool patchApplied = false;

            for (int i = 0; i < codes.Count; i++)
            {
                CodeInstruction code = codes[i];

                // Look for the call to CPlayerData.GetCardData that is used to set m_CardData_L.
                if (!patchApplied &&
                    code.opcode == OpCodes.Call &&
                    code.operand is MethodInfo mi &&
                    mi == originalGetCardData)
                {
                    // We assume the method's parameters (num7, eCardExpansionType, flag) are already on the stack.
                    // We now insert our conditional logic:
                    // if (customerTradeData == null && Plugin.ControlTrades() == true)
                    //    CardData = BuildCustomCardData();
                    // else
                    //    CardData = original GetCardData(...);

                    List<CodeInstruction> newInstructions = new List<CodeInstruction>();

                    // --- Check if customerTradeData is null ---
                    // Load argument 2 (customerTradeData)
                    newInstructions.Add(new CodeInstruction(OpCodes.Ldarg_2));
                    // Load null
                    newInstructions.Add(new CodeInstruction(OpCodes.Ldnull));
                    // Compare equality (customerTradeData == null)
                    newInstructions.Add(new CodeInstruction(OpCodes.Ceq));
                    // If false (i.e. customerTradeData != null), branch to skip custom logic.
                    newInstructions.Add(new CodeInstruction(OpCodes.Brfalse_S, skipCustom));

                    // --- Now check Plugin.ControlTrades() ---
                    // Call Plugin.ControlTrades()
                    newInstructions.Add(new CodeInstruction(OpCodes.Call, controlTradesMethod));
                    // Load constant 0 (false)
                    newInstructions.Add(new CodeInstruction(OpCodes.Ldc_I4_0));
                    // Compare equality (if Plugin.ControlTrades() == false)
                    newInstructions.Add(new CodeInstruction(OpCodes.Ceq));
                    // If true (i.e. Plugin.ControlTrades() returned false), branch to skip custom.
                    newInstructions.Add(new CodeInstruction(OpCodes.Brtrue_S, skipCustom));

                    // --- If both conditions passed, we want to call BuildCustomCardData() ---
                    // Pop the three parameters that were pushed for the original call.
                    newInstructions.Add(new CodeInstruction(OpCodes.Pop)); // flag
                    newInstructions.Add(new CodeInstruction(OpCodes.Pop)); // eCardExpansionType
                    newInstructions.Add(new CodeInstruction(OpCodes.Pop)); // num7
                                                                           // Call our custom method to build the CardData.
                    newInstructions.Add(new CodeInstruction(OpCodes.Call, myCustomMethod));
                    // Branch to end, skipping the original call.
                    newInstructions.Add(new CodeInstruction(OpCodes.Br_S, endIf));

                    // --- Label for skipping custom logic ---
                    CodeInstruction labelOriginal = new CodeInstruction(OpCodes.Nop) { labels = new List<Label> { skipCustom } };
                    newInstructions.Add(labelOriginal);

                    // Yield our new instructions instead of the original call.
                    foreach (var instr in newInstructions)
                    {
                        yield return instr;
                    }
                    // Yield the original call instruction for when the branch was skipped.
                    yield return code;

                    patchApplied = true;
                    continue;
                }
                // When we reach the stfld that assigns m_CardData_L, attach the endIf label.
                if (patchApplied &&
                    code.opcode == OpCodes.Stfld &&
                    code.operand is FieldInfo field &&
                    field == field_CardDataL)
                {
                    code.labels.Add(endIf);
                    yield return code;
                    patchApplied = false; // Only patch once.
                    continue;
                }

                // Otherwise, yield the instruction unmodified.
                yield return code;
            }
        }
    }
}

using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace ApClient.patches
{
    //GetPackContent(bool clearList, bool isPremiumPack, bool isSecondaryRolledData = false, ECollectionPackType overrideCollectionPackType = ECollectionPackType.None)
    public class CardOpeningPatches
    {
        [HarmonyPatch(typeof(CardOpeningSequence), "GetPackContent")]
        class Patch_TargetMethod
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var codes = new List<CodeInstruction>(instructions);
                var floatFields = new Dictionary<float, float>
                {
                    { 5f,   .5f }, //foil starts at 5%, increases by .5 for each luck item
                    { 20f,   .25f }, //First edition
                    { 8f,   .2f }, //Silver
                    { 4f,   .1f }, //Gold
                    { 1f,   .75f }, //EX
                    { 0.25f, .05f }, //full art
                };
                
                // Track if we already patched a float
                var patchedFloats = new HashSet<float>();
                MethodInfo getNumItemsFound = AccessTools.Method(typeof(Plugin), "getNumLuckItems");

                for (int i = 0; i < codes.Count - 1; i++)
                {
                    var code = codes[i];

                    // Match float value being loaded
                    if (code.opcode == OpCodes.Ldc_R4 && floatFields.TryGetValue((float)code.operand, out float multiplier))
                    {
                        float value = (float)code.operand;

                        // Only patch the *first* time we see this value
                        if (patchedFloats.Contains(value))
                            continue;

                        // Make sure it's followed by stloc to assign to a variable
                        if (codes[i + 1].opcode.Name.StartsWith("stloc"))
                        {
                            var targetLoc = codes[i + 1].operand;

                            // Insert after the stloc: modify the value
                            codes.InsertRange(i + 2, new[]
                            {
                        new CodeInstruction(OpCodes.Ldloc, targetLoc),            // load original value
                        new CodeInstruction(OpCodes.Call, getNumItemsFound),     // call getNumItemsFound()
                        new CodeInstruction(OpCodes.Ldc_R4, multiplier),         // load multiplier
                        new CodeInstruction(OpCodes.Mul),
                        new CodeInstruction(OpCodes.Add),
                        new CodeInstruction(OpCodes.Stloc, targetLoc),           // store modified value
                    });

                            patchedFloats.Add(value);
                            i += 6; // Skip over inserted instructions
                        }
                    }
                }

                return codes;
            }
        }
    }
}

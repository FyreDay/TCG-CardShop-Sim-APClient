using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApClient.patches;

public class BillPatches
{

    [HarmonyPatch(typeof(RentBillScreen))]
    public class NewDay
    {
        [HarmonyPatch("EvaluateNewDayBill")]
        [HarmonyPostfix]
        static void PostFix(RentBillScreen __instance)
        {
            BillData bill = CPlayerData.GetBill(EBillType.Rent);
            BillData bill2 = CPlayerData.GetBill(EBillType.Electric);
            BillData bill3 = CPlayerData.GetBill(EBillType.Employee);

            bool sendDeath = false;
            if (bill != null && __instance.m_DueDayMax - CPlayerData.GetBill(EBillType.Rent).billDayPassed <= 0)
            {
                sendDeath = true;
            }
            if (bill2 != null && __instance.m_DueDayMax - CPlayerData.GetBill(EBillType.Electric).billDayPassed <= 0)
            {
                sendDeath = true;
            }
            if (bill3 != null && __instance.m_DueDayMax - CPlayerData.GetBill(EBillType.Employee).billDayPassed <= 0)
            {
                sendDeath = true;
            }

            if (sendDeath)
            {
                Plugin.m_SessionHandler.sendDeath();
            }
        }
    }
            
}

using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApClient.patches;

public class BillPatches
{
    private static bool BillRentDeath = false;
    private static bool BillElectricDeath = false;
    private static bool BillEmployeeDeath = false;

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
            if (bill != null && !BillRentDeath && __instance.m_DueDayMax - CPlayerData.GetBill(EBillType.Rent).billDayPassed <= -1)
            {
                sendDeath = true;
                BillRentDeath = true;
            }
            if (bill2 != null && !BillElectricDeath && __instance.m_DueDayMax - CPlayerData.GetBill(EBillType.Electric).billDayPassed <= -1)
            {
                sendDeath = true;
                BillElectricDeath=true;
            }
            if (bill3 != null && !BillEmployeeDeath && __instance.m_DueDayMax - CPlayerData.GetBill(EBillType.Employee).billDayPassed <= -1)
            {
                sendDeath = true;
                BillEmployeeDeath = true;
            }

            if (sendDeath)
            {
                Plugin.m_SessionHandler.sendDeath();
            }
        }
    }

    [HarmonyPatch(typeof(RentBillScreen))]
    public class PaidRent
    {
        [HarmonyPatch("OnPressPayRentBill")]
        [HarmonyPostfix]
        static void PostFix(RentBillScreen __instance)
        {
            BillRentDeath = false;
        }
    }

    [HarmonyPatch(typeof(RentBillScreen))]
    public class PaidElectric
    {
        [HarmonyPatch("OnPressPayElectricBill")]
        [HarmonyPostfix]
        static void PostFix(RentBillScreen __instance)
        {
            BillElectricDeath = false;
        }
    }

    [HarmonyPatch(typeof(RentBillScreen))]
    public class PaidSalery
    {
        [HarmonyPatch("OnPressPaySalaryBill")]
        [HarmonyPostfix]
        static void PostFix(RentBillScreen __instance)
        {
            BillEmployeeDeath = false;
        }
    }

    [HarmonyPatch(typeof(RentBillScreen))]
    public class PaidAll
    {
        [HarmonyPatch("OnPressPayAllBill")]
        [HarmonyPostfix]
        static void PostFix(RentBillScreen __instance)
        {
            BillRentDeath = false;
            BillElectricDeath = false;
            BillEmployeeDeath = false;
        }
    }
}

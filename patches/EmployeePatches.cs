﻿using ApClient.mapping;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ApClient.patches;

public class EmployeePatches
{
    [HarmonyPatch(typeof(HireWorkerPanelUI), "Init")]
    public class Init
    {
        static void Postfix(HireWorkerPanelUI __instance, HireWorkerScreen hireWorkerScreen, int index)
        {
            //Plugin.Log($"Employee Index: {index}");
            var val = EmployeeMapping.mapping[index];
            __instance.m_LevelRequirementText.text = $"Requires AP Worker Unlock";

            if (Plugin.m_SessionHandler.hasItem(val.itemid))
            {
                __instance.m_LevelRequirementText.gameObject.SetActive(value: false);
                __instance.m_HireFeeText.gameObject.SetActive(value: true);
                __instance.m_LockPurchaseBtn.SetActive(value: false);
            }
            else {
                __instance.m_LevelRequirementText.gameObject.SetActive(value: true);
                __instance.m_HireFeeText.gameObject.SetActive(value: false);
                __instance.m_LockPurchaseBtn.gameObject.SetActive(value: true);
            }
            
        }
    }

    [HarmonyPatch(typeof(HireWorkerPanelUI), "OnPressHireButton")]
    public class OnClick
    {
        static bool Prefix(HireWorkerPanelUI __instance)
        {
            FieldInfo fieldInfo = typeof(HireWorkerPanelUI).GetField("m_Index", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo == null)
            {
                return false;
            }

            int index = (int)fieldInfo.GetValue(__instance);
            var val = EmployeeMapping.mapping[index];
            //Plugin.Log($"at employee hire {val.name} {val.itemid} {val.locid != -1 && Plugin.m_SessionHandler.hasItem(val.itemid)}");
            if (Plugin.m_SessionHandler.hasItem(val.itemid))
            {
                HireEmployee(__instance, index);
            }
            return true;
        }
    }

    public static void HireEmployee(HireWorkerPanelUI __instance, int index)
    {
        if (CPlayerData.GetIsWorkerHired(index))
        {
            return;
        }

        if(CPlayerData.m_CoinAmount < __instance.m_TotalHireFee)
        {
            NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.Money);
            return;
        }

        CEventManager.QueueEvent(new CEventPlayer_ReduceCoin(__instance.m_TotalHireFee));
        CPlayerData.SetIsWorkerHired(index, isHired: true);
        //send to AP
        //Plugin.session.Locations.CompleteLocationChecks(EmployeeMapping.mapping.GetValueOrDefault(index).locid);

        CSingleton<WorkerManager>.Instance.ActivateWorker(index, resetTask: true);
        CPlayerData.m_GameReportDataCollect.employeeCost -= __instance.m_TotalHireFee;
        CPlayerData.m_GameReportDataCollectPermanent.employeeCost -= __instance.m_TotalHireFee;
        int num = 0;
        for (int i = 0; i < CPlayerData.m_IsWorkerHired.Count; i++)
        {
            if (CPlayerData.m_IsWorkerHired[i])
            {
                num++;
            }
        }
        FieldInfo fieldInfoHired = typeof(HireWorkerPanelUI).GetField("m_IsHired", BindingFlags.NonPublic | BindingFlags.Instance);
        if (fieldInfoHired == null)
        {
            return;
        }
        AchievementManager.OnStaffHired(num);

        SoundManager.PlayAudio("SFX_CustomerBuy", 0.6f);
        fieldInfoHired.SetValue(__instance, CPlayerData.GetIsWorkerHired(index));
        bool m_IsHired = CPlayerData.GetIsWorkerHired(index);
        if (m_IsHired)
        {
            __instance.m_HiredText.SetActive(value: true);
            __instance.m_PurchaseBtn.SetActive(value: false);
            __instance.m_HireFeeText.gameObject.SetActive(value: false);
        }
        else
        {
            __instance.m_HiredText.SetActive(value: false);
            __instance.m_PurchaseBtn.SetActive(value: true);
        }
    }

}

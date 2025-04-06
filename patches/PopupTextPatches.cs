using HarmonyLib;
using I2.Loc;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ApClient.patches;

public class PopupTextPatches
{
    private static string currentText;
    public static void ShowCustomText(string notEnoughResourceText)
    {
        if (currentText == notEnoughResourceText)
        {
            return;
        }

        CSingleton<NotEnoughResourceTextPopup>.Instance.m_ResetTimer = 0f;
        currentText = notEnoughResourceText;
        for (int i = 0; i < CSingleton<NotEnoughResourceTextPopup>.Instance.m_ShowTextGameObjectList.Count; i++)
        {
            if (!CSingleton<NotEnoughResourceTextPopup>.Instance.m_ShowTextGameObjectList[i].activeSelf)
            {
                CSingleton<NotEnoughResourceTextPopup>.Instance.m_ShowTextList[i].text = notEnoughResourceText;
                CSingleton<NotEnoughResourceTextPopup>.Instance.m_ShowTextGameObjectList[i].gameObject.SetActive(value: true);
                break;
            }
        }
    }
    [HarmonyPatch]
    public class ShowText
    {
        static MethodBase TargetMethod()
        {
            var type = typeof(NotEnoughResourceTextPopup); // Singleton class, CPlayerData
            var method = type.GetMethod("ShowText", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static); // Static method

            if (method == null)
            {
                Plugin.Log("Static method 'ShowText' not found!");
            }

            return method;
        }
        static void Prefix(ENotEnoughResourceText notEnoughResourceText)
        {
            currentText = notEnoughResourceText.ToString();
        }
    }
}


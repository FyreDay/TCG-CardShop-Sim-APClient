using HarmonyLib;
using I2.Loc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace ApClient.Patches.Functionality;

public class PopupTextPatches
{
    private static Queue<string> textQueue = new Queue<string>();
    private static bool isProcessing = false;

    public static void ShowCustomText(string notEnoughResourceText)
    {
        var popup = CSingleton<NotEnoughResourceTextPopup>.Instance;

        if (popup == null)
        {
            Plugin.Logger.LogError("Popup instance is null");
            return;
        }

        // If already running → queue it
        if (popup.m_ResetTimer > 0f || isProcessing)
        {
            textQueue.Enqueue(notEnoughResourceText);
            return;
        }

        // Otherwise start immediately
        popup.StartCoroutine(ProcessQueue(notEnoughResourceText));
    }

    private static IEnumerator ProcessQueue(string firstText)
    {
        var popup = CSingleton<NotEnoughResourceTextPopup>.Instance;
        isProcessing = true;

        // Show first text
        yield return ShowTextRoutine(firstText);

        // Process queue
        while (textQueue.Count > 0)
        {
            string next = textQueue.Dequeue();
            yield return ShowTextRoutine(next);
        }

        isProcessing = false;
    }

    private static IEnumerator ShowTextRoutine(string text)
    {
        var popup = CSingleton<NotEnoughResourceTextPopup>.Instance;

        popup.m_ResetTimer = 0f;

        for (int i = 0; i < popup.m_ShowTextGameObjectList.Count; i++)
        {
            if (!popup.m_ShowTextGameObjectList[i].activeSelf)
            {
                popup.m_ShowTextList[i].text = text;
                popup.m_ShowTextGameObjectList[i].SetActive(true);
                break;
            }
        }

        // Wait until timer finishes
        while (popup.m_ResetTimer > 0f)
        {
            yield return null;
        }

        // Small buffer so texts don't overlap visually
        yield return new WaitForSeconds(0.1f);
    }
    //public static bool ShowCustomText(string notEnoughResourceText)
    //{

    //    if (CSingleton<NotEnoughResourceTextPopup>.Instance.m_ResetTimer > 0f)
    //    {
    //        return;
    //    }
    //    CSingleton<NotEnoughResourceTextPopup>.Instance.m_ResetTimer = 0f;
    //    currentText = notEnoughResourceText;
    //    for (int i = 0; i < CSingleton<NotEnoughResourceTextPopup>.Instance.m_ShowTextGameObjectList.Count; i++)
    //    {
    //        if (!CSingleton<NotEnoughResourceTextPopup>.Instance.m_ShowTextGameObjectList[i].activeSelf)
    //        {
    //            CSingleton<NotEnoughResourceTextPopup>.Instance.m_ShowTextList[i].text = notEnoughResourceText;
    //            CSingleton<NotEnoughResourceTextPopup>.Instance.m_ShowTextGameObjectList[i].gameObject.SetActive(value: true);
    //            break;
    //        }
    //    }
    //}
}


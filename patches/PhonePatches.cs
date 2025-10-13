using ApClient.ui;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ApClient.patches;

public class PhonePatches
{
    [HarmonyPatch(typeof(PhoneManager), "Awake")]
    public static class PhoneManager_Awake_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(PhoneManager __instance)
        {
            
            // Ensure UI is ready
            var phoneScreen = __instance.m_UI_PhoneScreen;
            if (phoneScreen == null)
                return;

            // Reference an existing button as a template
            if (phoneScreen.m_PhoneButtonList.Count == 0)
                return;

            Transform referenceGrp = phoneScreen.transform.Find("ScreenGrp/AnimGrp/PhoneButtonGrp_CustomerReview");

            // Instantiate as sibling, parent = the button container
            Transform parentContainer = referenceGrp.parent;

            GameObject newButtonObj = UnityEngine.Object.Instantiate(referenceGrp.gameObject, parentContainer);
            newButtonObj.name = "PhoneButtonGrp_APInfo";

            RectTransform rt = newButtonObj.GetComponent<RectTransform>();
            rt.anchoredPosition += new Vector2(0, 8);

            Image bg = newButtonObj.transform.Find("BG").GetComponent<Image>();
            Image border = newButtonObj.transform.Find("BG2").GetComponent<Image>();

            bg.color = Color.green;
            border.color = Color.black;

            TextMeshProUGUI tmpText = newButtonObj.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            tmpText.text = "AP Info";
            tmpText.color = Color.cyan;


            ControllerButton controllerButton = newButtonObj.GetComponent<ControllerButton>();
            Button newButton = controllerButton.m_Button;
            if (newButton != null)
            {
                Plugin.Log("button not null");
                newButton.onClick.AddListener(() =>
                {
                    if (APinfoMenu.Instance != null)
                    {
                        Plugin.Log("ap info open!!!");
                        APinfoMenu.Instance.setVisable(true);
                    }
                    else
                    {
                        Plugin.Log("ap info nullllll");
                    }

                });
            }
            else
            {
                Plugin.Log("button is nullllllllllllllllllll");
            }
        }
    }


    [HarmonyPatch(typeof(UI_PhoneScreen))]
    public class ControllerPatches
    {
        [HarmonyPatch("OnOpenScreen")]
        [HarmonyPostfix]
        static void OnFinishPostFix(UI_PhoneScreen __instance)
        {
            Plugin.Log($"button count: { __instance.m_PhoneButtonList.Count}");

        }
    }
}

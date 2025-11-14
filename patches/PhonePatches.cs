using ApClient.ui;
using HarmonyLib;
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
            rt.anchoredPosition += new Vector2(0, 7);

            Image bg = newButtonObj.transform.Find("BG").GetComponent<Image>();
            Image border = newButtonObj.transform.Find("BG2").GetComponent<Image>();

            //bg.color = Color.green;
            border.color = Color.white;

            TextMeshProUGUI tmpText = newButtonObj.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            tmpText.text = "AP Info";
            //tmpText.color = Color.cyan;

            Transform iconRoot = newButtonObj.transform.Find("Icon");

            Transform icon1 = iconRoot.Find("Icon (1)");
            Transform icon2 = iconRoot.Find("Icon (2)");
            icon1.gameObject.SetActive(false);
            icon2.gameObject.SetActive(false);

            Texture2D tex = EmbeddedResources.LoadTexture("ApClient.assets.color-icon.png");
            if (tex == null)
            {
                Debug.LogError("Failed to load embedded icon texture!");
            }
            else
            {

                Transform icon = iconRoot.Find("Icon");
                if (icon != null)
                {
                    Image img = icon.GetComponent<Image>();
                    RectTransform rect = icon.GetComponent<RectTransform>();
                    rect.anchoredPosition += new Vector2(0, -2F);
                    Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                    img.sprite = sprite;
                    img.preserveAspect = true; // keeps proportions clean
                }
                else
                {
                    Debug.LogWarning("Could not find Icon child on button!");
                }
            }
            ControllerButton controllerButton = newButtonObj.GetComponent<ControllerButton>();
            Button newButton = controllerButton.m_Button;
            if (newButton != null)
            {
                Plugin.Log("button not null");
                newButton.onClick.AddListener(() =>
                {
                    if (UIInfoPanel.getInstance() != null)
                    {
                        Plugin.Log("ap info open!!!");
                        UIInfoPanel.getInstance().setVisable(true);
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

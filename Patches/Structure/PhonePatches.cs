using ApClient.ui;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ApClient.Patches.Structure;

public class PhonePatches
{

    private static GameObject apButton;

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

            if (apButton != null)
            {
                Plugin.Logger.LogWarning("AP app already exists, skipping button creation.");
                return;
            }
            Transform referenceGrp = phoneScreen.transform.Find("ScreenGrp/AnimGrp/PhoneButtonGrp_CustomerReview");

            // Instantiate as sibling, parent = the button container
            Transform parentContainer = referenceGrp.parent;

            apButton = UnityEngine.Object.Instantiate(referenceGrp.gameObject, parentContainer);
            apButton.name = "PhoneButtonGrp_APInfo";

            RectTransform rt = apButton.GetComponent<RectTransform>();
            rt.anchoredPosition += new Vector2(-6, 12);

            Image bg = apButton.transform.Find("BG").GetComponent<Image>();
            Image border = apButton.transform.Find("BG2").GetComponent<Image>();

            //bg.color = Color.green;
            border.color = Color.white;

            TextMeshProUGUI tmpText = apButton.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            tmpText.text = "";
            //tmpText.text = "AP Info";
            //tmpText.color = Color.cyan;

            Transform iconRoot = apButton.transform.Find("Icon");

            Transform icon1 = iconRoot.Find("Icon (1)");
            Transform icon2 = iconRoot.Find("Icon (2)");
            icon1.gameObject.SetActive(false);
            icon2.gameObject.SetActive(false);

            Texture2D tex = Util.LoadTexture("ApClient.assets.color-icon.png");
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
            ControllerButton controllerButton = apButton.GetComponent<ControllerButton>();
            Button newButton = controllerButton.m_Button;
            if (newButton != null)
            {
                newButton.onClick.AddListener(() =>
                {
                    if (UIInfoPanel.getInstance() != null)
                    {
                        UIInfoPanel.getInstance().setVisable(true);
                    }

                });
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;


using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ApClient.ui;
public class ConnectionMenu : MonoBehaviour
{
    private GameObject canvasObj;
    private TMP_InputField ipPortInput;
    private TMP_InputField passwordInput;
    private TMP_InputField slotInput;
    private TMP_Text statusText;

    private bool isVisible = false;

    void Start()
    {
        CreateUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(Settings.Instance.MyHotkey.Value))
        {
            isVisible = !isVisible;
            canvasObj.SetActive(isVisible);
        }
    }

    void CreateUI()
    {
        // Root Canvas
        canvasObj = new GameObject("APMenuCanvas");
        var canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObj.AddComponent<GraphicRaycaster>();

        // EventSystem required for buttons/inputs
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        // Background Panel
        var panel = new GameObject("Panel");
        panel.transform.SetParent(canvasObj.transform);
        var img = panel.AddComponent<Image>();
        img.color = new Color(0, 0, 0, 0.85f);
        var rt = panel.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(300, 300);
        rt.anchoredPosition = Vector2.zero;

        // Address Input
        ipPortInput = CreateInput(panel.transform, "Address:Port", Settings.Instance.LastUsedIP.Value, 80);
        // Password Input
        passwordInput = CreateInput(panel.transform, "Password", Settings.Instance.LastUsedPassword.Value, 30);
        // Slot Input
        slotInput = CreateInput(panel.transform, "Slot", Settings.Instance.LastUsedSlot.Value, -20);

        // Connect Button
        var buttonObj = CreateButton(panel.transform, "Connect", -70);
        buttonObj.onClick.AddListener(OnConnectPressed);

        // Status Text
        var statusObj = new GameObject("StatusText");
        statusObj.transform.SetParent(panel.transform);
        statusText = statusObj.AddComponent<TextMeshProUGUI>();
        var srt = statusText.GetComponent<RectTransform>();
        srt.sizeDelta = new Vector2(280, 40);
        srt.anchoredPosition = new Vector2(0, -120);
        statusText.alignment = TextAlignmentOptions.Center;
        statusText.color = Color.white;
        statusText.fontSize = 16;
        statusText.text = "Not Connected";

        canvasObj.SetActive(false);
    }

    TMP_InputField CreateInput(Transform parent, string placeholder, string defaultText, float yOffset)
    {
        var go = new GameObject(placeholder + "Input");
        go.transform.SetParent(parent);
        var rt = go.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(260, 30);
        rt.anchoredPosition = new Vector2(0, yOffset);

        var bg = go.AddComponent<Image>();
        bg.color = new Color(1, 1, 1, 0.1f);

        var input = go.AddComponent<TMP_InputField>();

        var text = new GameObject("Text").AddComponent<TextMeshProUGUI>();
        text.transform.SetParent(go.transform);
        text.rectTransform.anchorMin = new Vector2(0, 0);
        text.rectTransform.anchorMax = new Vector2(1, 1);
        text.rectTransform.offsetMin = new Vector2(10, 5);
        text.rectTransform.offsetMax = new Vector2(-10, -5);
        text.text = defaultText;
        text.fontSize = 16;
        text.color = Color.white;
        input.textComponent = text;

        var ph = new GameObject("Placeholder").AddComponent<TextMeshProUGUI>();
        ph.transform.SetParent(go.transform);
        ph.rectTransform.anchorMin = new Vector2(0, 0);
        ph.rectTransform.anchorMax = new Vector2(1, 1);
        ph.rectTransform.offsetMin = new Vector2(10, 5);
        ph.rectTransform.offsetMax = new Vector2(-10, -5);
        ph.text = placeholder;
        ph.fontSize = 16;
        ph.color = new Color(1, 1, 1, 0.5f);
        input.placeholder = ph;

        input.text = defaultText;
        return input;
    }

    Button CreateButton(Transform parent, string label, float yOffset)
    {
        var go = new GameObject(label + "Button");
        go.transform.SetParent(parent);
        var rt = go.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(180, 40);
        rt.anchoredPosition = new Vector2(0, yOffset);

        var img = go.AddComponent<Image>();
        img.color = new Color(0.3f, 0.3f, 0.3f, 1);

        var btn = go.AddComponent<Button>();
        var txt = new GameObject("Label").AddComponent<TextMeshProUGUI>();
        txt.transform.SetParent(go.transform);
        txt.text = label;
        txt.alignment = TextAlignmentOptions.Center;
        txt.color = Color.white;
        txt.fontSize = 16;
        txt.rectTransform.anchorMin = new Vector2(0, 0);
        txt.rectTransform.anchorMax = new Vector2(1, 1);
        txt.rectTransform.offsetMin = txt.rectTransform.offsetMax = Vector2.zero;

        return btn;
    }

    void OnConnectPressed()
    {
        string ip = ipPortInput.text;
        string pwd = passwordInput.text;
        string slot = slotInput.text;

        statusText.text = $"Connecting to {ip}...";
        Plugin.m_SessionHandler.connect(ip, pwd, slot);
    }
}
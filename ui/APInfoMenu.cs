using System;
using System.Collections.Generic;
using System.Text;

namespace ApClient.ui;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class APinfoMenu : UIScreenBase
{
    public static APinfoMenu Instance;

    private Canvas canvas;
    private RectTransform window;

    private TextMeshProUGUI titleText;

    public void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);

        CreateUI();
    }

    private void CreateUI()
    {
        // Find or create main canvas
        canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject cObj = new GameObject("APInfoCanvas");
            canvas = cObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000;

            CanvasScaler scaler = cObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            cObj.AddComponent<GraphicRaycaster>();
        }

        // Background panel
        GameObject bgObj = new GameObject("APInfoBackground", typeof(Image));
        bgObj.transform.SetParent(canvas.transform, false);

        Image bgImage = bgObj.GetComponent<Image>();
        bgImage.color = new Color(0.1f, 0.1f, 0.1f, 0.92f);
        bgImage.type = Image.Type.Sliced;

        Outline outline = bgObj.AddComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(2, -2);

        window = bgObj.GetComponent<RectTransform>();

        // Half width, full height, centered
        window.sizeDelta = new Vector2(Screen.width / 2f, Screen.height);
        window.anchorMin = new Vector2(0.5f, 0.5f);
        window.anchorMax = new Vector2(0.5f, 0.5f);
        window.pivot = new Vector2(0.5f, 0.5f);
        window.anchoredPosition = Vector2.zero;

        // Title text
        GameObject titleObj = new GameObject("TitleText", typeof(TextMeshProUGUI));
        titleObj.transform.SetParent(bgObj.transform, false);

        titleText = titleObj.GetComponent<TextMeshProUGUI>();
        titleText.text = "AP Info";
        titleText.fontSize = 36;
        titleText.color = Color.white;
        titleText.alignment = TextAlignmentOptions.Center;

        RectTransform titleRT = titleObj.GetComponent<RectTransform>();
        titleRT.anchorMin = new Vector2(0.5f, 1f);
        titleRT.anchorMax = new Vector2(0.5f, 1f);
        titleRT.pivot = new Vector2(0.5f, 1f);
        titleRT.anchoredPosition = new Vector2(0, -20); // 20px down from top
        titleRT.sizeDelta = new Vector2(window.sizeDelta.x, 60); // height of title bar

        // Close button
        GameObject closeButtonObj = new GameObject("CloseButton", typeof(Image), typeof(Button));
        closeButtonObj.transform.SetParent(bgObj.transform, false);

        Image closeImg = closeButtonObj.GetComponent<Image>();
        closeImg.color = Color.red; // simple red square; you can swap with a sprite later

        Button closeBtn = closeButtonObj.GetComponent<Button>();
        closeBtn.onClick.AddListener(() =>
        {
            Hide();
        });

        RectTransform closeRT = closeButtonObj.GetComponent<RectTransform>();
        closeRT.anchorMin = new Vector2(1f, 1f);
        closeRT.anchorMax = new Vector2(1f, 1f);
        closeRT.pivot = new Vector2(1f, 1f);
        closeRT.anchoredPosition = new Vector2(-10, -10); // 10px padding from top-right corner
        closeRT.sizeDelta = new Vector2(40, 40); // size of the button

        // Optional: Add a "X" TextMeshProUGUI for visual
        GameObject xTextObj = new GameObject("X", typeof(TextMeshProUGUI));
        xTextObj.transform.SetParent(closeButtonObj.transform, false);

        TextMeshProUGUI xText = xTextObj.GetComponent<TextMeshProUGUI>();
        xText.text = "X";
        xText.alignment = TextAlignmentOptions.Center;
        xText.fontSize = 30;
        xText.color = Color.white;

        RectTransform xRT = xTextObj.GetComponent<RectTransform>();
        xRT.anchorMin = Vector2.zero;
        xRT.anchorMax = Vector2.one;
        xRT.pivot = new Vector2(0.5f, 0.5f);
        xRT.sizeDelta = Vector2.zero;
    }
    public void Show()
    {
        Plugin.Log("Open Info Screen");
        //gameObject.SetActive(true);
        base.OnOpenScreen();
    }

    public void Hide()
    {
        Plugin.Log("CLOSE THE SCREEN");
        //gameObject.SetActive(false);
        base.CloseScreen();
    }
}
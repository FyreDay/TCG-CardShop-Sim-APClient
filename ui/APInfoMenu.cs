using System;
using System.Collections.Generic;
using System.Text;

namespace ApClient.ui;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class APinfoMenu : MonoBehaviour
{
    public static APinfoMenu Instance;

    private bool showGUI = false;

    public void setVisable(bool visable)
    {
        Plugin.Log("toggle Ap info");
        showGUI = visable;
        if (window != null)
        {
            Plugin.Log("instance not null");
            window.gameObject.SetActive(showGUI);
        }

    }

    public void toggleVisability()
    {
        setVisable(!showGUI);
    }

    private Canvas canvas;
    private RectTransform window;
    void Start()
    {
        Instance = this;
        // Create main Canvas
        canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject cObj = new GameObject("InfoConnectionCanvas");
            canvas = cObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000;

            CanvasScaler scaler = cObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            cObj.AddComponent<GraphicRaycaster>();
        }

        // Background
        GameObject bgObj = new GameObject("InfoMenuBackground", typeof(Image));
        bgObj.transform.SetParent(canvas.transform, false);
        Image bgImage = bgObj.GetComponent<Image>();
        bgImage.color = new Color(0.1f, 0.1f, 0.1f, 0.92f);
        //bgImage.sprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd");
        bgImage.type = Image.Type.Sliced;

        Outline outline = bgObj.AddComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(2, -2);

        window = bgObj.GetComponent<RectTransform>();
        window.sizeDelta = new Vector2(320, 320);
        window.pivot = new Vector2(0, 1);
        window.anchoredPosition = new Vector2(20, -20);

        // Title
        TMP_Text title = CreateText("AP Info", new Vector2(0, 135), 20, bgObj.transform);
        title.alignment = TextAlignmentOptions.Center;
    }

    void Update()
    {
        // Toggle with hotkey
        if (Input.GetKeyDown(KeyCode.F7))
        {
            showGUI = !showGUI;
            if (window != null)
                window.gameObject.SetActive(showGUI);
        }

        //// Toggle with hotkey
        //if (showGUI &&
        //    CSingleton<PhoneManager>.Instance.m_UI_PhoneScreen.m_IsScreenOpen &&
        //    Input.GetKeyDown(KeyCode.Tab))
        //{
        //    Plugin.Log("close custom window");
        //    setVisable(false);

        //}
    }


    // --- Helpers ---
    private TMP_Text CreateText(string text, Vector2 pos, int size, Transform parent)
    {
        GameObject obj = new GameObject(text, typeof(TextMeshProUGUI));
        obj.transform.SetParent(parent, false);
        var tmp = obj.GetComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Left;
        RectTransform rect = tmp.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(280, 30);
        rect.anchoredPosition = pos;
        return tmp;
    }

    private TMP_Text CreateLabel(string text, Vector2 pos, Transform parent)
    {
        TMP_Text tmp = CreateText(text, pos, 14, parent);
        tmp.color = new Color(0.8f, 0.8f, 0.8f, 1f);
        tmp.alignment = TextAlignmentOptions.MidlineLeft;
        return tmp;
    }
  
}
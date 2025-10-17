using System;
using System.Collections.Generic;
using System.Text;

namespace ApClient.ui;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class APinfoMenu : MonoBehaviour
{
    public static APinfoMenu Instance;

    private bool showGUI = true;

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

    public void DelaySetVisable(bool visible)
    {
        Plugin.Log($"Toggle AP Info: {visible}");
        showGUI = visible;

        if (window == null)
            return;

        StopAllCoroutines(); // cancel any previous hide coroutine


        StartCoroutine(DelayedHide(0.3f, visible)); // delay 0.5 seconds; adjust as needed
    }

    private IEnumerator DelayedHide(float delay, bool visable)
    {
        yield return new WaitForSeconds(delay);
        if (!showGUI) // confirm it's still meant to be hidden
        {
            Plugin.Log("Hiding info screen after delay");
            window.gameObject.SetActive(false);
        }
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
        window.anchorMin = new Vector2(0.25f, 0f); // start 25% from left
        window.anchorMax = new Vector2(0.75f, 1f); // end 75% from left (half screen width)
        window.offsetMin = Vector2.zero;
        window.offsetMax = Vector2.zero;
        window.pivot = new Vector2(0.5f, 0.5f);
        // Title
        TMP_Text title = CreateText("AP Info", new Vector2(0, 135), 20, bgObj.transform);
        title.alignment = TextAlignmentOptions.Center;
        setVisable(false);
    }

    void Update()
    {
        //// Toggle with hotkey
        //if (Input.GetKeyDown(KeyCode.F7))
        //{
        //    showGUI = !showGUI;
        //    if (window != null)
        //        window.gameObject.SetActive(showGUI);
        //}

        // Toggle with hotkey
        if (showGUI && (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Escape)))
        {
            Plugin.Log("close custom window");
            DelaySetVisable(false);

        }
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
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

    private RectTransform window;

    public void Awake()
    {
        Instance = this;

        // Parent to phone screen container
        var phoneScreen = PhoneManager.m_Instance?.m_UI_PhoneScreen;
        if (phoneScreen != null)
        {
            transform.SetParent(phoneScreen.transform, false);
        }

        // Create UI
        CreateUI();

        // Start hidden
        base.CloseScreen();
    }

    private void CreateUI()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning("APinfoMenu: No parent canvas found!");
            return;
        }

        // Background panel
        GameObject bgObj = new GameObject("MenuBackground", typeof(Image));
        bgObj.transform.SetParent(transform, false);
        Image bgImage = bgObj.GetComponent<Image>();
        bgImage.color = new Color(0.1f, 0.1f, 0.1f, 0.92f);
        bgImage.type = Image.Type.Sliced;

        // Outline
        Outline outline = bgObj.AddComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(2, -2);

        // RectTransform
        window = bgObj.GetComponent<RectTransform>();
        window.sizeDelta = new Vector2(Screen.width / 2f, Screen.height);
        window.anchorMin = new Vector2(0.5f, 0.5f);
        window.anchorMax = new Vector2(0.5f, 0.5f);
        window.pivot = new Vector2(0.5f, 0.5f);
        window.anchoredPosition = Vector2.zero;

        // Title
        GameObject titleObj = new GameObject("Title", typeof(TextMeshProUGUI));
        titleObj.transform.SetParent(bgObj.transform, false);
        TextMeshProUGUI titleText = titleObj.GetComponent<TextMeshProUGUI>();
        titleText.text = "AP Info";
        titleText.fontSize = 36;
        titleText.color = Color.cyan;
        titleText.alignment = TextAlignmentOptions.Center;
        RectTransform titleRT = titleObj.GetComponent<RectTransform>();
        titleRT.anchorMin = new Vector2(0.5f, 1f);
        titleRT.anchorMax = new Vector2(0.5f, 1f);
        titleRT.pivot = new Vector2(0.5f, 1f);
        titleRT.anchoredPosition = new Vector2(0, -20);
        titleRT.sizeDelta = new Vector2(window.sizeDelta.x, 50);

        // X Button
        GameObject closeBtnObj = new GameObject("CloseButton", typeof(Button), typeof(Image));
        closeBtnObj.transform.SetParent(bgObj.transform, false);
        Image closeImg = closeBtnObj.GetComponent<Image>();
        closeImg.color = Color.red;
        RectTransform closeRT = closeBtnObj.GetComponent<RectTransform>();
        closeRT.anchorMin = new Vector2(1f, 1f);
        closeRT.anchorMax = new Vector2(1f, 1f);
        closeRT.pivot = new Vector2(1f, 1f);
        closeRT.anchoredPosition = new Vector2(-10, -10);
        closeRT.sizeDelta = new Vector2(40, 40);

        Button closeBtn = closeBtnObj.GetComponent<Button>();
        closeBtn.onClick.AddListener(() =>
        {
            Hide();
        });
    }

    public void Show()
    {
        base.OnOpenScreen();
    }

    public void Hide()
    {
        base.CloseScreen();
    }
}

namespace ApClient.ui;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ConnectionMenu : MonoBehaviour
{
    public static ConnectionMenu Instance; 
    private Canvas canvas;
    private RectTransform window;

    private TMP_InputField ipField, passField, slotField;
    private TMP_Text stateLabel;

    public bool showGUI = true;
    public static string state = "Not Connected";
    public void setVisable(bool visable)
    {
        showGUI = visable;
        if (window != null)
            window.gameObject.SetActive(showGUI);
    }

    public void toggleVisability()
    {
        setVisable(!showGUI);
    }
    void Start()
    {
        Instance = this;
        // Create main Canvas
        canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject cObj = new GameObject("ConnectionCanvas");
            canvas = cObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000;

            CanvasScaler scaler = cObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            cObj.AddComponent<GraphicRaycaster>();
        }

        // Background
        GameObject bgObj = new GameObject("MenuBackground", typeof(Image));
        bgObj.transform.SetParent(canvas.transform, false);
        Image bgImage = bgObj.GetComponent<Image>();
        bgImage.color = new Color(0.1f, 0.1f, 0.1f, 0.92f);
        //bgImage.sprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd");
        bgImage.type = Image.Type.Sliced;

        Outline outline = bgObj.AddComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(2, -2);

        window = bgObj.GetComponent<RectTransform>();
        window.sizeDelta = new Vector2(320, 350);
        window.anchorMin = new Vector2(0, 1);
        window.anchorMax = new Vector2(0, 1);
        window.pivot = new Vector2(0, 1);
        window.anchoredPosition = new Vector2(20, -20);

        // Title
        TMP_Text title = CreateText("AP Client", new Vector2(0, 150), 20, bgObj.transform);
        title.alignment = TextAlignmentOptions.Center;

        Texture2D logoTex = EmbeddedResources.LoadTexture("ApClient.assets.color-icon.png");
        if (logoTex != null)
        {
            GameObject logoObj = new GameObject("Logo", typeof(Image));
            logoObj.transform.SetParent(bgObj.transform, false);

            Image logoImage = logoObj.GetComponent<Image>();
            Sprite logoSprite = Sprite.Create(logoTex,
                new Rect(0, 0, logoTex.width, logoTex.height),
                new Vector2(0.5f, 0.5f));
            logoImage.sprite = logoSprite;

            RectTransform logoRect = logoObj.GetComponent<RectTransform>();
            logoRect.sizeDelta = new Vector2(50, 50); // adjust as needed
            logoRect.anchoredPosition = new Vector2(0, 110); // just below the title
        }

        // Address Input
        CreateLabel("Address:Port", new Vector2(0, 80), bgObj.transform);
        ipField = CreateInput(Settings.Instance.LastUsedIP.Value, new Vector2(0, 55), bgObj.transform);

        // Password Input
        CreateLabel("Password", new Vector2(0, 20), bgObj.transform);
        passField = CreateInput(Settings.Instance.LastUsedPassword.Value, new Vector2(0, -5), bgObj.transform);

        // Slot Input
        CreateLabel("Slot", new Vector2(0, -40), bgObj.transform);
        slotField = CreateInput(Settings.Instance.LastUsedSlot.Value, new Vector2(0, -65), bgObj.transform);

        // Connect Button
        GameObject buttonObj = new GameObject("ConnectButton", typeof(Image), typeof(Button));
        buttonObj.transform.SetParent(bgObj.transform, false);
        Image btnImage = buttonObj.GetComponent<Image>();
        //btnImage.sprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd");
        btnImage.type = Image.Type.Sliced;
        btnImage.color = new Color(0.2f, 0.4f, 0.8f, 0.9f);

        RectTransform btnRect = buttonObj.GetComponent<RectTransform>();
        btnRect.sizeDelta = new Vector2(160, 35);
        btnRect.anchoredPosition = new Vector2(0, -100);

        TMP_Text btnText = CreateText("Connect", Vector2.zero, 18, buttonObj.transform);
        btnText.alignment = TextAlignmentOptions.Center;

        Button button = buttonObj.GetComponent<Button>();
        button.onClick.AddListener(OnConnectPressed);

        // State label
        stateLabel = CreateText("Not Connected", new Vector2(0, -140), 14, bgObj.transform);
        stateLabel.alignment = TextAlignmentOptions.Center;
    }

    void Update()
    {
        // Toggle with hotkey
        if (Input.GetKeyDown(Settings.Instance.MyHotkey.Value))
        {
            showGUI = !showGUI;
            if (window != null)
                window.gameObject.SetActive(showGUI);
        }

        stateLabel.text = state;
    }

    private void OnConnectPressed()
    {
        Debug.Log("Connect pressed!");
        Plugin.m_SessionHandler.connect(ipField.text, passField.text, slotField.text);
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

    private TMP_InputField CreateInput(string initial, Vector2 pos, Transform parent)
    {
        GameObject inputObj = new GameObject("InputField", typeof(Image), typeof(TMP_InputField));
        inputObj.transform.SetParent(parent, false);

        Image bg = inputObj.GetComponent<Image>();
        //bg.sprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/InputFieldBackground.psd");
        bg.type = Image.Type.Sliced;
        bg.color = new Color(0.2f, 0.2f, 0.2f, 1f);

        RectTransform rect = inputObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(200, 35);
        rect.anchoredPosition = pos;

        GameObject textArea = new GameObject("TextArea", typeof(RectMask2D));
        textArea.transform.SetParent(inputObj.transform, false);
        RectTransform textRect = textArea.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(5, 5);
        textRect.offsetMax = new Vector2(-5, -5);

        TMP_Text textComp = new GameObject("Text", typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
        textComp.transform.SetParent(textArea.transform, false);
        textComp.text = initial;
        textComp.fontSize = 14;
        textComp.color = Color.white;
        textComp.alignment = TextAlignmentOptions.Left;

        RectTransform textCompRect = textComp.GetComponent<RectTransform>();
        textCompRect.anchorMin = Vector2.zero;
        textCompRect.anchorMax = Vector2.one;
        textCompRect.offsetMin = Vector2.zero;
        textCompRect.offsetMax = Vector2.zero;

        TMP_InputField input = inputObj.GetComponent<TMP_InputField>();
        input.textViewport = textRect;
        input.textComponent = textComp;
        input.text = initial;

        return input;
    }
}
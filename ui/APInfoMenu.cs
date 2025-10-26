

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
namespace ApClient.ui;
public class APinfoMenu : MonoBehaviour
{
    public static APinfoMenu Instance;

    private bool showGUI = true;
    private RectTransform scrollContent;

    // References for the buttons
    private Button cardOpenButton;
    private Button cardSellButton;
    private Button cardGradeButton;

    public List<CardLocation> cardOpenItems = new List<CardLocation>();
    public List<CardLocation> cardSellItems = new List<CardLocation>();
    public List<CardLocation> cardGradeItems = new List<CardLocation>();


    public void setCardOpenList(List<CardLocation> list)
    {
        cardOpenItems = list;
        UpdateList(cardOpenItems);
    }

    public void setCardSellList(List<CardLocation> list)
    {
        cardSellItems = list;
        UpdateList(cardSellItems);
    }

    public void setCardGradeList(List<CardLocation> list)
    {
        cardGradeItems = list;
        UpdateList(cardGradeItems);
    }

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


        StartCoroutine(DelayedHide(0.15f, visible)); // delay 0.5 seconds; adjust as needed
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
        bgImage.color = new Color(0.1f, 0.1f, 0.1f, 1f);
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

        // Create a container for the title and logo
        GameObject headerContainer = new GameObject("HeaderContainer", typeof(RectTransform), typeof(VerticalLayoutGroup));
        RectTransform headerRect = headerContainer.GetComponent<RectTransform>();
        headerRect.SetParent(bgObj.transform, false);

        // Anchor the container to the top-center of the background panel
        headerRect.anchorMin = new Vector2(0.5f, 1f);
        headerRect.anchorMax = new Vector2(0.5f, 1f);
        headerRect.pivot = new Vector2(0.5f, 1f);
        headerRect.anchoredPosition = new Vector2(0, -30); // Position it 30 pixels from the top

        // Set the alignment of the VerticalLayoutGroup
        VerticalLayoutGroup vLayoutGroup = headerContainer.GetComponent<VerticalLayoutGroup>();
        vLayoutGroup.childAlignment = TextAnchor.UpperCenter;
        vLayoutGroup.childControlHeight = false; // Allow manual height control
        vLayoutGroup.spacing = 10; // Space between the logo and text

        // --- Create and parent the Title to the header container ---
        TMP_Text title = CreateText("AP Client", Vector2.zero, 20, headerContainer.transform);
        title.alignment = TextAlignmentOptions.Center;
        title.rectTransform.sizeDelta = new Vector2(150, 30); // Give it a specific size

        // --- Create and parent the Logo to the header container ---
        Texture2D logoTex = EmbeddedResources.LoadTexture("ApClient.assets.color-icon.png");
        if (logoTex != null)
        {
            GameObject logoObj = new GameObject("Logo", typeof(Image));
            logoObj.transform.SetParent(headerContainer.transform, false);

            Image logoImage = logoObj.GetComponent<Image>();
            Sprite logoSprite = Sprite.Create(logoTex,
                new Rect(0, 0, logoTex.width, logoTex.height),
                new Vector2(0.5f, 0.5f));
            logoImage.sprite = logoSprite;

            RectTransform logoRect = logoObj.GetComponent<RectTransform>();
            logoRect.sizeDelta = new Vector2(50, 50); // Adjust as needed
        }

        // --- Create Right Side UI: Buttons and Scrollable List ---
        CreateRightPanel(bgObj.transform);
        //PopulateSampleData();
        UpdateList(cardOpenItems);

        setVisable(false);
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

    private Button CreateButton(string text, Vector2 pos, Vector2 size, Transform parent)
    {
        GameObject obj = new GameObject(text, typeof(Image), typeof(Button));
        obj.transform.SetParent(parent, false);

        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchoredPosition = pos;

        Image img = obj.GetComponent<Image>();
        img.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        img.type = Image.Type.Sliced;

        Button button = obj.GetComponent<Button>();

        TMP_Text buttonText = CreateText(text, Vector2.zero, 16, obj.transform);
        buttonText.alignment = TextAlignmentOptions.Center;
        RectTransform textRect = buttonText.GetComponent<RectTransform>();
        textRect.sizeDelta = size;

        return button;
    }

    private void CreateRightPanel(Transform parent)
    {
        // Container for the right side
        GameObject rightPanelObj = new GameObject("RightPanel", typeof(RectTransform));
        RectTransform rightPanel = rightPanelObj.GetComponent<RectTransform>();
        rightPanel.SetParent(parent, false);
        rightPanel.anchorMin = new Vector2(0.5f, 0f);
        rightPanel.anchorMax = new Vector2(1f, 1f);
        rightPanel.offsetMin = new Vector2(0, 0);
        rightPanel.offsetMax = new Vector2(0, 0);

        // Container for the buttons
        GameObject buttonContainerObj = new GameObject("ButtonContainer", typeof(RectTransform), typeof(HorizontalLayoutGroup));
        RectTransform buttonContainer = buttonContainerObj.GetComponent<RectTransform>();
        buttonContainer.SetParent(rightPanel, false);
        buttonContainer.anchorMin = new Vector2(0f, 1f);
        buttonContainer.anchorMax = new Vector2(1f, 1f);
        buttonContainer.pivot = new Vector2(0.5f, 1f);
        buttonContainer.sizeDelta = new Vector2(0, 30); // Height of the container, width will be handled by anchors
        buttonContainer.anchoredPosition = new Vector2(0, -60); // Position it below the top edge

        // Configure the HorizontalLayoutGroup
        HorizontalLayoutGroup buttonlayoutGroup = buttonContainerObj.GetComponent<HorizontalLayoutGroup>();
        buttonlayoutGroup.childAlignment = TextAnchor.MiddleCenter;
        buttonlayoutGroup.spacing = 5;
        buttonlayoutGroup.childForceExpandWidth = true;

        // Create and parent the buttons to the new container
        cardOpenButton = CreateButton("Card Open", Vector2.zero, new Vector2(100, 30), buttonContainer);
        cardSellButton = CreateButton("Card Sell", Vector2.zero, new Vector2(100, 30), buttonContainer);
        cardGradeButton = CreateButton("Card Grading", Vector2.zero, new Vector2(100, 30), buttonContainer);

        // The CreateButton method's size and position parameters become less important here,
        // as the HorizontalLayoutGroup will handle placement. You can simplify CreateButton.
        // For now, keep the original method signature, but note that pos will be ignored.

        // Add listeners to buttons
        cardOpenButton.onClick.AddListener(() => UpdateList(cardOpenItems));
        cardSellButton.onClick.AddListener(() => UpdateList(cardSellItems));
        cardGradeButton.onClick.AddListener(() => UpdateList(cardGradeItems));

        // Create Scroll View
        GameObject scrollViewObj = new GameObject("ScrollView", typeof(Image), typeof(ScrollRect));
        scrollViewObj.transform.SetParent(rightPanel, false);

        RectTransform scrollViewRect = scrollViewObj.GetComponent<RectTransform>();
        scrollViewRect.anchorMin = new Vector2(0, 0);
        scrollViewRect.anchorMax = new Vector2(1, 1);
        scrollViewRect.offsetMin = new Vector2(10, 10);
        scrollViewRect.offsetMax = new Vector2(-10, -100); // Adjust offset to make space for the buttons and title


        Image scrollViewImage = scrollViewObj.GetComponent<Image>();
        scrollViewImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        scrollViewImage.type = Image.Type.Sliced;

        ScrollRect scrollRect = scrollViewObj.GetComponent<ScrollRect>();
        scrollRect.vertical = true;
        scrollRect.horizontal = false;
        scrollRect.scrollSensitivity = 50f;
        // Viewport
        GameObject viewportObj = new GameObject("Viewport", typeof(RectMask2D), typeof(Image));
        viewportObj.transform.SetParent(scrollViewObj.transform, false);
        RectTransform viewportRect = viewportObj.GetComponent<RectTransform>();
        viewportRect.anchorMin = new Vector2(0, 0);
        viewportRect.anchorMax = new Vector2(1, 1);
        viewportRect.offsetMin = Vector2.zero;
        viewportRect.offsetMax = Vector2.zero;
        viewportObj.GetComponent<Image>().color = new Color(0, 0, 0, 0);

        scrollRect.viewport = viewportRect;

        // Content
        GameObject contentObj = new GameObject("Content", typeof(RectTransform));
        contentObj.transform.SetParent(viewportObj.transform, false);
        scrollContent = contentObj.GetComponent<RectTransform>();
        scrollContent.anchorMin = new Vector2(0, 1);
        scrollContent.anchorMax = new Vector2(1, 1);
        scrollContent.pivot = new Vector2(0.5f, 1);
        scrollContent.sizeDelta = new Vector2(0, 0);

        scrollRect.content = scrollContent;

        VerticalLayoutGroup vLayoutGroup = contentObj.AddComponent<VerticalLayoutGroup>();
        vLayoutGroup.childAlignment = TextAnchor.UpperLeft;
        vLayoutGroup.childForceExpandHeight = false;
        vLayoutGroup.childControlHeight = false;
        vLayoutGroup.spacing = 5;
        vLayoutGroup.padding = new RectOffset(5, 5, 5, 5); // Add padding to the layout group

        ContentSizeFitter sizeFitter = contentObj.AddComponent<ContentSizeFitter>();
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // --- ADDED: Vertical Scrollbar ---

        // Create the scrollbar container GameObject
        GameObject scrollbarObj = new GameObject("VerticalScrollbar", typeof(Image), typeof(Scrollbar));
        // Set scrollbar as child of scrollViewObj (the main panel)
        scrollbarObj.transform.SetParent(scrollViewObj.transform, false);

        // Position and size the scrollbar container
        RectTransform scrollbarRect = scrollbarObj.GetComponent<RectTransform>();
        scrollbarRect.anchorMin = new Vector2(1, 0);
        scrollbarRect.anchorMax = new Vector2(1, 1);
        scrollbarRect.pivot = new Vector2(1, 1);
        scrollbarRect.sizeDelta = new Vector2(20, 0); // Give it a decent width
        scrollbarRect.offsetMin = new Vector2(-20, 0); // Position it on the right side
        scrollbarRect.offsetMax = new Vector2(0, 0);

        // Set the sibling index so it appears behind the content
        scrollbarObj.transform.SetSiblingIndex(0);

        // Get and configure the Scrollbar component
        Scrollbar scrollbar = scrollbarObj.GetComponent<Scrollbar>();
        scrollbar.direction = Scrollbar.Direction.BottomToTop;

        Image scrollbarImage = scrollbarObj.GetComponent<Image>();
        scrollbarImage.color = new Color32(50, 50, 50, 255); // Background color for the scrollbar

        // Create the handle for the scrollbar
        GameObject handleObj = new GameObject("Handle", typeof(Image));
        handleObj.transform.SetParent(scrollbarObj.transform, false);

        // Assign the handle to the scrollbar component
        scrollbar.handleRect = handleObj.GetComponent<RectTransform>();

        // Configure the handle's RectTransform
        RectTransform handleRect = handleObj.GetComponent<RectTransform>();
        handleRect.anchorMin = new Vector2(1, 0);
        handleRect.anchorMax = new Vector2(2, 1);
        handleRect.pivot = new Vector2(0.5f, 0.5f);
        handleRect.offsetMin = new Vector2(13, 5); // Add padding for the handle
        handleRect.offsetMax = new Vector2(0, -5); // Add padding for the handle

        Image handleImage = handleObj.GetComponent<Image>();
        handleImage.color = new Color32(100, 100, 100, 255); // Color for the scrollbar handle

        // Link the scrollbar to the ScrollRect
        scrollRect.verticalScrollbar = scrollbar;
        scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
        scrollRect.verticalScrollbarSpacing = -3; // Adjust spacing if needed
    }

    private void UpdateList(List<CardLocation> items)
    {
        foreach (Transform child in scrollContent)
        {
            Destroy(child.gameObject);
        }

        foreach (CardLocation item in items)
        {
            CreateCardPanel(item, scrollContent);
        }
    }

    private void CreateCardPanel(CardLocation card, Transform parent)
    {
        // Panel for the card item
        GameObject panelObj = new GameObject("CardPanel", typeof(Image), typeof(RectTransform), typeof(HorizontalLayoutGroup));
        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.SetParent(parent, false);

        Image panelImage = panelObj.GetComponent<Image>();

        // Set background color based on status
        if (card.Status == CardStatus.Available)
            panelImage.color = new Color32(3, 104, 30, 255); // Green
        else if (card.Status == CardStatus.Unavailable)
            panelImage.color = new Color32(111, 26, 26, 255); // Red
        else
            panelImage.color = new Color32(73, 73, 73, 255); // Grey

        panelRect.sizeDelta = new Vector2(0, 50); // Increased height for better spacing

        // Configure the HorizontalLayoutGroup for this panel
        HorizontalLayoutGroup hLayoutGroup = panelObj.GetComponent<HorizontalLayoutGroup>();
        hLayoutGroup.childAlignment = TextAnchor.MiddleLeft;
        hLayoutGroup.padding = new RectOffset(10, 40, 0, 0);
        hLayoutGroup.spacing = 100;

        // Name and Hinted Text
        TMP_Text nameText = CreateText(card.AchievementData.name, Vector2.zero, 14, panelObj.transform);
        nameText.rectTransform.sizeDelta = new Vector2(240, 50); // Adjusted size to fit
        nameText.alignment = TextAlignmentOptions.Left;

        // Progress Text
        string progressString = $"{card.CurrentNum}/{card.AchievementData.threshold}";
        if (card.IsHinted)
        {
            progressString = $"Hinted {progressString}";
        }
        TMP_Text progressText = CreateText(progressString, Vector2.zero, 14, panelObj.transform);
        progressText.rectTransform.sizeDelta = new Vector2(100, 50);
        progressText.alignment = TextAlignmentOptions.Right;
    }
}
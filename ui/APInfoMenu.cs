

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ApClient.patches;
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
        cardOpenButton.interactable = true;
    }

    public void setCardSellList(List<CardLocation> list)
    {
        cardSellItems = list;
        Plugin.Log($"{list.Count}");
        UpdateList(cardSellItems);
        cardSellButton.interactable = true;
    }

    public void setCardGradeList(List<CardLocation> list)
    {
        cardGradeItems = list;
        UpdateList(cardGradeItems);
        cardGradeButton.interactable = true;
    }

    //returns complete location ids
    public long[] UpdateOpenLocationValues(CardData cardData)
    {
        return UpdateLocationValues(cardData, cardOpenItems);
    }

    public long[] UpdateSellLocationValues(CardData cardData)
    {
        return UpdateLocationValues(cardData, cardSellItems);
    }

    public long[] UpdateGradeLocationValues(CardData cardData, int grade)
    {
        //todo:use grade for logic
        return UpdateLocationValues(cardData, cardGradeItems);
    }


    public long[] UpdateLocationValues(CardData cardData, List<CardLocation> list)
    {
        List<long> ach = new List<long>();

        foreach (CardLocation c in list)
        {
            if (c.Status != CardStatus.Available)
            {
                continue;
            }
            var monsterData = InventoryBase.GetMonsterData(cardData.monsterType);

            //Plugin.Log($"Rarity: {c.AchievementData.rarity} : {(int)monsterData.Rarity + 1}");
            //Plugin.Log($"Border: {c.AchievementData.border} : {(int)cardData.borderType}");
            //Plugin.Log($"Expansion: {c.AchievementData.expansion} : {(int)cardData.expansionType}");
            //Plugin.Log($"Foil: {c.AchievementData.foil} : {cardData.isFoil}");
            //Plugin.Log(" ");
            if (c.AchievementData.rarity.Contains((int)monsterData.Rarity + 1)
                && c.AchievementData.border.Contains((int)cardData.borderType)
                && c.AchievementData.expansion.Contains((int)cardData.expansionType)
                && c.AchievementData.foil.Contains(cardData.isFoil ? 1 : 0)
                )
            {
                Plugin.Log($"Matched card with {c.AchievementData.name}");
                c.CurrentNum++;
                if (c.CurrentNum >= c.AchievementData.threshold)
                {
                    c.Status = CardStatus.Found;
                    ach.Add(Plugin.m_SessionHandler.GetLocationId(c.AchievementData.name));
                }
            }
        }
        return ach.ToArray();
    }

    private void CheckAvailable(CardLocation c, ERarity rarity, bool isDestiny)
    {
        if (c.Status == CardStatus.Unavailable
                && c.AchievementData.rarity.Contains((int)rarity + 1)
                && c.AchievementData.expansion.Contains(isDestiny ? 1 : 0))
        {
            c.Status = CardStatus.Available;
        }
    }

    public void UpdateAvailableAchievements(ERarity rarity, bool isDestiny)
    {
        foreach (CardLocation c in cardOpenItems)
        {
            CheckAvailable(c, rarity, isDestiny);
        }

        foreach (CardLocation c in cardSellItems)
        {
            CheckAvailable(c, rarity, isDestiny);
        }

        foreach (CardLocation c in cardGradeItems)
        {
            CheckAvailable(c, rarity, isDestiny);
        }
    }


    public void HintAchievement(string v)
    {
        throw new System.NotImplementedException();
    }

    public void setVisable(bool visable)
    {
        Plugin.Log("toggle Ap info");
        showGUI = visable;
        if (window != null)
        {
            Plugin.Log("instance not null");
            window.gameObject.SetActive(showGUI);
            UpdateList(cardOpenItems);
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
        vLayoutGroup.childControlHeight = false;
        vLayoutGroup.childControlWidth = false;
        vLayoutGroup.spacing = 0; // Space between the logo and text

        //// --- Create and parent the Title to the header container ---
        TMP_Text title = CreateText("Other Info goes here", new Vector2(-400, -200),20 , bgObj.transform);
        //title.alignment = TextAlignmentOptions.Center;
        title.rectTransform.sizeDelta = new Vector2(150, 30); // Give it a specific size

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
            logoRect.sizeDelta = new Vector2(70, 70); // adjust as needed
            logoRect.anchoredPosition = new Vector2(0, 0); // just below the title
        }

        // --- Create Right Side UI: Buttons and Scrollable List ---
        CreateRightPanel(bgObj.transform);
        CreateLeftPanel(bgObj.transform);
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
    private TMP_Text CreateTextInRow(string text, Transform parent, TextAlignmentOptions alignment = TextAlignmentOptions.Left, int size = 18)
    {
        GameObject obj = new GameObject(text, typeof(TextMeshProUGUI));
        obj.transform.SetParent(parent, false);

        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0); // stretch in both directions
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = Vector2.zero; // layout controls size

        var tmp = obj.GetComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.color = Color.white;
        tmp.alignment = alignment;
        tmp.enableWordWrapping = false;
        tmp.verticalAlignment = VerticalAlignmentOptions.Top;

        // Make the HorizontalLayoutGroup give this text flexible width
        var layoutElement = obj.AddComponent<LayoutElement>();
        layoutElement.flexibleWidth = 0;
        layoutElement.minHeight = 30; // optional
        layoutElement.minWidth = 0; // Essential for allowing the text to shrink

        return tmp;
    }
    private TMP_Text CreateText(string text, Vector2 pos, int fontsize, Transform parent, TextAlignmentOptions alignment = TextAlignmentOptions.Left)
    {
        return CreateText(text, pos, new Vector2(280, 30), fontsize, parent, alignment);
    }
    private TMP_Text CreateText(string text, Vector2 pos, Vector2 size, int fontsize, Transform parent, TextAlignmentOptions alignment = TextAlignmentOptions.Left)
    {
            GameObject obj = new GameObject(text, typeof(TextMeshProUGUI));
        obj.transform.SetParent(parent, false);

        var tmp = obj.GetComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontsize;
        tmp.color = Color.white;
        tmp.alignment = alignment; // ✅ use the passed alignment
        tmp.enableWordWrapping = false;

        RectTransform rect = tmp.GetComponent<RectTransform>();
        rect.sizeDelta = size;
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
    private void CreateLeftPanel(Transform parent)
    {
        // ... (Left Panel setup as before) ...
        GameObject leftPanelObj = new GameObject("LeftPanel", typeof(RectTransform));
        RectTransform leftPanel = leftPanelObj.GetComponent<RectTransform>();
        leftPanel.SetParent(parent, false);
        leftPanel.anchorMin = new Vector2(0f, 0.0f);
        leftPanel.anchorMax = new Vector2(0.5f, .9f);
        leftPanel.offsetMin = new Vector2(0, 0);
        leftPanel.offsetMax = new Vector2(0, 0);

        VerticalLayoutGroup panelLayout = leftPanelObj.AddComponent<VerticalLayoutGroup>();
        panelLayout.childAlignment = TextAnchor.UpperCenter;
        panelLayout.childForceExpandWidth = true;
        panelLayout.childForceExpandHeight = false;
        panelLayout.childControlHeight = true;
        panelLayout.childControlWidth = true;
        panelLayout.spacing = 5;

        // --- Info Row Setup ---
        GameObject infoRowObj = new GameObject("InfoRow", typeof(RectTransform));
        RectTransform infoRow = infoRowObj.GetComponent<RectTransform>();
        infoRow.SetParent(leftPanelObj.transform, false);

        LayoutElement infoRowLayoutElement = infoRowObj.AddComponent<LayoutElement>();
        infoRowLayoutElement.minHeight = 30;
        infoRowLayoutElement.flexibleHeight = 0;
        infoRowLayoutElement.flexibleWidth = 1;

        HorizontalLayoutGroup rowLayout = infoRowObj.AddComponent<HorizontalLayoutGroup>();
        rowLayout.childAlignment = TextAnchor.MiddleCenter;
        rowLayout.childForceExpandWidth = true; // We want full expansion of children
        rowLayout.childForceExpandHeight = true;
        rowLayout.childControlWidth = true;
        rowLayout.childControlHeight = true;
        rowLayout.spacing = 0; // We'll use flexible width for spacing now

        // --- 1. Left Text ---
        // Use the CreateTextInRow function, which uses flexibleWidth=1 and minWidth=0
        CreateTextInRow($"Level Max: {GameUIScreenPatches.ExactMaxLevel}", infoRowObj.transform, TextAlignmentOptions.Left);

        // --- 2. Center Text (This needs to be fixed width, or also flexible width) ---
        // To make this work robustly with L/C/R alignment, we make all 3 flexible.
        CreateTextInRow($"Stored XP: {Plugin.m_SaveManager.TotalStoredXP()}", infoRowObj.transform, TextAlignmentOptions.Center);

        // --- 3. Right Text ---
        CreateTextInRow($"Licenses to Level: {Plugin.m_SessionHandler.GetRemainingLicenses((((CPlayerData.m_ShopLevel + 1) + 4) / 5) * 5)}", infoRowObj.transform, TextAlignmentOptions.Right);


        GameObject scrollViewObj = new GameObject("ScrollView", typeof(Image), typeof(ScrollRect));
        scrollViewObj.transform.SetParent(leftPanel, false);

        RectTransform scrollViewRect = scrollViewObj.GetComponent<RectTransform>();
        scrollViewRect.anchorMin = new Vector2(0, 0);
        scrollViewRect.anchorMax = new Vector2(1, 1);
        scrollViewRect.offsetMin = new Vector2(10, 10);
        scrollViewRect.offsetMax = new Vector2(0, 0); // Adjust offset to make space for the buttons and title


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
    }

    private void CreateRightPanel(Transform parent)
    {
        // Container for the right side
        GameObject rightPanelObj = new GameObject("RightPanel", typeof(RectTransform));
        RectTransform rightPanel = rightPanelObj.GetComponent<RectTransform>();
        rightPanel.SetParent(parent, false);
        rightPanel.anchorMin = new Vector2(0.5f, 0f);
        rightPanel.anchorMax = new Vector2(1f, .96f);
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

        cardOpenButton.interactable = false;
        cardSellButton.interactable = false;
        cardGradeButton.interactable = false;

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
            progressString = $"{progressString}";
        }
        TMP_Text progressText = CreateText(progressString, Vector2.zero, 14, panelObj.transform);
        progressText.rectTransform.sizeDelta = new Vector2(150, 50);
        progressText.alignment = TextAlignmentOptions.Right;
    }

}
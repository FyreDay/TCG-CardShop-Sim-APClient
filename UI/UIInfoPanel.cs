using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using ApClient;
using ApClient.ui;
using System.Linq;
using UnityEngine.UI;
using ApClient.data;
using ApClient.Archipelago;

public class UIInfoPanel : MonoBehaviour
{
    private static UIInfoPanel Instance;

    public static void setInstance(UIInfoPanel instance, GameObject apinfoobject, GameObject achievementPrefab, GameObject productPrefab, Dictionary<string, List<CompiledAchievement>> achievements)
    {
        Instance = instance;
        Instance.window = apinfoobject;
        Instance.achievementPrefab = achievementPrefab;
        Instance.productPrefab = productPrefab;

        foreach (var tmp in apinfoobject.GetComponentsInChildren<TextMeshProUGUI>(true))
        {
            switch (tmp.name)
            {
                case "MaxLevel": Instance.levelMaxText = tmp; break;
                case "StoredXp": Instance.storedXPText = tmp; break;
                case "LicensesRequired": Instance.licensesText = tmp; break;
            }
        }

        foreach (var btn in apinfoobject.GetComponentsInChildren<Button>(true))
        {
            switch (btn.name)
            {
                case "CardGradeButton": Instance.cardGradeButton = btn; break;
                case "CardSellButton": Instance.cardSellButton = btn; break;
                case "CardOpenButton": Instance.cardOpenButton = btn; break;
            }
        }

        Instance.cardOpenButton.onClick.AddListener(() => Instance.UpdateAchievementList(Instance.cardOpenItems));
        Instance.cardSellButton.onClick.AddListener(() => Instance.UpdateAchievementList(Instance.cardSellItems));
        Instance.cardGradeButton.onClick.AddListener(() => Instance.UpdateAchievementList(Instance.cardGradeItems));

        Instance.achievementContent = apinfoobject.GetComponentsInChildren<Transform>(true)
            .FirstOrDefault(t => t.name == "AchievementContent");

        Instance.productContent = apinfoobject.GetComponentsInChildren<Transform>(true)
            .FirstOrDefault(t => t.name == "ProductContent");

        //Instance.SetLevelMax(APLogicUtil.GetMaxLevel(CPlayerData.m_ShopLevel));
        //Instance.SetLicensesToLevel(APLogicUtil.GetRemainingLicenses(APLogicUtil.GetMaxLevel(CPlayerData.m_ShopLevel)));

        //pass by reference so that they update when we update cards
        Instance.cardOpenItems = achievements[Constants.OPEN_ACHIEVEMENT_TYPE];
        Instance.cardSellItems = achievements[Constants.SELL_ACHIEVEMENT_TYPE];
        Instance.cardGradeItems = achievements[Constants.GRADE_ACHIEVEMENT_TYPE];
        if (Instance.cardGradeItems.Count > 0)
        {
            Instance.cardGradeButton.interactable = true;
            Instance.UpdateAchievementList(Instance.cardGradeItems);
        }

        if (Instance.cardSellItems.Count > 0)
        {
            Instance.cardSellButton.interactable = true;
            Instance.UpdateAchievementList(Instance.cardSellItems);
        }
        if (Instance.cardOpenItems.Count > 0)
        {
            Instance.cardOpenButton.interactable = true;
            Instance.UpdateAchievementList(Instance.cardOpenItems);
        }


        Instance.UpdateProductList(new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
        var sr = apinfoobject.GetComponentInChildren<ScrollRect>(true);
    }

    public static UIInfoPanel getInstance()
    {
        return Instance;
    }

    public GameObject window;

    private bool showGUI = true;

    public TextMeshProUGUI levelMaxText;
    public TextMeshProUGUI storedXPText;
    public TextMeshProUGUI licensesText;
    public Transform achievementContent;
    public Transform productContent;
    public GameObject achievementPrefab;
    public GameObject productPrefab;

    private Button cardOpenButton;
    private Button cardSellButton;
    private Button cardGradeButton;


    public List<CompiledAchievement> cardOpenItems;
    public List<CompiledAchievement> cardSellItems;
    public List<CompiledAchievement> cardGradeItems;

    public void setVisable(bool visable)
    {
        Plugin.Logger.LogInfo("toggle Ap info");
        showGUI = visable;
        if (window != null)
        {
            Plugin.Logger.LogInfo("instance not null");
            window.SetActive(visable);
            UpdateAchievementList(cardOpenItems);
            int maxLevel = APLogicUtil.GetMaxLevel(CPlayerData.m_ShopLevel);
            SetLevelMax(maxLevel);
            SetStoredXP(Plugin.SaveHandler.GetSaveData().StoredXP);
            SetLicensesToLevel(APLogicUtil.GetRemainingLicenses(maxLevel));
        }

    }

    public void toggleVisability()
    {
        setVisable(!showGUI);
    }

    public void DelaySetVisable(bool visible)
    {
        if (!showGUI)
        {
            return;
        }
        Plugin.Logger.LogInfo($"Toggle AP Info: {visible}");
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
            Plugin.Logger.LogInfo("Hiding info screen after delay");
            window.SetActive(false);
        }
    }
    public void SetLevelMax(int value)
    {
        levelMaxText.text = $"{value}";
    }

    public void SetStoredXP(int value)
    {
        storedXPText.text = $"{value}";
    }

    public void SetLicensesToLevel(int value)
    {
        licensesText.text = $"{value}";
    }

    public void UpdateAchievementList(List<CompiledAchievement> items)
    {

        Plugin.Logger.LogInfo($"Update list {items.Count}");
        foreach (Transform child in achievementContent)
        {
            Destroy(child.gameObject);
        }

        var orderedAchievements = items
            .OrderByDescending(a => a.IsHinted) // hinted first
            .ThenBy(a =>
            {
                if (a.completed) return 2;   // completed last
                if (a.Available) return 0;   // available group first
                return 1;                     // unavailable group in the middle
            })
            .ToList();

        foreach (CompiledAchievement item in orderedAchievements)
        {
            
            var achievement = Instantiate(achievementPrefab, achievementContent);
            achievement.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = item.data.name;
            achievement.transform.Find("ProgressText").GetComponent<TextMeshProUGUI>().text = $"{item.progress}/{item.data.threshold}";
            var hint = achievement.transform.Find("Hint");
            hint.Find("HintText").GetComponent<TextMeshProUGUI>().enabled = item.IsHinted;
            hint.GetComponent<Image>().enabled = item.IsHinted;
            achievement.transform.GetComponent<Image>().color = item.completed ? new Color(.6f,.6f,.6f) : item.Available ? Color.green : Color.red;
        }
    }

    public void UpdateProductList(List<int> products)
    {
        foreach (int item in products)
        {
            var product = Instantiate(productPrefab);
            product.transform.SetParent(productContent, false);
            product.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "test name";
            var hintbutton = product.transform.Find("Button");
            hintbutton.Find("ButtonText").GetComponent<TextMeshProUGUI>().text = "Hinted";
            hintbutton.GetComponent<Button>().enabled = true;
            hintbutton.GetComponent<Button>().onClick.AddListener(() => Plugin.Logger.LogInfo($"Hint {item}"));
        }
    }

    void Update()
    {
        
    }
}

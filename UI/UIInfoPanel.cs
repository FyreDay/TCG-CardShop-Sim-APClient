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

        int mult = 0;
        switch (Plugin.ArchipelagoHandler.slotData.CardOpeningCheckDifficulty)
        {
            case 0: mult = 12; break;
            case 1: mult = 1; break;
            case 2: mult = 3; break;
            case 3: mult = 4; break;
            case 4: mult = 6; break;
        }
        if(mult != 12 && Plugin.ArchipelagoHandler.slotData.CardSanity == 2)
        {
            mult *= 2;
        }

        foreach (var tmp in apinfoobject.GetComponentsInChildren<TextMeshProUGUI>(true))
        {

            switch (tmp.name)
            {
                case "MaxLevel": Instance.levelMaxText = tmp; break;
                case "StoredXp": Instance.storedXPText = tmp; break;
                case "LicensesRequired": Instance.licensesText = tmp; break;

                case "PercentCollected": Instance.percentCollected = tmp; break;
                case "PercentGoal": Instance.percentGoal = tmp; break;

                case "LevelGoalText": Instance.levelGoalText = tmp; break;

                case "CurrentGhostsSold": Instance.currentGhostsSold = tmp; break;
                case "GhostGoalText": Instance.ghostGoalText = tmp; break;

                case "Basic Count": Instance.basicCount = tmp; break;
                case "Rare Count": Instance.rareCount = tmp; break;
                case "Epic Count": Instance.epicCount = tmp; break;
                case "Legendary Count": Instance.legendaryCount = tmp; break;
                case "Destiny Basic Count": Instance.destinyBasicCount = tmp; break;
                case "Destiny Rare Count": Instance.destinyRareCount = tmp; break;
                case "Destiny Epic Count": Instance.destinyEpicCount = tmp; break;
                case "Destiny Legendary Count": Instance.destinyLegendaryCount = tmp; break;

                case "Basic Total": 
                    Instance.basicCountTotal = tmp;
                    Instance.basicCountTotal.text = $"{27 * mult}";
                    break;
                case "Rare Total": 
                    Instance.rareCountTotal = tmp;
                    Instance.rareCountTotal.text = $"{34 * mult}";
                    break;
                case "Epic Total": 
                    Instance.epicCountTotal = tmp;
                    Instance.epicCountTotal.text = $"{34 * mult}";
                    break;
                case "Legendary Total": 
                    Instance.legendaryCountTotal = tmp;
                    Instance.legendaryCountTotal.text = $"{26 * mult}";
                    break;
                case "Destiny Basic Total": 
                    Instance.destinyBasicCountTotal = tmp; 
                    Instance.destinyBasicCountTotal.text = $"{27 * mult}";
                    break;
                case "Destiny Rare Total": 
                    Instance.destinyRareCountTotal = tmp; 
                    Instance.destinyRareCountTotal.text = $"{34 * mult}";
                    break;
                case "Destiny Epic Total": 
                    Instance.destinyEpicCountTotal = tmp; 
                    Instance.destinyEpicCountTotal.text = $"{34 * mult}";
                    break;
                case "Destiny Legendary Total": 
                    Instance.destinyLegendaryCountTotal = tmp; 
                    Instance.destinyLegendaryCountTotal.text = $"{26 * mult}";
                    break;
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

        Instance.CollectionGoalUI = apinfoobject.GetComponentsInChildren<Transform>(true)
            .FirstOrDefault(t => t.name == "CollectionGoal");

        Instance.LevelGoalUI = apinfoobject.GetComponentsInChildren<Transform>(true)
            .FirstOrDefault(t => t.name == "LevelGoal");

        Instance.GhostGoalUI = apinfoobject.GetComponentsInChildren<Transform>(true)
            .FirstOrDefault(t => t.name == "GhostGoal");

        switch(Plugin.ArchipelagoHandler.slotData.Goal)
        {
            case 0: 
                Instance.LevelGoalUI.gameObject.SetActive(true);
                Instance.GhostGoalUI.gameObject.SetActive(false);
                Instance.CollectionGoalUI.gameObject.SetActive(false);
                Instance.levelGoalText.text = $"{Plugin.ArchipelagoHandler.slotData.MaxLevel}";
                break;
            case 1: 
                Instance.CollectionGoalUI.gameObject.SetActive(true);
                Instance.GhostGoalUI.gameObject.SetActive(false);
                Instance.LevelGoalUI.gameObject.SetActive(false);
                Instance.percentGoal.text = $"{Plugin.ArchipelagoHandler.slotData.CollectionGoalPercent}%";
                break;
            case 2: 
                Instance.GhostGoalUI.gameObject.SetActive(true);
                Instance.CollectionGoalUI.gameObject.SetActive(false);
                Instance.LevelGoalUI.gameObject.SetActive(false);
                Instance.ghostGoalText.text = $"{Plugin.ArchipelagoHandler.slotData.GhostGoalAmount}";
                break;
        }
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

    public Transform CollectionGoalUI;
    public Transform LevelGoalUI;
    public Transform GhostGoalUI;

    public TextMeshProUGUI percentCollected;
    public TextMeshProUGUI percentGoal;
    public TextMeshProUGUI levelGoalText;
    public TextMeshProUGUI currentGhostsSold;
    public TextMeshProUGUI ghostGoalText;

    public GameObject achievementPrefab;
    public GameObject productPrefab;

    private Button cardOpenButton;
    private Button cardSellButton;
    private Button cardGradeButton;

    public TextMeshProUGUI basicCount;
    public TextMeshProUGUI rareCount;
    public TextMeshProUGUI epicCount;
    public TextMeshProUGUI legendaryCount;
    public TextMeshProUGUI destinyBasicCount;
    public TextMeshProUGUI destinyRareCount;
    public TextMeshProUGUI destinyEpicCount;
    public TextMeshProUGUI destinyLegendaryCount;

    public TextMeshProUGUI basicCountTotal;
    public TextMeshProUGUI rareCountTotal;
    public TextMeshProUGUI epicCountTotal;
    public TextMeshProUGUI legendaryCountTotal;
    public TextMeshProUGUI destinyBasicCountTotal;
    public TextMeshProUGUI destinyRareCountTotal;
    public TextMeshProUGUI destinyEpicCountTotal;
    public TextMeshProUGUI destinyLegendaryCountTotal;

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

    public void UpdateCardCollection(ECollectionPackType packType, int count)
    {
        switch (packType)
        {
            case ECollectionPackType.BasicCardPack:
                basicCount.text = $"{count}"; 
                break;
            case ECollectionPackType.RareCardPack:
                epicCount.text = $"{count}";
                break;
            case ECollectionPackType.EpicCardPack:
                epicCount.text = $"{count}";
                break;
            case ECollectionPackType.LegendaryCardPack:
                legendaryCount.text = $"{count}";
                break;
            case ECollectionPackType.DestinyBasicCardPack:
                destinyBasicCount.text = $"{count}";
                break;
            case ECollectionPackType.DestinyRareCardPack:
                destinyRareCount.text = $"{count}";
                break;
            case ECollectionPackType.DestinyEpicCardPack:
                destinyEpicCount.text = $"{count}";
                break;
            case ECollectionPackType.DestinyLegendaryCardPack:
                destinyLegendaryCount.text = $"{count}";
                break;
        }
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
            achievement.transform.GetComponent<Image>().color = item.completed ? new Color(.6f,.6f,.6f) : item.Available ? new Color(0,.5f,055f) : new Color(.86f, 0, 0);
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

    public void setPercentGoalCollected(decimal percent)
    {
        percentCollected.text = $"{Math.Round(percent, 1)}%";
    }

    public void setGhostGoalSold(int totalNum)
    {
        currentGhostsSold.text = $"{totalNum}";
    }

    internal void UpdateImportantLicenses(List<RestockData> restockDatas)
    {
        throw new NotImplementedException();
    }
}

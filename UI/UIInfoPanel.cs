using ApClient;
using ApClient.Archipelago;
using ApClient.Archipelago.Mapping;
using ApClient.mapping;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


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
        if (Plugin.ArchipelagoHandler.slotData.CardSanity > 0)
        {
            switch (Plugin.ArchipelagoHandler.slotData.CardOpeningCheckDifficulty)
            {
                case 0: mult = 12; break;
                case 1: mult = 1; break;
                case 2: mult = 3; break;
                case 3: mult = 4; break;
                case 4: mult = 6; break;
            }
            if (mult != 12 && Plugin.ArchipelagoHandler.slotData.CardSanity == 2)
            {
                mult *= 2;
            }
        }
        else
        {
            mult = 12;
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

                case "Basic Count": 
                    Instance.basicCount = tmp; Instance.setCardCollectionCount(ECollectionPackType.BasicCardPack); break;
                case "Rare Count": Instance.rareCount = tmp; Instance.setCardCollectionCount(ECollectionPackType.RareCardPack); break;
                case "Epic Count": Instance.epicCount = tmp; Instance.setCardCollectionCount(ECollectionPackType.EpicCardPack); break;
                case "Legendary Count": Instance.legendaryCount = tmp; Instance.setCardCollectionCount(ECollectionPackType.LegendaryCardPack); break;
                case "Destiny Basic Count": Instance.destinyBasicCount = tmp; Instance.setCardCollectionCount(ECollectionPackType.DestinyBasicCardPack); break;
                case "Destiny Rare Count": Instance.destinyRareCount = tmp; Instance.setCardCollectionCount(ECollectionPackType.DestinyRareCardPack); break;
                case "Destiny Epic Count": Instance.destinyEpicCount = tmp; Instance.setCardCollectionCount(ECollectionPackType.DestinyEpicCardPack); break;
                case "Destiny Legendary Count": Instance.destinyLegendaryCount = tmp; Instance.setCardCollectionCount(ECollectionPackType.DestinyLegendaryCardPack); break;

                case "Generic Count": Instance.GenericCount = tmp; break;
                case "Standard Count": Instance.StandardCount = tmp; break;
                case "Pauper Count": Instance.PauperCount = tmp; break;
                case "Fire Count": Instance.FireCount = tmp; break;
                case "Earth Count": Instance.EarthCount = tmp; break;
                case "Water Count": Instance.WaterCount = tmp; break;
                case "Wind Count": Instance.WindCount = tmp; break;
                case "1st Ed Count": Instance.FirstEdCount = tmp; break;
                case "Silver Count": Instance.SilverCount = tmp; break;
                case "Gold Count": Instance.GoldCount = tmp; break;
                case "EX Count": Instance.EXCount = tmp; break;
                case "Full Art Count": Instance.FullArtCount = tmp; break;
                case "Foil Count": Instance.FoilCount = tmp; break;

                case "Generic Total": Instance.GenericTotal = tmp; break;
                case "Standard Total": Instance.StandardTotal = tmp; break;
                case "Pauper Total": Instance.PauperTotal = tmp; break;
                case "Fire Total": Instance.FireTotal = tmp; break;
                case "Earth Total": Instance.EarthTotal = tmp; break;
                case "Water Total": Instance.WaterTotal = tmp; break;
                case "Wind Total": Instance.WindTotal = tmp; break;
                case "1st Ed Total": Instance.FirstEdTotal = tmp; break;
                case "Silver Total": Instance.SilverTotal = tmp; break;
                case "Gold Total": Instance.GoldTotal = tmp; break;
                case "EX Total": Instance.EXTotal = tmp; break;
                case "Full Art Total": Instance.FullArtTotal = tmp; break;
                case "Foil Total": Instance.FoilTotal = tmp; break;

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

        foreach (var panel in apinfoobject.GetComponentsInChildren<Image>(true))
        {
            switch (panel.name)
            {
                case "Generic Panel": Instance.GenericImage = panel; break;
                case "Standard Panel": Instance.StandardImage = panel; break;
                case "Pauper Panel": Instance.PauperImage = panel; break;
                case "Fire Panel": Instance.FireImage = panel; break;
                case "Earth Panel": Instance.EarthImage = panel; break;
                case "Water Panel": Instance.WaterImage = panel; break;
                case "Wind Panel": Instance.WindImage = panel; break;
                case "1st Ed Panel": Instance.FirstEdImage = panel; break;
                case "Silver Panel": Instance.SilverImage = panel; break;
                case "Gold Panel": Instance.GoldImage = panel; break;
                case "EX Panel": Instance.EXImage = panel; break;
                case "Full Art Panel": Instance.FullArtImage = panel; break;
                case "Foil Panel": Instance.FoilImage = panel; break;
            }
        }

        Instance.cardOpenButton.onClick.AddListener(() => {
            Instance.UpdateAchievementList(Instance.cardOpenItems);
            Instance.currentCardItems = Instance.cardOpenItems;
        });
        Instance.cardSellButton.onClick.AddListener(() => {
            Instance.UpdateAchievementList(Instance.cardSellItems);
            Instance.currentCardItems = Instance.cardSellItems;
        });
        Instance.cardGradeButton.onClick.AddListener(() => {
            Instance.UpdateAchievementList(Instance.cardGradeItems);
            Instance.currentCardItems = Instance.cardGradeItems;
        });

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

        Instance.GenericEventGroup = apinfoobject.GetComponentsInChildren<Transform>(true)
            .FirstOrDefault(t => t.name == "Generic Group");

        Instance.Formats1 = apinfoobject.GetComponentsInChildren<Transform>(true)
            .FirstOrDefault(t => t.name == "Formats1");

        Instance.Formats2 = apinfoobject.GetComponentsInChildren<Transform>(true)
            .FirstOrDefault(t => t.name == "Formats2");

        Instance.Formats3 = apinfoobject.GetComponentsInChildren<Transform>(true)
            .FirstOrDefault(t => t.name == "Formats3");

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
            Instance.currentCardItems = Instance.cardGradeItems;
        }

        if (Instance.cardSellItems.Count > 0)
        {
            Instance.cardSellButton.interactable = true;
            Instance.UpdateAchievementList(Instance.cardSellItems);
            Instance.currentCardItems = Instance.cardSellItems;
        }
        if (Instance.cardOpenItems.Count > 0)
        {
            Instance.cardOpenButton.interactable = true;
            Instance.UpdateAchievementList(Instance.cardOpenItems);
            Instance.currentCardItems = Instance.cardOpenItems;
        }

        Instance.UpdateProductList(new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
        var sr = apinfoobject.GetComponentInChildren<ScrollRect>(true);

        for (int packtype = 0; packtype < (int)ECollectionPackType.MAX; packtype++)
        {
            Instance.UpdateCardCollection((ECollectionPackType)packtype, 0);
        }

        if (Plugin.ArchipelagoHandler.slotData.Goal == 1)
        {
            decimal countCollected = 0;
            foreach (var pair in Plugin.SaveHandler.GetSaveData().foundCards.foundByPackType)
            {
                countCollected += pair.Value.Count;
            }
            decimal percentCollected = countCollected / (decimal)(countCollected + Plugin.SaveHandler.GetSaveData().foundCards.notfound.Count);
            Instance.setPercentGoalCollected(percentCollected * 100);
        }
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

    public Transform GenericEventGroup;
    public Transform Formats1;
    public Transform Formats2;
    public Transform Formats3;

    public Image GenericImage;
    public Image StandardImage;
    public Image PauperImage;
    public Image FireImage;
    public Image EarthImage;
    public Image WaterImage;
    public Image WindImage;
    public Image FirstEdImage;
    public Image SilverImage;
    public Image GoldImage;
    public Image EXImage;
    public Image FullArtImage;
    public Image FoilImage;

    public TextMeshProUGUI GenericCount;
    public TextMeshProUGUI StandardCount;
    public TextMeshProUGUI PauperCount;
    public TextMeshProUGUI FireCount;
    public TextMeshProUGUI EarthCount;
    public TextMeshProUGUI WaterCount;
    public TextMeshProUGUI WindCount;
    public TextMeshProUGUI FirstEdCount;
    public TextMeshProUGUI SilverCount;
    public TextMeshProUGUI GoldCount;
    public TextMeshProUGUI EXCount;
    public TextMeshProUGUI FullArtCount;
    public TextMeshProUGUI FoilCount;

    public TextMeshProUGUI GenericTotal;
    public TextMeshProUGUI StandardTotal;
    public TextMeshProUGUI PauperTotal;
    public TextMeshProUGUI FireTotal;
    public TextMeshProUGUI EarthTotal;
    public TextMeshProUGUI WaterTotal;
    public TextMeshProUGUI WindTotal;
    public TextMeshProUGUI FirstEdTotal;
    public TextMeshProUGUI SilverTotal;
    public TextMeshProUGUI GoldTotal;
    public TextMeshProUGUI EXTotal;
    public TextMeshProUGUI FullArtTotal;
    public TextMeshProUGUI FoilTotal;

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

    public List<CompiledAchievement> currentCardItems;
    public void setVisable(bool visable)
    {
        showGUI = visable;
        if (window != null)
        {
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
            window.SetActive(false);
        }
    }

    public void setCardCollectionCount(ECollectionPackType type)
    {
        Instance.UpdateCardCollection(type, Plugin.SaveHandler.GetSaveData().foundCards.sanityCount[type]);
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

    public void InitializeEventGames(bool allevents,int total)
    {
        GenericImage.color = Color.red.AlphaMultiplied(0.4f);
        StandardImage.color = Color.red.AlphaMultiplied(0.4f);
        PauperImage.color = Color.red.AlphaMultiplied(0.4f);
        FireImage.color = Color.red.AlphaMultiplied(0.4f);
        EarthImage.color = Color.red.AlphaMultiplied(0.4f);
        WaterImage.color = Color.red.AlphaMultiplied(0.4f);
        WindImage.color = Color.red.AlphaMultiplied(0.4f);
        FirstEdImage.color = Color.red.AlphaMultiplied(0.4f);
        SilverImage.color = Color.red.AlphaMultiplied(0.4f);
        GoldImage.color = Color.red.AlphaMultiplied(0.4f);
        EXImage.color = Color.red.AlphaMultiplied(0.4f);
        FullArtImage.color = Color.red.AlphaMultiplied(0.4f);
        FoilImage.color = Color.red.AlphaMultiplied(0.4f);

        if (allevents)
        {
            GenericEventGroup.gameObject.SetActive(false);
            StandardTotal.text = $"{total}";
            PauperTotal.text = $"{total}";
            FireTotal.text = $"{total}";
            EarthTotal.text = $"{total}";
            WaterTotal.text = $"{total}";
            WindTotal.text = $"{total}";
            FirstEdTotal.text = $"{total}";
            SilverTotal.text = $"{total}";
            GoldTotal.text = $"{total}";
            EXTotal.text = $"{total}";
            FullArtTotal.text = $"{total}";
            FoilTotal.text = $"{total}";
        }
        else
        {
            Formats1.gameObject.SetActive(false);
            Formats2.gameObject.SetActive(false);
            Formats3.gameObject.SetActive(false);
            GenericTotal.text = $"{total}";
        }
        CheckAllFormatsAvailability();
    }

    public void CheckAllFormatsAvailability()
    {
        if(Plugin.ArchipelagoHandler.slotData.NoFormat)
        {
            UpdateFormatAvailability(EGameEventFormat.MAX);
        }

        for (int i = 0; i < (int)EGameEventFormat.MAX; i++)
        {
            if (Plugin.ArchipelagoHandler.GetItemCount(PlayTableMapping.FormatStartingId + i) > 0)
            {

                UpdateFormatAvailability((EGameEventFormat)i);
            }
        }

    }

    private Color formatAvailableColor = new(.164f, .164f, .55f, 0.4f);
    public void UpdateFormatAvailability(EGameEventFormat eventType)
    {
        if(Plugin.ArchipelagoHandler.GetItemCount(FurnatureMapping.getIdFromType(EObjectType.PlayTable)) == 0)
        {
            return;
        }

        switch (eventType)
        {
            
            case EGameEventFormat.MAX:
                GenericImage.color = formatAvailableColor;
                return;
            case EGameEventFormat.Standard:
                StandardImage.color = formatAvailableColor;
                return;
            case EGameEventFormat.Pauper:
                PauperImage.color = formatAvailableColor;
                return;
            case EGameEventFormat.FireCup:
                FireImage.color = formatAvailableColor;
                return;
            case EGameEventFormat.EarthCup:
                EarthImage.color = formatAvailableColor;
                return;
            case EGameEventFormat.WaterCup:
                WaterImage.color = formatAvailableColor;
                return;
            case EGameEventFormat.WindCup:
                WindImage.color = formatAvailableColor;
                return;
            case EGameEventFormat.FirstEditionVintage:
                FirstEdImage.color = formatAvailableColor;
                return;
            case EGameEventFormat.SilverBorder:
                SilverImage.color = formatAvailableColor;
                return;
            case EGameEventFormat.GoldBorder:
                GoldImage.color = formatAvailableColor;
                return;
            case EGameEventFormat.ExBorder:
                EXImage.color = formatAvailableColor;
                return;
            case EGameEventFormat.FullArtBorder:
                FullArtImage.color = formatAvailableColor;
                return;
            case EGameEventFormat.Foil:
                FoilImage.color = formatAvailableColor;
                return;
            case EGameEventFormat.None:
                return;
        }
    }

    public void UpdateFormatCount(EGameEventFormat packType, int count)
    {
        switch (packType)
        {
            case EGameEventFormat.MAX:
                if (GenericCount != null)
                {
                    GenericCount.text = $"{count}";
                }
                break;

            case EGameEventFormat.Standard:
                if (StandardCount != null)
                {
                    StandardCount.text = $"{count}";
                }
                break;

            case EGameEventFormat.Pauper:
                if (PauperCount != null)
                {
                    PauperCount.text = $"{count}";
                }
                break;

            case EGameEventFormat.FireCup:
                if (FireCount != null)
                {
                    FireCount.text = $"{count}";
                }
                break;

            case EGameEventFormat.EarthCup:
                if (EarthCount != null)
                {
                    EarthCount.text = $"{count}";
                }
                break;

            case EGameEventFormat.WaterCup:
                if (WaterCount != null)
                {
                    WaterCount.text = $"{count}";
                }
                break;

            case EGameEventFormat.WindCup:
                if (WindCount != null)
                {
                    WindCount.text = $"{count}";
                }
                break;

            case EGameEventFormat.FirstEditionVintage:
                if (FirstEdCount != null)
                {
                    FirstEdCount.text = $"{count}";
                }
                break;

            case EGameEventFormat.SilverBorder:
                if (SilverCount != null)
                {
                    SilverCount.text = $"{count}";
                }
                break;

            case EGameEventFormat.GoldBorder:
                if (GoldCount != null)
                {
                    GoldCount.text = $"{count}";
                }
                break;

            case EGameEventFormat.ExBorder:
                if (EXCount != null)
                {
                    EXCount.text = $"{count}";
                }
                break;

            case EGameEventFormat.FullArtBorder:
                if (FullArtCount != null)
                {
                    FullArtCount.text = $"{count}";
                }
                break;

            case EGameEventFormat.Foil:
                if (FoilCount != null)
                {
                    FoilCount.text = $"{count}";
                }
                break;
        }
    }

    public void UpdateCardCollection(ECollectionPackType packType, int count)
    {
        switch (packType)
        {
            case ECollectionPackType.BasicCardPack:
                basicCount.text = $"{count}"; 
                break;
            case ECollectionPackType.RareCardPack:
                rareCount.text = $"{count}";
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

    public void Refresh()
    {
        if (Instance.currentCardItems != null)
        {
            Instance.UpdateAchievementList(Instance.currentCardItems);
        }
    }
}

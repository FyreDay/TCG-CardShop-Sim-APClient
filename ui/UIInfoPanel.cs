using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using ApClient;
using ApClient.ui;
using System.Linq;
using UnityEngine.UI;
using System.Runtime.CompilerServices;
using UnityEngine.SocialPlatforms.Impl;
using Unity.Services.Analytics;

public class UIInfoPanel : MonoBehaviour
{
    private static UIInfoPanel Instance;

    public static void setInstance(UIInfoPanel instance, GameObject apinfoobject, GameObject achievementPrefab, GameObject productPrefab)
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
            Plugin.Log(tmp.name);
        }

        Instance.cardOpenButton.onClick.AddListener(() => Instance.UpdateAchievementList(Instance.cardOpenItems));
        Instance.cardSellButton.onClick.AddListener(() => Instance.UpdateAchievementList(Instance.cardSellItems));
        Instance.cardGradeButton.onClick.AddListener(() => Instance.UpdateAchievementList(Instance.cardGradeItems));

        Instance.achievementContent = apinfoobject.GetComponentsInChildren<Transform>(true)
            .FirstOrDefault(t => t.name == "AchievementContent");
        Instance.productContent = apinfoobject.GetComponentsInChildren<Transform>(true)
            .FirstOrDefault(t => t.name == "ProductContent");

        Instance.UpdateProductList(new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
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


    public List<CardLocation> cardOpenItems = new List<CardLocation>();
    public List<CardLocation> cardSellItems = new List<CardLocation>();
    public List<CardLocation> cardGradeItems = new List<CardLocation>();


    public void setCardOpenList(List<CardLocation> list)
    {
        cardOpenItems = list;
        UpdateAchievementList(cardOpenItems);
        cardOpenButton.interactable = true;
    }

    public void setCardSellList(List<CardLocation> list)
    {
        cardSellItems = list;
        Plugin.Log($"{list.Count}");
        UpdateAchievementList(cardSellItems);
        cardSellButton.interactable = true;
    }

    public void setCardGradeList(List<CardLocation> list)
    {
        cardGradeItems = list;
        UpdateAchievementList(cardGradeItems);
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
            window.SetActive(visable);
            UpdateAchievementList(cardOpenItems);
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

    public void UpdateAchievementList(List<CardLocation> items)
    {
        Plugin.Log($"{items.Count}");
        foreach (Transform child in achievementContent)
        {
            Destroy(child.gameObject);
        }

        foreach (CardLocation item in items)
        {
            var achievement = Instantiate(achievementPrefab);
            achievement.transform.SetParent(achievementContent, false);
            achievement.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = item.AchievementData.name;
            achievement.transform.Find("ProgressText").GetComponent<TextMeshProUGUI>().text = $"{item.CurrentNum}/{item.AchievementData.threshold}";
            var hint = achievement.transform.Find("Hint");
            achievement.transform.Find("HintText").GetComponent<TextMeshProUGUI>().enabled = item.IsHinted;
            hint.GetComponent<Image>().enabled = item.IsHinted;
        }
    }

    public void UpdateProductList(List<int> products)
    {
        foreach (int item in products)
        {
            var product = Instantiate(productPrefab);
            product.transform.SetParent(productContent, false);
            product.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "test name";
            product.transform.Find("ButtonText").GetComponent<TextMeshProUGUI>().text = "Hinted";
            Button hintbutton = product.GetComponent<Button>();
            hintbutton.enabled = false;
            hintbutton.onClick.AddListener(() => Plugin.Log($"Hint {item}"));
        }
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
}

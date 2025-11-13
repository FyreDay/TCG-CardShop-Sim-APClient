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

public class UIInfoPanel : MonoBehaviour
{
    [Header("Info Texts")]
    public TextMeshProUGUI levelMaxText;
    public TextMeshProUGUI storedXPText;
    public TextMeshProUGUI licensesText;
    public Transform achievementContent;
    public GameObject achievemntPrefab;
    // Called by your BepInEx mod
    public void SetLevelMax(int value)
    {
        levelMaxText.text = $"Level Max: {value}";
    }

    public void SetStoredXP(int value)
    {
        storedXPText.text = $"Stored XP: {value}";
    }

    public void SetLicensesToLevel(int value)
    {
        licensesText.text = $"Licenses to Level: {value}";
    }

    public void UpdateList(List<CardLocation> items)
    {
        Plugin.Log($"{items.Count}");
        foreach (Transform child in achievementContent)
        {
            Destroy(child.gameObject);
        }

        foreach (CardLocation item in items)
        {
            var achievement = Instantiate(achievemntPrefab, achievementContent);
            achievement.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = item.AchievementData.name;
            Plugin.Log(achievement.transform.Find("Text").GetComponent<TextMeshProUGUI>().text);
            achievement.transform.Find("ProgressText").GetComponent<TextMeshProUGUI>().text = $"{item.CurrentNum}/{item.AchievementData.threshold}";
            Plugin.Log(achievement.transform.Find("ProgressText").GetComponent<TextMeshProUGUI>().text);
            var hint = achievement.transform.Find("Hint");
            achievement.transform.Find("HintText").GetComponent<TextMeshProUGUI>().enabled = item.IsHinted;
            hint.GetComponent<Image>().enabled = item.IsHinted;
            achievement.transform.localPosition = Vector3.zero;
        }
    }

    public void Setup(GameObject apinfoobject, GameObject achievementPrefab)
    {
        achievemntPrefab = achievementPrefab;

        foreach (var tmp in apinfoobject.GetComponentsInChildren<TextMeshProUGUI>(true))
        {
            switch (tmp.name)
            {
                case "MaxLevel": levelMaxText = tmp; break;
                case "StoredXP": storedXPText = tmp; break;
                case "Licenses": licensesText = tmp; break;
            }
            Plugin.Log(tmp.name);
        }

        achievementContent = apinfoobject.GetComponentsInChildren<Transform>(true)
            .FirstOrDefault(t => t.name == "AchievementContent");
        Plugin.Log($" is null? {achievementContent == null}");
    }
}

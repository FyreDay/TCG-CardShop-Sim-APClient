﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.UIR;

namespace ApClient.patches;


public class BinderPagePatches
{
    private static TextMeshProUGUI sanityText;
    private static TextMeshProUGUI commonText;
    private static TextMeshProUGUI rareText;
    private static TextMeshProUGUI epicText;
    private static TextMeshProUGUI legendaryText;

    private static void updatepackText(ECardExpansionType eCardExpansionType)
    {
        if (eCardExpansionType == ECardExpansionType.Tetramon)
        {
            commonText.text = $"{Plugin.m_SaveManager.GetSentChecks(ECollectionPackType.BasicCardPack)} / {Plugin.m_SessionHandler.GetSlotData().ChecksPerPack} Basic Pack Checks";
            rareText.text = $"{Plugin.m_SaveManager.GetSentChecks(ECollectionPackType.RareCardPack)} / {Plugin.m_SessionHandler.GetSlotData().ChecksPerPack} Rare Pack Checks";
            epicText.text = $"{Plugin.m_SaveManager.GetSentChecks(ECollectionPackType.EpicCardPack)} / {Plugin.m_SessionHandler.GetSlotData().ChecksPerPack} Epic Pack Checks";
            legendaryText.text = $"{Plugin.m_SaveManager.GetSentChecks(ECollectionPackType.LegendaryCardPack)} / {Plugin.m_SessionHandler.GetSlotData().ChecksPerPack} Legendary Pack Checks";
        }
        else
        {
            commonText.text = $"{Plugin.m_SaveManager.GetSentChecks(ECollectionPackType.DestinyBasicCardPack)} / {Plugin.m_SessionHandler.GetSlotData().ChecksPerPack} Destiny Basic Pack Checks";
            rareText.text = $"{Plugin.m_SaveManager.GetSentChecks(ECollectionPackType.DestinyRareCardPack)} / {Plugin.m_SessionHandler.GetSlotData().ChecksPerPack} Destiny Rare Pack Checks";
            epicText.text = $"{Plugin.m_SaveManager.GetSentChecks(ECollectionPackType.DestinyEpicCardPack)} / {Plugin.m_SessionHandler.GetSlotData().ChecksPerPack} Destiny Epic Pack Checks";
            legendaryText.text = $"{Plugin.m_SaveManager.GetSentChecks(ECollectionPackType.DestinyLegendaryCardPack)} / {Plugin.m_SessionHandler.GetSlotData().ChecksPerPack} Destiny Legendary Pack Checks";
        }
        sanityText.text = "";
    }

    [HarmonyPatch(typeof(CollectionBinderUI))]
    public class Awake
    {

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        static void Postfix(CollectionBinderUI __instance)
        {
            
            commonText = GameObject.Instantiate(__instance.m_CardCollectedText, __instance.m_CardCollectedText.transform.parent);
            rareText = GameObject.Instantiate(__instance.m_CardCollectedText, __instance.m_CardCollectedText.transform.parent);
            epicText = GameObject.Instantiate(__instance.m_CardCollectedText, __instance.m_CardCollectedText.transform.parent);
            legendaryText = GameObject.Instantiate(__instance.m_CardCollectedText, __instance.m_CardCollectedText.transform.parent);
            commonText.text = "";
            rareText.text = "";
            epicText.text = "";
            legendaryText.text = "";

            sanityText = GameObject.Instantiate(__instance.m_CardCollectedText, __instance.m_CardCollectedText.transform.parent);
            sanityText.rectTransform.anchoredPosition += new Vector2(0, -80);
            ECardExpansionType eCardExpansionType = __instance.m_CollectionAlbum.m_ExpansionType;
            if (eCardExpansionType == ECardExpansionType.Ghost)
            {
                sanityText.text = "";
                if (Plugin.m_SessionHandler.GetSlotData().Goal == 2)
                {
                    sanityText.text = $"Sold {Plugin.m_SaveManager.GetExpansionChecks(eCardExpansionType)} / {Plugin.m_SaveManager.getTotalExpansionChecks(eCardExpansionType)} {eCardExpansionType.ToString()} Checks";
                }
                
            }
            else if (Plugin.m_SessionHandler.GetSlotData().CardSanity > 0)
            {

                sanityText.text = $"{Plugin.m_SaveManager.GetExpansionChecks(eCardExpansionType)} / {Plugin.m_SaveManager.getTotalExpansionChecks(eCardExpansionType)} {eCardExpansionType.ToString()} Checks";
                
            }
            else
            {
                updatepackText(eCardExpansionType);
                commonText.rectTransform.anchoredPosition += new Vector2(-100, -60);
                rareText.rectTransform.anchoredPosition += new Vector2(100, -60);
                epicText.rectTransform.anchoredPosition += new Vector2(-100, -120);
                legendaryText.rectTransform.anchoredPosition += new Vector2(100, -120);
            }
        }
    }

    [HarmonyPatch(typeof(CollectionBinderUI))]
    public class SetCardCollected
    {

        [HarmonyPatch("SetCardCollected")]
        [HarmonyPostfix]
        static void Postfix(CollectionBinderUI __instance, int current, ECardExpansionType expansionType)
        {
            if (expansionType == ECardExpansionType.Ghost)
            {
                sanityText.text = "";
                if (Plugin.m_SessionHandler.GetSlotData().Goal == 2)
                {
                    sanityText.text = $"Sold {Plugin.m_SaveManager.GetExpansionChecks(expansionType)} / {Plugin.m_SaveManager.getTotalExpansionChecks(expansionType)} {expansionType.ToString()} Checks";
                }
                commonText.text = "";
                rareText.text = "";
                epicText.text = "";
                legendaryText.text = "";
            }
            else if (Plugin.m_SessionHandler.GetSlotData().CardSanity > 0)
            {
                ECardExpansionType eCardExpansionType = __instance.m_CollectionAlbum.m_ExpansionType;
                sanityText.text = $"{Plugin.m_SaveManager.GetExpansionChecks(eCardExpansionType)} / {Plugin.m_SaveManager.getTotalExpansionChecks(eCardExpansionType)} {eCardExpansionType.ToString()} Checks";

            }
            else
            {
                ECardExpansionType eCardExpansionType = __instance.m_CollectionAlbum.m_ExpansionType;

                updatepackText(eCardExpansionType);
                //GetCardChecks / GetTotalCardChecks for each card expansion
            }
        }
    }

    [HarmonyPatch(typeof(CollectionBinderUI))]
    public class CardText {

        [HarmonyPatch("OnPressSwitchExpansion")]
        [HarmonyPostfix]
        static void Postfix(CollectionBinderUI __instance, int expansionIndex)
        {
            ECardExpansionType expansionType = (ECardExpansionType)expansionIndex;
            if (Plugin.m_SessionHandler.GetSlotData().Goal == 2 && expansionType == ECardExpansionType.Ghost)
            {
                sanityText.text = "";
                if (Plugin.m_SessionHandler.GetSlotData().Goal == 2)
                {
                    sanityText.text = $"Sold {Plugin.m_SaveManager.GetExpansionChecks(expansionType)} / {Plugin.m_SaveManager.getTotalExpansionChecks(expansionType)} {expansionType.ToString()} Checks";
                }
                commonText.text = "";
                rareText.text = "";
                epicText.text = "";
                legendaryText.text = "";
            }
            else if (Plugin.m_SessionHandler.GetSlotData().CardSanity > 0)
            {
                ECardExpansionType eCardExpansionType = __instance.m_CollectionAlbum.m_ExpansionType;
                sanityText.text = $"{Plugin.m_SaveManager.GetExpansionChecks(eCardExpansionType)} / {Plugin.m_SaveManager.getTotalExpansionChecks(eCardExpansionType)} {eCardExpansionType.ToString()} Checks";

            }
            else
            {
                ECardExpansionType eCardExpansionType = __instance.m_CollectionAlbum.m_ExpansionType;

                updatepackText(eCardExpansionType);
            }
        }
    }

    [HarmonyPatch(typeof(BinderPageGrp))]
    public class single {

        [HarmonyPatch("SetSingleCard")]
        [HarmonyPostfix]
        static void SetSingleCard(BinderPageGrp __instance, int cardIndex, CardData cardData, int cardCount, ECollectionSortingType sortingType)
        {
            int num = CPlayerData.GetCardSaveIndex(cardData);
            bool found = CPlayerData.GetIsCardCollectedList(cardData.expansionType, cardData.expansionType == ECardExpansionType.Ghost)[num];
            if (found && cardCount <= 0 && cardData != null)
            {
                if (cardData.expansionType != ECardExpansionType.Ghost)
                {
                    SetCardUIAlpha(__instance.m_CardList[cardIndex], 0.35f);
                    __instance.m_CardList[cardIndex].m_CardUI.SetCardUI(cardData);
                    __instance.m_CardList[cardIndex].SetVisibility(isVisible: true);
                    //DisableInteractability(__instance.m_CardList[cardIndex].m_CardUI);

                    __instance.m_CardList[cardIndex].m_CardCountText.text = "Check Collected";
                    __instance.m_CardList[cardIndex].SetCardCountTextVisibility(isVisible: true);
                }
            }
            else
            {
                SetCardUIAlpha(__instance.m_CardList[cardIndex], 1f);
            }
        }
    }
    [HarmonyPatch(typeof(CollectionBinderFlipAnimCtrl))]
    public class onclick {

        [HarmonyPatch("OnMouseButtonUp")]
        [HarmonyPrefix]
        static bool Prefix(CollectionBinderFlipAnimCtrl __instance)
        {
            if(__instance.m_CurrentRaycastedInteractableCard3d == null)
            {
                return true;
            }
            CardData cardData = __instance.m_CurrentRaycastedInteractableCard3d.m_Card3dUI.m_CardUI.GetCardData();
            int num = CPlayerData.GetCardSaveIndex(cardData);
            int cardAmountByIndex = CPlayerData.GetCardAmountByIndex(num, cardData.expansionType, cardData.isDestiny);
            if (cardAmountByIndex <= 0)
            {
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(CollectionBinderFlipAnimCtrl))]
    public class onrightclick {

        [HarmonyPatch("OnRightMouseButtonUp")]
        [HarmonyPrefix]
        static bool Prefix(CollectionBinderFlipAnimCtrl __instance)
        {
            if (__instance.m_CurrentRaycastedInteractableCard3d == null)
            {
                return true;
            }
            CardData cardData = __instance.m_CurrentRaycastedInteractableCard3d.m_Card3dUI.m_CardUI.GetCardData();
            int num = CPlayerData.GetCardSaveIndex(cardData);
            int cardAmountByIndex = CPlayerData.GetCardAmountByIndex(num, cardData.expansionType, cardData.isDestiny);
            if (cardAmountByIndex <= 0)
            {
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(CollectionBinderFlipAnimCtrl))]
    public class Anim {

        [HarmonyPatch("UpdateBinderAllCardUI")]
        [HarmonyPostfix]
        static void UpdateBinderAllCardUI(CollectionBinderFlipAnimCtrl __instance, int binderIndex, int pageIndex)
        {
            if (pageIndex <= 0 || pageIndex > __instance.m_MaxIndex)
            {
                return;
            }

            for (int i = 0; i < __instance.m_BinderPageGrpList[binderIndex].m_CardList.Count; i++)
            {
                int num = (pageIndex - 1) * 12 + i;
                if (num >= __instance.m_SortedIndexList.Count)
                {
                    int num2 = __instance.m_SortedIndexList[num];
                    bool isDestiny = false;
                    if (__instance.m_ExpansionType == ECardExpansionType.Ghost && num2 >= InventoryBase.GetShownMonsterList(__instance.m_ExpansionType).Count * CPlayerData.GetCardAmountPerMonsterType(__instance.m_ExpansionType))
                    {
                        isDestiny = true;
                        num2 -= InventoryBase.GetShownMonsterList(__instance.m_ExpansionType).Count * CPlayerData.GetCardAmountPerMonsterType(__instance.m_ExpansionType);
                    }

                    CardData cardData = new CardData();
                    cardData.monsterType = CPlayerData.GetMonsterTypeFromCardSaveIndex(num2, __instance.m_ExpansionType);
                    cardData.isFoil = num2 % CPlayerData.GetCardAmountPerMonsterType(__instance.m_ExpansionType) >= CPlayerData.GetCardAmountPerMonsterType(__instance.m_ExpansionType, includeFoilCount: false);
                    cardData.borderType = (ECardBorderType)(num2 % CPlayerData.GetCardAmountPerMonsterType(__instance.m_ExpansionType, includeFoilCount: false));
                    cardData.isDestiny = isDestiny;
                    cardData.expansionType = __instance.m_ExpansionType;
                    __instance.m_BinderPageGrpList[binderIndex].SetSingleCard(i, cardData, 0, __instance.m_SortingType);
                }
            }
        }
    }

    [HarmonyPatch(typeof(BinderPageGrp))]
    public class set
    {
        [HarmonyPatch("SetCard")]
        [HarmonyPostfix]
        static void PostFix(BinderPageGrp __instance, int index, ECardExpansionType cardExpansionType, bool isDimensionCard)
        {


            for (int i = 0; i < __instance.m_CardList.Count; i++)
            {
                int num = (index - 1) * CPlayerData.GetCardAmountPerMonsterType(cardExpansionType) + i;
                int cardAmountByIndex = CPlayerData.GetCardAmountByIndex(num, cardExpansionType, isDimensionCard);
                bool found = CPlayerData.GetIsCardCollectedList(cardExpansionType, isDimensionCard)[num];
                if (found && cardAmountByIndex <= 0)
                {
                    bool isDestiny = false;
                    if (cardExpansionType == ECardExpansionType.Ghost)// && num >= InventoryBase.GetShownMonsterList(cardExpansionType).Count * CPlayerData.GetCardAmountPerMonsterType(cardExpansionType))
                    {
                        continue;
                        //isDestiny = true;
                        //num -= InventoryBase.GetShownMonsterList(cardExpansionType).Count * CPlayerData.GetCardAmountPerMonsterType(cardExpansionType);
                    }

                    CardData cardData = new CardData();
                    ECardBorderType borderType = (ECardBorderType)(i % CPlayerData.GetCardAmountPerMonsterType(cardExpansionType, includeFoilCount: false));
                    cardData.monsterType = CPlayerData.GetMonsterTypeFromCardSaveIndex(num, cardExpansionType);
                    cardData.borderType = borderType;
                    cardData.expansionType = cardExpansionType;
                    cardData.isDestiny = isDestiny;
                    cardData.isNew = false;
                    cardData.isFoil = i >= CPlayerData.GetCardAmountPerMonsterType(cardExpansionType, includeFoilCount: false);
                    __instance.m_CardList[i].m_CardUI.SetCardUI(cardData);
                    __instance.m_CardList[i].SetVisibility(isVisible: true);
                    __instance.m_CardList[i].m_CardCountText.text = cardExpansionType == ECardExpansionType.Ghost ? "Ghost Found" : "Check Collected";
                    __instance.m_CardList[i].SetCardCountTextVisibility(isVisible: true);

                    SetCardUIAlpha(__instance.m_CardList[i], 0.35f);
                }
            }
        }
    }

    static void SetCardUIAlpha(Card3dUIGroup cardUI, float alpha)
    {
        //Debug.Log($"Found {cardUI.GetComponentsInChildren<UnityEngine.UI.Image>(true).Length} images to alpha");
        if (cardUI == null) return;

        // Set alpha on all images
        Image[] targets = {
        cardUI.m_CardUI.m_CardBGImage,
        cardUI.m_CardUI.m_CardBorderImage,
        cardUI.m_CardUI.m_MonsterImage,
        cardUI.m_CardUI.m_MonsterMaskImage,
        cardUI.m_CardUI.m_RarityImage
        };
        
        Material backMat = new Material(cardUI.m_CardUI.m_CardBackImage.material);
        backMat.SetColor("_Color", new Color(1, 1, 1, 0));
        cardUI.m_CardUI.m_CardBackImage.material = backMat;

        foreach (var img in targets)
        {
            if (img == null) continue;
            Material customMat = new Material(img.material);
            customMat.SetColor("_Color", new Color(1, 1, 1, alpha));
            img.material = customMat;
        }

        // Optional: fade out text
        foreach (var text in cardUI.m_CardUI.GetComponentsInChildren<TMPro.TextMeshProUGUI>(true))
        {
            var c = text.color;
            c.a = 0.5f;
            c.r = c.r / 2;
            c.b = c.b / 2;
            c.g = c.g / 2;
            text.color = c;
        }
        cardUI.m_CardCountText.color = Color.white;
        //// Optional: darken the card count text too
        //if (cardUI.TryGetComponent(out Card3dUIGroup parentGroup) && parentGroup.m_CardCountText != null)
        //{
        //    var countColor = parentGroup.m_CardCountText.color;
        //    countColor.a = alpha;
        //    parentGroup.m_CardCountText.color = countColor;
        //}
    }

    static void DisableInteractability(CardUI cardUI)
    {
        foreach (var img in cardUI.GetComponentsInChildren<UnityEngine.UI.Image>(true))
        {
            img.raycastTarget = false;
        }

        foreach (var text in cardUI.GetComponentsInChildren<TMPro.TextMeshProUGUI>(true))
        {
            text.raycastTarget = false;
        }
    }
}
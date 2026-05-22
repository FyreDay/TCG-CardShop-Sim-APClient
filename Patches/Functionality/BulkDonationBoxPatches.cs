using HarmonyLib;
using I2.Loc;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ApClient.Patches.Functionality;

public class BulkDonationBoxPatches
{
    [HarmonyPatch(typeof(BulkDonationBoxPlusMinusScreen), "OnInputTextUpdated")]
    class TextUpdated
    {
        [HarmonyPrefix]
        static bool Prefix(BulkDonationBoxPlusMinusScreen __instance, string text)
        {
            int stackCardCount = __instance.m_StackCardCount;
            int num = Mathf.FloorToInt(GameInstance.GetInvariantCultureDecimal(text));
            if (num <= 0)
            {
                __instance.OnPressRemoveAllBtn();
                return false;
            }

            int num2 = num - stackCardCount;
            if (num2 > 0)
            {
                int num3 = __instance.m_BulkDonationBoxUIScreen.GetBoxTotalCardCountMax() - __instance.m_BoxTotalCardCount;
                if (num2 > num3)
                {
                    num2 = num3;
                }

                if (num2 > __instance.m_AlbumCount)
                {
                    num2 = __instance.m_AlbumCount;
                }

                if (num2 > 0)
                {
                    CPlayerData.ReduceCard(__instance.m_CardData, num2);
                    __instance.m_BulkDonationBoxUIScreen.UpdateCompactData(num2);
                    __instance.m_AlbumCount -= num2;
                    __instance.m_StackCardCount += num2;
                    __instance.m_BoxTotalCardCount += num2;
                    __instance.m_InAlbumCountText.text = LocalizationManager.GetTranslation("In Album") + " : " + __instance.m_AlbumCount;
                    __instance.m_TotalCardCountText.text = __instance.m_StackCardCount.ToString();
                    __instance.m_IsUpdateDeckEditUI = true;
                }
            }
            else if (num2 < 0)
            {
                int num4 = Mathf.Abs(num2);
                CPlayerData.AddCard(__instance.m_CardData, num4);
                __instance.m_BulkDonationBoxUIScreen.UpdateCompactData(-num4);
                __instance.m_AlbumCount += num4;
                __instance.m_StackCardCount -= num4;
                __instance.m_BoxTotalCardCount -= num4;
                __instance.m_InAlbumCountText.text = LocalizationManager.GetTranslation("In Album") + " : " + __instance.m_AlbumCount;
                __instance.m_TotalCardCountText.text = __instance.m_StackCardCount.ToString();
                __instance.m_IsUpdateDeckEditUI = true;
            }

            __instance.m_CardAmountInputDisplay.text = __instance.m_StackCardCount.ToString();
            __instance.m_TotalCardCountText.text = __instance.m_StackCardCount.ToString();
            __instance.m_CardAmountInputDisplay.gameObject.SetActive(value: false);
            __instance.m_TotalCardCountText.gameObject.SetActive(value: true);
            return false;
        }
    }
    [HarmonyPatch(typeof(BulkDonationBoxPlusMinusScreen), "OnPressRemoveAllBtn")]
    class RemoveAllBtn
    {
        [HarmonyPrefix]
        static bool Prefix(BulkDonationBoxPlusMinusScreen __instance)
        {
            if (__instance.m_StackCardCount > 0)
            {
                int stackCardCount = __instance.m_StackCardCount;
                PlayerDataPatches.stopCountingCard = true;
                CPlayerData.AddCard(__instance.m_CardData, stackCardCount);
                PlayerDataPatches.stopCountingCard = false;
                if (__instance.m_CardData.cardGrade > 0)
                {
                    CSingleton<InteractionPlayerController>.Instance.m_CollectionBinderFlipAnimCtrl.SetCanUpdateSort(canSort: true);
                    __instance.m_BulkDonationBoxUIScreen.RemoveCompactData();
                }
                else
                {
                    __instance.m_BulkDonationBoxUIScreen.UpdateCompactData(-stackCardCount);
                }

                __instance.m_AlbumCount += stackCardCount;
                __instance.m_StackCardCount = 0;
                __instance.m_BoxTotalCardCount -= stackCardCount;
                __instance.m_InAlbumCountText.text = LocalizationManager.GetTranslation("In Album") + " : " + __instance.m_AlbumCount;
                __instance.m_TotalCardCountText.text = __instance.m_StackCardCount.ToString();
                __instance.OnPressBack();
                __instance.m_BulkDonationBoxUIScreen.OnCloseCardPlusMinusScreen();
                SoundManager.GenericConfirm();
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(BulkDonationBoxPlusMinusScreen), "OnPressMinusBtn")]
    class MinusBtn
    {
        [HarmonyPrefix]
        static bool Prefix(BulkDonationBoxPlusMinusScreen __instance)
        {
            if (__instance.m_StackCardCount <= 1)
            {
                __instance.OnPressRemoveAllBtn();
                return false;
            }
            PlayerDataPatches.stopCountingCard = true;
            CPlayerData.AddCard(__instance.m_CardData, 1);
            PlayerDataPatches.stopCountingCard = false;
            if (__instance.m_CardData.cardGrade > 0)
            {
                CSingleton<InteractionPlayerController>.Instance.m_CollectionBinderFlipAnimCtrl.SetCanUpdateSort(canSort: true);
                __instance.m_BulkDonationBoxUIScreen.RemoveCompactData();
            }
            else
            {
                __instance.m_BulkDonationBoxUIScreen.UpdateCompactData(-1);
            }

            __instance.m_AlbumCount++;
            __instance.m_StackCardCount--;
            __instance.m_BoxTotalCardCount--;
            __instance.m_InAlbumCountText.text = LocalizationManager.GetTranslation("In Album") + " : " + __instance.m_AlbumCount;
            __instance.m_TotalCardCountText.text = __instance.m_StackCardCount.ToString();
            __instance.m_IsUpdateDeckEditUI = true;
            SoundManager.GenericConfirm();
            return false;
        }

    }

    [HarmonyPatch(typeof(BulkDonationBoxUIScreen), "UpdateSelectedCardData")]
    class Update
    {
        [HarmonyPrefix]
        static bool Prefix(BulkDonationBoxUIScreen __instance, CardData cardData)
        {
            if (cardData != null)
            {
                int num = 1;
                if (cardData.cardGrade > 0)
                {
                    num = 100;
                }

                if (__instance.m_TotalCardCount + num > __instance.GetBoxTotalCardCountMax())
                {
                    CSingleton<InteractionPlayerController>.Instance.m_CollectionBinderFlipAnimCtrl.SetCanUpdateSort(canSort: true);
                    PlayerDataPatches.stopCountingCard = true;
                    CPlayerData.AddCard(cardData, 1);
                    PlayerDataPatches.stopCountingCard = false;
                    NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.MaxDeckCardLimitReached);
                }
                else
                {
                    __instance.m_CurrentSelectedSlotIndex = __instance.AddCardDataToCompactCardData(ref __instance.m_CompactCardDataList, cardData, num);
                    __instance.EvaluateCardPanelUI(__instance.m_PageIndex);
                    if (CPlayerData.GetCardAmount(cardData) > 0 && cardData.cardGrade <= 0)
                    {
                        __instance.m_CardPlusMinusScreen.OpenPlusMinusScreen(cardData, __instance.m_CompactCardDataList[__instance.m_CurrentSelectedSlotIndex].amount, __instance.m_TotalCardCount);
                    }

                    if ((bool)__instance.m_InteractableBulkDonationBox)
                    {
                        __instance.m_InteractableBulkDonationBox.SetCompactCardDataAmountList(__instance.m_CompactCardDataList);
                    }
                    else if ((bool)__instance.m_InteractableCardStorageShelf)
                    {
                        __instance.m_InteractableCardStorageShelf.SetCompactCardDataAmountList(__instance.m_CompactCardDataList);
                    }
                }
            }

            __instance.m_ControllerScreenUIExtension.SetControllerUIActive(isActive: true);
            return false;
        }
    }

    [HarmonyPatch(typeof(BulkDonationBoxUIScreen), "RemoveAllCard")]
    class RemoveAll
    {
        [HarmonyPrefix]
        static bool Prefix(BulkDonationBoxUIScreen __instance)
        {
            for (int num = __instance.m_CompactCardDataList.Count - 1; num >= 0; num--)
            {
                CardData cardData = null;
                int addAmount = __instance.m_CompactCardDataList[num].amount;
                if (__instance.m_CompactCardDataList[num].gradedCardIndex > 0)
                {
                    cardData = CPlayerData.GetGradedCardData(__instance.m_CompactCardDataList[num]);
                    addAmount = 1;
                }
                else
                {
                    cardData = CPlayerData.GetCardData(__instance.m_CompactCardDataList[num].cardSaveIndex, __instance.m_CompactCardDataList[num].expansionType, __instance.m_CompactCardDataList[num].isDestiny);
                }
                PlayerDataPatches.stopCountingCard = true;
                CPlayerData.AddCard(cardData, addAmount);
                PlayerDataPatches.stopCountingCard = false;
                __instance.m_CompactCardDataList.RemoveAt(num);
            }

            __instance.m_PageIndex = 0;
            __instance.EvaluateCardPanelUI(__instance.m_PageIndex);
            if ((bool)__instance.m_InteractableBulkDonationBox)
            {
                __instance.m_InteractableBulkDonationBox.SetCompactCardDataAmountList(__instance.m_CompactCardDataList);
            }
            else if ((bool)__instance.m_InteractableCardStorageShelf)
            {
                __instance.m_InteractableCardStorageShelf.SetCompactCardDataAmountList(__instance.m_CompactCardDataList);
            }
            return false;
        }
    }
}

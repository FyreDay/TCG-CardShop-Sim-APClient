using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;

namespace ApClient.patches;

//GetPackContent(bool clearList, bool isPremiumPack, bool isSecondaryRolledData = false, ECollectionPackType overrideCollectionPackType = ECollectionPackType.None)
public class CardOpeningPatches
{
    [HarmonyPatch(typeof(CardOpeningSequence), "GetPackContent")]
    public class CardROlling
    {
        
        static bool Prefix(CardOpeningSequence __instance, bool clearList, int godPackRollIndex, bool isSecondaryRolledData = false, ECollectionPackType overrideCollectionPackType = ECollectionPackType.None)
        {
            int num = 0;
            if (clearList)
            {
                if (isSecondaryRolledData)
                {
                    __instance.m_SecondaryRolledCardDataList.Clear();
                }
                else
                {
                    __instance.m_RolledCardDataList.Clear();
                }
            }

            List<EMonsterType> list = new List<EMonsterType>();
            List<EMonsterType> BasicMonsters = new List<EMonsterType>();
            List<EMonsterType> RareMonsters = new List<EMonsterType>();
            List<EMonsterType> EpicMonsters = new List<EMonsterType>();
            List<EMonsterType> legendaryMonsters = new List<EMonsterType>();
            ECardExpansionType cardExpansionType = InventoryBase.GetCardExpansionType(__instance.m_CollectionPackType);
            if (isSecondaryRolledData)
            {
                cardExpansionType = InventoryBase.GetCardExpansionType(overrideCollectionPackType);
            }

            bool openPackCanUseRarity = InventoryBase.GetCardUISetting(cardExpansionType).openPackCanUseRarity;
            bool openPackCanHaveDuplicate = InventoryBase.GetCardUISetting(cardExpansionType).openPackCanHaveDuplicate;
            for (int i = 0; i < InventoryBase.GetShownMonsterList(cardExpansionType).Count; i++)
            {
                EMonsterType monsterType = InventoryBase.GetMonsterData(InventoryBase.GetShownMonsterList(cardExpansionType)[i]).MonsterType;
                ERarity rarity = InventoryBase.GetMonsterData(InventoryBase.GetShownMonsterList(cardExpansionType)[i]).Rarity;
                list.Add(monsterType);
                switch (rarity)
                {
                    case ERarity.Legendary:
                        legendaryMonsters.Add(monsterType);
                        break;
                    case ERarity.Epic:
                        EpicMonsters.Add(monsterType);
                        break;
                    case ERarity.Rare:
                        RareMonsters.Add(monsterType);
                        break;
                    default:
                        BasicMonsters.Add(monsterType);
                        break;
                }
            }

            int num2 = 0;
            int num3 = 0;
            int num4 = 0;
            int num5 = 0;
            int num6 = 1;
            int num7 = 0;
            int num8 = 0;
            int num9 = 0;
            float num10 = 10f;
            float num11 = 2f;
            float num12 = 0.1f;
            float foilPullRate = 5f + .45f * Plugin.getNumLuckItems();
            ECardBorderType borderType = ECardBorderType.Base;
            float FirstEdPullRate = 20f;
            float SilverEdPullRate = 8f + .12f * Plugin.getNumLuckItems(); ;
            float GoldEdPullRate = 4f + .26f * Plugin.getNumLuckItems(); ;
            float EXPullRate = 1f + .29f * Plugin.getNumLuckItems(); ;
            float FullArtPullRate = 0.25f + .5f * Plugin.getNumLuckItems(); ;
            ERarity eRarity = ERarity.Common;
            int num19 = 7;
            if (__instance.m_CollectionPackType == ECollectionPackType.RareCardPack || __instance.m_CollectionPackType == ECollectionPackType.DestinyRareCardPack)
            {
                num6 = 0;
                num7 = 2;
                num7 = 7;
                num10 += 45f;
                num11 += 2f;
                num12 += 1f;
            }
            else if (__instance.m_CollectionPackType == ECollectionPackType.EpicCardPack || __instance.m_CollectionPackType == ECollectionPackType.DestinyEpicCardPack)
            {
                num6 = 0;
                num7 = 1;
                num8 = 2;
                num8 = 7;
                num9 = 0;
                num10 += 65f;
                num11 += 45f;
                num12 += 3f;
            }
            else if (__instance.m_CollectionPackType == ECollectionPackType.LegendaryCardPack || __instance.m_CollectionPackType == ECollectionPackType.DestinyLegendaryCardPack)
            {
                num6 = 0;
                num7 = 0;
                num8 = 1;
                num9 = 2;
                num9 = 7;
                num10 += 65f;
                num11 += 55f;
                num12 += 35f;
            }
            else if (__instance.m_CollectionPackType == ECollectionPackType.BasicCardPack || __instance.m_CollectionPackType == ECollectionPackType.DestinyBasicCardPack)
            {
                num6 = 7;
            }

            if (godPackRollIndex > 0)
            {
                foilPullRate = 0f;
                FirstEdPullRate = 0f;
                SilverEdPullRate = 0f;
                GoldEdPullRate = 0f;
                EXPullRate = 0f;
                FullArtPullRate = 0f;
            }

            switch (godPackRollIndex)
            {
                case 1:
                    FirstEdPullRate = 100f;
                    break;
                case 2:
                    SilverEdPullRate = 100f;
                    break;
                case 3:
                    GoldEdPullRate = 100f;
                    break;
                case 4:
                    EXPullRate = 100f;
                    break;
                case 5:
                    FullArtPullRate = 100f;
                    break;
                case 6:
                    foilPullRate = 10000f;
                    FirstEdPullRate = 100f;
                    break;
                case 7:
                    foilPullRate = 10000f;
                    SilverEdPullRate = 100f;
                    break;
                case 8:
                    foilPullRate = 10000f;
                    GoldEdPullRate = 100f;
                    break;
                case 9:
                    foilPullRate = 10000f;
                    EXPullRate = 100f;
                    break;
                case 10:
                    foilPullRate = 10000f;
                    FullArtPullRate = 100f;
                    break;
                case 12:
                    foilPullRate = 10000f;
                    break;
                case 13:
                    foilPullRate = 10000f;
                    break;
            }

            for (int j = 0; j < num19; j++)
            {
                if (list.Count <= 0)
                {
                    break;
                }

                int num20 = UnityEngine.Random.Range(0, list.Count);
                if (num9 - num5 > 0 && legendaryMonsters.Count > 0)
                {
                    eRarity = ERarity.Legendary;
                    num5++;
                }
                else if (num8 - num4 > 0 && EpicMonsters.Count > 0)
                {
                    eRarity = ERarity.Epic;
                    num4++;
                }
                else if (num7 - num3 > 0 && RareMonsters.Count > 0)
                {
                    eRarity = ERarity.Rare;
                    num3++;
                }
                else if (num6 - num2 > 0 && BasicMonsters.Count > 0)
                {
                    eRarity = ERarity.Common;
                    num2++;
                }
                else
                {
                    int num21 = UnityEngine.Random.Range(0, 10000);
                    int num22 = 0;
                    int num23 = 4 - num3;
                    int num24 = 4 - num4;
                    int num25 = 4 - num5;
                    bool flag = false;
                    if (!flag && num12 > 0f && legendaryMonsters.Count > 0 && num25 > 0)
                    {
                        num22 = Mathf.RoundToInt(num12 * 100f);
                        if (num21 < num22)
                        {
                            flag = true;
                            eRarity = ERarity.Legendary;
                            num5++;
                        }
                    }

                    if (!flag && num11 > 0f && EpicMonsters.Count > 0 && num24 > 0)
                    {
                        num22 = Mathf.RoundToInt(num11 * 100f);
                        if (num21 < num22)
                        {
                            flag = true;
                            eRarity = ERarity.Epic;
                            num4++;
                        }
                    }

                    if (!flag && num10 > 0f && RareMonsters.Count > 0 && num23 > 0)
                    {
                        num22 = Mathf.RoundToInt(num10 * 100f);
                        if (num21 < num22)
                        {
                            flag = true;
                            eRarity = ERarity.Rare;
                            num3++;
                        }
                    }

                    if (!flag)
                    {
                        flag = true;
                        eRarity = ERarity.Common;
                        num2++;
                    }
                }

                if (openPackCanUseRarity)
                {
                    switch (eRarity)
                    {
                        case ERarity.Legendary:
                            num20 = UnityEngine.Random.Range(0, legendaryMonsters.Count);
                            num = (int)legendaryMonsters[num20];
                            if (!openPackCanHaveDuplicate)
                            {
                                legendaryMonsters.RemoveAt(num20);
                            }

                            break;
                        case ERarity.Epic:
                            num20 = UnityEngine.Random.Range(0, EpicMonsters.Count);
                            num = (int)EpicMonsters[num20];
                            if (!openPackCanHaveDuplicate)
                            {
                                EpicMonsters.RemoveAt(num20);
                            }

                            break;
                        case ERarity.Rare:
                            num20 = UnityEngine.Random.Range(0, RareMonsters.Count);
                            num = (int)RareMonsters[num20];
                            if (!openPackCanHaveDuplicate)
                            {
                                RareMonsters.RemoveAt(num20);
                            }

                            break;
                        default:
                            num20 = UnityEngine.Random.Range(0, BasicMonsters.Count);
                            num = (int)BasicMonsters[num20];
                            if (!openPackCanHaveDuplicate)
                            {
                                BasicMonsters.RemoveAt(num20);
                            }

                            break;
                    }
                }
                else
                {
                    num20 = UnityEngine.Random.Range(0, list.Count);
                    num = (int)list[num20];
                    if (!openPackCanHaveDuplicate)
                    {
                        list.RemoveAt(num20);
                    }
                }

                CardData cardData = __instance.m_CardDataPool[j];
                if (isSecondaryRolledData)
                {
                    cardData = __instance.m_CardDataPool2[j];
                }

                cardData.monsterType = (EMonsterType)num;
                if (UnityEngine.Random.Range(0, 10000) < Mathf.RoundToInt(foilPullRate * 100f))
                {
                    cardData.isFoil = true;
                    __instance.m_HasFoilCard = true;
                }
                else
                {
                    cardData.isFoil = false;
                }

                if (CPlayerData.m_TutorialIndex < 10 && CPlayerData.m_GameReportDataCollectPermanent.cardPackOpened == 0 && !__instance.m_HasFoilCard && j == num19 - 1)
                {
                    cardData.isFoil = true;
                    __instance.m_HasFoilCard = true;
                }

                bool flag2 = false;
                if (UnityEngine.Random.Range(0, 10000) < Mathf.RoundToInt(FullArtPullRate * 100f))
                {
                    borderType = ECardBorderType.FullArt;
                    flag2 = true;
                }

                if (!flag2 && UnityEngine.Random.Range(0, 10000) < Mathf.RoundToInt(EXPullRate * 100f))
                {
                    borderType = ECardBorderType.EX;
                    flag2 = true;
                }

                if (!flag2 && UnityEngine.Random.Range(0, 10000) < Mathf.RoundToInt(GoldEdPullRate * 100f))
                {
                    borderType = ECardBorderType.Gold;
                    flag2 = true;
                }

                if (!flag2 && UnityEngine.Random.Range(0, 10000) < Mathf.RoundToInt(SilverEdPullRate * 100f))
                {
                    borderType = ECardBorderType.Silver;
                    flag2 = true;
                }

                if (!flag2 && UnityEngine.Random.Range(0, 10000) < Mathf.RoundToInt(FirstEdPullRate * 100f))
                {
                    borderType = ECardBorderType.FirstEdition;
                    flag2 = true;
                }

                if (!flag2 || cardExpansionType == ECardExpansionType.Ghost)
                {
                    borderType = ECardBorderType.Base;
                }

                cardData.borderType = borderType;
                cardData.expansionType = cardExpansionType;
                if (cardData.expansionType == ECardExpansionType.Tetramon)
                {
                    cardData.isDestiny = false;
                }
                else if (cardData.expansionType == ECardExpansionType.Destiny)
                {
                    cardData.isDestiny = true;
                }
                else if (cardData.expansionType == ECardExpansionType.Ghost)
                {
                    int num26 = UnityEngine.Random.Range(0, 100);
                    cardData.isDestiny = num26 < 50;
                }
                else
                {
                    cardData.isDestiny = false;
                }

                if (isSecondaryRolledData)
                {
                    __instance.m_SecondaryRolledCardDataList.Add(cardData);
                }
                else
                {
                    __instance.m_RolledCardDataList.Add(cardData);
                }
            }

            list.Clear();
            BasicMonsters.Clear();
            RareMonsters.Clear();
            EpicMonsters.Clear();
            legendaryMonsters.Clear();
            __instance.m_GCCollectCount++;
            if (__instance.m_GCCollectCount >= 100)
            {
                __instance.m_GCCollectCount = 0;
                GC.Collect();
            }
            return false;
        }
    }
}

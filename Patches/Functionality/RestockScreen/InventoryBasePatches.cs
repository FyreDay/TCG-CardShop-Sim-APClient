using ApClient.data;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApClient.Patches.Functionality.RestockScreen;

public class InventoryBasePatches
{
    [HarmonyPatch(typeof(InventoryBase), "Start")]
    public class StartPatches
    {
        [HarmonyPostfix]

        static void StartPatch(InventoryBase __instance)
        {
            
            var stockData = __instance.m_StockItemData_SO;

            List<EItemType> itemTypes = new List<EItemType>();

            foreach (DictionaryEntry item in Plugin.ArchipelagoHandler.slotData.pg1IndexMapping)
            {
                Plugin.Logger.LogInfo($"{(EItemType)item.Key} : level - {(int)item.Value}");
                itemTypes.Add((EItemType)item.Key);

                foreach (RestockData restock in stockData.m_RestockDataList)
                {
                    
                    if (restock.itemType == (EItemType)item.Key)
                    {
                        restock.licenseShopLevelRequired = (int)item.Value;
                        restock.index = itemTypes.Count() - 1;
                        restock.isHideItemUntilUnlocked = false;
                    }
                }
                
            }

            var notInMultiworld = stockData.m_ShownItemType.Except(itemTypes);
            foreach (var item in notInMultiworld) Plugin.Logger.LogInfo(stockData.m_ItemDataList[(int)item].GetName());

            stockData.m_ShownItemType = itemTypes;
        }
    }
   
}

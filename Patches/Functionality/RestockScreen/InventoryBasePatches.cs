using ApClient.Archipelago.Mapping;
using ApClient.data;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ApClient.Patches.Functionality.RestockScreen;

public class InventoryBasePatches
{
    [HarmonyPatch(typeof(InventoryBase), "Start")]
    public class StartPatches
    {
        private static List<EItemType> UpdateShopList(OrderedDictionary mapping, StockItemData_ScriptableObject stockData)
        {
            List<EItemType> itemTypes = new List<EItemType>();

            foreach (DictionaryEntry item in mapping)
            {
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
            return itemTypes;
        }

        [HarmonyPostfix]
        static void StartPatch(InventoryBase __instance)
        {
            for (int i = 0; i < __instance.m_ObjectData_SO.m_FurniturePurchaseDataList.Count; i++)
            {
                Plugin.Logger.LogInfo(__instance.m_ObjectData_SO.m_FurniturePurchaseDataList[i].objectType);
            }

            var stockData = __instance.m_StockItemData_SO;
            stockData.m_ShownItemType = UpdateShopList(Plugin.ArchipelagoHandler.slotData.pg1IndexMapping, stockData);
            stockData.m_ShownAccessoryItemType = UpdateShopList(Plugin.ArchipelagoHandler.slotData.pg2IndexMapping, stockData);
            stockData.m_ShownFigurineItemType = UpdateShopList(Plugin.ArchipelagoHandler.slotData.pg3IndexMapping, stockData);
            stockData.m_ShownBoardGameItemType = UpdateShopList(Plugin.ArchipelagoHandler.slotData.ttIndexMapping, stockData);

            var orderLookup = FurnatureMapping.Furnature
                .Select((entry, index) => new { entry.Item1, index })
                .ToDictionary(x => x.Item1, x => x.index);

            foreach (var item in __instance.m_ObjectData_SO.m_FurniturePurchaseDataList)
            {
                item.levelRequirement = 1;

                if (!orderLookup.ContainsKey(item.objectType))
                    Plugin.Logger.LogWarning($"!!WARNING!! New Furnature added: {item.objectType}");
            }

            __instance.m_ObjectData_SO.m_FurniturePurchaseDataList = __instance.m_ObjectData_SO.m_FurniturePurchaseDataList
                .OrderBy(x => orderLookup.TryGetValue(x.objectType, out var index) ? index : int.MaxValue)
                .ToList();
        }
    }
   
}

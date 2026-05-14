using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApClient.Archipelago.Mapping;

public class LicenseMapping
{
    public const int SELL_CHECK_START_ID = 3000;
    public const int BASIC_CARD_PACK_ID = 190;
    public static List<(int id, int count)> GetLocations(EItemType type)
    {
        int id = type == EItemType.BasicCardPack ? 190 : (int)type;
        int baseAmount = Plugin.ArchipelagoHandler.slotData.SellCheckAmount;
        int startingAmount = Plugin.ArchipelagoHandler.slotData.startingItems.Contains(id) ? Plugin.ArchipelagoHandler.slotData.ExtraStartingItemChecks : 0;
        var list = new List<(int id, int count)>();
        int amountInBox = InventoryBase.GetRestockDataUsingItemType(type).OrderBy(x => x.amount).FirstOrDefault().amount;
        for (int i = 1; i <= baseAmount + startingAmount; i++)
        {
            list.Add((SELL_CHECK_START_ID + (id * 16) + (i - 1), amountInBox * i));
        }
        return list;
    }
}

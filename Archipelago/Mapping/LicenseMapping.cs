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
        try
        {
            int amountInBox = 0;
            if (!BoxOverrideAmount.TryGetValue(type, out amountInBox))
            {
                amountInBox = InventoryBase.GetRestockDataUsingItemType(type).OrderBy(x => x.amount).FirstOrDefault().amount;
            }
            for (int i = 1; i <= baseAmount + startingAmount; i++)
            {
                list.Add((SELL_CHECK_START_ID + (id * 16) + (i - 1), amountInBox * i));
            }
        }
        catch (Exception e)
        {
            Plugin.Logger.LogError($"Failed to get restock data for {type} with id {id} : {e}");
        }
        return list;
    }

    public static Dictionary<EItemType, int> BoxOverrideAmount = new Dictionary<EItemType, int>()
    {
        { EItemType.Deodorant, 8 },
        { EItemType.DeckBox1, 8 },
        { EItemType.DeckBox2, 8 },
        { EItemType.DeckBox3, 8 },
        { EItemType.DeckBox4, 8 },
        { EItemType.Toy_PiggyA, 12 },
        { EItemType.Toy_GolemA, 18 },
        { EItemType.Toy_StarfishA, 24 },
        { EItemType.Toy_BatA, 6 },
        { EItemType.Toy_PiggyB, 12 },
        { EItemType.Toy_GolemB, 8 },
        { EItemType.Toy_StarfishB, 12 },
        { EItemType.Toy_BatB, 8 },
        { EItemType.Toy_PiggyC, 2 },
        { EItemType.Toy_GolemC, 2 },
        { EItemType.Toy_StarfishC, 2 },
        { EItemType.Toy_BatC, 2 },
        { EItemType.Toy_PiggyD, 2 },
        { EItemType.Toy_GolemD, 2 },
        { EItemType.Toy_StarfishD, 2 },
        { EItemType.Toy_BatD, 1 },
        { EItemType.Toy_Beetle, 4 },
        { EItemType.Toy_ToonZ, 6 },
        { EItemType.PreconDeck_Fire, 18 },
        { EItemType.PreconDeck_Earth, 18 },
        { EItemType.PreconDeck_Water, 18 },
        { EItemType.PreconDeck_Wind, 18 },
        { EItemType.PreconDeck_EarthDestiny, 18 },
        { EItemType.PreconDeck_FireDestiny, 18 },
        { EItemType.PreconDeck_WaterDestiny, 18 },
        { EItemType.PreconDeck_WindDestiny, 18 },
        { EItemType.CardSleeve_Clear, 40 },
        { EItemType.CardSleeve_Earth, 40 },
        { EItemType.CardSleeve_Fire, 40 },
        { EItemType.CardSleeve_Water, 40 },
        { EItemType.CardSleeve_Wind, 40 },
        { EItemType.CardSleeve_Tetramon, 40 },
        { EItemType.Playmat1, 16 },
        { EItemType.Playmat10, 16 },
        { EItemType.Playmat11, 16 },
        { EItemType.Playmat12, 16 },
        { EItemType.Playmat13, 16 },
        { EItemType.Playmat14, 16 },
        { EItemType.PlayMat15, 16 },
        { EItemType.PlayMat16, 16 },
        { EItemType.PlayMat17, 16 },
        { EItemType.PlayMat18, 16 },
        { EItemType.Playmat2, 16 },
        { EItemType.Playmat2b, 16 },
        { EItemType.Playmat3, 16 },
        { EItemType.Playmat4, 16 },
        { EItemType.Playmat5, 16 },
        { EItemType.Playmat6, 16 },
        { EItemType.Playmat7, 16 },
        { EItemType.Playmat8, 16 },
        { EItemType.Playmat9, 16 },
        { EItemType.Boardgame_Speedrobo_Necro, 6 },
        { EItemType.Boardgame_Speedrobo_Claim, 12 },
        { EItemType.Boardgame_Speedrobo_SystemGate1, 2 },
        { EItemType.Boardgame_Speedrobo_SystemGate2, 2 },
        { EItemType.Boardgame_Speedrobo_Mafia, 2 },
        { EItemType.Manga1, 36 },
        { EItemType.Manga2, 36 },
        { EItemType.Manga3, 36 },
        { EItemType.Manga4, 36 },
        { EItemType.Manga5, 36 },
        { EItemType.Manga6, 36 },
        { EItemType.Manga7, 36 },
        { EItemType.Manga8, 36 },
        { EItemType.Manga9, 36 },
        { EItemType.Manga10, 36 },
        { EItemType.Manga11, 36 },
        { EItemType.Manga12, 36 },
        { EItemType.Manga13, 36 },
        { EItemType.Manga14, 36 },
        { EItemType.UP_MagneticHolder, 40 },
        { EItemType.UP_PennySleeves, 40 },
        { EItemType.UP_TowerDeckbox, 16 },
        { EItemType.UP_Toploader, 32 },
        { EItemType.UP_CardPreserver, 40 },
        { EItemType.UP_Playmat1, 16 },
        { EItemType.UP_Playmat2, 16 },
        { EItemType.UP_Playmat3, 16 },
        { EItemType.UP_Playmat4, 16 },
        { EItemType.UP_Album, 4 },
        { EItemType.BulkBox_TetramonBase, 1 },
        { EItemType.BulkBox_TetramonDestiny, 1 },
    };
}

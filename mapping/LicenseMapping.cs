using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApClient.mapping;

public class LicenseMapping
{
    public static SortedDictionary<int, (int itemid, string name, int count, int locid, EItemType type)> mapping = new SortedDictionary<int, (int itemid, string name, int count, int locid, EItemType type)>
    {
        {0, (0x1F280001, "Basic Card Pack (32)", 32, 0x1f2800f0, EItemType.BasicCardPack) },
        {1, (0x1F2800D8, "Basic Card Pack (64)", 64, 0x1f2800f0, EItemType.BasicCardPack) },
        {2, (0x1F2800D9, "Progressive Basic Card Pack", 4,0x1f2800f1, EItemType.BasicCardBox) },
        {3, (0x1F2800DA, "Progressive Basic Card Pack", 8,0x1F2800F1, EItemType.BasicCardBox) },
        {4, (0x1F280002, "Progressive Rare Card Pack",32,0x1F2800F2, EItemType.RareCardPack) },
        {5, (0x1F2800DB, "Progressive Rare Card Pack",64,0x1F2800F2, EItemType.RareCardPack) },
        {6, (0x1F2800DC, "Progressive Rare Card Pack",4,0x1F2800F3, EItemType.RareCardBox) },
        {7, (0x1F2800DD, "Progressive Rare Card Pack",8,0x1F2800F3, EItemType.RareCardBox) },
        {8, (0x1F280003, "Progressive Epic Card Pack",32,0x1F2800F4, EItemType.EpicCardPack) },
        {9, (0x1F2800DE, "Progressive Epic Card Pack",64,0x1F2800F4, EItemType.EpicCardPack) },
        {10, (0x1F2800DF, "Progressive Epic Card Pack",4,0x1F2800F5, EItemType.EpicCardBox) },
        {11, (0x1F2800E0, "Progressive Epic Card Pack",8,0x1F2800F5, EItemType.EpicCardBox) },
        {12, (0x1F280004, "Progressive Legendary Card Pack",32,0x1F2800F6, EItemType.LegendaryCardPack) },
        {13, (0x1F2800E1, "Progressive Legendary Card Pack",64,0x1F2800F6, EItemType.LegendaryCardPack) },
        {14, (0x1F2800E2, "Progressive Legendary Card Pack",4,0x1F2800F7, EItemType.LegendaryCardBox) },
        {15, (0x1F2800E3, "Progressive Legendary Card Pack",8,0x1F2800F7, EItemType.LegendaryCardBox) },
        {67, (0x1F280005, "Fire Battle Deck",9,0x1F280100, EItemType.PreconDeck_Fire) },
        {68, (0x1F280006, "Earth Battle Deck",9,0x1F280101, EItemType.PreconDeck_Earth) },
        {69, (0x1F280007, "Water Battle Deck",9,0x1F280102,EItemType.PreconDeck_Water ) },
        {70, (0x1F280008, "Wind Battle Deck",9,0x1F280103,EItemType.PreconDeck_Wind ) },
        {24, (0x1F280009, "Progressive Basic Destiny Pack", 32, 0x1F2800F8,EItemType.DestinyBasicCardPack ) },
        {25, (0x1F2800E4, "Progressive Basic Destiny Pack", 64,0x1F2800F8,EItemType.DestinyBasicCardPack ) },
        {26, (0x1F2800E5, "Progressive Basic Destiny Pack", 4,0x1F2800F9,EItemType.DestinyBasicCardBox) },
        {27, (0x1F2800E6, "Progressive Basic Destiny Pack", 8, 0x1F2800F9,EItemType.DestinyBasicCardBox) },
        {28, (0x1F28000A, "Progressive Rare Destiny Pack",32,0x1F2800FA,EItemType.DestinyRareCardPack ) },
        {29, (0x1F2800E7, "Progressive Rare Destiny Pack",64,0x1F2800FA,EItemType.DestinyRareCardPack ) },
        {30, (0x1F2800E8, "Progressive Rare Destiny Pack",4,0x1F2800FB,EItemType.DestinyRareCardBox  ) },
        {31, (0x1F2800E9, "Progressive Rare Destiny Pack",8,0x1F2800FB,EItemType.DestinyRareCardBox  ) },
        {32, (0x1F28000B, "Progressive Epic Destiny Pack",32,0x1F2800FC,EItemType.DestinyEpicCardPack) },
        {33, (0x1F2800EA, "Progressive Epic Destiny Pack",64,0x1F2800FC,EItemType.DestinyEpicCardPack) },
        {34, (0x1F2800EB, "Progressive Epic Destiny Pack",4,0x1F2800FD,EItemType.DestinyEpicCardBox) },
        {35, (0x1F2800EC, "Progressive Epic Destiny Pack",8,0x1F2800FD,EItemType.DestinyEpicCardBox) },
        {36, (0x1F28000C, "Progressive Legendary Destiny Pack",32,0x1F2800FE,EItemType.DestinyLegendaryCardPack) },
        {37, (0x1F2800ED, "Progressive Legendary Destiny Pack",64,0x1F2800FE,EItemType.DestinyLegendaryCardPack) },
        {38, (0x1F2800EE, "Progressive Legendary Destiny Pack",4,0x1F2800FF,EItemType.DestinyLegendaryCardBox ) },
        {39, (0x1F2800EF, "Progressive Legendary Destiny Pack",8,0x1F2800FF,EItemType.DestinyLegendaryCardBox ) },
        {71, (0x1F28000D, "Fire Destiny Deck",9,0x1F280104,EItemType.PreconDeck_FireDestiny ) },
        {72, (0x1F28000E, "Earth Destiny Deck",9,0x1F280105,EItemType.PreconDeck_EarthDestiny  ) },
        {73, (0x1F28000F, "Water Destiny Deck",9,0x1F280106,EItemType.PreconDeck_WaterDestiny  ) },
        {74, (0x1F280010, "Wind Destiny Deck",9,0x1F280107,EItemType.PreconDeck_WindDestiny ) },//end
        {40, (0x1F280011, "Cleanser (8)",8,0x1f280108,EItemType.Deodorant ) },
        {41, (0x1F2800F0, "Progressive Cleanser",16,0x1f280108,EItemType.Deodorant ) },
        {75, (0x1F280012, "Card Sleeves (Clear)",32,0x1f280109,EItemType.CardSleeve_Clear  ) },
        {76, (0x1F280013, "Card Sleeves (Tetramon)",32,0x1f28010A,EItemType.CardSleeve_Tetramon) },
        {43, (0x1F280014, "D20 Dice Red (16)",16,0x1f28010B,EItemType.D20DiceBox ) },
        {44, (0x1F280015, "D20 Dice Blue (16)",16,0x1f28010C,EItemType.D20DiceBox2) },
        {45, (0x1F280016, "D20 Dice Black (16)",16,0x1f28010D,EItemType.D20DiceBox3) },
        {46, (0x1F280017, "D20 Dice White (16)",16,0x1f28010E,EItemType.D20DiceBox4) },
        {77, (0x1F280018, "Card Sleeves (Fire)",32,0x1f28010F,EItemType.CardSleeve_Fire) },
        {78, (0x1F280019, "Card Sleeves (Earth)",32,0x1F280110,EItemType.CardSleeve_Earth) },
        {79, (0x1F28001A, "Card Sleeves (Water)",32,0x1F280111,EItemType.CardSleeve_Water) },
        {80, (0x1F28001B, "Card Sleeves (Wind)",32,0x1F280112,EItemType.CardSleeve_Wind) },
        {16, (0x1F28001C, "Progressive Deck Box Red",8,0x1F280113,EItemType.DeckBox1) },
        {17, (0x1F2800F1, "Progressive Deck Box Red",16,0x1F280113,EItemType.DeckBox1) },
        {18, (0x1F28001D, "Progressive Deck Box Green",8,0x1F280114,EItemType.DeckBox2) },
        {19, (0x1F2800F2, "Progressive Deck Box Green",16,0x1F280114,EItemType.DeckBox2) },
        {20, (0x1F28001E, "Progressive Deck Box Blue",8,0x1F280115,EItemType.DeckBox3) },
        {21, (0x1F2800F3, "Progressive Deck Box Blue",16,0x1F280115,EItemType.DeckBox3) },
        {22, (0x1F28001F, "Progressive Deck Box Yellow",8,0x1F280116,EItemType.DeckBox4) },
        {23, (0x1F2800F4, "Progressive Deck Box Yellow",16,0x1F280116, EItemType.DeckBox4) },//end
        {42, (0x1F280020, "Collection Book ",4,0x1F280117,EItemType.BinderBook) },
        {66, (0x1F280021, "Premium Collection Book", 4, 0x1F280118,EItemType.BinderBookPremium) },
        {83, (0x1F280022, "Playmat (Drilceros)", 8, 0x1f280119,EItemType.Playmat2b) },
        {81, (0x1F280023, "Playmat (Clamigo)", 8, 0x1f28011A,EItemType.Playmat1) },
        {87, (0x1F280024, "Playmat (Wispo)", 8, 0x1f28011B,EItemType.Playmat6) },
        {95, (0x1F280025, "Playmat (Lunight)", 8, 0x1f28011C,EItemType.Playmat14) },
        {90, (0x1F280026, "Playmat (Kyrone)", 8, 0x1f28011D,EItemType.Playmat9) },
        {82, (0x1F280027, "Playmat (Duel)", 8, 0x1f28011E,EItemType.Playmat2) },
        {86, (0x1F280028, "Playmat (Dracunix)", 8, 0x1f28011F,EItemType.Playmat5) },
        {85, (0x1F280029, "Playmat (The Four Dragons)", 8, 0x1f280120,EItemType.Playmat4) },
        {84, (0x1F28002A, "Playmat (Drakon)", 8, 0x1f280121,EItemType.Playmat3) },
        {88, (0x1F28002B, "Playmat (GigatronX Evo)", 8, 0x1f280122,EItemType.Playmat7) },
        {91, (0x1F28002C, "Playmat (Fire)", 8, 0x1f280123,EItemType.Playmat10) },
        {92, (0x1F28002D, "Playmat (Earth)", 8, 0x1f280124,EItemType.Playmat11) },
        {94, (0x1F28002E, "Playmat (Water)", 8, 0x1f280125,EItemType.Playmat13) },
        {93, (0x1F28002F, "Playmat (Wind)", 8, 0x1f280126,EItemType.Playmat12) },
        {89, (0x1F280030, "Playmat (Tetramon)", 8, 0x1f280127,EItemType.Playmat8) },
        {115, (0x1F2800BE, "Playmat (Dracunix2)", 8, 0x1f280128,EItemType.PlayMat15) },
        {116, (0x1F2800BF, "Playmat (GigatronX)", 8, 0x1f280129,EItemType.PlayMat16) },
        {117, (0x1F2800C2, "Playmat (Katengu Black)", 8, 0x1f28012A,EItemType.PlayMat17) },
        {118, (0x1F2800C3, "Playmat (Katengu White)", 8, 0x1f28012B,EItemType.PlayMat18) },
        {101, (0x1F2800C4, "Manga 1", 8, 0x1f28012C,EItemType.Manga1) },
        {102, (0x1F2800C5, "Manga 2", 8, 0x1f28012D,EItemType.Manga2) },
        {103, (0x1F2800C6, "Manga 3", 8, 0x1f28012E,EItemType.Manga3) },
        {104, (0x1F2800C7, "Manga 4", 8, 0x1f28012F,EItemType.Manga4) },
        {105, (0x1F2800C8, "Manga 5", 8, 0x1f280130,EItemType.Manga5) },
        {106, (0x1F2800C9, "Manga 6", 8, 0x1f280131,EItemType.Manga6) },
        {107, (0x1F2800CA, "Manga 7", 8, 0x1f280132,EItemType.Manga7) },
        {108, (0x1F2800CB, "Manga 8", 8, 0x1f280133,EItemType.Manga8) },
        {109, (0x1F2800CC, "Manga 9", 8, 0x1f280134,EItemType.Manga9) },
        {110, (0x1F2800CD, "Manga 10", 8, 0x1f280135,EItemType.Manga10) },
        {111, (0x1F2800CE, "Manga 11", 8, 0x1f280136,EItemType.Manga11) },
        {112, (0x1F2800CF, "Manga 12", 8, 0x1f280137,EItemType.Manga12) },//end
        {47, (0x1F280031, "Pigni Plushie (12)", 12, 0x1f280138,EItemType.Toy_PiggyA) },
        {48, (0x1F280032, "Nanomite Plushie (16)", 16, 0x1f280139,EItemType.Toy_GolemA) },
        {49, (0x1F280033, "Minstar Plushie (24)", 24, 0x1f28013A,EItemType.Toy_StarfishA) },
        {50, (0x1F280034, "Nocti Plushie (6)", 6, 0x1f28013B,EItemType.Toy_BatA) },
        {52, (0x1F280035, "Burpig Figurine (12)", 12, 0x1f28013C,EItemType.Toy_PiggyB) },
        {55, (0x1F280036, "Decimite Figurine (8)", 8, 0x1f28013D,EItemType.Toy_GolemB) },
        {58, (0x1F280037, "Trickstar Figurine (12)", 12, 0x1f28013E,EItemType.Toy_StarfishB) },
        {61, (0x1F280038, "Lunight Figurine (8)", 8, 0x1f28013F,EItemType.Toy_BatB) },
        {53, (0x1F280039, "Inferhog Figurine (2)", 2, 0x1f280140,EItemType.Toy_PiggyC) },
        {56, (0x1F28003A, "Meganite Figurine (2)", 2, 0x1F280141,EItemType.Toy_GolemC) },
        {59, (0x1F28003B, "Princestar Figurine (2)", 2, 0x1F280142,EItemType.Toy_StarfishC) },
        {62, (0x1F28003C, "Vampicant Figurine (2)", 2, 0x1F280143,EItemType.Toy_BatC) },
        {54, (0x1F28003D, "Blazoar Plushie (2)", 2, 0x1F280144,EItemType.Toy_PiggyD) },
        {57, (0x1F28003E, "Giganite Statue (2)", 2, 0x1F280145,EItemType.Toy_GolemD) },
        {60, (0x1F28003F, "Kingstar Plushie (2)", 2, 0x1F280146,EItemType.Toy_StarfishD) },
        {63, (0x1F280040, "Dracunix Figurine (1)", 1, 0x1F280147,EItemType.Toy_BatD) },
        {65, (0x1F280041, "Bonfiox Plushie (8)", 8, 0x1F280148,EItemType.Toy_FoxB) },
        {64, (0x1F280042, "Drilceros Action Figure (4)", 4, 0x1F280149,EItemType.Toy_Beetle) },
        {51, (0x1F280043, "ToonZ Plushie (6)", 6, 0x1F28014A,EItemType.Toy_ToonZ) },//end
        {99, (0x1F280052, "System Gate #1", 8, 0x1F280157,EItemType.Boardgame_Speedrobo_SystemGate1) },
        {100, (0x1F280053, "System Gate #2", 8, 0x1F280158,EItemType.Boardgame_Speedrobo_SystemGate2) },
        {97, (0x1F280054, "Mafia Works", 8, 0x1F280159,EItemType.Boardgame_Speedrobo_Mafia  ) },
        {96, (0x1F280055, "Necromonsters", 12, 0x1F28015A,EItemType.Boardgame_Speedrobo_Necro) },
        {98, (0x1F280056, "Claim!", 8, 0x1F28015B,EItemType.Boardgame_Speedrobo_Claim) },//end
        {124, (0x1F280057, "Penny Sleeves", 32, 0x1f28014b,EItemType.UP_PennySleeves ) },
        {130, (0x1F280058, "Tower Deckbox", 8, 0x1f28014c,EItemType.UP_TowerDeckbox) },
        {119, (0x1F280059, "Magnetic Holder", 8, 0x1f28014d,EItemType.UP_MagneticHolder) },
        {123, (0x1F28005A, "Toploader", 8, 0x1f28014E,EItemType.UP_Toploader  ) },
        {120, (0x1F28005B, "Card Preserver", 16, 0x1f28014F,EItemType.UP_CardPreserver) },
        {125, (0x1F28005C, "Playmat Gray", 8, 0x1f280150,EItemType.UP_Playmat1) },
        {126, (0x1F28005D, "Playmat Green", 8, 0x1F280151,EItemType.UP_Playmat2) },
        {127, (0x1F28005E, "Playmat Purple", 8, 0x1F280152,EItemType.UP_Playmat3) },
        {128, (0x1F28005F, "Playmat Yellow", 8, 0x1F280153,EItemType.UP_Playmat4) },
        {121, (0x1F280060, "Pocket Pages", 32, 0x1F280154,EItemType.UP_PlatinumSeriesPocketPages ) },
        {122, (0x1F280061, "Card Holder", 32, 0x1F280155,EItemType.UP_SemiRigidCardHolder ) },
        {129, (0x1F2800B5, "Collectors Album", 8, 0x1F280156,EItemType.UP_Album) },
    };

    public static KeyValuePair<int, (int itemid, string name, int count, int locid, EItemType type)> getKeyValue(int itemid, int count = 1)
    {
        //Plugin.Log($"id: {itemid}");
        var result = mapping.FirstOrDefault(pair => pair.Value.itemid == itemid && count == pair.Value.count);

        var defaultPair = new KeyValuePair<int, (int, string, int, int, EItemType)>(-1, (-1, "Unknown", 0, -1, EItemType.None));

        if (result.Equals(default(KeyValuePair<int, (int, string, int, int, EItemType)>)))
        {
            result = defaultPair;
        }
         return result;
    }

    public static (int itemid, string name, int count, int locid, EItemType type) getValueOrEmpty(int key)
    {
       
        return mapping.GetValueOrDefault<int, (int itemid, string name, int count, int locid, EItemType type)>(key, (-1, "", 0, -1, EItemType.None));
    }

    public static List<KeyValuePair<int, (int itemid, string name, int count, int locid, EItemType)>> GetKeyValueFromType(EItemType type)
    {
        var result = mapping.Where(pair => pair.Value.type == type).ToList();
        var duplicatedList = new List<KeyValuePair<int, (int itemid, string name, int count, int locid, EItemType type)>>();

        for (int i = 0; i < Plugin.m_SessionHandler.GetSlotData().SellCheckAmount; i++)
        {
            foreach (var kvp in result)
            {
                var item = kvp.Value;

                // Create the modified tuple with the updated count
                var modifiedValue = (item.itemid, item.name, item.count * (i+1), (kvp.Value.locid & ~0xF000) | ((i & 0xF) << 12), item.type);

                // Add the modified KeyValuePair to the duplicated list
                duplicatedList.Add(new KeyValuePair<int, (int itemid, string name, int count, int locid, EItemType type)>(kvp.Key, modifiedValue));
            }
        }
        return duplicatedList;
    }

    public static int locs1Starting = 0x1F280217;
    public static int locs2Starting = 0x1F28021F;
    public static int locs3Starting = 0x1F280228;


    public static int[] pg1_ids = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 67, 68, 69, 70, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 71, 72, 73, 74];
    public static int[] pg2_ids = [40, 41, 75, 76, 43, 44, 45, 46, 77, 78, 79, 80, 16, 17, 18, 19, 20, 21, 22, 23, 42, 66, 83, 81, 87, 95, 90, 82, 86, 85, 84, 88, 91, 92, 94, 93, 89, 115, 116, 117, 118, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112];
    public static int[] pg3_ids = [47, 48, 49, 50, 52, 55, 58, 61, 53, 56, 59, 62, 54, 57, 60, 63, 65, 64, 51];
    public static int[] tt_ids = [124, 130, 119, 123, 120, 125, 126, 127, 128, 121, 122, 129, 99, 100, 97, 96, 98];
}

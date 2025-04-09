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
        {0, (0x1F280001, "Basic Card Pack (32)", 32, 0x1F2800F0, EItemType.BasicCardPack) },
        {1, (0x1F2800D8, "Basic Card Pack (64)", 64, 0x1F2800F1, EItemType.BasicCardPack) },
        {2, (0x1F2800D9, "Progressive Basic Card Pack", 4,0x1F2800F2, EItemType.BasicCardBox) },
        {3, (0x1F2800DA, "Progressive Basic Card Pack", 8,0x1F2800F3, EItemType.BasicCardBox) },
        {4, (0x1F280002, "Progressive Rare Card Pack",32,0x1F2800F4, EItemType.RareCardPack) },
        {5, (0x1F2800DB, "Progressive Rare Card Pack",64,0x1F2800F5, EItemType.RareCardPack) },
        {6, (0x1F2800DC, "Progressive Rare Card Pack",4,0x1F2800F6, EItemType.RareCardBox) },
        {7, (0x1F2800DD, "Progressive Rare Card Pack",8,0x1F2800F7, EItemType.RareCardBox) },
        {8, (0x1F280003, "Progressive Epic Card Pack",32,0x1F2800F8, EItemType.EpicCardPack) },
        {9, (0x1F2800DE, "Progressive Epic Card Pack",64,0x1F2800F9, EItemType.EpicCardPack) },
        {10, (0x1F2800DF, "Progressive Epic Card Pack",4,0x1F2800FA, EItemType.EpicCardBox) },
        {11, (0x1F2800E0, "Progressive Epic Card Pack",8,0x1F2800FB, EItemType.EpicCardBox) },
        {12, (0x1F280004, "Progressive Legendary Card Pack",32,0x1F2800FC, EItemType.LegendaryCardPack) },
        {13, (0x1F2800E1, "Progressive Legendary Card Pack",64,0x1F2800FD, EItemType.LegendaryCardPack) },
        {14, (0x1F2800E2, "Progressive Legendary Card Pack",4,0x1F2800FE, EItemType.LegendaryCardBox) },
        {15, (0x1F2800E3, "Progressive Legendary Card Pack",8,0x1F2800FF, EItemType.LegendaryCardBox) },
        {67, (0x1F280005, "Fire Battle Deck",18,0x1F280100, EItemType.PreconDeck_Fire) },
        {68, (0x1F280006, "Earth Battle Deck",18,0x1F280101, EItemType.PreconDeck_Earth) },
        {69, (0x1F280007, "Water Battle Deck",18,0x1F280102,EItemType.PreconDeck_Water ) },
        {70, (0x1F280008, "Wind Battle Deck",18,0x1F280103,EItemType.PreconDeck_Wind ) },
        {24, (0x1F280009, "Progressive Basic Destiny Pack", 32, 0x1F280104,EItemType.DestinyBasicCardPack ) },
        {25, (0x1F2800E4, "Progressive Basic Destiny Pack", 64,0x1F280105,EItemType.DestinyBasicCardPack ) },
        {26, (0x1F2800E5, "Progressive Basic Destiny Pack", 4,0x1F280106,EItemType.DestinyBasicCardBox) },
        {27, (0x1F2800E6, "Progressive Basic Destiny Pack", 8, 0x1F280107,EItemType.DestinyBasicCardBox) },
        {28, (0x1F28000A, "Progressive Rare Destiny Pack",32,0x1F280108,EItemType.DestinyRareCardPack ) },
        {29, (0x1F2800E7, "Progressive Rare Destiny Pack",64,0x1F280109,EItemType.DestinyRareCardPack ) },
        {30, (0x1F2800E8, "Progressive Rare Destiny Pack",4,0x1F28010A,EItemType.DestinyRareCardBox  ) },
        {31, (0x1F2800E9, "Progressive Rare Destiny Pack",8,0x1F28010B,EItemType.DestinyRareCardBox  ) },
        {32, (0x1F28000B, "Progressive Epic Destiny Pack",32,0x1F28010C,EItemType.DestinyEpicCardPack) },
        {33, (0x1F2800EA, "Progressive Epic Destiny Pack",64,0x1F28010D,EItemType.DestinyEpicCardPack) },
        {34, (0x1F2800EB, "Progressive Epic Destiny Pack",4,0x1F28010E,EItemType.DestinyEpicCardBox) },
        {35, (0x1F2800EC, "Progressive Epic Destiny Pack",8,0x1F28010F,EItemType.DestinyEpicCardBox) },
        {36, (0x1F28000C, "Progressive Legendary Destiny Pack",32,0x1F280110,EItemType.DestinyLegendaryCardPack) },
        {37, (0x1F2800ED, "Progressive Legendary Destiny Pack",64,0x1F280111,EItemType.DestinyLegendaryCardPack) },
        {38, (0x1F2800EE, "Progressive Legendary Destiny Pack",4,0x1F280112,EItemType.DestinyLegendaryCardBox ) },
        {39, (0x1F2800EF, "Progressive Legendary Destiny Pack",8,0x1F280113,EItemType.DestinyLegendaryCardBox ) },
        {71, (0x1F28000D, "Fire Destiny Deck",18,0x1F280114,EItemType.PreconDeck_FireDestiny ) },
        {72, (0x1F28000E, "Earth Destiny Deck",18,0x1F280115,EItemType.PreconDeck_EarthDestiny  ) },
        {73, (0x1F28000F, "Water Destiny Deck",18,0x1F280116,EItemType.PreconDeck_WaterDestiny  ) },
        {74, (0x1F280010, "Wind Destiny Deck",18,0x1F280117,EItemType.PreconDeck_WindDestiny ) },//end
        {40, (0x1F280011, "Progressive Cleanser",8,0x1F280118,EItemType.Deodorant ) },
        {41, (0x1F2800F0, "Progressive Cleanser",16,0x1F280119,EItemType.Deodorant ) },
        {75, (0x1F280012, "Card Sleeves (Clear)",32,0x1F28011A,EItemType.CardSleeve_Clear  ) },
        {76, (0x1F280013, "Card Sleeves (Tetramon)",32,0x1F28011B,EItemType.CardSleeve_Tetramon) },
        {43, (0x1F280014, "D20 Dice Red (16)",16,0x1F28011C,EItemType.D20DiceBox ) },
        {44, (0x1F280015, "D20 Dice Blue (16)",16,0x1F28011D,EItemType.D20DiceBox2) },
        {45, (0x1F280016, "D20 Dice Black (16)",16,0x1F28011E,EItemType.D20DiceBox3) },
        {46, (0x1F280017, "D20 Dice White (16)",16,0x1F28011F,EItemType.D20DiceBox4) },
        {77, (0x1F280018, "Card Sleeves (Fire)",32,0x1F280120,EItemType.CardSleeve_Fire) },
        {78, (0x1F280019, "Card Sleeves (Earth)",32,0x1F280121,EItemType.CardSleeve_Earth) },
        {79, (0x1F28001A, "Card Sleeves (Water)",32,0x1F280122,EItemType.CardSleeve_Water) },
        {80, (0x1F28001B, "Card Sleeves (Wind)",32,0x1F280123,EItemType.CardSleeve_Wind) },
        {16, (0x1F28001C, "Progressive Deck Box Red",8,0x1F280124,EItemType.DeckBox1) },
        {17, (0x1F2800F1, "Progressive Deck Box Red",16,0x1F280125,EItemType.DeckBox1) },
        {18, (0x1F28001D, "Progressive Deck Box Green",8,0x1F280126,EItemType.DeckBox2) },
        {19, (0x1F2800F2, "Progressive Deck Box Green",16,0x1F280127,EItemType.DeckBox2) },
        {20, (0x1F28001E, "Progressive Deck Box Blue",8,0x1F280128,EItemType.DeckBox3) },
        {21, (0x1F2800F3, "Progressive Deck Box Blue",16,0x1F280129,EItemType.DeckBox3) },
        {22, (0x1F28001F, "Progressive Deck Box Yellow",8,0x1F28012A,EItemType.DeckBox4) },
        {23, (0x1F2800F4, "Progressive Deck Box Yellow",16,0x1F28012B, EItemType.DeckBox4) },//end
        {42, (0x1F280020, "Collection Book ",4,0x1F28012C,EItemType.BinderBook) },
        {66, (0x1F280021, "Premium Collection Book", 4, 0x1F28012D,EItemType.BinderBookPremium) },
        {83, (0x1F280022, "Playmat (Drilceros)", 8, 0x1F28012e,EItemType.Playmat2b) },
        {81, (0x1F280023, "Playmat (Clamigo)", 8, 0x1F28012F,EItemType.Playmat1) },
        {87, (0x1F280024, "Playmat (Wispo)", 8, 0x1F280130,EItemType.Playmat6) },
        {95, (0x1F280025, "Playmat (Lunight)", 8, 0x1F280131,EItemType.Playmat14) },
        {90, (0x1F280026, "Playmat (Kyrone)", 8, 0x1F280132,EItemType.Playmat9) },
        {82, (0x1F280027, "Playmat (Duel)", 8, 0x1F280133,EItemType.Playmat2) },
        {86, (0x1F280028, "Playmat (Dracunix)", 8, 0x1F280134,EItemType.Playmat5) },
        {85, (0x1F280029, "Playmat (The Four Dragons)", 8, 0x1F280135,EItemType.Playmat4) },
        {84, (0x1F28002A, "Playmat (Drakon)", 8, 0x1F280136,EItemType.Playmat3) },
        {88, (0x1F28002B, "Playmat (GigatronX Evo)", 8, 0x1F280137,EItemType.Playmat7) },
        {91, (0x1F28002C, "Playmat (Fire)", 8, 0x1F280138,EItemType.Playmat10) },
        {92, (0x1F28002D, "Playmat (Earth)", 8, 0x1F280139,EItemType.Playmat11) },
        {94, (0x1F28002E, "Playmat (Water)", 8, 0x1F28013A,EItemType.Playmat13) },
        {93, (0x1F28002F, "Playmat (Wind)", 8, 0x1F28013B,EItemType.Playmat12) },
        {89, (0x1F280030, "Playmat (Tetramon)", 8, 0x1F28013C,EItemType.Playmat8) },
        {115, (0x1F2800BE, "Playmat (Dracunix2)", 8, 0x1F280161,EItemType.PlayMat15) },
        {116, (0x1F2800BF, "Playmat (GigatronX)", 8, 0x1F280162,EItemType.PlayMat16) },
        {117, (0x1F2800C2, "Playmat (Katengu Black)", 8, 0x1F280163,EItemType.PlayMat17) },
        {118, (0x1F2800C3, "Playmat (Katengu White)", 8, 0x1F280164,EItemType.PlayMat18) },
        {101, (0x1F2800C4, "Manga 1", 16, 0x1F280165,EItemType.Manga1) },
        {102, (0x1F2800C5, "Manga 2", 16, 0x1F280166,EItemType.Manga2) },
        {103, (0x1F2800C6, "Manga 3", 16, 0x1F280167,EItemType.Manga3) },
        {104, (0x1F2800C7, "Manga 4", 16, 0x1F280168,EItemType.Manga4) },
        {105, (0x1F2800C8, "Manga 5", 16, 0x1F280169,EItemType.Manga5) },
        {106, (0x1F2800C9, "Manga 6", 16, 0x1F28016A,EItemType.Manga6) },
        {107, (0x1F2800CA, "Manga 7", 16, 0x1F28016B,EItemType.Manga7) },
        {108, (0x1F2800CB, "Manga 8", 16, 0x1F28016C,EItemType.Manga8) },
        {109, (0x1F2800CC, "Manga 9", 16, 0x1F28016D,EItemType.Manga9) },
        {110, (0x1F2800CD, "Manga 10", 16, 0x1F28016E,EItemType.Manga10) },
        {111, (0x1F2800CE, "Manga 11", 16, 0x1F28016F,EItemType.Manga11) },
        {112, (0x1F2800CF, "Manga 12", 16, 0x1F280170,EItemType.Manga12) },//end
        {47, (0x1F280031, "Pigni Plushie (12)", 12, 0x1F28013D,EItemType.Toy_PiggyA) },
        {48, (0x1F280032, "Nanomite Plushie (16)", 16, 0x1F28013E,EItemType.Toy_GolemA) },
        {49, (0x1F280033, "Minstar Plushie (24)", 24, 0x1F28013F,EItemType.Toy_StarfishA) },
        {50, (0x1F280034, "Nocti Plushie (6)", 6, 0x1F280140,EItemType.Toy_BatA) },
        {52, (0x1F280035, "Burpig Figurine (12)", 12, 0x1F280141,EItemType.Toy_PiggyB) },
        {55, (0x1F280036, "Decimite Figurine (8)", 8, 0x1F280142,EItemType.Toy_GolemB) },
        {58, (0x1F280037, "Trickstar Figurine (12)", 12, 0x1F280143,EItemType.Toy_StarfishB) },
        {61, (0x1F280038, "Lunight Figurine (8)", 8, 0x1F280144,EItemType.Toy_BatB) },
        {53, (0x1F280039, "Inferhog Figurine (2)", 2, 0x1F280145,EItemType.Toy_PiggyC) },
        {56, (0x1F28003A, "Meganite Figurine (2)", 2, 0x1F280146,EItemType.Toy_GolemC) },
        {59, (0x1F28003B, "Princestar Figurine (2)", 2, 0x1F280147,EItemType.Toy_StarfishC) },
        {62, (0x1F28003C, "Vampicant Figurine (2)", 2, 0x1F280148,EItemType.Toy_BatC) },
        {54, (0x1F28003D, "Blazoar Plushie (2)", 2, 0x1F280149,EItemType.Toy_PiggyD) },
        {57, (0x1F28003E, "Giganite Statue (2)", 2, 0x1F28014A,EItemType.Toy_GolemD) },
        {60, (0x1F28003F, "Kingstar Plushie (2)", 2, 0x1F28014B,EItemType.Toy_StarfishD) },
        {63, (0x1F280040, "Dracunix Figurine (1)", 1, 0x1F28014C,EItemType.Toy_BatD) },
        {65, (0x1F280041, "Bonfiox Plushie (8)", 8, 0x1F28014D,EItemType.Toy_FoxB) },
        {64, (0x1F280042, "Drilceros Action Figure (4)", 4, 0x1F28014E,EItemType.Toy_Beetle) },
        {51, (0x1F280043, "ToonZ Plushie (6)", 6, 0x1F28014F,EItemType.Toy_ToonZ) },//end
        {99, (0x1F280052, "System Gate #1", 8, 0x1F280150,EItemType.Boardgame_Speedrobo_SystemGate1) },
        {100, (0x1F280053, "System Gate #2", 8, 0x1F280151,EItemType.Boardgame_Speedrobo_SystemGate2) },
        {97, (0x1F280054, "Mafia Works", 8, 0x1F280152,EItemType.Boardgame_Speedrobo_Mafia  ) },
        {96, (0x1F280055, "Necromonsters", 12, 0x1F280152,EItemType.Boardgame_Speedrobo_Necro) },
        {98, (0x1F280056, "Claim!", 8, 0x1F280154,EItemType.Boardgame_Speedrobo_Claim) },//end
        {124, (0x1F280057, "Penny Sleeves", 32, 0x1F280155,EItemType.UP_PennySleeves ) },
        {130, (0x1F280058, "Tower Deckbox", 8, 0x1F280156,EItemType.UP_TowerDeckbox) },
        {119, (0x1F280059, "Magnetic Holder", 8, 0x1F280157,EItemType.UP_MagneticHolder) },
        {123, (0x1F28005A, "Toploader", 8, 0x1F280158,EItemType.UP_Toploader  ) },
        {120, (0x1F28005B, "Card Preserver", 16, 0x1F280159,EItemType.UP_CardPreserver) },
        {125, (0x1F28005C, "Playmat Gray", 16, 0x1F28015A,EItemType.UP_Playmat1) },
        {126, (0x1F28005D, "Playmat Green", 16, 0x1F28015B,EItemType.UP_Playmat2) },
        {127, (0x1F28005E, "Playmat Purple", 16, 0x1F28015C,EItemType.UP_Playmat3) },
        {128, (0x1F28005F, "Playmat Yellow", 16, 0x1F28015D,EItemType.UP_Playmat4) },
        {121, (0x1F280060, "Pocket Pages", 32, 0x1F28015E,EItemType.UP_PlatinumSeriesPocketPages ) },
        {122, (0x1F280061, "Card Holder", 32, 0x1F28015F,EItemType.UP_SemiRigidCardHolder ) },
        {129, (0x1F2800B5, "Collectors Album", 8, 0x1F280160,EItemType.UP_Album) },
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
        return mapping.Where(pair => pair.Value.type == type).ToList();
    }

    public static int locs1Starting = 0x1F280217;
    public static int locs2Starting = 0x1F28021F;
    public static int locs3Starting = 0x1F280228;


    public static int[] pg1_ids = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 67, 68, 69, 70, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 71, 72, 73, 74];
    public static int[] pg2_ids = [40, 41, 75, 76, 43, 44, 45, 46, 77, 78, 79, 80, 16, 17, 18, 19, 20, 21, 22, 23, 42, 66, 83, 81, 87, 95, 90, 82, 86, 85, 84, 88, 91, 92, 94, 93, 89, 115, 116, 117, 118, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112];
    public static int[] pg3_ids = [47, 48, 49, 50, 52, 55, 58, 61, 53, 56, 59, 62, 54, 57, 60, 63, 65, 64, 51];
    public static int[] tt_ids = [124, 130, 119, 123, 120, 125, 126, 127, 128, 121, 122, 129, 99, 100, 97, 96, 98];
}

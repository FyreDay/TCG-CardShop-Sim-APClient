using ApClient.data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using UnityEngine.UIElements;
using UnityEngine;

namespace ApClient.mapping;

public class LicenseMapping
{
    public static Dictionary<EItemType, (string name, int count, int oldindex)> mapping = new Dictionary<EItemType, (string name, int count, int oldindex)>
    {
        {EItemType.BasicCardPack,("Basic Card Pack (32)", 32, 0) },
        {EItemType.BasicCardBox,("Progressive Basic Card Pack", 8, 2) },
        {EItemType.RareCardPack,("Progressive Rare Card Pack",32, 4) },
        {EItemType.RareCardBox,("Progressive Rare Card Pack",8, 6) },
        {EItemType.EpicCardPack,("Progressive Epic Card Pack",32, 8) },
        {EItemType.EpicCardBox,("Progressive Epic Card Pack",8, 10) },
        {EItemType.LegendaryCardPack,("Progressive Legendary Card Pack",32, 12) },
        {EItemType.LegendaryCardBox,("Progressive Legendary Card Pack",8, 14) },
        {EItemType.PreconDeck_Fire,("Fire Battle Deck",9, 67) },
        {EItemType.PreconDeck_Earth,("Earth Battle Deck",9, 68) },
        {EItemType.PreconDeck_Water ,("Water Battle Deck",9, 69) },
        {EItemType.PreconDeck_Wind ,("Wind Battle Deck",9, 70) },
        {EItemType.DestinyBasicCardPack ,("Progressive Basic Destiny Pack", 32, 24) },
        {EItemType.DestinyBasicCardBox,("Progressive Basic Destiny Pack", 8, 26) },
        {EItemType.DestinyRareCardPack ,("Progressive Rare Destiny Pack",32, 28) },
        {EItemType.DestinyRareCardBox  ,("Progressive Rare Destiny Pack",8, 30) },
        {EItemType.DestinyEpicCardPack,("Progressive Epic Destiny Pack",32, 32) },
        {EItemType.DestinyEpicCardBox,("Progressive Epic Destiny Pack",8, 34) },
        {EItemType.DestinyLegendaryCardPack,("Progressive Legendary Destiny Pack",32, 36) },
        {EItemType.DestinyLegendaryCardBox ,("Progressive Legendary Destiny Pack",8, 38) },
        {EItemType.PreconDeck_FireDestiny ,("Fire Destiny Deck",9, 71) },
        {EItemType.PreconDeck_EarthDestiny  ,("Earth Destiny Deck",9, 72) },
        {EItemType.PreconDeck_WaterDestiny  ,("Water Destiny Deck",9, 73) },
        {EItemType.PreconDeck_WindDestiny ,("Wind Destiny Deck",9, 74) },//end
        {EItemType.Deodorant ,("Cleanser (8)",8, 40) },
        {EItemType.CardSleeve_Clear  ,("Card Sleeves (Clear)",32, 75) },
        {EItemType.CardSleeve_Tetramon,("Card Sleeves (Tetramon)",32, 76) },
        {EItemType.D20DiceBox ,("D20 Dice Red (16)",8, 43) },
        {EItemType.D20DiceBox2,("D20 Dice Blue (16)",8, 44) },
        {EItemType.D20DiceBox3,("D20 Dice Black (16)",8, 45) },
        {EItemType.D20DiceBox4,("D20 Dice White (16)",8, 46) },
        {EItemType.CardSleeve_Fire,("Card Sleeves (Fire)",32, 77) },
        {EItemType.CardSleeve_Earth,("Card Sleeves (Earth)",32, 78) },
        {EItemType.CardSleeve_Water,("Card Sleeves (Water)",32, 79) },
        {EItemType.CardSleeve_Wind,("Card Sleeves (Wind)",32, 80) },
        {EItemType.DeckBox1,("Progressive Deck Box Red",8, 16) },
        {EItemType.DeckBox2,("Progressive Deck Box Green",8, 18) },
        {EItemType.DeckBox3,("Progressive Deck Box Blue",8, 20) },
        {EItemType.DeckBox4,("Progressive Deck Box Yellow",8, 22) },
        {EItemType.BinderBook,("Collection Book ",4, 42) },
        {EItemType.BinderBookPremium,("Premium Collection Book", 4, 66) },
        {EItemType.Playmat2b,("Playmat (Drilceros)", 8, 83) },
        {EItemType.Playmat1,("Playmat (Clamigo)", 8, 81) },
        {EItemType.Playmat6,("Playmat (Wispo)", 8, 87) },
        {EItemType.Playmat14,("Playmat (Lunight)", 8, 95) },
        {EItemType.Playmat9,("Playmat (Kyrone)", 8, 90) },
        {EItemType.Playmat2,("Playmat (Duel)", 8, 82) },
        {EItemType.Playmat5,("Playmat (Dracunix)", 8, 86) },
        {EItemType.Playmat4,("Playmat (The Four Dragons)", 8, 85) },
        {EItemType.Playmat3,("Playmat (Drakon)", 8, 84) },
        {EItemType.Playmat7,("Playmat (GigatronX Evo)", 8, 88) },
        {EItemType.Playmat10,("Playmat (Fire)", 8, 91) },
        {EItemType.Playmat11,("Playmat (Earth)", 8, 92) },
        {EItemType.Playmat13,("Playmat (Water)", 8, 94) },
        {EItemType.Playmat12,("Playmat (Wind)", 8, 93) },
        {EItemType.Playmat8,("Playmat (Tetramon)", 8, 89) },
        {EItemType.PlayMat15,("Playmat (Dracunix2)", 8, 115) },
        {EItemType.PlayMat16,("Playmat (GigatronX)", 8, 116) },
        {EItemType.PlayMat17,("Playmat (Katengu Black)", 8, 117) },
        {EItemType.PlayMat18,("Playmat (Katengu White)", 8, 118) },
        {EItemType.Manga1,("Manga 1", 8, 101) },
        {EItemType.Manga2,("Manga 2", 8, 102) },
        {EItemType.Manga3,("Manga 3", 8, 103) },
        {EItemType.Manga4,("Manga 4", 8, 104) },
        {EItemType.Manga5,("Manga 5", 8, 105) },
        {EItemType.Manga6,("Manga 6", 8, 106) },
        {EItemType.Manga7,("Manga 7", 8, 107) },
        {EItemType.Manga8,("Manga 8", 8, 108) },
        {EItemType.Manga9,("Manga 9", 8, 109) },
        {EItemType.Manga10,("Manga 10", 8, 110) },
        {EItemType.Manga11,("Manga 11", 8, 111) },
        {EItemType.Manga12,("Manga 12", 8, 112) },//end
        {EItemType.Toy_PiggyA,("Pigni Plushie (12)", 12, 47) },
        {EItemType.Toy_GolemA,("Nanomite Plushie (16)", 16, 48) },
        {EItemType.Toy_StarfishA,("Minstar Plushie (24)", 24, 49) },
        {EItemType.Toy_BatA,("Nocti Plushie (6)", 6, 50) },
        {EItemType.Toy_PiggyB,("Burpig Figurine (12)", 12, 52) },
        {EItemType.Toy_GolemB,("Decimite Figurine (8)", 8, 55) },
        {EItemType.Toy_StarfishB,("Trickstar Figurine (12)", 12, 58) },
        {EItemType.Toy_BatB,("Lunight Figurine (8)", 8, 61) },
        {EItemType.Toy_PiggyC,("Inferhog Figurine (2)", 2, 53) },
        {EItemType.Toy_GolemC,("Meganite Figurine (2)", 2, 56) },
        {EItemType.Toy_StarfishC,("Princestar Figurine (2)", 2, 59) },
        {EItemType.Toy_BatC,("Vampicant Figurine (2)", 2, 62) },
        {EItemType.Toy_PiggyD,("Blazoar Plushie (2)", 2, 54) },
        {EItemType.Toy_GolemD,("Giganite Statue (2)", 2, 57) },
        {EItemType.Toy_StarfishD,("Kingstar Plushie (2)", 2, 60) },
        {EItemType.Toy_BatD,("Dracunix Figurine (1)", 1, 63) },
        {EItemType.Toy_FoxB,("Bonfiox Plushie (8)", 8, 65) },
        {EItemType.Toy_Beetle,("Drilceros Action Figure (4)", 4, 64) },
        {EItemType.Toy_ToonZ,("ToonZ Plushie (6)", 6, 51) },//end
        {EItemType.Boardgame_Speedrobo_SystemGate1,("System Gate #1", 8, 99) },
        {EItemType.Boardgame_Speedrobo_SystemGate2,("System Gate #2", 8, 100) },
        {EItemType.Boardgame_Speedrobo_Mafia  ,("Mafia Works", 8, 97) },
        {EItemType.Boardgame_Speedrobo_Necro,("Necromonsters", 12, 96) },
        {EItemType.Boardgame_Speedrobo_Claim,("Claim!", 8, 98) },//end
        {EItemType.UP_PennySleeves ,("Penny Sleeves", 32, 124) },
        {EItemType.UP_TowerDeckbox,("Tower Deckbox", 8, 130) },
        {EItemType.UP_MagneticHolder,("Magnetic Holder", 8, 119) },
        {EItemType.UP_Toploader  ,("Toploader", 8, 123) },
        {EItemType.UP_CardPreserver,("Card Preserver", 16, 120) },
        {EItemType.UP_Playmat1,("Playmat Gray", 8, 125) },
        {EItemType.UP_Playmat2,("Playmat Green", 8, 126) },
        {EItemType.UP_Playmat3,("Playmat Purple", 8, 127) },
        {EItemType.UP_Playmat4,("Playmat Yellow", 8, 128) },
        {EItemType.UP_PlatinumSeriesPocketPages ,("Pocket Pages", 32, 121) },
        {EItemType.UP_SemiRigidCardHolder ,("Card Holder", 8, 122) },
        {EItemType.UP_Album,("Collectors Album", 8, 129) },
    };
    public static List<(int id,int count)> GetLocations(EItemType type)
    {
        int id = type == EItemType.BasicCardPack ? 190 : (int)type;
        int baseAmount = Plugin.m_SessionHandler.GetSlotData().SellCheckAmount;
        int startingAmount = Plugin.m_SessionHandler.GetSlotData().startingItems.Contains((int)type) ? Plugin.m_SessionHandler.GetSlotData().ExtraStartingItemChecks : 0;
        var list = new List<(int id, int count)>();
        for (int i = 1; i <= baseAmount + startingAmount; i++)
        {
            list.Add((SELL_CHECK_START_ID + (id * 16) + (i - 1), mapping[type].count * i));
        }
        return list;
    }

    static int SELL_CHECK_START_ID = 3000;
    public static int BASIC_CARD_PACK_ID = 190;
}

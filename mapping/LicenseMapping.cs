using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApClient.mapping
{
    public class LicenseMapping
    {
        public static Dictionary<int, (int itemid, string name, int count, int locid)> mapping = new Dictionary<int, (int itemid, string name, int count, int locid)>
        {
            {1, (0x1F280001, "Progressive Basic Card Pack", 1, 0x1F2800F1) },
            {2, (0x1F280001, "Progressive Basic Card Pack", 2,0x1F2800F2) },
            {3, (0x1F280001, "Progressive Basic Card Pack", 3,0x1F2800F3) },
            {4, (0x1F280002, "Progressive Rare Card Pack",1,0x1F2800F4) },
            {5, (0x1F280002, "Progressive Rare Card Pack",2,0x1F2800F5) },
            {6, (0x1F280002, "Progressive Rare Card Pack",3,0x1F2800F6) },
            {7, (0x1F280002, "Progressive Rare Card Pack",4,0x1F2800F7) },
            {8, (0x1F280003, "Progressive Epic Card Pack",1,0x1F2800F8) },
            {9, (0x1F280003, "Progressive Epic Card Pack",2,0x1F2800F9) },
            {10, (0x1F280003, "Progressive Epic Card Pack",3,0x1F2800FA) },
            {11, (0x1F280003, "Progressive Epic Card Pack",4,0x1F2800FB) },
            {12, (0x1F280004, "Progressive Legendary Card Pack",1,0x1F2800FC) },
            {13, (0x1F280004, "Progressive Legendary Card Pack",2,0x1F2800FD) },
            {14, (0x1F280004, "Progressive Legendary Card Pack",3,0x1F2800FE) },
            {15, (0x1F280004, "Progressive Legendary Card Pack",4,0x1F2800FF) },
            {67, (0x1F280005, "Fire Battle Deck",1,0x1F280100) },
            {68, (0x1F280006, "Earth Battle Deck",2,0x1F280101) },
            {69, (0x1F280007, "Water Battle Deck",3,0x1F280102) },
            {70, (0x1F280008, "Wind Battle Deck",4,0x1F280103) },
            {24, (0x1F280009, "Progressive Basic Destiny Pack", 1, 0x1F280104) },
            {25, (0x1F280009, "Progressive Basic Destiny Pack", 2,0x1F280105) },
            {26, (0x1F280009, "Progressive Basic Destiny Pack", 3,0x1F280106) },
            {27, (0x1F280009, "Progressive Basic Destiny Pack", 4, 0x1F280107) },
            {28, (0x1F28000A, "Progressive Rare Destiny Pack",1,0x1F280108) },
            {29, (0x1F28000A, "Progressive Rare Destiny Pack",2,0x1F280109) },
            {30, (0x1F28000A, "Progressive Rare Destiny Pack",3,0x1F28010A) },
            {31, (0x1F28000A, "Progressive Rare Destiny Pack",4,0x1F28010B) },
            {32, (0x1F28000B, "Progressive Epic Destiny Pack",1,0x1F28010C) },
            {33, (0x1F28000B, "Progressive Epic Destiny Pack",2,0x1F28010D) },
            {34, (0x1F28000B, "Progressive Epic Destiny Pack",3,0x1F28010E) },
            {35, (0x1F28000B, "Progressive Epic Destiny Pack",4,0x1F28010F) },
            {36, (0x1F28000C, "Progressive Legendary Destiny Pack",1,0x1F280110) },
            {37, (0x1F28000C, "Progressive Legendary Destiny Pack",2,0x1F280111) },
            {38, (0x1F28000C, "Progressive Legendary Destiny Pack",3,0x1F280112) },
            {39, (0x1F28000C, "Progressive Legendary Destiny Pack",4,0x1F280113) },
            {71, (0x1F28000D, "Fire Destiny Deck",1,0x1F280114) },
            {72, (0x1F28000E, "Earth Destiny Deck",2,0x1F280115) },
            {73, (0x1F28000F, "Water Destiny Deck",3,0x1F280116) },
            {74, (0x1F280010, "Wind Destiny Deck",4,0x1F280117) },

            {40, (0x1F280011, "Progressive Cleanser",1,0x1F280118) },
            {41, (0x1F280011, "Progressive Cleanser",2,0x1F280119) },
            {75, (0x1F280012, "Card Sleeves (Clear)",1,0x1F28011A) },
            {76, (0x1F280013, "Card Sleeves (Tetramon)",1,0x1F28011B) },
            {43, (0x1F280014, "",1,0x1F28011C) },
            {44, (0x1F280015, "",1,0x1F28011D) },
            {45, (0x1F280016, "",1,0x1F28011E) },
            {46, (0x1F280017, "",1,0x1F28011F) },
            {77, (0x1F280018, "",1,0x1F280120) },
            {78, (0x1F280019, "",1,0x1F280121) },
            {79, (0x1F28001A, "",1,0x1F280122) },
            {80, (0x1F28001B, "",1,0x1F280123) },
            {16, (0x1F28001C, "Progressive Deck Box Red",1,0x1F280124) },
            {17, (0x1F28001C, "",2,0x1F280125) },
            {18, (0x1F28001D, "Progressive Deck Box Green",1,0x1F280126) },
            {19, (0x1F28001D, "",2,0x1F280127) },
            {20, (0x1F28001E, "Progressive Deck Box Blue",1,0x1F280128) },
            {21, (0x1F28001E, "",2,0x1F280129) },
            {22, (0x1F28001F, "Progressive Deck Box Yellow",1,0x1F28012A) },
            {23, (0x1F28001F, "",2,0x1F28012B) },

            {42, (0x1F280020, "Collection Book ",1,0x1F28012C) },
            {66, (0x1F280021, "Premium Collection Book", 1, 0x1F28012D) },
            {83, (0x1F280022, "Playmat (Drilceros)", 1, 0x1F28012e) },
            {81, (0x1F280023, "Playmat (Clamigo)", 1, 0x1F28012F) },
            {87, (0x1F280024, "Playmat (Wispo)", 1, 0x1F280130) },
            {95, (0x1F280025, "Playmat (Lunight)", 1, 0x1F280131) },
            {90, (0x1F280026, "Playmat (Kyrone)", 1, 0x1F280132) },
            {82, (0x1F280027, "Playmat (Duel)", 1, 0x1F280133) },
            {86, (0x1F280028, "Playmat (Dracunix)", 1, 0x1F280134) },
            {85, (0x1F280029, "Playmat (The Four Dragons)", 1, 0x1F280135) },
            {84, (0x1F28002A, "Playmat (Drakon)", 1, 0x1F280136) },
            {88, (0x1F28002B, "Playmat (GigatronX Evo)", 1, 0x1F280137) },
            {91, (0x1F28002C, "Playmat (Fire)", 1, 0x1F280138) },
            {92, (0x1F28002D, "Playmat (Earth)", 1, 0x1F280139) },
            {94, (0x1F28002E, "Playmat (Water)", 1, 0x1F28013A) },
            {93, (0x1F28002F, "Playmat (Wind)", 1, 0x1F28013B) },
            {89, (0x1F280030, "Playmat (Tetramon)", 1, 0x1F28013C) },
            {115, (0x1F2800BE, "Playmat (Dracunix)", 1, 0x1F280177) },
            {116, (0x1F2800BF, "Playmat", 1, 0x1F280178) },
            {117, (0x1F2800C2, "Playmat", 1, 0x1F280179) },
            {118, (0x1F2800C3, "Playmat", 1, 0x1F28017A) },
            {101, (0x1F2800C4, "Manga 1", 1, 0x1F28017B) },
            {102, (0x1F2800C5, "Manga 2", 1, 0x1F28017C) },
            {103, (0x1F2800C6, "Manga 3", 1, 0x1F28017D) },
            {104, (0x1F2800C7, "Manga 4", 1, 0x1F28017E) },
            {105, (0x1F2800C8, "Manga 5", 1, 0x1F28017F) },
            {106, (0x1F2800C9, "Manga 6", 1, 0x1F280180) },
            {107, (0x1F2800CA, "Manga 7", 1, 0x1F280181) },
            {108, (0x1F2800CB, "Manga 8", 1, 0x1F280182) },
            {109, (0x1F2800CC, "Manga 9", 1, 0x1F280183) },
            {110, (0x1F2800CD, "Manga 10", 1, 0x1F280184) },
            {111, (0x1F2800CE, "Manga 11", 1, 0x1F280185) },
            {112, (0x1F2800CF, "Manga 12", 1, 0x1F280186) },

            {47, (0x1F280031, "Pigni Plushie (12)", 1, 0x1F28013D) },
            {48, (0x1F280032, "Nanomite Plushie (16)", 1, 0x1F28013E) },
            {49, (0x1F280033, "Minstar Plushie (24)", 1, 0x1F28013F) },
            {50, (0x1F280034, "Nocti Plushie (6)", 1, 0x1F280140) },
            {52, (0x1F280035, "Burpig Figurine (12)", 1, 0x1F280141) },
            {55, (0x1F280036, "Decimite Figurine (8)", 1, 0x1F280142) },
            {58, (0x1F280037, "Trickstar Figurine (12)", 1, 0x1F280143) },
            {61, (0x1F280038, "Lunight Figurine (8)", 1, 0x1F280144) },
            {53, (0x1F280039, "Inferhog Figurine (2)", 1, 0x1F280145) },
            {56, (0x1F28003A, "Meganite Figurine (2)", 1, 0x1F280146) },
            {59, (0x1F28003B, "Princestar Figurine (2)", 1, 0x1F280147) },
            {62, (0x1F28003C, "Vampicant Figurine (2)", 1, 0x1F280148) },
            {54, (0x1F28003D, "Blazoar Plushie (2)", 1, 0x1F280149) },
            {57, (0x1F28003E, "Giganite Statue (2)", 1, 0x1F28014A) },
            {60, (0x1F28003F, "Kingstar Plushie (2)", 1, 0x1F28014B) },
            {63, (0x1F280040, "Dracunix Figurine (1)", 1, 0x1F28014C) },
            {65, (0x1F280041, "Bonfiox Plushie (8)", 1, 0x1F28014D) },
            {64, (0x1F280042, "Drilceros Action Figure (4)", 1, 0x1F28014E) },
            {51, (0x1F280043, "ToonZ Plushie (6)", 1, 0x1F28014F) },

            {99, (0x1F280052, "System Gate #1", 1, 0x1F280166) },
            {100, (0x1F280053, "System Gate #2", 1, 0x1F280167) },
            {97, (0x1F280054, "Mafia Works", 1, 0x1F280168) },
            {96, (0x1F280055, "Necromonsters", 1, 0x1F280169) },
            {98, (0x1F280056, "Claim!", 1, 0x1F28016A) },

            {124, (0x1F280057, "Penny Sleeves", 1, 0x1F28016B) },
            {130, (0x1F280058, "Tower Deckbox", 1, 0x1F28016C) },
            {119, (0x1F280059, "Magnetic Holder", 1, 0x1F28016D) },
            {123, (0x1F28005A, "Toploader", 1, 0x1F28016E) },
            {120, (0x1F28005B, "Card Preserver", 1, 0x1F28016F) },
            {125, (0x1F28005C, "Playmat Gray", 1, 0x1F280170) },
            {126, (0x1F28005D, "Playmat Green", 1, 0x1F280171) },
            {127, (0x1F28005E, "Playmat Purple", 1, 0x1F280172) },
            {128, (0x1F28005F, "Playmat Yellow", 1, 0x1F280173) },
            {121, (0x1F280060, "Pocket Pages", 1, 0x1F280174) },
            {122, (0x1F280061, "Card Holder", 1, 0x1F280175) },
            {129, (0x1F2800B5, "Collectors Album", 1, 0x1F280176) },
        };

        public static KeyValuePair<int, (int itemid, string name, int count, int locid)> getKeyValue(int itemid)
        {
            Plugin.Log($"id: {itemid}");
            var result = mapping.FirstOrDefault(pair => pair.Value.itemid == itemid);

            var defaultPair = new KeyValuePair<int, (int, string, int, int)>(-1, (-1, "Unknown", 0, -1));

            if (result.Equals(default(KeyValuePair<int, (int, string, int, int)>)))
            {
                result = defaultPair;
            }
             return result;
        }

        public static (int itemid, string name, int count, int locid) getValueOrEmpty(int key)
        {
           
            return mapping.GetValueOrDefault<int, (int itemid, string name, int count, int locid)>(key, (-1, "", 0, -1));
        }
    }
}

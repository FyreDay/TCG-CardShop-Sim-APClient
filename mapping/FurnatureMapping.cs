using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApClient.mapping;

public class FurnatureMapping
{
    public static Dictionary<int, (int itemid, string name, int count)> mapping = new Dictionary<int, (int itemid, string name, int count)>
    {
        {0, (227, "Small Cabinet", 1) },
        {1, (227, "Small Metal Rack", 2) },
        {2, (232, "Play Table", 1) },

        {3, (227, "2x2 Cabinet", 3) },
        {4, (200, "Card Table",1) },
        {5, (227, "Single Sided Shelf", 4) },

        {6, (204, "Small Warehouse Shelf",1) },
        {7, (203, "Auto Scent M100",1) },
        {8, (202, "Small Personal Shelf", 1) },

        {9, (233, "Workbench",1) },
        {10, (234, "Trash Bin",1) },
        {11, (201, "Small Card Display",1) },

        {12, (227, "Double Sided Shelf",5) },
        {13, (228, "Wall Display Case",1) },
        {14, (204, "Big Warehouse Shelf",2) },

        {15, (229, "Card Projector S",1) },
        {16, (235, "Checkout Counter",1) },
        {17, (230, "Pack Opener Machine S",1) },

        {18, (203, "Auto Scent G500",2) },
        {19, (201, "Card Display Table",2) },
        {20, (202, "Big Personal Shelf",2) },

        {21, (200, "Vintage Card Table",2) },
        {22, (228, "Wall Display Case 3x2",2) },
        {23, (228, "Wall Display Case 5x2",3) },

        {24, (227, "Wide Shelf",6) },
        {25, (229, "Card Projector M",2) },
        {26, (230, "Pack Opener Machine M",2) },

        {27, (202, "Huge Personal Shelf",3) },
        {28, (203, "Auto Scent T100",3) },
        {29, (232, "Play Table Black", 2) },

        {30, (232, "Play Table White", 3) },
        {31, (201, "Big Card Display",3) },
        {32, (228, "Wall Display Case 6x6 Black",4) },

        {33, (228, "Wall Display Case 6x6 White",5) },
        {34, (230, "Pack Opener Machine L",3) },
        {35, (229, "Card Projector L",3) },
    };


    public static int getindexFromId(int itemid)
    {
        foreach (var kvp in mapping)
        {
            if (kvp.Value.itemid == itemid)
                return kvp.Key;
        }
        return -1;
    }
}

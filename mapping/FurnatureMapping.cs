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
        {10, (236, "Empty Box Storage",1) },
        {11, (234, "Trash Bin",1) },
        
        {12, (201, "Small Card Display",1) },
        {13, (227, "Double Sided Shelf",5) },
        {14, (228, "Wall Display Case",1) },
        
        {15, (204, "Big Warehouse Shelf",2) },
        {16, (229, "Card Projector S",1) },
        {17, (235, "Checkout Counter",1) },
        
        {18, (230, "Pack Opener Machine S",1) },
        {19, (203, "Auto Scent G500",2) },
        {20, (237, "Bulk Donation Box",1) },
        
        {21, (238, "Card Storage Shelf",1) },
        {22, (201, "Card Display Table",2) },
        {23, (202, "Big Personal Shelf",2) },
        
        {24, (200, "Vintage Card Table",2) },
        {25, (228, "Wall Display Case 3x2",2) },
        {26, (228, "Wall Display Case 5x2",3) },

        {27, (227, "Wide Shelf",6) },
        {28, (229, "Card Projector M",2) },
        {29, (230, "Pack Opener Machine M",2) },

        {30, (202, "Huge Personal Shelf",3) },
        {31, (203, "Auto Scent T100",3) },
        {32, (232, "Play Table Black", 2) },

        {33, (232, "Play Table White", 3) },
        {34, (201, "Big Card Display",3) },
        {35, (228, "Wall Display Case 6x6 Black",4) },

        {36, (228, "Wall Display Case 6x6 White",5) },
        {37, (230, "Pack Opener Machine L",3) },
        {38, (229, "Card Projector L",3) },
        
        {39, (227, "Corner Shelf",7) },
        {40, (227, "Box Shelf",7) },
        {41, (227, "Tetramon Shelf",8) },
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

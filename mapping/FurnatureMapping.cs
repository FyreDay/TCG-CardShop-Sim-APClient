using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApClient.mapping;

public class FurnatureMapping
{
    public static int[] reorder = [0, 5, 17,  4, 11,18,  1,6,12,  2, 10, 13,  8,14,20,  3,12,19, 7,15,21, 9];
    public static Dictionary<int, (int itemid, string name, int count)> mapping = new Dictionary<int, (int itemid, string name, int count)>
    {
        {0, (227, "Small Cabinet", 1) },
        {1, (228, "Small Metal Rack", 1) },
        {2, (232, "Play Table", 1) },
        {3, (202, "Small Personal Shelf", 1) },
        {4, (229, "Single Sided Shelf", 1) },
        {5, (200, "Card Table",1) },
        {6, (204, "Small Warehouse Shelf",1) },
        {7, (201, "Small Card Display",1) },
        {8, (203, "Auto Scent M100",1) },
        {9, (233, "Workbench",1) },
        {10, (234, "Trash Bin",1) },
        {11, (230, "Double Sided Shelf",1) },
        {12, (204, "Big Warehouse Shelf",2) },
        {13, (235, "Checkout Counter",1) },
        {14, (203, "Auto Scent G500",2) },
        {15, (201, "Card Display Table",2) },
        {16, (202, "Big Personal Shelf",2) },
        {17, (200, "Vintage Card Table",2) },
        {18, (231, "Wide Shelf",1) },
        {19, (202, "Huge Personal Shelf",3) },
        {20, (203, "Auto Scent T100",3) },
        {21, (201, "Big Card Display",3) },
       
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

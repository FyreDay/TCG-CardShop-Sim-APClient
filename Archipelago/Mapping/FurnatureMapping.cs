using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace ApClient.Archipelago.Mapping;

public class FurnatureMapping
{
    public class FurnatureTypeToId
    {
        public EObjectType type {  get; set; }
        public long id { get; set; }
        public int ProgressiveNum { get; set; }
        public FurnatureTypeToId(EObjectType type, long id, int progressiveNum)
        {
            this.type = type;
            this.id = id;
            ProgressiveNum = progressiveNum;
        }
    }
    public static List<FurnatureTypeToId> Furnature = new()
    {
        new FurnatureTypeToId(EObjectType.CabinetA, 227,1), 
        new FurnatureTypeToId(EObjectType.ShelfSmallB, 227,2), 
        new FurnatureTypeToId(EObjectType.Shelf2X2, 227,3), 
        new FurnatureTypeToId(EObjectType.ShelfSmall, 227,4), 
        new FurnatureTypeToId(EObjectType.Shelf, 227,5),
        new FurnatureTypeToId(EObjectType.LongShelfA, 236, 1),
        new FurnatureTypeToId(EObjectType.LShapedShelf, 236, 2),
        new FurnatureTypeToId(EObjectType.BoxShelf, 236, 3),
        new FurnatureTypeToId(EObjectType.TetramonCircularShelf, 236, 4),

        new FurnatureTypeToId(EObjectType.PlayTable, 232, 1),
        new FurnatureTypeToId(EObjectType.BlackPlayTable, 232, 2),
        new FurnatureTypeToId(EObjectType.WhitePlayTable, 232, 3),

        new FurnatureTypeToId(EObjectType.WarehouseShelfA, 204, 1),
        new FurnatureTypeToId(EObjectType.WarehouseShelf, 204, 2),

        new FurnatureTypeToId(EObjectType.CardShelf, 200, 1),
        new FurnatureTypeToId(EObjectType.VintageCardTable, 200, 2),

        new FurnatureTypeToId(EObjectType.AutoCleanser1, 203, 1),
        new FurnatureTypeToId(EObjectType.AutoCleanser2, 203, 2),
        new FurnatureTypeToId(EObjectType.AutoCleanser3, 203, 3),

        new FurnatureTypeToId(EObjectType.DisplayCardShelfA, 201, 1),
        new FurnatureTypeToId(EObjectType.DisplayCardTableA, 201, 2),
        new FurnatureTypeToId(EObjectType.DisplayCardShelfB, 201, 3),

        new FurnatureTypeToId(EObjectType.CashCounter, 235, 1),

        new FurnatureTypeToId(EObjectType.AutoPackOpener1, 230, 1),
        new FurnatureTypeToId(EObjectType.AutoPackOpener2, 230, 2),
        new FurnatureTypeToId(EObjectType.AutoPackOpener3, 230, 3),

        new FurnatureTypeToId(EObjectType.CardDisplayWallSmall, 228, 1),
        new FurnatureTypeToId(EObjectType.CardDisplayWallBig, 228, 2),
        new FurnatureTypeToId(EObjectType.CardDisplayWall3X2, 228, 3),
        new FurnatureTypeToId(EObjectType.CardDisplayWall1, 228, 4),
        new FurnatureTypeToId(EObjectType.CardDisplayWallBigWhite, 228, 5),

        new FurnatureTypeToId(EObjectType.CardDisplayElectronicSmall, 229, 1),
        new FurnatureTypeToId(EObjectType.CardDisplayElectronic, 229, 2),
        new FurnatureTypeToId(EObjectType.CardDisplayElectronicLarge, 229, 3),

        new FurnatureTypeToId(EObjectType.BulkDonationBox, 240, 1),

        new FurnatureTypeToId(EObjectType.Trashbin, 234, 1),

        new FurnatureTypeToId(EObjectType.EmptyBoxStorage, 239, 1),

        new FurnatureTypeToId(EObjectType.Workbench, 233, 1),

        new FurnatureTypeToId(EObjectType.CardStorageShelf, 241, 1),

        new FurnatureTypeToId(EObjectType.PersonalShelfA, 202, 1),
        new FurnatureTypeToId(EObjectType.PersonalShelfB, 202, 2),
        new FurnatureTypeToId(EObjectType.PersonalShelfC, 202, 3),
    };
}

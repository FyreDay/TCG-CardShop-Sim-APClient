using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace ApClient.Archipelago.Mapping;

public class FurnatureMapping
{
    public static List<(EObjectType, long)> Furnature = new()
    {
        (EObjectType.CabinetA, 227),
        (EObjectType.ShelfSmallB, 227),
        (EObjectType.Shelf2X2, 227),
        (EObjectType.ShelfSmall, 227),
        (EObjectType.Shelf, 227),
        (EObjectType.LongShelfA, 236),
        (EObjectType.LShapedShelf, 236),
        (EObjectType.BoxShelf, 236),
        (EObjectType.TetramonCircularShelf, 236),
        (EObjectType.PlayTable, 232),
        (EObjectType.BlackPlayTable, 232),
        (EObjectType.WhitePlayTable, 232),
        (EObjectType.WarehouseShelfA, 204),
        (EObjectType.WarehouseShelf, 204),
        (EObjectType.CardShelf, 200),
        (EObjectType.VintageCardTable, 200),
        (EObjectType.AutoCleanser1, 203),
        (EObjectType.AutoCleanser2, 203),
        (EObjectType.AutoCleanser3, 203),
        (EObjectType.DisplayCardShelfA, 201),
        (EObjectType.DisplayCardTableA, 201),
        (EObjectType.DisplayCardShelfB, 201),
        (EObjectType.CashCounter, 235),
        (EObjectType.AutoPackOpener1, 230),
        (EObjectType.AutoPackOpener2, 230),
        (EObjectType.AutoPackOpener3, 230),
        (EObjectType.CardDisplayWallSmall, 228),
        (EObjectType.CardDisplayWallBig, 228),
        (EObjectType.CardDisplayWall3X2, 228),
        (EObjectType.CardDisplayWall1, 228),
        (EObjectType.CardDisplayWallBigWhite, 228),
        (EObjectType.CardDisplayElectronicSmall, 229),
        (EObjectType.CardDisplayElectronic, 229),
        (EObjectType.CardDisplayElectronicLarge, 229),
        (EObjectType.BulkDonationBox, 240),
        (EObjectType.Trashbin, 234),
        (EObjectType.EmptyBoxStorage, 239),
        (EObjectType.Workbench, 233),
        (EObjectType.CardStorageShelf, 241),
        (EObjectType.PersonalShelfA, 202),
        (EObjectType.PersonalShelfB, 202),
        (EObjectType.PersonalShelfC, 202),
    };
}

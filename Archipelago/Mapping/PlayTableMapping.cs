using System;
using System.Collections.Generic;
using System.Text;
using TMPro;

namespace ApClient.mapping;

public class PlayTableMapping
{
    public static EGameEventFormat GetFormatFromInt(int formatId)
    {
        return formatId switch
        {
            207 => EGameEventFormat.Standard,
            208 => EGameEventFormat.Pauper,
            209 => EGameEventFormat.FireCup,
            210 => EGameEventFormat.EarthCup,
            211 => EGameEventFormat.WaterCup,
            212 => EGameEventFormat.WindCup,
            213 => EGameEventFormat.FirstEditionVintage,
            214 => EGameEventFormat.SilverBorder,
            215 => EGameEventFormat.GoldBorder,
            216 => EGameEventFormat.ExBorder,
            217 => EGameEventFormat.FullArtBorder,
            218 => EGameEventFormat.Foil,
            _ => EGameEventFormat.None
        };
    }

    public static int PlayCheckStartingId = 300;
    public static int FormatStartingId = 207;
}

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
            0x1F2800FC => EGameEventFormat.Standard,
            0x1F2800FD => EGameEventFormat.Pauper,
            0x1F2800FE => EGameEventFormat.FireCup,
            0x1F2800FF => EGameEventFormat.EarthCup,
            0x1F280100 => EGameEventFormat.WaterCup,
            0x1F280101 => EGameEventFormat.WindCup,
            0x1F280102 => EGameEventFormat.FirstEditionVintage,
            0x1F280103 => EGameEventFormat.SilverBorder,
            0x1F280104 => EGameEventFormat.GoldBorder,
            0x1F280105 => EGameEventFormat.ExBorder,
            0x1F280106 => EGameEventFormat.FullArtBorder,
            0x1F280107 => EGameEventFormat.Foil,
            _ => EGameEventFormat.None
        };
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace ApClient.data;

[Serializable]
public class AchievementData
{
    public string type;
    public string name;
    public int difficulty;
    public int threshold;
    public int[] rarity;
    public int[] border;
    public int[] expansion;
    public int[] foil;
}

using System;
using System.Collections.Generic;
using System.Text;

namespace ApClient.Core.Archipelago;

public class ItemEffect
{
    public Func<long, bool> ValidId { get; }
    public Action<long> Effect { get; }
    public ItemEffect(Func<long, bool> validId, Action<long> effect)
    {
        this.ValidId = validId;
        this.Effect = effect;
    }
}

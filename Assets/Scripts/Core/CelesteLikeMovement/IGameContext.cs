using System;
using System.Collections.Generic;

namespace Core.CelesteLikeMovement
{
    public interface IGameContext
    {
        IEffectControl EffectControl { get; }

        ISoundControl SoundControl { get; }


    }
}
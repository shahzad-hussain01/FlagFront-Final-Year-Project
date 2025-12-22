using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lovatto.FloatingTextAsset
{
    [Flags]
    public enum FloatingTextFlags
    { 
        None = 0,
        DontRewind = 1,
        StickAtOriginPosition = 2,
        InvertHorizontalDirectionRandomly = 4,
    }
}
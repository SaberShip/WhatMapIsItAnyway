using HarmonyLib;
using Verse;

namespace WhatMapIsItAnyway
{
    public class WhatMapIsItAnyway : Mod
    {
        public WhatMapIsItAnyway(ModContentPack content) : base(content)
        {
#if DEBUG
            Harmony.DEBUG = true;
#endif

            Harmony harmony = new Harmony("rimworld.whatmapisitanyway");
            harmony.PatchAll();
        }
    }
}

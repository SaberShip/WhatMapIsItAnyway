using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace WhatMapIsItAnyway
{
    // Patch colonist bar reference to use tactical colonist bar instead
    [HarmonyPatch(typeof(ColonistBarRef), nameof(ColonistBarRef.GroupFrameRect))]
    public static class TacticalGroupFrameRectPatch
    {
        public static void Postfix(int group, ref Rect __result) {
            __result = TacticalColonistBarRef.GroupFrameRect(group);
        }
    }

    // Patch colonist bar reference to use tactical colonist bar instead
    [HarmonyPatch(typeof(ColonistBarRef), nameof(ColonistBarRef.GetDrawLocs))]
    public static class TacticalGetDrawLocsPatch
    {
        public static void Postfix(ref List<Rect> __result) {
            __result = TacticalColonistBarRef.GetDrawLocs();
        }
    }
}
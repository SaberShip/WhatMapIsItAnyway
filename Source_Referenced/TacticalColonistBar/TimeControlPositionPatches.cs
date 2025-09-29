using HarmonyLib;
using RimWorld;
using Multiplayer.API;
using System;
using System.Linq;
using UnityEngine;
using Verse;
using System.Reflection;

namespace WhatMapIsItAnyway.TacticalColonistBar
{
    [HarmonyPatch]
    public static class DrawingMpTimeControlButtons
    {
        static MethodBase TargetMethod()
        {
            var type = AccessTools.TypeByName("ColonistBarTimeControl");
            return AccessTools.Method(typeof(Dialog_BillConfig), "DrawButtons");
        }

        static bool Prepare()
        {
            // Only Patch is multiplayer is present and active.
            return CompatUtils.Compatibility.IsModActive("rwmt.multiplayer");
        }

        static void Prefix()
        {
            if (!MP.enabled || !MP.IsInMultiplayer) return;

            GetGroupFrameRectPatch.DrawingTimeControls = true;
        }

        static void Postfix()
        {
            if (!MP.enabled || !MP.IsInMultiplayer) return;

            GetGroupFrameRectPatch.DrawingTimeControls = false;
        }
    }

    [HarmonyPatch(typeof(ColonistBarColonistDrawer), "GroupFrameRect")]
    public static class GetGroupFrameRectPatch
    {
        public static bool DrawingTimeControls = false;

        public static void Postfix(ref Rect __result, int group)
        {
            if (DrawingTimeControls)
            {
                __result.x = TacticalColonistBarRef.GroupFrameRect(group).x;
            }
        }
    }
}

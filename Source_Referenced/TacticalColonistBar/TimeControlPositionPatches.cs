using HarmonyLib;
using RimWorld;
using Multiplayer.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TacticalGroups;
using UnityEngine;
using Verse;

namespace WhatMapIsItAnyway.TacticalColonistBar
{
    [HarmonyPatch("ColonistBarTimeControl", "DrawButtons", MethodType.Normal)]
    public static class DrawingMpTimeControlButtons
    {
        public static void Prefix()
        {
            if (!MP.enabled || !MP.IsInMultiplayer) return;

            GetGroupFrameRectPatch.DrawingTimeControls = true;
        }

        public static void Postfix()
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

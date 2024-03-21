using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace WhatMapIsItAnyway 
{
    [HarmonyPatch(typeof(MainTabWindow_PawnTable), nameof(MainTabWindow_PawnTable.DoWindowContents))]
    public static class ColonistAssignmentContentsPatch
    {
        public static bool OnlyMapPawns = true;

        private static bool lastCheckboxState = true;
        private static MethodInfo setDirtyMethod = AccessTools.Method(typeof(MainTabWindow_PawnTable), "SetDirty");

        static void Postfix(MainTabWindow_PawnTable __instance, Rect rect)
        {
            if (__instance is MainTabWindow_Assign)
            {
                Rect filterLabelRect = new Rect(rect.x + 8, rect.yMax - 32, 128, rect.yMax);
                Widgets.Label(filterLabelRect, "Current map pawns");
                Widgets.Checkbox(new Vector2(filterLabelRect.xMax, filterLabelRect.yMin), ref OnlyMapPawns);

                if (lastCheckboxState != OnlyMapPawns) 
                {
                    setDirtyMethod.Invoke(__instance, null);
                    lastCheckboxState = OnlyMapPawns;
                }
            }
        }
    }

    [HarmonyPatch]
    public static class ColonistAssignmentOnlyCurrentMap {
        static MethodBase TargetMethod()
        {
            return AccessTools.PropertyGetter(typeof(MainTabWindow_Assign), "Pawns");
        }

        static void Postfix(ref IEnumerable<Pawn> __result)
        {
            if (ColonistAssignmentContentsPatch.OnlyMapPawns)
            {
                __result = from pawn in __result
                           where pawn.Map != null && pawn.Map == Find.CurrentMap
                           select pawn;
            }
        }
    }
}
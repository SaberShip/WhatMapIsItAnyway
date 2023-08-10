using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace WhatMapIsItAnyway 
{
    [HarmonyPatch]
    public static class ColonistAssignmentOnlyCurrentMap {
        static MethodBase TargetMethod()
        {
            return AccessTools.PropertyGetter(typeof(MainTabWindow_Assign), "Pawns");
        }

        static void Postfix(ref IEnumerable<Pawn> __result)
        {
            __result = from pawn in __result
                where pawn.Map != null && pawn.Map == Find.CurrentMap
                select pawn;
        }
    }
}
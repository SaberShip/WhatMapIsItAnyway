using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace WhatMapIsItAnyway 
{
    [HarmonyPatch]
    public static class BillRestrictionsOnlyCurrentMap {
        static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(Dialog_BillConfig), "GeneratePawnRestrictionOptions");
        }

        static void Postfix(ref IEnumerable<Widgets.DropdownMenuElement<Pawn>> __result)
        {
            __result = from element in __result
                where element.payload == null
                    || (element.payload.Map != null
                    && element.payload.Map == Find.CurrentMap)
                select element;
        }
    }
}
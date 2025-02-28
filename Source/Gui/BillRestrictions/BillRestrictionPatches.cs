using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FloatSubMenus;
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
            var pawnsByMap = __result.Where(e => e.payload != null && e.payload.Map != null)
                .GroupBy(e => e.payload.Map)
                .ToDictionary(e => e.Key.ToString(), e => e.Select(menu => menu.option).ToList());

            if (pawnsByMap.Count < 2)
            {
                // Only one map, change nothing
                return;
            }

            // Start with generic restrictions (not pawn specific)
            var restrictions = from element in __result where element.payload == null select element;

            // Add remaining restriction options organized by map for which they reside.
            foreach (String mapName in pawnsByMap.Keys)
            {
                var pawns = pawnsByMap[mapName];

                if (!pawns.NullOrEmpty())
                {
                    restrictions = restrictions.AddItem(new Widgets.DropdownMenuElement<Pawn>
                    {
                        option = FloatSubMenu.CompatCreate(mapName, pawns),
                        payload = null
                    });
                }
            }

            __result = restrictions;
        }
    }
}
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
                .ToDictionary(e => e.Key, e => e.Select(menu => menu.option).ToList());

            /*if (pawnsByMap.Count < 2)
            {
                // Only one map, change nothing
                return;
            }*/

            Log.Message("Making sub menus:" + pawnsByMap.Count());
            Log.Message("Keys:" + pawnsByMap.Keys);

            // Start with generic restrictions (not pawn specific)
            var restrictions = from element in __result where element.payload == null select element;

            // Add remaining restriction options organized by map for which they reside.
            foreach (Map map in pawnsByMap.Keys)
            {
                var pawns = pawnsByMap[map];



                if (!pawns.NullOrEmpty())
                {
                    Log.Message("Making sub menu map for:" + map.ToString() + pawns.Count());

                    restrictions = restrictions.AddItem(new Widgets.DropdownMenuElement<Pawn>
                    {
                        option = FloatSubMenu.CompatCreate(map.ToString(), pawns),
                        payload = null
                    });
                }
            }

            __result = restrictions;
        }
    }
}
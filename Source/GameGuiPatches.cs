using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;

namespace WhatMapIsItAnyway
{
    [HarmonyPatch(typeof(Root), nameof(Root.Update))]
    public static class TickResetMapHighlightsPatch
    {
        static void Prefix()
        {
            TargetHighligterPatch.MapTargetsToHighlight.Clear();
        }
    }

    [HarmonyPatch(typeof(TargetHighlighter), nameof(TargetHighlighter.Highlight))]
    public static class TargetHighligterPatch
    {
        public static List<Map> MapTargetsToHighlight = new List<Map>();

        static void Postfix(GlobalTargetInfo target, bool colonistBar)
        { 
            if (!colonistBar) return;

            if (target.IsMapTarget && target.Tile >= 0)
            {
                ColonistBar bar = Find.ColonistBar;
                if (target.Thing != null)
                {
                    if ((target.Thing is Pawn) && !bar.GetColonistsInOrder().Contains((Pawn) target.Thing))
                    {
                        MapTargetsToHighlight.Add(Find.Maps.Find(m => m.Tile == target.Tile));
                    }
                    else if (target.Thing is not Pawn && target.Thing is not Corpse) 
                    {
                        MapTargetsToHighlight.Add(Find.Maps.Find(m => m.Tile == target.Tile));
                    }
                }
                else
                {
                    MapTargetsToHighlight.Add(Find.Maps.Find(m => m.Tile == target.Tile));
                }
            }
            else if (target.Tile >= 0)
            {
                MapTargetsToHighlight.Add(Find.Maps.Find(m => m.Tile == target.Tile));
            }
        }
    }


    [HarmonyPatch(typeof(ColonistBar), nameof(ColonistBar.ColonistBarOnGUI))]
    public static class ColonistBarGuiPatch
    {
        public static List<Map> mapsHighlighting = new List<Map>();

        static MethodBase groupRect = AccessTools.Method(typeof(ColonistBarColonistDrawer), "GroupFrameRect");

        static void Prefix(ColonistBar __instance)
        {
            List<Map> maps = __instance.Entries.Select(e => e.map)
                .Distinct()
                .Where(m => m != null && TargetHighligterPatch.MapTargetsToHighlight.Contains(m))
                .ToList();

            if (maps == null)
            {
                return;
            }

            if (Find.World.renderer.wantedMode == WorldRenderMode.Planet && maps.Count == 0)
            {
                Map tileMap;
                WorldObject worldObject = Find.WorldInterface.selector.SingleSelectedObject;
                if (worldObject != null)
                {
                    tileMap = Find.Maps.Find(m => m.Tile == worldObject.Tile);
                }
                else
                {
                    tileMap = Find.Maps.Find(m => m.Tile == Find.WorldInterface.SelectedTile);
                }

                if (tileMap != null)
                {
                    maps.Add(tileMap);
                }
            }

            foreach (Map map in maps)
            {
                mapsHighlighting.Add(map);
                highlightColonistMap(__instance, map);
            }
        }

        static void Postfix()
        {
            mapsHighlighting.Clear();
        }

        static void highlightColonistMap(ColonistBar __instance, Map mapGroup)
        {
            if (__instance.Entries.Count == 0) return;

            int? groupNum = __instance.Entries.Where(e => e.map == mapGroup)
                .Select(e => e.group)
                .FirstOrDefault();

            if (groupNum.HasValue)
            {
                Rect groupBar = (Rect)groupRect.Invoke(__instance.drawer, new[] { (object)groupNum.Value });
                groupBar.yMin = 0;
                Widgets.DrawBoxSolidWithOutline(groupBar, Color.clear, Color.white, 3);
            }
        }
    }

    // Don't highlight all pawns if the pawn's map is being highlighted
    [HarmonyPatch(typeof(ColonistBarColonistDrawer), nameof(ColonistBarColonistDrawer.DrawColonist))]
    public static class ColonistHighlightGuiPatch
    {

        static void Prefix(Map pawnMap, ref bool highlight)
        {
            if (highlight && ColonistBarGuiPatch.mapsHighlighting.Contains(pawnMap))
            {
                highlight = false;
            }
        }
    }
}

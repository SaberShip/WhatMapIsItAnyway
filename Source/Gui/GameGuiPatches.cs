using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace WhatMapIsItAnyway 
{
    // Reset maps being highlighted starting with every update.
    [HarmonyPatch(typeof(Root), nameof(Root.Update))]
    public static class TickResetMapHighlightsPatch
    {
        static void Prefix()
        {
            ColonistBarHighlighter.mapsToHighlight.Clear();
            ColonistBarHighlighter.additionalHighlights.Clear();
        }
    }

    // Highlight colonist groups when hovering over UI elements
    [HarmonyPatch(typeof(TargetHighlighter), nameof(TargetHighlighter.Highlight))]
    public static class TargetHighligterPatch
    {
        public static bool skipNextTarget = false;
        static void Postfix(GlobalTargetInfo target, bool colonistBar)
        {
            if (!colonistBar || skipNextTarget)
            {
                skipNextTarget = false;
                return;
            }

            if (target.IsMapTarget && target.Tile >= 0)
            {
                ColonistBar bar = Find.ColonistBar;
                if (target.Thing != null)
                {
                    if ((target.Thing is Pawn) && !bar.GetColonistsInOrder().Contains((Pawn) target.Thing))
                    {
                        // Target pawn exists on the given map but is not in the colonist bar to highlight, so highlight the group
                        ColonistBarHighlighter.mapsToHighlight.Add(Find.Maps.Find(m => m.Tile == target.Tile));
                    }
                    else if (target.Thing is not Pawn && target.Thing is not Corpse) 
                    {
                        // Target is not a pawn or corpse and it exists on the given map, so highlight the group
                        ColonistBarHighlighter.mapsToHighlight.Add(Find.Maps.Find(m => m.Tile == target.Tile));
                    }
                }
                else
                {
                    // No target provided to highlight, so highlight the group
                    ColonistBarHighlighter.mapsToHighlight.Add(Find.Maps.Find(m => m.Tile == target.Tile));
                }
            }
            else if (target.Tile >= 0)
            {
                // Only a map tile is provided, highlight the group it corresponds to
                ColonistBarHighlighter.mapsToHighlight.Add(Find.Maps.Find(m => m.Tile == target.Tile));
            }
        }
    }

    // Don't highlight all pawns if the pawn's map is being highlighted
    [HarmonyPatch(typeof(ColonistBarColonistDrawer), nameof(ColonistBarColonistDrawer.DrawColonist))]
    public static class ColonistHighlightGuiPatch
    {

        static void Prefix(Map pawnMap, ref bool highlight)
        {
            if (highlight && ColonistBarHighlighter.mapsToHighlight.Contains(pawnMap))
            {
                highlight = false;
            }
        }
    }
}

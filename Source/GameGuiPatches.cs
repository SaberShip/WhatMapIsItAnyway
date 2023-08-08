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
        }
    }

    // Highlight colonist groups when hovering over UI elements
    [HarmonyPatch(typeof(TargetHighlighter), nameof(TargetHighlighter.Highlight))]
    public static class TargetHighligterPatch
    {
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
                        ColonistBarHighlighter.mapsToHighlight.Add(Find.Maps.Find(m => m.Tile == target.Tile));
                        Log.Message($"Adding {target.Tile} to highlight list");
                    }
                    else if (target.Thing is not Pawn && target.Thing is not Corpse) 
                    {
                        ColonistBarHighlighter.mapsToHighlight.Add(Find.Maps.Find(m => m.Tile == target.Tile));
                        Log.Message($"Adding {target.Tile} to highlight list");
                    }
                }
                else
                {
                    ColonistBarHighlighter.mapsToHighlight.Add(Find.Maps.Find(m => m.Tile == target.Tile));
                    Log.Message($"Adding {target.Tile} to highlight list");
                }
            }
            else if (target.Tile >= 0)
            {
                ColonistBarHighlighter.mapsToHighlight.Add(Find.Maps.Find(m => m.Tile == target.Tile));
                Log.Message($"Adding {target.Tile} to highlight list");
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
                Log.Message($"Stopping pawn highlight");
            }
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using TacticalGroups;
using UnityEngine;
using Verse;

namespace WhatMapIsItAnyway
{
    // Patch colonist bar reference to use tactical colonist bar instead
    [HarmonyPatch(typeof(ColonistBarRef), nameof(ColonistBarRef.GroupFrameRect))]
    public static class TacticalGroupFrameRectPatch
    {
        public static bool Prefix(int group, ref Rect __result) {
            __result = TacticalColonistBarRef.GroupFrameRect(group);
            return false;
        }
    }

    // Patch colonist bar reference to use tactical colonist bar instead
    [HarmonyPatch(typeof(ColonistBarRef), nameof(ColonistBarRef.GetDrawLocs))]
    public static class TacticalGetDrawLocsPatch
    {
        public static void Postfix(ref List<Rect> __result) {
            __result = TacticalColonistBarRef.GetDrawLocs();
        }
    }


    // Highlight user created "ColonistGroups" when pawns in them are not displayed on the colonist bar
    [HarmonyPatch(typeof(TargetHighlighter), nameof(TargetHighlighter.Highlight))]
    public static class TacticalColonistGroupPatch
    {
        public static bool Prefix(GlobalTargetInfo target, bool colonistBar)
        {
            if (colonistBar && target.IsMapTarget && target.Tile >= 0 && target.Thing is Pawn)
            {
                Pawn pawn = (Pawn)target.Thing;
                HashSet<ColonistGroup> groups = new HashSet<ColonistGroup>();
                TacticUtils.TryGetGroups(pawn, out groups);
                List<ColonistGroup> manualGroups = groups.Where(group => group is PawnGroup).ToList();
                List<ColonistGroup> tacticalColonyGroupBoxes = groups.Where(group => group is ColonyGroup).ToList();

                if (manualGroups.Any(group => !group.pawnIcons.GetValueOrDefault(pawn, new PawnIcon(pawn)).isVisibleOnColonistBar))
                {
                    if (WorldRendererUtility.WorldRendered || pawn.Map != Find.CurrentMap)
                    {
                        return true;
                    }
                    // Highlight the user defined group and skip the postfix
                    ColonistBarHighlighter.additionalHighlights.AddRange(manualGroups.Select(group => group.curRect));
                    TargetHighligterPatch.skipNextTarget = true;
                    return false;
                }
                else if(tacticalColonyGroupBoxes.Any(group => !group.pawnIcons.GetValueOrDefault(pawn, new PawnIcon(pawn)).isVisibleOnColonistBar))
                {
                    // Highlight the non user defined colony groups made by this mod and skip the postfix
                    ColonistBarHighlighter.additionalHighlights.AddRange(tacticalColonyGroupBoxes.Select(group => group.curRect));
                    TargetHighligterPatch.skipNextTarget = true;
                    return false;
                }
            }

            return true;
        }
    }
}
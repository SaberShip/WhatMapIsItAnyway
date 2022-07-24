using HarmonyLib;
using RimWorld;
using RimWorld.QuestGen;
using System;
using System.Reflection;
using Verse;

namespace WhatMapIsItAnyway
{
    [HarmonyPatch]
    public static class IncidentWorkerPatch
    {
        static MethodBase TargetMethod()
        {
            Type[] args = new[] {typeof(TaggedString), typeof(TaggedString), typeof(LetterDef), typeof(IncidentParms), typeof(LookTargets), typeof(NamedArgument[])};
            return AccessTools.Method(typeof(IncidentWorker), "SendStandardLetter", args);
        }

        static void Prefix(IncidentParms parms, ref LookTargets lookTargets)
        {
            Map map;
            if ((map = parms.target as Map) != null && (lookTargets == null || !lookTargets.IsValid)) {
                lookTargets = new LookTargets(map.Tile);
            }
        }
    }

    [HarmonyPatch]
    public static class WandererJoinsQuestPatch
    {
        public static Map mapToJoin = null;
        static MethodBase TargetMethod()
        {
            Type[] args = new[] {typeof(Quest), typeof(Map), typeof(Pawn)};
            return AccessTools.Method(typeof(QuestNode_Root_WandererJoin_WalkIn), "AddSpawnPawnQuestParts", args);
        }

        static void Prefix(Map map)
        {
            mapToJoin = map;
        }
    }

    [HarmonyPatch]
    public static class WandererJoinsQuestLetterPatch
    {
        static MethodBase TargetMethod()
        {
            Type[] args = new[] {typeof(TaggedString), typeof(TaggedString), typeof(LetterDef), typeof(Faction), typeof(Quest)};
            return AccessTools.Method(typeof(LetterMaker), nameof(LetterMaker.MakeLetter), args);
        }

        static void Postfix(ref ChoiceLetter __result)
        {
            if (WandererJoinsQuestPatch.mapToJoin != null)
            {
                __result.lookTargets = new LookTargets(WandererJoinsQuestPatch.mapToJoin.Tile);
            }
        }
    }
}

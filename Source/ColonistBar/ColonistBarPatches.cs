using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace WhatMapIsItAnyway
{
    [HarmonyPatch(typeof(ColonistBar), nameof(ColonistBar.ColonistBarOnGUI))]
    public static class ColonistBarHighlighter
    {
        public static HashSet<Map> mapsToHighlight = new();

        public static void Postfix() {
            foreach (Map map in mapsToHighlight) {
                HighlightMap(map);
            }
        }

        public static void HighlightMap(Map map) {
            int? groupNum = ColonistBarRef.GetColonistBarEntries()
                .Where(e => e.map == map)
                .Select(e => e.group)
                .FirstOrDefault();

            if (groupNum.HasValue) {
                Rect groupBar = ColonistBarRef.GroupFrameRect(groupNum.Value);
                groupBar.yMin = 3;
                HighlightColonistGroup(groupBar);
            }
        }

        static void HighlightColonistGroup(Rect highlightBox)
        {
            if (highlightBox == null) {
                return;
            }

            Widgets.DrawBoxSolidWithOutline(highlightBox, Color.clear, Color.white, 3);
        }
    }
}
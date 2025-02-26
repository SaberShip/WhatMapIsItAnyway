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
        public static HashSet<Rect> additionalHighlights = new();

        public static void Postfix() {
            foreach (Map map in mapsToHighlight) {
                HighlightMap(map);
            }
            foreach (Rect rect in additionalHighlights)
            {
                HighlightColonistGroup(rect);
            }
        }

        public static void HighlightMap(Map map) {
            int groupNum = ColonistBarRef.GetColonistBarEntries()
                .Where(e => e.map == map)
                .Select(e => e.group)
                .DefaultIfEmpty(-1)
                .FirstOrDefault();


            if (groupNum != -1) {
                Rect groupBar = ColonistBarRef.GroupFrameRect(groupNum);
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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using TacticalGroups;
using UnityEngine;
using Verse;

namespace WhatMapIsItAnyway
{
    public class TacticalColonistBarRef {

        static MethodInfo RectGetter = AccessTools.Method(typeof(TacticalGroups_ColonistBarColonistDrawer), "GroupFrameRect");

        public static float GetColonistBarScale() {
            return TacticUtils.TacticalColonistBar.Scale;
        }

        public static Vector2 GetColonistBarSize() {
            return TacticUtils.TacticalColonistBar.Size;
        }

        public static List<Rect> GetDrawLocs() {
            return TacticUtils.TacticalColonistBar.DrawLocs.ToList();
        }

        public static List<TacticalGroups.TacticalColonistBar.Entry> GetColonistBarEntries() {
			return TacticUtils.TacticalColonistBar.Entries;
        }

        public static Rect GroupFrameRect(int group)
		{
            return (Rect) RectGetter.Invoke(TacticUtils.TacticalColonistBar.drawer, new object[1] { group });
		}
    }
}
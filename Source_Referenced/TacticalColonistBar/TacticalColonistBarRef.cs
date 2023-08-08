using System.Collections.Generic;
using System.Linq;
using RimWorld;
using TacticalGroups;
using UnityEngine;
using Verse;

namespace WhatMapIsItAnyway
{
    public class TacticalColonistBarRef {

        public static float GetColonistBarScale() {
            return TacticUtils.TacticalColonistBar.Scale;
        }

        public static Vector2 GetColonistBarSize() {
            return TacticUtils.TacticalColonistBar.Size;
        }

        public static List<Rect> GetDrawLocs() {
            return TacticUtils.TacticalColonistBar.DrawLocs;
        }

        public static List<ColonistBar.Entry> GetColonistBarEntries() {
            return TacticUtils.TacticalColonistBar.Entries
                .Select(e => new ColonistBar.Entry(e.pawn, e.map, e.group))
                .ToList();
        }

        public static Rect GroupFrameRect(int group)
		{
			float num = 99999f;
			float num2 = 0f;
			float num3 = 0f;
			List<ColonistBar.Entry> entries = GetColonistBarEntries();
			List<Rect> drawLocs = GetDrawLocs();
			for (int i = 0; i < entries.Count; i++)
			{
				if (entries[i].group == group)
				{
					num = Mathf.Min(num, drawLocs[i].x);
					num2 = Mathf.Max(num2, drawLocs[i].x + GetColonistBarSize().x);
					num3 = Mathf.Max(num3, drawLocs[i].y + GetColonistBarSize().y);
				}
			}
			return new Rect(num, 0f, num2 - num, num3 - 0f).ContractedBy(-12f * GetColonistBarScale());
		}
    }
}
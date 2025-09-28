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

        public static List<Vector2> GetDrawLocs() {
            return TacticUtils.TacticalColonistBar.DrawLocs.Select(rect => rect.position).ToList();
        }

        public static List<TacticalGroups.TacticalColonistBar.Entry> GetColonistBarEntries() {
			return TacticUtils.TacticalColonistBar.Entries;
        }

        public static Rect GroupFrameRect(int group)
		{
			float num = 99999f;
			float num2 = 0f;
			float num3 = 0f;
			List<TacticalGroups.TacticalColonistBar.Entry> entries = GetColonistBarEntries();
			List<Vector2> drawLocs = GetDrawLocs();
			for (int i = 0; i < entries.Count; i++)
			{
                TacticalGroups.TacticalColonistBar.Entry entry = entries[i];
				if (entry.group == group)
				{

					ColonistGroup colonistGroup = (ColonistGroup)entry.colonyGroup ?? entry.caravanGroup ?? null;

                    if (colonistGroup != null && entry.pawn != null)
					{

                        Pawn pawn = entry.pawn;
						PawnIcon icon = colonistGroup.pawnIcons.GetValueOrDefault(pawn);
						if (icon != null && !icon.isVisibleOnColonistBar)
						{
							continue;
						}

                        num = Mathf.Min(num, drawLocs[i].x);
                        num2 = Mathf.Max(num2, drawLocs[i].x + GetColonistBarSize().x);
                        num3 = Mathf.Max(num3, drawLocs[i].y + GetColonistBarSize().y);
                    }
				}
			}
			return new Rect(num, 0f, num2 - num, num3 - 0f).ContractedBy(-12f * GetColonistBarScale());
		}
    }
}
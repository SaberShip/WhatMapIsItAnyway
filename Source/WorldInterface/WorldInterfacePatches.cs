using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld.Planet;
using Verse;

namespace WhatMapIsItAnyway
{
    public class WorldInterfacePatches {
        [HarmonyPatch(typeof(WorldSelector), nameof(WorldSelector.WorldSelectorOnGUI))]
        public static class TickResetMapHighlightsPatch
        {
            static void Postfix()
            {
                if (Find.World.renderer.wantedMode == WorldRenderMode.Planet)
                {
                    HashSet<Map> selectedMaps = Find.WorldInterface.selector.SelectedObjects
                        .Select(obj => Find.Maps.Find(m => m.Tile == obj.Tile))
                        .Where(map => map != null)
                        .ToHashSet();

                    if (!selectedMaps.EnumerableNullOrEmpty()) {
                        ColonistBarHighlighter.mapsToHighlight.AddRange(selectedMaps);
                    } else {
                        Map tileMap;
                        WorldObject worldObject = Find.WorldInterface.selector.SingleSelectedObject;
                        IEnumerable<WorldObject> objects = Find.WorldInterface.selector.SelectableObjectsUnderMouse();
                        if (!objects.EnumerableNullOrEmpty())
                        {
                            tileMap = Find.Maps.Find(m => m.Tile == objects.FirstOrDefault().Tile);

                            if (tileMap != null)
                            {
                                ColonistBarHighlighter.mapsToHighlight.Add(tileMap);
                            }
                        }
                    }
                }
            }
        }
    }
}
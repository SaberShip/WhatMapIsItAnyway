using System;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Mono.Cecil;
using RimWorld;
using Verse;

namespace WhatMapIsItAnyway
{
    public class WhatMapIsItAnyway : Mod
    {
        public WhatMapIsItAnyway(ModContentPack content) : base(content)
        {
            Harmony harmony = new Harmony("rimworld.sabership.whatmapisitanyway");

            var colonistBarOnGui = typeof(ColonistBar).GetMethod("ColonistBarOnGUI");

            if (CompatUtils.Compatibility.IsModActive("DerekBickley.LTOColonyGroupsFinal")) {
                Log.Message($"WhatMapIsItAnyway :: [LTO] Colony Groups mod is active.");
                ModConstants.LTO_COLONY_GROUPS_ACTIVE = true;

                AddReferenceAsm(content);

                Assembly asm = AppDomain.CurrentDomain.GetAssemblies().Reverse()
                    .Where(asm => asm.GetName().FullName.Contains("WhatMapIsItAnyway_Ref"))
                    .FirstOrDefault();

                harmony.PatchAll(asm);
                
            } else {
                Log.Message("WhatMapIsItAnyway :: [LTO] Colony Groups mod is not active.");
                ModConstants.LTO_COLONY_GROUPS_ACTIVE = false;

                RemoveReferenceAsm(content);
            }

            harmony.PatchAll();
        }

        static void RemoveReferenceAsm(ModContentPack content) {
            var asmPath = ModContentPack
                .GetAllFilesForModPreserveOrder(content, "Referenced/", f => f.ToLower() == ".dll")
                .FirstOrDefault(f => f.Item2.Name == "WhatMapIsItAnyway_Referenced.dll")?.Item2;

            if (asmPath == null) {
                return;
            }

            var asm = AssemblyDefinition.ReadAssembly(asmPath.FullName);

            foreach (var t in asm.MainModule.Types.ToArray()) {
                Log.Message($"WhatMapIsItAnyway :: Remove Type: {t.FullName}");
                asm.MainModule.Types.Remove(t);
            }
        }

        static void AddReferenceAsm(ModContentPack content) {
            var asmPath = ModContentPack
                .GetAllFilesForModPreserveOrder(content, "Referenced/", f => f.ToLower() == ".dll")
                .FirstOrDefault(f => f.Item2.Name == "WhatMapIsItAnyway_Referenced.dll")?.Item2;

            if (asmPath == null) {
                return;
            }

            var stream = new MemoryStream();
            AssemblyDefinition.ReadAssembly(asmPath.FullName).Write(stream);

            var loadedAsm = AppDomain.CurrentDomain.Load(stream.ToArray());
            content.assemblies.loadedAssemblies.Add(loadedAsm);
        }
    }
}

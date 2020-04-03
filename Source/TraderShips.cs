using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;
using Verse;

namespace TraderShips
{
    public class TraderShips : Mod
    {
        public static TraderShipsSettings settings;

        public TraderShips(ModContentPack pack) :base(pack)
        {
            var harmony = new Harmony("com.github.automatic1111.traderships");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
			
            settings = GetSettings<TraderShipsSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            settings.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "TraderShipsTitle".Translate();
        }
    }
}

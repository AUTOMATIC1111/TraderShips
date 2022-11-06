using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TraderShips
{
    public class TraderShipsSettings : ModSettings
    {
        public float lootPercent = 10f;
        public float traderEventChance = 1f;
        public float traderCrashEventChance = 1f;
        public bool disableOrbital = true;
        public bool enableShipComponentDrops = true;
        public bool colors = true;
        public bool requireCommsConsole = true;
        public bool enableQuests = true;
        public IntRange shipColorSaturation = new IntRange(-50, 100);
        public IntRange shipColorValue = new IntRange(50, 100);

        float[] colorHues = new float[100];
        float[] colorSaturations = new float[100];
        float[] colorValues = new float[100];

        void RedoColors()
        {
            for (int i = 0; i < colorHues.Length; i++) colorHues[i] = Rand.Value;
            for (int i = 0; i < colorSaturations.Length; i++) colorSaturations[i] = Rand.Value;
            for (int i = 0; i < colorValues.Length; i++) colorValues[i] = Rand.Value;
        }

        override public void ExposeData()
        {
            Scribe_Values.Look(ref lootPercent, "lootPercent");
            Scribe_Values.Look(ref traderEventChance, "traderChance");
            Scribe_Values.Look(ref traderCrashEventChance, "traderCrashChance");
            Scribe_Values.Look(ref disableOrbital, "disableOrbital");
            Scribe_Values.Look(ref enableShipComponentDrops, "enableShipComponentDrops");
            Scribe_Values.Look(ref colors, "colors");
            Scribe_Values.Look(ref requireCommsConsole, "requireCommsConsole");
            Scribe_Values.Look(ref enableQuests, "enableQuests");
            Scribe_Values.Look(ref shipColorSaturation, "shipColorSaturation");
            Scribe_Values.Look(ref shipColorValue, "shipColorValue");
        }
        
        public void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.Begin(inRect);
            listing_Standard.CheckboxLabeled("TraderShipsDisableOrbitalName".Translate(), ref disableOrbital, "TraderShipsDisableOrbitalDesc".Translate());
            listing_Standard.CheckboxLabeled("TraderShipsRequireCommssConsoleName".Translate(), ref requireCommsConsole, "TraderShipsRequireCommssConsoleDesc".Translate());
            listing_Standard.CheckboxLabeled("TraderShipsEnableQuestsName".Translate(), ref enableQuests, "TraderShipsEnableQuestsDesc".Translate());
            listing_Standard.GapLine();
            TextAnchor anchor = Text.Anchor;
            Text.Anchor = anchor;
            Text.Anchor = TextAnchor.MiddleLeft;

            // I'm sorry for anyone who has to look at these new sections here, I just started modding, it's my first time doing UI things, so forgive me... -BlackFox
            //trader chance multiplier
            listing_Standard.Label("TraderShipsChance".Translate(), -1, "TraderShipsChanceDesc".Translate());
            traderEventChance =Widgets.HorizontalSlider(listing_Standard.GetRect(Text.LineHeight), traderEventChance, 0f, 2.0f, middleAlignment: false, null, null, null, 0.1f);
            Text.Anchor = TextAnchor.MiddleRight;

            Widgets.Label(listing_Standard.GetRect(Text.LineHeight), string.Format("{0}x", traderEventChance.ToString("0.00")));
            Text.Anchor = anchor;
            listing_Standard.GapLine();
            Text.Anchor = TextAnchor.MiddleLeft;
            //crash chance multiplier
            listing_Standard.Label("TraderShipsCrashChance".Translate(), -1, "TraderShipsCrashChanceDesc".Translate());
            traderCrashEventChance = Widgets.HorizontalSlider(listing_Standard.GetRect(Text.LineHeight), traderCrashEventChance, 0f, 2.0f, middleAlignment: false, null, null, null, 0.1f);
            Text.Anchor = TextAnchor.MiddleRight;
            Widgets.Label(listing_Standard.GetRect(Text.LineHeight), string.Format("{0}x", traderCrashEventChance.ToString("0.00")));
            Text.Anchor = anchor;

            //crash loot amount
            Text.Anchor = TextAnchor.MiddleLeft;
            listing_Standard.Label("TraderShipsCrashLoot".Translate(), -1, "TraderShipsCrashLootDesc".Translate());
            lootPercent = Widgets.HorizontalSlider(listing_Standard.GetRect(Text.LineHeight), lootPercent, 0f, 100f, middleAlignment: false, null, null, null, 1f);
            Text.Anchor = TextAnchor.MiddleRight;
            Widgets.Label(listing_Standard.GetRect(Text.LineHeight), string.Format("{0}%", lootPercent.ToString("0")));
            Text.Anchor = anchor;

            //crash part drop
            listing_Standard.CheckboxLabeled("TraderShipsCompDrop".Translate(), ref enableShipComponentDrops, "TraderShipsCompDropDesc".Translate());

            listing_Standard.GapLine();
            listing_Standard.CheckboxLabeled("TraderShipsColorsName".Translate(), ref colors, "TraderShipsColorsDesc".Translate());
            if (colors)
            {

                listing_Standard.Label("TraderShipsSaturationName".Translate(), -1, "TraderShipsSaturationDesc".Translate());
                listing_Standard.IntRange(ref shipColorSaturation, -100, 200);
                listing_Standard.Label("TraderShipsValueName".Translate(), -1, "TraderShipsValueDesc".Translate());
                listing_Standard.IntRange(ref shipColorValue, -100, 200);

                if (colorValues[0] == 0) RedoColors();

                listing_Standard.Gap(24);

                Rect row = listing_Standard.GetRect(64).TopHalf();

                for (int i = 0; i < colorSaturations.Length; i++)
                {
                    Rect rect = new Rect(row.x + i * (row.height + 11), row.y, row.height, row.height);
                    if (rect.x + rect.width >= row.width) break;

                    GUI.color = Color.white;
                    GUI.DrawTexture(rect, BaseContent.WhiteTex);
                    GUI.color = Color.HSVToRGB(colorHues[i], Mathf.Clamp(shipColorSaturation.Lerped(colorSaturations[i]) * 0.01f, 0f, 1f), Mathf.Clamp(shipColorValue.Lerped(colorValues[i]) * 0.01f, 0f, 1f));
                    GUI.DrawTexture(rect.ContractedBy(1), BaseContent.WhiteTex);
                }
                GUI.color = Color.white;

                Rect rowButton = listing_Standard.GetRect(48);
                rowButton.width = 180;
                if (Widgets.ButtonText(rowButton, "TraderShipsRerollColors".Translate()))
                {
                    RedoColors();
                }
            }

            listing_Standard.End();
        }

    }
}

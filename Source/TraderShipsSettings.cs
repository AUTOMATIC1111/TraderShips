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
        public bool disableOrbital = true;

        override public void ExposeData()
        {
            Scribe_Values.Look(ref disableOrbital, "disableOrbital");
        }

        public void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.Begin(inRect);
            listing_Standard.CheckboxLabeled("TradeShipsDisableOrbitalName".Translate(), ref disableOrbital, "TradeShipsDisableOrbitalDesc".Translate());
            listing_Standard.End();
        }
    }
}

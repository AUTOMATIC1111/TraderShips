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
    public class Designator_AreaLandingZoneExpand : Designator_AreaLandingZone
    {
        public Designator_AreaLandingZoneExpand() : base(DesignateMode.Add)
        {
            defaultLabel = "TraderShipsLandingZoneExpandName".Translate();
            defaultDesc = "TraderShipsLandingZoneExpandDesc".Translate();
            icon = ContentFinder<Texture2D>.Get("TraderShips/LandZoneAreaOn", true);
            soundDragSustain = SoundDefOf.Designate_DragAreaAdd;
            soundDragChanged = null;
            soundSucceeded = SoundDefOf.Designate_ZoneAdd;
        }
    }
}

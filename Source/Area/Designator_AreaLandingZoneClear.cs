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
    public class Designator_AreaLandingZoneClear : Designator_AreaLandingZone
    {
        public Designator_AreaLandingZoneClear() : base(DesignateMode.Remove)
        {
            defaultLabel = "TraderShipsLandingZoneClearName".Translate();
            defaultDesc = "TraderShipsLandingZoneClearDesc".Translate();
            icon = ContentFinder<Texture2D>.Get("TraderShips/LandZoneAreaOff", true);
            soundDragSustain = SoundDefOf.Designate_DragAreaDelete;
            soundDragChanged = null;
            soundSucceeded = SoundDefOf.Designate_ZoneDelete;
        }
    }
}

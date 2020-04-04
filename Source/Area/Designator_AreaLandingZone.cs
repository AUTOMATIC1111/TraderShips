using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace TraderShips
{
    public static class AreaManagerLandingZone
    {
        public static Area_LandingZone LandingZone(this AreaManager areaManager)
        {
            Area_LandingZone res = areaManager.Get<Area_LandingZone>();
            if (res == null)
            {
                areaManager.AllAreas.Add(res = new Area_LandingZone(areaManager));
            }

            return res;
        }
    }

    public abstract class Designator_AreaLandingZone : Designator_Area
    {
        public override int DraggableDimensions => 2;
        public override bool DragDrawMeasurements => true;

        public Designator_AreaLandingZone(DesignateMode m)
        {
            mode = m;
            soundDragSustain = SoundDefOf.Designate_DragStandard;
            soundDragChanged = SoundDefOf.Designate_DragStandard_Changed;
            useMouseIcon = true;
        }

        public override AcceptanceReport CanDesignateCell(IntVec3 c)
        {
            if (c.InNoZoneEdgeArea(Map))
            {
                return false;
            }

            return Map.areaManager.LandingZone()[c] != (mode == DesignateMode.Add);
        }

        public override void DesignateSingleCell(IntVec3 c)
        {
            Map.areaManager.LandingZone()[c] = mode == DesignateMode.Add;
        }

        public override void SelectedUpdate()
        {
            GenUI.RenderMouseoverBracket();
            Map.areaManager.LandingZone().MarkForDraw();
        }

        private DesignateMode mode;
    }
}

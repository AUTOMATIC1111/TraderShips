using HarmonyLib;
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
    class SkyfallerTradeShip : Skyfaller
    {
        CompShip Ship => innerContainer.Any ? innerContainer[0].TryGetComp<CompShip>() : null;

        private Material cachedShadowMaterial;
        private Material ShadowMaterial
        {
            get
            {
                if (cachedShadowMaterial == null && !def.skyfaller.shadow.NullOrEmpty())
                {
                    cachedShadowMaterial = MaterialPool.MatFrom(this.def.skyfaller.shadow, ShaderDatabase.Transparent);
                }
                return cachedShadowMaterial;
            }
        }

        protected override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            float pos = Traverse.Create(this).Property<float>("TimeInAnimation").Value;

            CompShip comp = Ship;
            if (comp == null) return;

            float num = 0f;
            if (def.skyfaller.rotateGraphicTowardsDirection)
            {
                num = angle;
            }
            if (def.skyfaller.angleCurve != null)
            {
                angle = def.skyfaller.angleCurve.Evaluate(pos);
            }
            if (def.skyfaller.rotationCurve != null)
            {
                num += def.skyfaller.rotationCurve.Evaluate(pos);
            }
            if (def.skyfaller.xPositionCurve != null)
            {
                drawLoc.x += def.skyfaller.xPositionCurve.Evaluate(pos);
            }
            if (def.skyfaller.zPositionCurve != null)
            {
                drawLoc.z += def.skyfaller.zPositionCurve.Evaluate(pos);
            }

            comp.Sprite.Draw(drawLoc, num);

            if (ShadowMaterial != null)
            {
                drawLoc.z = GenThing.TrueCenter(this).z;

                DrawDropSpotShadow(drawLoc, Rotation, ShadowMaterial, def.skyfaller.shadowSize, ticksToImpact);
            }
            
        }

        protected override void Impact()
        {
            IntVec3 position = Position;
            Map map = Map;
            CompShip comp = Ship;

            base.Impact();

            if (map == null || comp == null) return;
            if (comp.mustCrash) comp.Crash();

        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TraderShips
{
    public class ShipSpritePart : IExposable
    {
        public ShipSpritePartDef def;
        public Vector2 offset;
        public float angle;
        public float distance;
        public int layer;
        public Graphic graphic;

        public void ExposeData()
        {
            Scribe_Defs.Look(ref def, "def");
            Scribe_Values.Look(ref offset, "offset");
            Scribe_Values.Look(ref angle, "angle");
            Scribe_Values.Look(ref distance, "distance");
            Scribe_Values.Look(ref layer, "layer");
        }
    }
}

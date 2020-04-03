using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TraderShips
{
    public class StepShipgen : ShipgenStep
    {
        public ShipgenDef shipgen;
        public Vector2 offset;
        public FloatRange offsetVarianceX;
        public FloatRange offsetVarianceY;
        public FloatRange distance;
        public float chance = 1f;

        public override void Apply(ShipgenState state, ShipSprite sprite)
        {
            if (Rand.Value > chance) return;

            Vector2 storedOffset = state.offset;
            float storedDistance = state.distance;
            state.offset += offset + new Vector2(offsetVarianceX.RandomInRange, offsetVarianceY.RandomInRange);
            state.distance += distance.RandomInRange;
            shipgen.Apply(state, sprite);
            state.offset = storedOffset;
            state.distance = storedDistance;
        }
    }
}

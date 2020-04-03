using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace TraderShips
{
    public class CompPropertiesShip :CompProperties
    {
        public ShipgenDef shipgen;

        public SoundDef soundThud;
        public ThingDef landAnimation;
        public ThingDef takeoffAnimation;
        public int crashScatterRadius = 6;
        public ThingDef crashScatterChunk;
        public IntRange crashScatterChunkCount;
        public IntRange crashPilotCount;
        public PawnKindDef crashPilotKind;

        public CompPropertiesShip() {
            compClass = typeof(CompShip);
        }
    }
}

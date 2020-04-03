using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace TraderShips
{
    class IncidentWorkerTraderShipCrash : IncidentWorker
    {
        public static IntVec3 RandomSpot(Map map)
        {
            return CellFinderLoose.RandomCellWith((IntVec3 c) => c.Standable(map) && !c.CloseToEdge(map, 12) && !c.Fogged(map), map, 1000);
        }


        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map) parms.target;
            Thing ship = ThingMaker.MakeThing(Globals.TraderShipsShip, null);
            IntVec3 spot = RandomSpot(map);

            CompShip comp = ship.TryGetComp<CompShip>();
            comp.GenerateInternalTradeShip(map);
            comp.mustCrash = true;
            GenPlace.TryPlaceThing(SkyfallerMaker.MakeSkyfaller(Globals.TraderShipsShipCrashing, ship), spot, map, ThingPlaceMode.Near);
            return true;
        }

    }
}

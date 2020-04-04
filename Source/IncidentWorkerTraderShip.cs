using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace TraderShips
{
    class IncidentWorkerTraderShip : IncidentWorker
    {

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            return !TraderShips.settings.disableOrbital;
        }

        public static bool FindAnyLandingSpot(out IntVec3 spot, Faction faction, Map map, IntVec2? size)
        {
            if (!DropCellFinder.FindSafeLandingSpot(out spot, faction, map, 0, 15, 25, size))
            {
                IntVec3 intVec = DropCellFinder.RandomDropSpot(map);
                if (!DropCellFinder.TryFindDropSpotNear(intVec, map, out spot, false, false, false, size))
                {
                    spot = intVec;
                }
            }

            return true;
        }

        public static void FindCloseLandingSpot(out IntVec3 spot, Faction faction, Map map, IntVec2? size)
        {
            IntVec3 center = default;
            int count = 0;
            foreach (Building building in map.listerBuildings.allBuildingsColonist.Where(x => x.def.size.x > 1 || x.def.size.z > 1))
            {
                center += building.Position;
                count++;
            }

            if (count == 0)
            {
                FindAnyLandingSpot(out spot, faction, map, size);
                return;
            }

            center.x /= count;
            center.z /= count;

            int spotCount = 20;
            float bestDistance = 999999f;

            spot = default;

            for (int i = 0; i < spotCount; i++)
            {
                IntVec3 s;
                FindAnyLandingSpot(out s, faction, map, size);

                if ((s - center).LengthManhattan < bestDistance)
                {
                    bestDistance = (s - center).LengthManhattan;
                    spot = s;
                }
            }
        }

        public bool TryExecuteWorkerPub(IncidentParms parms)
        {
            return TryExecuteWorker(parms);
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            Thing blockingThing;
            IntVec3 center = IntVec3.Invalid;
            Thing ship = ThingMaker.MakeThing(Globals.TraderShipsShip, null);
            TraderKindDef traderKindDef = (from x in DefDatabase<TraderKindDef>.AllDefs where CanSpawn(map, x) select x).RandomElementByWeightWithFallback((TraderKindDef traderDef) => traderDef.CalculatedCommonality);
            if (traderKindDef == null) throw new InvalidOperationException();

            CompShip comp = ship.TryGetComp<CompShip>();
            comp.GenerateInternalTradeShip(map, traderKindDef);

            Area_LandingZone lz = map.areaManager.LandingZone();
            if (lz != null && !lz.TryFindShipLandingArea(ship.def.size, out center, out blockingThing) && blockingThing != null)
            {
                Messages.Message("TraderShipsLandingZoneBlocked".Translate("TraderShipsBlockedBy".Translate(blockingThing)), blockingThing, MessageTypeDefOf.NeutralEvent, true);
            }

            if (!center.IsValid && ThingDefOf.ShipLandingBeacon != null && !DropCellFinder.TryFindShipLandingArea(map, out center, out blockingThing) && blockingThing != null)
            {
                Messages.Message("TraderShipsLandingZoneBlocked".Translate("TraderShipsBlockedBy".Translate(blockingThing)), blockingThing, MessageTypeDefOf.NeutralEvent, true);
            }

            if (!center.IsValid)
            {
                FindCloseLandingSpot(out center, comp.tradeShip.Faction, map, ship.def.size);
            }

            GenPlace.TryPlaceThing(SkyfallerMaker.MakeSkyfaller(comp.Props.landAnimation, ship), center, map, ThingPlaceMode.Near);
            return true;
        }

        protected Faction GetFaction(TraderKindDef trader)
        {
            if (trader.faction == null) return null;
            return (from f in Find.FactionManager.AllFactions where f.def == trader.faction select f).RandomElementWithFallback();
        }

        protected bool CanSpawn(Map map, TraderKindDef trader)
        {
            if (!trader.orbital)
            {
                return false;
            }
            if (trader.faction == null)
            {
                return true;
            }
            Faction faction = GetFaction(trader);
            if (faction == null)
            {
                return false;
            }

            foreach (Pawn pawn in map.mapPawns.FreeColonists)
            {
                if (pawn.CanTradeWith(faction, trader))
                {
                    return true;
                }
            }

            return false;
        }
    }
}

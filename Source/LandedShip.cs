using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace TraderShips
{
    public class LandedShip : TradeShip, ITrader
    {
        Map map;

        public LandedShip()
        { 
        
        }
        public LandedShip(Map map, TraderKindDef def, Faction faction = null) : base(def, faction)
        {
            this.map = map;
            passingShipManager = map.passingShipManager;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_References.Look(ref map, "map");
        }

        public override void PassingShipTick()
        {
            base.PassingShipTick();

            if (passingShipManager == null)
            {
                passingShipManager = map.passingShipManager;
            }
        }

        public new void GiveSoldThingToPlayer(Thing toGive, int countToGive, Pawn playerNegotiator)
        {
            Thing thing = toGive.SplitOff(countToGive);
            thing.PreTraded(TradeAction.PlayerBuys, playerNegotiator, this);

            if (! GenPlace.TryPlaceThing(thing, playerNegotiator.Position, Map, ThingPlaceMode.Near))
            {
                Log.Error(string.Concat(new object[]
                {
                        "Could not place bought thing ",
                        thing,
                        " at ",
                        playerNegotiator.Position
                }), false);
                thing.Destroy(DestroyMode.Vanish);
            }
        }


        public new IEnumerable<Thing> ColonyThingsWillingToBuy(Pawn playerNegotiator) {
            IEnumerable<Thing> enumerable = from x in Map.listerThings.AllThings
                                            where x.def.category == ThingCategory.Item && TradeUtility.PlayerSellableNow(x, this) && !x.Position.Fogged(x.Map) && (Map.areaManager.Home[x.Position] || x.IsInAnyStorage()) && ReachableForTrade(playerNegotiator, x)
                                            select x;
            foreach (Thing thing in enumerable)

            {
                yield return thing;
            }

            foreach (Pawn pawn in from x in TradeUtility.AllSellableColonyPawns(Map) where !x.Downed && ReachableForTrade(playerNegotiator, x) select x)
            {
                yield return pawn;
            }
            yield break;
        }

        private bool ReachableForTrade(Pawn seller, Thing thing)
        {
            return Map == thing.Map && Map.reachability.CanReach(seller.Position, thing, PathEndMode.Touch, TraverseMode.PassDoors, Danger.Some);
        }

    }
}
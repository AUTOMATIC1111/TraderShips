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

        public override void Depart()
        {

        }

        public new void GiveSoldThingToPlayer(Thing toGive, int countToGive, Pawn playerNegotiator)
        {
            Thing thing = toGive.SplitOff(countToGive);
            thing.PreTraded(TradeAction.PlayerBuys, playerNegotiator, this);

            if (!GenPlace.TryPlaceThing(thing, playerNegotiator.Position, Map, ThingPlaceMode.Near))
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


        public new IEnumerable<Thing> ColonyThingsWillingToBuy(Pawn playerNegotiator)
        {
            // technically, this is pawn selling things to itself, but the code doesn't seem to care, and
            // using this method instead of just listing all things allows for compatibility with storage
            // mods that patch Pawn_TraderTracker.ColonyThingsWillingToBuy.
            foreach (Thing thing in new Pawn_TraderTracker(playerNegotiator).ColonyThingsWillingToBuy(playerNegotiator))
            {
                yield return thing;
            }

            yield break;
        }
    }
}
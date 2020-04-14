using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace TraderShips.Quest
{
    class QuestPart_ShipsTradeRequest : QuestPart
    {

        public override void Notify_QuestSignalReceived(Signal signal)
        {
            base.Notify_QuestSignalReceived(signal);
            if (signal.tag != inSignal) return;

            CompShip comp = requester.TryGetComp<CompShip>();
            if (comp != null) comp.tradeRequest = this;

            IncidentWorkerTraderShip.LandShip(map, requester);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref inSignal, "inSignal", null, false);
            Scribe_References.Look(ref requester, "requester", false);
            Scribe_References.Look(ref map, "map", false);
            Scribe_Defs.Look(ref requestedThingDef, "requestedThingDef");
            Scribe_Values.Look(ref requestedCount, "requestedCount", 0, false);
        }

        public override void AssignDebugData()
        {
            base.AssignDebugData();
            inSignal = "DebugSignal" + Rand.Int;
            requester = IncidentWorkerTraderShip.MakeTraderShip(Find.CurrentMap);
            requestedThingDef = ThingDefOf.Silver;
            requestedCount = 100;
        }

        public override void Cleanup()
        {
            base.Cleanup();

            CompShip comp = requester?.TryGetComp<CompShip>();
            if (comp != null) comp.tradeRequest = null;
        }

        public static bool PlayerCanGive(Thing thing)
        {
            if (thing.GetRotStage() != RotStage.Fresh) return false;

            if (thing.IsForbidden(Faction.OfPlayer)) return false;

            Apparel apparel = thing as Apparel;
            if (apparel != null && apparel.WornByCorpse) return false;

            CompQuality compQuality = thing.TryGetComp<CompQuality>();
            return compQuality == null || compQuality.Quality >= QualityCategory.Normal;
        }

        public bool CanFulfillRequest(LandedShip tradeShip)
        {
            Pawn pawn = tradeShip.Map.PlayerPawnsForStoryteller.FirstOrDefault();
            if (pawn == null) return false;

            int foundCount = 0;
            foreach (Thing thing in tradeShip.ColonyThingsWillingToBuy(pawn).Where(x => x.def == requestedThingDef && PlayerCanGive(x)))
            {
                foundCount += thing.stackCount;
            }

            return foundCount >= requestedCount;
        }

        public void FulfillRequest(LandedShip tradeShip)
        {
            Pawn pawn = tradeShip.Map.PlayerPawnsForStoryteller.FirstOrDefault();
            if (pawn == null) return;

            int toSend = requestedCount;
            foreach (Thing thing in tradeShip.ColonyThingsWillingToBuy(pawn).Where(x => x.def == requestedThingDef && PlayerCanGive(x)).OrderBy(x => x.MarketValue).ToList())
            {
                int count = Math.Min(thing.stackCount, toSend);
                tradeShip.GiveSoldThingToTrader(thing, count, pawn);

                toSend -= count;
                if (toSend <= 0) break;
            }
        }

        public string inSignal;
        public Thing requester;
        public Map map;
        public ThingDef requestedThingDef;
        public int requestedCount;

    }
}

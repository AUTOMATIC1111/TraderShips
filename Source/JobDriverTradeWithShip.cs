using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace TraderShips
{
    class JobDriverTradeWithShip : JobDriver
    {
        private ThingWithComps Trader
        {
            get
            {
                return TargetThingA as ThingWithComps;
            }
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(Trader, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            CompShip comp = Trader.TryGetComp<CompShip>();

            this.FailOnDespawnedOrNull(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell).FailOn(() => comp == null || !comp.tradeShip.CanTradeNow);
            Toil trade = new Toil();
            trade.initAction = delegate ()
            {
                Pawn actor = trade.actor;
                if (comp.tradeShip.CanTradeNow)
                {
                    Find.WindowStack.Add(new Dialog_Trade(actor, comp.tradeShip, false));
                }
            };
            yield return trade;
            yield break;
        }
    }
}

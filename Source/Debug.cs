using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace TraderShips
{
    public static class Debug
    {
        // Token: 0x06001895 RID: 6293 RVA: 0x0008DD70 File Offset: 0x0008BF70
        [DebugAction("Spawning", "Spawn trade ship", allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void SpawnTraderShip()
        {

            List<DebugMenuOption> list = new List<DebugMenuOption>();
            list.Add(new DebugMenuOption("Incoming", DebugMenuOptionMode.Tool, delegate ()
            {
                Thing thing = ThingMaker.MakeThing(Globals.TraderShipsShip, null);
                CompShip comp = thing.TryGetComp<CompShip>();
                comp.GenerateInternalTradeShip(Find.CurrentMap);

                GenPlace.TryPlaceThing(SkyfallerMaker.MakeSkyfaller(comp.Props.landAnimation, thing), UI.MouseCell(), Find.CurrentMap, ThingPlaceMode.Near, null, null, default(Rot4));
            }));

            list.Add(new DebugMenuOption("Stationary", DebugMenuOptionMode.Tool, delegate ()
            {
                Thing thing = ThingMaker.MakeThing(Globals.TraderShipsShip, null);
                thing.TryGetComp<CompShip>().GenerateInternalTradeShip(Find.CurrentMap);

                GenPlace.TryPlaceThing(thing, UI.MouseCell(), Find.CurrentMap, ThingPlaceMode.Near, null, null, default(Rot4));
            }));


            list.Add(new DebugMenuOption("Crash", DebugMenuOptionMode.Tool, delegate ()
            {
                Thing ship = ThingMaker.MakeThing(Globals.TraderShipsShip, null);
                CompShip comp = ship.TryGetComp<CompShip>();
                comp.GenerateInternalTradeShip(Find.CurrentMap);
                comp.mustCrash = true;
                GenPlace.TryPlaceThing(SkyfallerMaker.MakeSkyfaller(Globals.TraderShipsShipCrashing, ship), UI.MouseCell(), Find.CurrentMap, ThingPlaceMode.Near);
            }));

            List<DebugMenuOption> options = list;
            Find.WindowStack.Add(new Dialog_DebugOptionListLister(options));
        }
    }
}

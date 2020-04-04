using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraderShips.Patch
{
    [HarmonyPatch(typeof(IncidentWorker_OrbitalTraderArrival), "CanFireNowSub")]
    class PatchIncidentWorker_OrbitalTraderArrivalCanFireNowSub
    {
        public static bool Prefix(ref bool __result)
        {
            if (TraderShips.settings.disableOrbital)
            {
                __result = true;
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(IncidentWorker_OrbitalTraderArrival), "TryExecuteWorker")]
    class PatchIncidentWorker_OrbitalTraderArrivalTryExecuteWorker
    {
        public static bool Prefix(ref bool __result, IncidentParms parms)
        {
            if (TraderShips.settings.disableOrbital)
            {
                __result = (Globals.TraderShipsArrival.Worker as IncidentWorkerTraderShip).TryExecuteWorkerPub(parms);
                return false;
            }

            return true;
        }
    }
}

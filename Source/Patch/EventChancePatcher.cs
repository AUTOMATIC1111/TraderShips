using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;

namespace TraderShips.Patch
{
	[HarmonyPatch(typeof(IncidentWorker), "BaseChanceThisGame", MethodType.Getter)]
	internal class PatchIncidentWorkerBaseChanceThisGame
	{
		private static void Postfix(ref float __result, IncidentWorker __instance)
		{
			if (__instance.def.ToStringSafe()=="OrbitalTraderArrival" || __instance.def.ToStringSafe() == "TraderShipsArrival")
			{
				if (TraderShips.settings.disableOrbital == true)
				{
					__result *= TraderShips.settings.traderEventChance;
				}
                else
                {
					__result = (__result/2) * TraderShips.settings.traderEventChance;
				}
			}
			if (__instance.def.ToStringSafe() == "TraderShipsCrash")
            {
				__result *= TraderShips.settings.traderCrashEventChance;
            }
		}
	}
}

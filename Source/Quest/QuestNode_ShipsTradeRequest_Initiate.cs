using RimWorld;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TraderShips.Quest
{
    class QuestNode_ShipsTradeRequest_Initiate : QuestNode
    {
        protected override bool TestRunInt(Slate slate)
        {
            return
                slate.Get<Map>("map") != null &&
                requestedThingCount.GetValue(slate) > 0 &&
                requestedThingDef.GetValue(slate) != null;
        }

        protected override void RunInt()
        {
            Slate slate = QuestGen.slate;
            QuestPart_ShipsTradeRequest part = new QuestPart_ShipsTradeRequest();
            part.map = slate.Get<Map>("map");
            part.requester = requester.GetValue(slate);
            if (part.requester == null) part.requester = IncidentWorkerTraderShip.MakeTraderShip(part.map);
            part.requestedThingDef = requestedThingDef.GetValue(slate);
            part.requestedCount = requestedThingCount.GetValue(slate);
            part.inSignal = slate.Get<string>("inSignal");
            QuestGen.quest.AddPart(part);
        }

        public SlateRef<Thing> requester;
        public SlateRef<ThingDef> requestedThingDef;
        public SlateRef<int> requestedThingCount;
    }
}

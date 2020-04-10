using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace TraderShips.Quest
{
    class QuestNode_CreateTraderShip : QuestNode
    {
        protected override bool TestRunInt(Slate slate)
        {

            if (!TraderShips.settings.enableQuests) return false;

            if (slate.Get<Map>("map") == null) return false;

            return true;
        }

        protected override void RunInt()
        {
            Slate slate = QuestGen.slate;
            Map map = QuestGen.slate.Get<Map>("map");

            Thing ship = IncidentWorkerTraderShip.MakeTraderShip(map);
            QuestGen.slate.Set(storeAs.GetValue(slate), ship, false);
            QuestGen.slate.Set(storeAs.GetValue(slate) + "_label", ship.LabelCap, false);
        }

        [NoTranslate]
        public SlateRef<string> storeAs;
    }
}

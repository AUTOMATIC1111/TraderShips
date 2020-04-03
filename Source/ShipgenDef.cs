using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace TraderShips
{
    public class ShipgenDef : Def
    {
        public List<ShipgenStep> steps;

        public ShipSprite CreateSprite()
        {
            ShipgenState state = new ShipgenState();
            ShipSprite res = new ShipSprite();

            Apply(state, res);

            res.FinishCreating();

            return res;
        }

        public void Apply(ShipgenState state, ShipSprite sprite)
        {
            foreach (ShipgenStep step in steps)
            {
                step.Apply(state, sprite);
            }
        }
    }
}

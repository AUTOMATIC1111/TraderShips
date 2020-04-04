using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TraderShips
{
    public class Area_LandingZone : Area
    {
        public override string Label
        {
            get
            {
                return "SnowClear".Translate();
            }
        }

        public override Color Color
        {
            get
            {
                return new ColorInt(204, 202, 62).ToColor;
            }
        }

        public override int ListPriority
        {
            get
            {
                return 5000;
            }
        }

        public Area_LandingZone()
        {
        }

        public Area_LandingZone(AreaManager areaManager) : base(areaManager)
        {
        }

        public override string GetUniqueLoadID()
        {
            return "Area_" + ID + "_LandingZone";
        }

        public IEnumerable<IntVec3> LandingLocations()
        {
            var indices = Map.cellIndices;
            List<IntVec3> res = new List<IntVec3>();

            HashSet<int> visited = new HashSet<int>();
            List<int> toVisit = new List<int>();

            foreach (IntVec3 cell in ActiveCells)
            {
                int index = indices.CellToIndex(cell);
                if (visited.Contains(index)) continue;

                toVisit.Add(index);

                int sx = 0;
                int sz = 0;
                int count = 0;

                while (toVisit.Any())
                {
                    index = toVisit[toVisit.Count - 1];
                    toVisit.RemoveAt(toVisit.Count - 1);

                    if (!this[index]) continue;

                    visited.Add(index);
                    IntVec3 vec = indices.IndexToCell(index);
                    int x = vec.x;
                    int z = vec.z;
                    sx += x;
                    sz += z;
                    count++;

                    int subindex;

                    subindex = indices.CellToIndex(x + 1, z);
                    if (!visited.Contains(subindex)) { toVisit.Add(subindex); }

                    subindex = indices.CellToIndex(x - 1, z);
                    if (!visited.Contains(subindex)) { toVisit.Add(subindex); }

                    subindex = indices.CellToIndex(x, z + 1);
                    if (!visited.Contains(subindex)) { toVisit.Add(subindex); }

                    subindex = indices.CellToIndex(x, z - 1);
                    if (!visited.Contains(subindex)) { toVisit.Add(subindex); }
                }

                if (count > 0)
                {
                    yield return new IntVec3(sx / count, 0, sz / count);
                }
            }

            yield break;
        }

        public bool TryFindShipLandingArea(IntVec2 size, out IntVec3 result, out Thing blocker)
        {
            blocker = null;

            foreach (IntVec3 vec in LandingLocations())
            {
                blocker = null;
                foreach (IntVec3 c in new CellRect(vec.x - (size.x + 1) / 2 + 1, vec.z - (size.z + 1) / 2 + 1, size.x, size.z))
                {
                    foreach (Thing thing in c.GetThingList(Map))
                    {
                        if (thing is Pawn || thing.def.Fillage == FillCategory.None) continue;

                        blocker = thing;
                        break;
                    }

                    if (blocker != null) break;
                }

                if (blocker == null)
                {
                    result = vec;
                    return true;
                }
            }

            result = IntVec3.Invalid;
            return false;
        }

    }
}

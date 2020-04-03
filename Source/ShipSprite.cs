using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TraderShips
{
    public class ShipSprite : IExposable
    {
        static public Matrix4x4 drawingMatrix = default(Matrix4x4);

        [TweakValue("Gameplay", -3f, 3f)]
        static float dxMultiplier = -1f;

        [TweakValue("Gameplay", -3f, 3f)]
        static float dzMultiplier = 0.8f;


        public List<ShipSpritePart> parts = new List<ShipSpritePart>();
        public float scale = 0.015f;
        public Vector2 offset;

        public void ExposeData()
        {
            Scribe_Collections.Look(ref parts, "parts");
            Scribe_Values.Look(ref scale, "scale");
            Scribe_Values.Look(ref offset, "offset");
        }

        public void FinishCreating()
        {
            parts.SortBy(x => x.layer);

            float minX = parts.Min(p => p.offset.x + p.distance * dxMultiplier - p.def.graphicData.drawSize.x / 2);
            float maxX = parts.Max(p => p.offset.x - p.distance * dxMultiplier + p.def.graphicData.drawSize.x / 2);

            float width = maxX - minX;
            offset.x = -minX - width/2;

            offset.y = -40;
        }

        public void Draw(Vector3 drawPos, float additionalAngle = 0)
        {
            int i = 0;
            foreach (ShipSpritePart part in parts)
            {
                Graphic graphic = part.def.graphicData.Graphic;
                Vector3 drawingPosition = drawPos;
                drawingPosition.x += (offset.x + part.offset.x + part.distance * dxMultiplier) * scale;
                drawingPosition.y = drawPos.y + 0.1f + part.layer * 0.0001f;
                drawingPosition.z -= (offset.y + part.offset.y + part.distance * dzMultiplier) * scale;

                Vector3 drawingScale = new Vector3(scale * graphic.drawSize.x, 1f, scale * graphic.drawSize.y);
                Quaternion quaternion = Quaternion.LookRotation(Vector3.forward) * Quaternion.Euler(Vector3.up * additionalAngle);
                drawingMatrix.SetTRS(drawingPosition, quaternion, drawingScale);

                Graphics.DrawMesh(MeshPool.plane10, drawingMatrix, graphic.MatSingle, 0);

                if (part.distance != 0)
                {
                    drawingPosition.x += -2 * part.distance * dxMultiplier * scale;
                    drawingPosition.y = drawPos.y + 0.1f - part.layer * 0.0001f;
                    drawingPosition.z -= -2 * part.distance * dzMultiplier * scale;

                    drawingMatrix.SetTRS(drawingPosition, quaternion, drawingScale);
                    Graphics.DrawMesh(MeshPool.plane10, drawingMatrix, graphic.MatSingle, 0);
                }

                i++;
            }
        }
    }
}

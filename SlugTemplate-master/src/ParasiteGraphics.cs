using System;
using System.Collections.Generic;
using UnityEngine;

namespace Symbiosis
{
    public class ParasiteGraphics : GraphicsModule
    {
        private  readonly int Segments = 8;
        private List<Vector2> blobVerts = new List<Vector2>();
        private readonly float Radius = 4.5f;
        private TriangleMesh BlobMesh;

        private Parasite Own;

      

        public ParasiteGraphics(PhysicalObject ow, bool internalContainer) : base(ow, internalContainer)
        {
            this.owner = ow;

            if (ow is Parasite p)
            {
                Own = p;
            }
        }









        public override void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer? newContatiner)
        {
            newContatiner ??= rCam.ReturnFContainer("Items");

            foreach (FSprite fsprite in sLeaser.sprites)
            {
                newContatiner.AddChild(fsprite);
            }

        }



        public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            Color baseColor = palette.skyColor;
            for (int i = 0; i < sLeaser.sprites.Length; i++)
            {
                // sLeaser.sprites[i].color = palette.blackColor;

                sLeaser.sprites[i].color = Color.Lerp(baseColor, Color.black, 0.9f);
            }
        }


        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            Vector2 drawPos = Vector2.Lerp(owner.firstChunk.lastPos, owner.firstChunk.pos, timeStacker) - camPos;
            sLeaser.sprites[0].SetPosition(drawPos);

             

            if (owner.slatedForDeletetion || owner.room != rCam.room)
            {
                sLeaser.CleanSpritesAndRemove();
            }
        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites = new FSprite[1];
            blobVerts.Clear();
            blobVerts.Add(Vector2.zero);

            // Circle verts
            for (int i = 0; i < Segments; i++)
            {
                float angle = Mathf.PI * 2f * i / Segments;
                blobVerts.Add(new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * Radius);
            }

            // Blob triangles
            TriangleMesh.Triangle[] blobTris = new TriangleMesh.Triangle[Segments];
            for (int i = 0; i < Segments; i++)
            {
                int curr = i + 1;
                int next = (i == Segments - 1) ? 1 : curr + 1;
                blobTris[i] = new TriangleMesh.Triangle(0, curr, next);
            }

            BlobMesh = new TriangleMesh("Futile_White", blobTris, false, false);
            for (int i = 0; i < blobVerts.Count; i++)
            {
                BlobMesh.MoveVertice(i, blobVerts[i]);
            }

            sLeaser.sprites[0] = BlobMesh;
            AddToContainer(sLeaser, rCam, null);
        }
    }
}

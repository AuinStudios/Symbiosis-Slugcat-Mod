using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Symbiosis
{
    public class ParasiteGraphics : GraphicsModule
    {
        private readonly int Segments = 16;
        private readonly float Radius = 7.5f;

        private List<Vector2> blobVerts = new List<Vector2>();
       
        private TriangleMesh BlobMeshOriginal;
        private TriangleMesh.Triangle[] blobTris;
        
        private List<TriangleMesh> triangleMeshes = new List<TriangleMesh>();
        private Parasite Own;

        private  Color col;

        public ParasiteGraphics(PhysicalObject ow, bool internalContainer) : base(ow, internalContainer)
        {
            this.owner = ow;
            blobTris = new TriangleMesh.Triangle[Segments];
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
            col = Color.Lerp(baseColor, Color.black, 0.9f);
            for (int i = 0; i < sLeaser.sprites.Length; i++)
            {
                // sLeaser.sprites[i].color = palette.blackColor;

                sLeaser.sprites[i].color = Color.Lerp(baseColor, Color.black, 0.9f);
            }
        }


        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            Vector2 drawPos = Vector2.Lerp(owner.firstChunk.lastPos, owner.firstChunk.pos, timeStacker) - camPos;
            for (int i = 0; i < sLeaser.sprites.Length; i++)
            {
                sLeaser.sprites[i].SetPosition(drawPos);
            }



            if (owner.slatedForDeletetion || owner.room != rCam.room)
            {
                sLeaser.CleanSpritesAndRemove();
            }




            //TriangleMesh mesh = sLeaser.sprites[0] as TriangleMesh;
            if (Own.creaturemodule == null || Own.Interpolate  <= 0)
            {
                if(sLeaser.sprites.Length > 1)
                {
                    triangleMeshes.Clear();
                    sLeaser.RemoveAllSpritesFromContainer();
                    sLeaser.sprites = new FSprite[1];
                    sLeaser.sprites[0] = BlobMeshOriginal;
                    AddToContainer(sLeaser, rCam, null);
                }
                //lerpamount = Mathf.Lerp(lerpamount, 0.0f, timeStacker / 60.0f);
                Debug.Log("creature is null");
                return;
            }

            RoomCamera cam = Own.creaturemodule.room.game.cameras.FirstOrDefault(i => i.room == Own.creaturemodule.room);

            if (cam == null)
            {
                Debug.Log("RoomCamera Didnt get");
                return;
            }
            RoomCamera.SpriteLeaser sprites = cam.spriteLeasers.FirstOrDefault(g => g.drawableObject == Own.creaturemodule.graphicsModule);

            if (sprites == null)
            {
                Debug.Log("Spriteleaser Didnt get");
                return;
            }

            if (sLeaser.sprites.Length != 7)
            {
                sLeaser.RemoveAllSpritesFromContainer();
                sLeaser.sprites = new FSprite[7];
                if(triangleMeshes.Count > 0)
                {
                    triangleMeshes.Clear();
                }
                
                for (int i = 0; i < 7; i++)
                {
                    string element_name = sprites.sprites[i].element.name;
                    
                    TriangleMesh newmesh = new TriangleMesh("Futile_White", blobTris, false, false);
                    newmesh.color = col;
                    //sLeaser.sprites[i] = sprites.sprites[i];

                    triangleMeshes.Add(newmesh);
                    sLeaser.sprites[i] = newmesh;
                }
                AddToContainer(sLeaser, rCam, null);
                
            }
            if(triangleMeshes.Count > 0)
            {
                for (int i = 0; i < triangleMeshes.Count; i++)
                {
                    for (int g = 0; g < blobVerts.Count; g++)
                    {
                        triangleMeshes[i].MoveVertice(g, DeformToChunk(sprites.sprites[i], g , sLeaser.sprites[i], rCam, timeStacker, camPos, Own.Interpolate));
                    }

                }
            }
           

            //lerpamount = Mathf.Lerp(lerpamount, 1.0f, timeStacker / 60.0f);
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

            for (int i = 0; i < Segments; i++)
            {
                int curr = i + 1;
                int next = (i == Segments - 1) ? 1 : curr + 1;
                blobTris[i] = new TriangleMesh.Triangle(0, curr, next);
            }

            BlobMeshOriginal = new TriangleMesh("Futile_White", blobTris, false, false);
            for (int i = 0; i < blobVerts.Count; i++)
            {
                BlobMeshOriginal.MoveVertice(i, blobVerts[i]);
            }
            sLeaser.sprites[0] = BlobMeshOriginal;
            AddToContainer(sLeaser, rCam, null);
        }


        private Vector2 DeformToChunk(FSprite CreatureSprite , int verticeIndex, FSprite ParasiteSprite , RoomCamera rCam, float timeStacker, Vector2 camPos, int lerpamount)
        {
            Rect rect = CreatureSprite.localRect;
            float width = rect.width;
            float height = rect.height;

            float radiusX = width / 2f;
            float radiusY = height / 2f;

            Vector2 direction = (CreatureSprite.GetPosition() - ParasiteSprite.GetPosition()).normalized; //owner.firstChunk.pos).normalized;
            Vector2 blobCenter = Vector2.Lerp(owner.firstChunk.lastPos, owner.firstChunk.pos, timeStacker) - camPos;


            float angle = (Mathf.PI * 2f / Segments) * verticeIndex;

            float x = Mathf.Cos(angle) * radiusX;

            float y = Mathf.Sin(angle) * radiusY;

            Vector2 localPoint = new Vector2(x, y);

            Vector2 worldPoint = CreatureSprite.LocalToGlobal(localPoint);

            float angle2 = Mathf.PI * 2f * verticeIndex / Segments;

            Vector2 dir = new Vector2(Mathf.Cos(angle2), Mathf.Sin(angle2));

            float dot = Vector2.Dot(direction, dir);

            float stretch = Mathf.Max(0f, dot) * 0.9f;

            Vector2 defaultPos = dir * (1.0f + stretch);

            Vector2 worldDefaultPos = blobCenter + defaultPos;

            Vector2 NewWorldPoint = worldPoint - worldDefaultPos;

            Vector2 LerpToWorldPoint = Vector2.Lerp(defaultPos, NewWorldPoint, (float)lerpamount / 20);

            return LerpToWorldPoint;

        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using RWCustom;
namespace Symbiosis
{
    public class Parasite : PlayerCarryableItem
    {
        public int Interpolate { get; private set; } = 0;
        public Creature creaturemodule { get; private set; }
        public Parasite(ParasiteAbstract abstractPhysicalObject) : base(abstractPhysicalObject)
        {
            float mass = 0.2f;
            var positions = new List<Vector2>();
            positions.Add(Vector2.zero);
            
            bodyChunks = new BodyChunk[positions.Count];

            for (int i = 0; i < bodyChunks.Length; i++)
            {
                bodyChunks[i] = new BodyChunk(this, i, Vector2.zero, 4.0f, mass / bodyChunks.Length);
            }

            bodyChunks[0].rad = 4.0f;


            bodyChunkConnections = new BodyChunkConnection[bodyChunks.Length * (bodyChunks.Length - 1) / 2];
            int connection = 0;

            // Create all chunk connections

            for (int x = 0; x < bodyChunks.Length; x++)
            {
                for (int y = x + 1; y < bodyChunks.Length; y++)
                {
                    bodyChunkConnections[connection] = new BodyChunkConnection(bodyChunks[x], bodyChunks[y], Vector2.Distance(positions[x], positions[y]), BodyChunkConnection.Type.Normal, 0.5f, -1f);
                    connection++;
                }
            }


            airFriction = 0.999f;
            gravity = 0.9f;
            bounce = 0.3f;
            surfaceFriction = 0.2f;
            collisionLayer = 1;
            waterFriction = 0.92f;
            buoyancy = 0.75f;
            GoThroughFloors = false;
        }


        public override void InitiateGraphicsModule()
        {
            if(graphicsModule == null)
            {
                graphicsModule = new ParasiteGraphics(this, false);
            }
           
        }

        public override void Update(bool eu)
        {
            base.Update(eu);

            //  if (grabbedBy[0].grabber != null && grabbedBy[0].grabber is Creature c && c is not Player)
            //  {
            //      Interpolate++;
            //      creaturemodule = c;
            //  }
            if (grabbedBy.Count > 0 && grabbedBy[0].grabber is Player p)
            {
                if (p.input[0].pckp)
                {
                    Interpolate++;
                }
                else
                {
                    Interpolate--;
                }
                creaturemodule = p;
            }
            else if(Interpolate > 0 && grabbedBy.Count == 0)
            {
                Interpolate--;
                creaturemodule = null;
            }

            Interpolate = Mathf.Clamp(Interpolate , 0 , 20);
        }


        public override void PlaceInRoom(Room placeRoom)
        {
            base.PlaceInRoom(placeRoom);

            Vector2 center = placeRoom.MiddleOfTile(abstractPhysicalObject.pos);
            bodyChunks[0].HardSetPosition(new Vector2(0, 0) * 20f + center);
        }

        public override void TerrainImpact(int chunk,   IntVector2 direction, float speed, bool firstContact)
        {
            base.TerrainImpact(chunk, direction, speed, firstContact);

            if (speed > 10)
            {
                room.PlaySound(SoundID.Spear_Fragment_Bounce, bodyChunks[chunk].pos, 0.35f, 2f);
            }
        }
    }
}

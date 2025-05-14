using System;
using BepInEx;
using UnityEngine;
using SlugBase.Features;
using static SlugBase.Features.FeatureTypes;
using Fisobs.Core;

namespace Symbiosis
{
    [BepInPlugin(MOD_ID, "Symbiosis", "0.1.0")]
    class Plugin : BaseUnityPlugin
    {
        private const string MOD_ID = "IncoDev.Symbiosis";

        public static readonly PlayerFeature<bool> canspawnsymbiote = PlayerBool("Symbiosis/SpawnSymbiote");
      //  public static readonly PlayerFeature<float> SuperJump = PlayerFloat("slugtemplate/super_jump");
      //  public static readonly PlayerFeature<bool> ExplodeOnDeath = PlayerBool("slugtemplate/explode_on_death");
      //  public static readonly GameFeature<float> MeanLizards = GameFloat("slugtemplate/mean_lizards");


        // Add hooks
        public void OnEnable()
        {
            On.RainWorld.OnModsInit += Extras.WrapInit(LoadResources);
            On.Player.Update += Player_Update;
            // Put your custom hooks here!
            // hello
            #region base code
            // On.Player.Jump += Player_Jump;
            // On.Player.Die += Player_Die;
            // On.Lizard.ctor += Lizard_ctor;
            #endregion
        }

        private void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
        {
            orig(self ,eu);

            if(canspawnsymbiote.TryGet(self , out bool value))
            {
                if (CWTS.TryGetCWT(self, out CWTS.Data data) && !data.spawnonce)
                {

                    if( self.input[0].spec && !self.input[1].spec)
                    {
                        SpawnSymbiote(self);
                        data.spawnonce = true;
                    }
                }
                
            }
        }

        // Load any resources, such as sprites or sounds
        private void LoadResources(RainWorld rainWorld)
        {
            Content.Register(new ParasiteFisObs());
        }


        private void SpawnSymbiote(Player self)
        {
            var tilePos = self.room.GetTilePosition(self.mainBodyChunk.pos);

            var pos = new WorldCoordinate(self.room.abstractRoom.index, tilePos.x, tilePos.y, 0);

            var abstr = new ParasiteAbstract(self.room.world, pos, self.room.game.GetNewID());
            
            self.room.abstractRoom.AddEntity(abstr);
            abstr.RealizeInRoom();
        }
        #region base code
        // Implement MeanLizards
       // private void Lizard_ctor(On.Lizard.orig_ctor orig, Lizard self, AbstractCreature abstractCreature, World world)
       // {
       //     orig(self, abstractCreature, world);
       //
       //     if(MeanLizards.TryGet(world.game, out float meanness))
       //     {
       //         self.spawnDataEvil = Mathf.Min(self.spawnDataEvil, meanness);
       //     }
       // }
       //
       //
       // // Implement SuperJump
       // private void Player_Jump(On.Player.orig_Jump orig, Player self)
       // {
       //     orig(self);
       //
       //     if (SuperJump.TryGet(self, out var power))
       //     {
       //         self.jumpBoost *= 1f + power;
       //     }
       // }
       //
       // // Implement ExlodeOnDeath
       // private void Player_Die(On.Player.orig_Die orig, Player self)
       // {
       //     bool wasDead = self.dead;
       //
       //     orig(self);
       //
       //     if(!wasDead && self.dead
       //         && ExplodeOnDeath.TryGet(self, out bool explode)
       //         && explode)
       //     {
       //         // Adapted from ScavengerBomb.Explode
       //         var room = self.room;
       //         var pos = self.mainBodyChunk.pos;
       //         var color = self.ShortCutColor();
       //         room.AddObject(new Explosion(room, self, pos, 7, 250f, 6.2f, 2f, 280f, 0.25f, self, 0.7f, 160f, 1f));
       //         room.AddObject(new Explosion.ExplosionLight(pos, 280f, 1f, 7, color));
       //         room.AddObject(new Explosion.ExplosionLight(pos, 230f, 1f, 3, new Color(1f, 1f, 1f)));
       //         room.AddObject(new ExplosionSpikes(room, pos, 14, 30f, 9f, 7f, 170f, color));
       //         room.AddObject(new ShockWave(pos, 330f, 0.045f, 5, false));
       //
       //         room.ScreenMovement(pos, default, 1.3f);
       //         room.PlaySound(SoundID.Bomb_Explode, pos);
       //         room.InGameNoise(new Noise.InGameNoise(pos, 9000f, self, 1f));
       //     }
       // }
        #endregion
    }
}
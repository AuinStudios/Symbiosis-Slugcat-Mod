using Fisobs.Core;
using Fisobs.Properties;
using Fisobs.Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symbiosis
{
    sealed class ParasiteFisObs : Fisobs.Items.Fisob
    {

        public static readonly AbstractPhysicalObject.AbstractObjectType Parasite = new AbstractPhysicalObject.AbstractObjectType("Parasite", true);

        public static readonly MultiplayerUnlocks.SandboxUnlockID BlackParasite = new MultiplayerUnlocks.SandboxUnlockID("Parasite", true);


        public ParasiteFisObs() : base(Parasite)
        {
            //  Icon = new SimpleIcon("Pebble", new Color(1f, 0.2f, 0.2f));
            Icon = new DefaultIcon();
            RegisterUnlock(BlackParasite);//, MultiplayerUnlocks.SandboxUnlockID.Lantern, data: 0);

            SandboxPerformanceCost = new(linear: 0.35f, exponential: 0f);

          
        }

        public override AbstractPhysicalObject Parse(World world, EntitySaveData saveData, SandboxUnlock? unlock)
        {
            // Crate data is just floats separated by ; characters.
            string[] p = saveData.CustomData.Split(';');
            
            if (p.Length < 4)
            {
                p = new string[4];
            }

            var result = new ParasiteAbstract(world, saveData.Pos, saveData.ID)
            {
                hue = float.TryParse(p[0], out var h) ? h : 0,
                saturation = float.TryParse(p[1], out var s) ? s : 1,
                scaleX = float.TryParse(p[2], out var x) ? x : 1,
                scaleY = float.TryParse(p[3], out var y) ? y : 1,
            };

            // If this is coming from a sandbox unlock, the hue and size should depend on the data value (see CrateIcon below).
            if (unlock is SandboxUnlock u)
            {
                result.hue = u.Data / 1000f;

                if (u.Data == 0)
                {
                    result.scaleX += 0.2f;
                    result.scaleY += 0.2f;
                }
            }

            return result;
        }

        private static readonly ParasitePropertys properties = new();

        public override ItemProperties Properties(PhysicalObject forObject)
        {
            return properties;
        }
    }
}

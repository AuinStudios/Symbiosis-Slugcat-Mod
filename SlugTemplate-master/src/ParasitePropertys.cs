﻿using Fisobs.Properties;
using System.Linq;

namespace Symbiosis
{
    public class ParasitePropertys : ItemProperties
    {
        public override void Throwable(Player player, ref bool throwable)
           => throwable = true;

        public override void ScavCollectScore(Scavenger scavenger, ref int score)
            => score = 0;

        public override void ScavWeaponPickupScore(Scavenger scav, ref int score)
            => score = 0;

        // Don't throw shields
        public override void ScavWeaponUseScore(Scavenger scav, ref int score)
            => score = 0;

        public override void Grabability(Player player, ref Player.ObjectGrabability grabability)
        {
            // The player can only grab one centishield at a time,
            // but that shouldn't prevent them from grabbing a spear,
            // so don't use Player.ObjectGrabability.BigOneHand

            if (player.grasps.Any(g => g?.grabbed is Parasite))
            {
                grabability = Player.ObjectGrabability.CantGrab;
            }
            else
            {
                grabability = Player.ObjectGrabability.OneHand;
            }
        }
    }
}

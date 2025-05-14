using Fisobs.Core;
namespace Symbiosis
{
    public class ParasiteAbstract : AbstractPhysicalObject
    {
        public float hue;
        public float saturation;
        public float scaleX;
        public float scaleY;
        //  public float damage;
        public ParasiteAbstract(World world, WorldCoordinate pos, EntityID ID) : base(world, ParasiteFisObs.Parasite, null, pos, ID)
        {
            scaleX = 1;
            scaleY = 1;
            saturation = 0.5f;
            hue = 1f;
        }

        public override void Realize()
        {
            base.Realize();
            if (realizedObject == null)
            {
                realizedObject = new Parasite(this);
            }
        }
        public override string ToString()
        {
            return this.SaveToString($"{hue};{saturation};{scaleX};{scaleY}");
        }
    }
}

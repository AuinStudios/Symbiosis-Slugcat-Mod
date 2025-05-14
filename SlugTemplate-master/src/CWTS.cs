using System.Runtime.CompilerServices;

namespace Symbiosis
{
    // make it later on used for the transformation
    public static class CWTS
    {

        public static readonly ConditionalWeakTable<Player, Data> dataCWT = new();

        public static bool TryGetCWT(Player self, out Data data)
        {
            if (self != null)
            {
                data = dataCWT.GetOrCreateValue(self);
            }
            else
            {
                data = null;
            }
            return data != null;
        }


        public class Data
        {
            public bool spawnonce = false;
        }


    }
}

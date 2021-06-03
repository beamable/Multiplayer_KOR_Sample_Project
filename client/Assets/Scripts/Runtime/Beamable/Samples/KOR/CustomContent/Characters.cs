using Beamable.Common.Content;

namespace Beamable.Samples.KOR
{
    [ContentType("characters")]
    public class Characters : ContentObject
    {
        public string Name = "";
        public int MovementSpeed = 0;
        public int ChargeSpeed = 0;
    }
}
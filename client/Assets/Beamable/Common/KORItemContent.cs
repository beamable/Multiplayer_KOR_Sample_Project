using Beamable.Common.Content;
using Beamable.Common.Inventory;

namespace Beamable.Samples.KOR.CustomContent
{
    /// <summary>
    /// Custom Inventory item with game-play affecting
    /// values
    /// </summary>
    [ContentType("KORItem")]
    public class KORItemContent : ItemContent
    {
        //  Fields ---------------------------------------
        // NOTE: If you change these variable NAMES, also change them in CharacterContentObject.cs
        public int MovementSpeed = 0;
        public int ChargeSpeed = 0;

    }
}
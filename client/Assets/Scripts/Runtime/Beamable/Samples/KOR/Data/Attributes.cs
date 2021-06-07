using System;
using Beamable.Samples.KOR.CustomContent;

namespace Beamable.Samples.KOR.Data
{
    /// <summary>
    /// Store data related to: The cumulative Attributes values
    /// which combine values from <see cref="CharacterContentObject"/> and <see cref="KORItemContent"/>
    /// </summary>
    [Serializable]
    public class Attributes
    {
        //  Fields  -----------------------------------
        public int ChargeSpeed { get { return _chargeSpeed; } }
        public int MovementSpeed { get { return _movementSpeed; } }
        
        //  Properties ---------------------------------
        private int _chargeSpeed = 0;
        private int _movementSpeed = 0;
        
        //  Constructor --------------------------------
        public Attributes(int chargeSpeed, int movementSpeed)
        {
            _chargeSpeed = chargeSpeed;
            _movementSpeed = movementSpeed;
        }
    }
}
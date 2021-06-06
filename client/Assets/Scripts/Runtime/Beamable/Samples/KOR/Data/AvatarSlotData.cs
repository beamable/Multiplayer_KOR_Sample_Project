using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beamable.Samples.KOR.Data
{
    [Serializable]
    public class AvatarSlotData
    {
        public Color Color { get { return _color; } }

        [SerializeField]
        private Color _color = Color.blue;
    }
}
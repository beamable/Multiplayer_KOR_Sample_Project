using System;
using Beamable.Samples.KOR.Views;
using UnityEngine;

namespace Beamable.Samples.KOR.Data
{
    /// <summary>
    /// Store data related to: Avatar
    /// </summary>
    [Serializable]
    public class AvatarData
    {
        //  Fields  -----------------------------------

        //public Color Color { get { return _color; } }

        public AvatarView AvatarViewPrefab { get { return _avatarViewPrefab; } }

        //  Properties -----------------------------------

        //[SerializeField]
        //private Color _color = Color.blue;

        [SerializeField]
        private AvatarView _avatarViewPrefab = null;
    }
}
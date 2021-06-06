using Beamable.Common.Content;
using Beamable.Samples.KOR.Views;
using UnityEngine;

namespace Beamable.Samples.KOR.CustomContent
{
    [ContentType("characters")]
    [System.Serializable]
    public class CharacterContentObject : ContentObject
    {
        public string ReadableName = "";
        public UnityEngine.AddressableAssets.AssetReferenceGameObject avatarViewPrefab = null;
        public UnityEngine.AddressableAssets.AssetReferenceTexture2D bigIcon = null;
        public int MovementSpeed = 0;
        public int ChargeSpeed = 0;
    }
}
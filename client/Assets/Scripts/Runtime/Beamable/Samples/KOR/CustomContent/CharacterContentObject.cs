using System.Threading.Tasks;
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

        // NOTE: If you change these variable NAMES, also change them in KORItem.cs
        public int MovementSpeed = 0;

        public int ChargeSpeed = 0;

        public async Task<GameObject> ResolveAvatarViewPrefab()
        {
            var taskHandle = !avatarViewPrefab.OperationHandle.IsValid()
                ? avatarViewPrefab.LoadAssetAsync()
                : avatarViewPrefab.OperationHandle.Convert<GameObject>();
            return await taskHandle.Task;
        }

        public async Task<Texture2D> ResolveBigIcon()
        {
            var taskHandle = !bigIcon.OperationHandle.IsValid()
                ? bigIcon.LoadAssetAsync()
                : bigIcon.OperationHandle.Convert<Texture2D>();
            return await taskHandle.Task;
        }
    }
}
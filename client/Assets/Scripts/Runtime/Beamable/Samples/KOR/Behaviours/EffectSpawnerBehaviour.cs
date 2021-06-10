using Beamable.Samples.KOR.Audio;
using UnityEngine;

namespace Beamable.Samples.KOR.Behaviours
{
    public class EffectSpawnerBehaviour : MonoBehaviour
    {
        public GameObject Prefab;
        public Transform SpawnLocation;
        public float Time = 2;

        public void SpawnEffect()
        {
            SoundManager.Instance.PlayAudioClip(SoundConstants.GameOverWin, SoundManager.GetRandomPitch(1.0f, 0.3f));

            var effect = Instantiate(Prefab);
            effect.transform.position = SpawnLocation.position;
            Destroy(effect, Time);
        }
    }
}
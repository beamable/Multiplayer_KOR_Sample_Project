using Beamable.Samples.KOR.Audio;
using UnityEngine;

namespace Beamable.Samples.KOR.Behaviours
{
    public class HealthBehaviour : MonoBehaviour
    {
        public const int MaxHealth = 100;
        public const int MinHealth = 0;

        public int Health = 100;

        public sfloat HealthRatio => (sfloat)(Health - MinHealth) / (sfloat)(MaxHealth - MinHealth);

        public int HealthPercentage => (int)((float)HealthRatio * (MaxHealth - MinHealth) + .5f);

        public bool IsMin => Health == MinHealth;
        public bool IsMax => Health == MaxHealth;

        public void ChangeHealth(int delta)
        {
            if (delta < 0)
                SoundManager.Instance.PlayAudioClip(SoundConstants.TakeDamage01, SoundManager.GetRandomPitch(1.0f, 0.3f));

            Health += delta;
            if (Health < 0)
            {
                Health = 0;
            }
        }
    }
}
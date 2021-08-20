using System.Collections.Generic;
using System.Text;
using Beamable.Samples.Core.Components;
using Beamable.Samples.KOR.Data;
using UnityEngine;

namespace Beamable.Samples.Core.Audio
{
    /// <summary>
    /// Maintain a list of AudioSources and play the next
    /// AudioClip on the first available AudioSource.
    /// </summary>
    public class SoundManager : SingletonMonobehavior<SoundManager>
    {
        private const float UnsetFloat = -1;
        private const float PitchDefault = 1;

        [SerializeField] private Configuration _configuration = null;

        [SerializeField]
        private List<AudioClip> _audioClips = new List<AudioClip>();

        [SerializeField]
        private List<AudioSource> _audioSources = new List<AudioSource>();

        public static float GetRandomPitch(float baseShift, float shift)
        {
            return baseShift + UnityEngine.Random.Range(-shift, shift);
        }

        protected override void Awake()
        {
            base.Awake();

            // Run Unity once with this uncommented to
            // rebuild the list of constants for the const *.cs
            //DebugLogCodeSnippet();
        }

        /// <summary>
        /// Create a list to help in creating a constants class. Optional.
        /// </summary>
        private void DebugLogCodeSnippet()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DebugLogCodeSnippet...");

            foreach (AudioClip audioClip in _audioClips)
            {
                stringBuilder.AppendLine($"public const string {audioClip.name} = \"{audioClip.name}\";");
            }

            //Keep as "Debug.Log"
            Debug.Log(stringBuilder.ToString());
        }

        /// <summary>
        /// Play the audio
        /// </summary>
        /// <param name="audioClipName"></param>
        /// <param name="pitch"></param>
        public void PlayAudioClip(string audioClipName, float pitch)
        {
            foreach (AudioClip audioClip in _audioClips)
            {
                if (audioClip.name == audioClipName)
                {
                    PlayAudioClipInternal(audioClip, pitch);
                    return;
                }
            }
        }

        /// <summary>
        /// Play the audio
        /// </summary>
        /// <param name="audioClipName"></param>
        /// <param name="delay"></param>
        /// <param name="pitch"></param>
        public void PlayAudioClipDelayed(string audioClipName, float delay, float pitch)
        {
            foreach (AudioClip audioClip in _audioClips)
            {
                if (audioClip.name == audioClipName)
                {
                    PlayAudioClipInternal(audioClip, pitch, delay);
                    return;
                }
            }
        }

        /// <summary>
        /// Play the audio
        /// </summary>
        /// <param name="audioClipName"></param>
        /// <param name="delay"></param>
        public void PlayAudioClipDelayed(string audioClipName, float delay)
        {
            foreach (AudioClip audioClip in _audioClips)
            {
                if (audioClip.name == audioClipName)
                {
                    PlayAudioClipInternal(audioClip, PitchDefault, delay);
                    return;
                }
            }
        }

        /// <summary>
        /// Play the AudioClip by name.
        /// </summary>
        public void PlayAudioClip(string audioClipName)
        {
            PlayAudioClip(audioClipName, PitchDefault);
        }

        /// <summary>
        /// Play the AudioClip by reference.
        /// If all sources are occupied, nothing will play.
        /// </summary>
        public void PlayAudioClip(AudioClip audioClip)
        {
            PlayAudioClipInternal(audioClip, 1);
        }

        /// <summary>
        /// Play the AudioClip by reference.
        /// If all sources are occupied, nothing will play.
        /// </summary>
        private void PlayAudioClipInternal(AudioClip audioClip, float pitch, float delay = UnsetFloat)
        {
            foreach (AudioSource audioSource in _audioSources)
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.clip = audioClip;
                    audioSource.pitch = pitch;
                    if (delay == UnsetFloat)
                    {
                        if (!_configuration.IsAudioMuted)
                        {
                            audioSource.Play();
                        }
                    }
                    else
                    {
                        if (!_configuration.IsAudioMuted)
                        {
                            //delay in seconds
                            audioSource.PlayDelayed(delay);
                        }
                    }

                    return;
                }
            }
        }
    }
}
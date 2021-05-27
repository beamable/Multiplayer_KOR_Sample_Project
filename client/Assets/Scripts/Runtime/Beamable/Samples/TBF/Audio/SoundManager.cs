using Beamable.Samples.Core;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Beamable.Samples.TBF.Audio
{
	/// <summary>
	/// Maintain a list of AudioSources and play the next 
	/// AudioClip on the first available AudioSource.
	/// </summary>
	public class SoundManager : SingletonMonobehavior<SoundManager>
	{
		private const float UnsetFloat = -1;
		private const float PitchDefault = 1;

		[SerializeField]
		private List<AudioClip> _audioClips = new List<AudioClip>();

		[SerializeField]
		private List<AudioSource> _audioSources = new List<AudioSource>();

		protected override void Awake()
		{
			base.Awake();
			/// If/after updating AudioClips in the UnityEditor, run this once to rebuild const *.cs
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
						audioSource.Play();
					}
					else
               {
						//delay in seconds
						audioSource.PlayDelayed(delay);
					}
					
					return;
				}
			}
		}


	}
}
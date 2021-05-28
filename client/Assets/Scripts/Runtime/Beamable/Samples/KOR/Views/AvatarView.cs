using Beamable.Samples.KOR.Audio;
using Beamable.Samples.KOR.Data;
using UnityEngine;

namespace Beamable.Samples.KOR.Views
{
   /// <summary>
   /// Handles the audio/graphics rendering logic: Avatar
   /// </summary>
   public class AvatarView : MonoBehaviour
   {
      //  Properties -----------------------------------

      //  Fields ---------------------------------------
      [SerializeField]
      private Animator _animator = null;

      [SerializeField]
      private Collider _collider = null;
      
      [SerializeField]
      private Configuration _configuration = null;

      /// <summary>
      /// Store Idle info so we can check "IsIdleAnimation"
      /// </summary>
      private int _idleAnimationFullPathHash;

      /// <summary>
      /// Determines if the current playing animation is the idle animation.
      /// </summary>
      public bool IsIdleAnimation 
      {
         get
         {
            return _animator.GetCurrentAnimatorStateInfo(0).fullPathHash == _idleAnimationFullPathHash;
         }
      }


      //  Other Methods --------------------------------
      public void PlayAnimationIdle()
      {
         _animator.SetTrigger(KORConstants.Avatar_Idle);
         _idleAnimationFullPathHash = _animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
      }

      public void PlayAnimationWin()
      {
         _animator.SetTrigger(KORConstants.Avatar_Idle);
      }

      public void PlayAnimationLoss()
      {
         _animator.SetTrigger(KORConstants.Avatar_Death);
      }

      private void PlayAudioClipDelayed(string audioClipName, float delay)
      {
         SoundManager.Instance.PlayAudioClipDelayed(audioClipName, delay);
      }
   }
}
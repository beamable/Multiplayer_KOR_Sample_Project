using Beamable.Samples.Core;
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

      [ReadOnly]
      public long playerDbid;

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

      public void SetForPlayer(long dbid)
      {
         playerDbid = dbid;
      }

      public void PlayAnimationIdle()
      {
         var dummy = _collider;
         var foo = _configuration;
         _animator.SetBool(KORConstants.Avatar_WalkForward, false);
         _animator.SetBool(KORConstants.Avatar_RunForward, false);
         _idleAnimationFullPathHash = _animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
      }

      public void PlayAnimationWalkForward()
      {
         _animator.SetBool(KORConstants.Avatar_RunForward, false);
         _animator.SetBool(KORConstants.Avatar_WalkForward, true);
      }

      public void PlayAnimationRunForward()
      {
         _animator.SetBool(KORConstants.Avatar_WalkForward, false);
         _animator.SetBool(KORConstants.Avatar_RunForward, true);
      }

      public void PlayAnimationAttack01()
      {
         _animator.SetTrigger(KORConstants.Avatar_Attack01);
      }

      public void PlayAnimationAttack02()
      {
         _animator.SetTrigger(KORConstants.Avatar_Attack02);
      }

      public void PlayAnimationTakeDamage()
      {
         _animator.SetTrigger(KORConstants.Avatar_TakeDamage);
      }
      public void PlayAnimationDie()
      {
         _animator.SetTrigger(KORConstants.Avatar_Die);
      }

      // Play a sound related to this view, its ok if it happens
      // before or after the object is destroyed.
      private void PlayAudioClipDelayed(string audioClipName, float delay)
      {
         SoundManager.Instance.PlayAudioClipDelayed(audioClipName, delay);
      }
   }
}
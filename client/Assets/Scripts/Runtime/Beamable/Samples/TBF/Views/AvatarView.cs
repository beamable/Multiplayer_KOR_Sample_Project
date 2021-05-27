using Beamable.Samples.TBF.Audio;
using Beamable.Samples.TBF.Data;
using Beamable.Samples.TBF.Exceptions;
using UnityEngine;

namespace Beamable.Samples.TBF.Views
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
         _animator.SetTrigger(TBFConstants.Avatar_Idle);
         _idleAnimationFullPathHash = _animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
      }


      public void PlayAnimationWin()
      {
         _animator.SetTrigger(TBFConstants.Avatar_Idle);
      }


      public void PlayAnimationLoss()
      {
         _animator.SetTrigger(TBFConstants.Avatar_Death);
      }


      public void PlayAnimationByGameMoveType(GameMoveType gameMoveType)
      {
         switch (gameMoveType)
         {
            case GameMoveType.High:
               _animator.SetTrigger(TBFConstants.Avatar_Attack_01);
               PlayAudioClipDelayed(SoundConstants.Attack_01, _configuration.DelayBeforeSoundAttack_01a);
               PlayAudioClipDelayed(SoundConstants.Attack_01, _configuration.DelayBeforeSoundAttack_01b);
               break;
            case GameMoveType.Mid:
               _animator.SetTrigger(TBFConstants.Avatar_Attack_02);
               PlayAudioClipDelayed(SoundConstants.Attack_02, _configuration.DelayBeforeSoundAttack_02a);
               PlayAudioClipDelayed(SoundConstants.Attack_02, _configuration.DelayBeforeSoundAttack_02b);
               break;
            case GameMoveType.Low:
               _animator.SetTrigger(TBFConstants.Avatar_Attack_03);
               PlayAudioClipDelayed(SoundConstants.Attack_03, _configuration.DelayBeforeSoundAttack_03);
               break;
            default:
               SwitchDefaultException.Throw(gameMoveType);
               break;
         }
      }

      private void PlayAudioClipDelayed(string audioClipName, float delay)
      {
         SoundManager.Instance.PlayAudioClipDelayed(audioClipName, delay);
      }
   }
}
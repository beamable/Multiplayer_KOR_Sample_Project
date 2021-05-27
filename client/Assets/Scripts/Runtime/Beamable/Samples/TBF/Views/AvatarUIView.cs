using Beamable.Samples.TBF.Data;
using UnityEngine;

namespace Beamable.Samples.TBF.Views
{
   /// <summary>
   /// Handles the audio/graphics rendering logic: Avatar UI
   /// </summary>
   public class AvatarUIView : MonoBehaviour
   {
      //  Properties -----------------------------------
      public HealthBarView HealthBarView { get { return _healthBarView; } }
      public AvatarData AvatarData { set { _avatarData = value; Render(); } get { return _avatarData; } }

      //  Fields ---------------------------------------
      [SerializeField]
      private HealthBarView _healthBarView = null;

      private AvatarData _avatarData = null;

      //  Other Methods   ------------------------------
      private void Render()
      {
         _healthBarView.BackgroundColor = _avatarData.Color;
         _healthBarView.Title = _avatarData.Location;
      }
   }
}
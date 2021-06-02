using System.Collections.Generic;
using Beamable.Samples.KOR.Animation;
using UnityEngine;

namespace Beamable.Samples.KOR.Views
{
   /// <summary>
   /// Handles the audio/graphics rendering logic: Lobby UI
   /// </summary>
   public class LeaderboardUIView : BaseUIView
   {
      //  Properties -----------------------------------

      //  Fields ---------------------------------------
      [Header ("Cosmetic Animation")]
      [SerializeField]
      private List<CanvasGroup> _canvasGroups = null;

      //  Unity Methods   ------------------------------
      protected void Start()
      {
         TweenHelper.CanvasGroupsDoFade(_canvasGroups, 0, 1, 1, Configuration.DelayBeforeFadeInUI, Configuration.DelayBetweenFadeInUI);
      }
   }
}
using System.Collections.Generic;
using Beamable.Samples.KOR.Animation;
using Beamable.Samples.KOR.Data;
using Beamable.Samples.KOR.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Samples.KOR.Views
{
   /// <summary>
   /// Handles the audio/graphics rendering logic: Game
   /// </summary>
   public class GameUIView : MonoBehaviour
   {
      //  Properties -----------------------------------
      public RingView RingView { get { return _ringView; } }
      public Button BackButton { get { return _backButton; } }

      public TMP_BufferedText BufferedText { get { return _bufferedText; } }
      public TMP_Text RoundText { get { return _roundText; } }

      //  Fields ---------------------------------------
      [SerializeField]
      private Configuration _configuration = null;

      [SerializeField]
      private TMP_BufferedText _bufferedText = null;

      [SerializeField]
      private TMP_Text _roundText = null;

      [SerializeField]
      private Button _backButton = null;

      [SerializeField]
      private RingView _ringView = null;
      
      [Header("Cosmetic Animation")]
      [SerializeField]
      private List<CanvasGroup> _canvasGroups = null;

      //  Unity Methods   ------------------------------
      protected void Start()
      {
         TweenHelper.CanvasGroupsDoFade(_canvasGroups, 0, 1, 1, 0, _configuration.DelayFadeInUI);
      }
   }
}
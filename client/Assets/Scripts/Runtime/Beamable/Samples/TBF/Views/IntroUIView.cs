using Beamable.Samples.TBF.Animation;
using Beamable.Samples.TBF.Data;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Samples.TBF.Views
{
   /// <summary>
   /// Handles the audio/graphics rendering logic: Intro UI
   /// </summary>
   public class IntroUIView : MonoBehaviour
   {
      //  Properties -----------------------------------
      public string AboutBodyText { set { _aboutBodyText.text = value; } }

      public Button StartGameOnePlayerButton { get { return _startGameOnePlayerButton; } }
      public Button StartGameTwoPlayerButton { get { return _startGameTwoPlayerButton; } }
      public Button QuitButton { get { return _quitButton; } }
      public CanvasGroup ButtonsCanvasGroup { get { return _buttonsCanvasGroup; } }

      //  Fields ---------------------------------------
      [SerializeField]
      private Configuration _configuration = null;

      [SerializeField]
      private Button _startGameOnePlayerButton = null;

      [SerializeField]
      private Button _startGameTwoPlayerButton = null;

      [SerializeField]
      private Button _quitButton = null;

      [SerializeField]
      private TMP_Text _aboutBodyText = null;

      [SerializeField]
      private CanvasGroup _buttonsCanvasGroup = null;

      [Header ("Cosmetic Animation")]
      [SerializeField]
      private List<CanvasGroup> _canvasGroups = null;

      //  Unity Methods   ------------------------------
      protected void Start()
      {
         TweenHelper.CanvasGroupsDoFade(_canvasGroups, 0, 1, 1, 0, _configuration.DelayFadeInUI);
      }
   }
}
using System.Collections.Generic;
using Beamable.Samples.KOR.Animation;
using Beamable.Samples.KOR.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Samples.KOR.Views
{
   /// <summary>
   /// Handles the audio/graphics rendering logic: Intro UI
   /// </summary>
   public class IntroUIView : MonoBehaviour
   {
      //  Properties -----------------------------------
      public string AboutBodyText { set { _aboutBodyText.text = value; } }

      public Button StartGameButton { get { return _startGameButton; } }
      public Button LeaderboardButton { get { return _leaderboardButton; } }
      public Button StoreButton { get { return _storeButton; } }
      public Button QuitButton { get { return _quitButton; } }
      public CanvasGroup ButtonsCanvasGroup { get { return _buttonsCanvasGroup; } }

      //  Fields ---------------------------------------
      [SerializeField]
      private Configuration _configuration = null;

      [SerializeField]
      private Button _startGameButton = null;

      [SerializeField]
      private Button _leaderboardButton = null;

      [SerializeField]
      private Button _storeButton = null;

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
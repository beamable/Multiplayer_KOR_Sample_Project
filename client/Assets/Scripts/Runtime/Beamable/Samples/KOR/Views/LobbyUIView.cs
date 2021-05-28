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
   /// Handles the audio/graphics rendering logic: Lobby UI
   /// </summary>
   public class LobbyUIView : MonoBehaviour
   {
      //  Properties -----------------------------------
      public TMP_BufferedText BufferedText { get { return _bufferedText; } }
      public Button BackButton { get { return _backButton; } }
      public Button StartGameButton { get { return _startGameButton; } }

      //  Fields ---------------------------------------
      [SerializeField]
      private Configuration _configuration = null;

      [SerializeField]
      private TMP_BufferedText _bufferedText = null;

      [SerializeField]
      private Button _startGameButton = null;
      
      [SerializeField]
      private Button _backButton = null;

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
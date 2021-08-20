using System.Collections.Generic;
using Beamable.Samples.Core.UI;
using Beamable.Samples.KOR.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Samples.KOR.Views
{
   /// <summary>
   /// Handles the audio/graphics rendering logic: Lobby UI
   /// </summary>
   public class LobbyUIView : BaseUIView
   {
      //  Properties -----------------------------------
      public TMP_BufferedText BufferedText { get { return _bufferedText; } }
      public Button BackButton { get { return _backButton; } }

      //  Fields ---------------------------------------
      [SerializeField]
      private TMP_BufferedText _bufferedText = null;

      [SerializeField]
      private Button _backButton = null;

      //  Unity Methods   ------------------------------
      public void Start()
      {
         CanvasGroupsDoFadeOut();
      }
   }
}
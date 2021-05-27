using Beamable.Samples.TBF.Animation;
using Beamable.Samples.TBF.Data;
using Beamable.Samples.TBF.Exceptions;
using Beamable.Samples.TBF.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Samples.TBF.Views
{
   /// <summary>
   /// Handles the audio/graphics rendering logic: Game
   /// </summary>
   public class GameUIView : MonoBehaviour
   {
      //  Properties -----------------------------------
      public List<AvatarView> AvatarViews { get { return _avatarViews; } }
      public List<AvatarUIView> AvatarUIViews { get { return _avatarUIViews; } }
      //
      public Button BackButton { get { return _backButton; } }
      public Button MoveButton_01 { get { return _moveButton_01; } }
      public Button MoveButton_02 { get { return _moveButton_02; } }
      public Button MoveButton_03 { get { return _moveButton_03; } }
      //
      public CanvasGroup MoveButtonsCanvasGroup { get { return _moveButtonsCanvasGroup; } }

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
      private Button _moveButton_01 = null;

      [SerializeField]
      private Button _moveButton_02 = null;

      [SerializeField]
      private Button _moveButton_03 = null;

      [SerializeField]
      private CanvasGroup _moveButtonsCanvasGroup = null;

      [SerializeField]
      private List<AvatarUIView> _avatarUIViews = null;

      [SerializeField]
      private List<AvatarView> _avatarViews = null;

      [Header("Cosmetic Animation")]
      [SerializeField]
      private List<CanvasGroup> _canvasGroups = null;

      //  Unity Methods   ------------------------------
      protected void Start()
      {
         for (int i = 0; i < _avatarUIViews.Count; i++)
         {
            _avatarUIViews[i].AvatarData = _configuration.AvatarDatas[i];
         }

         TweenHelper.CanvasGroupsDoFade(_canvasGroups, 0, 1, 1, 0, _configuration.DelayFadeInUI);
      }

   }
}
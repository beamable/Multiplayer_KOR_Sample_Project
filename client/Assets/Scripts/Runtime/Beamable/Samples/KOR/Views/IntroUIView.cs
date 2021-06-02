﻿using System.Collections.Generic;
using Beamable.Samples.KOR.Animation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Samples.KOR.Views
{
   /// <summary>
   /// Handles the audio/graphics rendering logic: Intro UI
   /// </summary>
   public class IntroUIView : BaseUIView 
   {
      //  Properties -----------------------------------
      public string AboutBodyText { set { _aboutBodyText.text = value; } }

      public Button StartGame01Button { get { return startGame01Button; } }
      public Button StartGame02Button { get { return startGame02Button; } }
      public Button LeaderboardButton { get { return _leaderboardButton; } }
      public Button StoreButton { get { return _storeButton; } }
      public Button QuitButton { get { return _quitButton; } }
      public CanvasGroup ButtonsCanvasGroup { get { return _buttonsCanvasGroup; } }

      //  Fields ---------------------------------------
      [Header("UI")]
      [SerializeField]
      private Button startGame01Button = null;

      [SerializeField]
      private Button startGame02Button = null;

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
         
      }
      
      public void CanvasGroupsDoFade()
      {
         TweenHelper.CanvasGroupsDoFade(_canvasGroups, 0, 1, 1, Configuration.DelayBeforeFadeInUI, Configuration.DelayBetweenFadeInUI);
      }
   }
}
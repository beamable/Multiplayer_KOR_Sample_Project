﻿using System.Collections.Generic;
using Beamable.Samples.KOR.Animation;
using Beamable.Samples.KOR.Data;
using Beamable.Samples.KOR.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Samples.KOR.Views
{
   /// <summary>
   /// Handles the audio/graphics rendering logic: Store UI
   /// </summary>
   public class StoreUIView : BaseUIView
   {
      //  Properties -----------------------------------
      public TMP_BufferedText BufferedText { get { return _bufferedText; } }
      public PanelUI InventoryPanelUI { get { return inventoryPanelUI; } }
      public PanelUI StorePanelUI { get { return storePanelUI; } }
      
      public Button BuyButton { get { return _buyButton; } }
      public Button BackButton { get { return _backButton; } }
      
      //  Fields ---------------------------------------
      [Header("UI")]
      [SerializeField]
      private TMP_BufferedText _bufferedText = null;

      [SerializeField]
      private PanelUI inventoryPanelUI = null;
      
      [SerializeField]
      private PanelUI storePanelUI = null;
      
      [SerializeField]
      private Button _buyButton = null;
      
      [SerializeField]
      private Button _backButton = null;

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
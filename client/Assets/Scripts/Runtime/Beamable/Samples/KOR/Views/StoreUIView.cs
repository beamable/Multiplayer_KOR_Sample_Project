using System.Collections.Generic;
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
      public StoreItemUI StoreItemUIPrefab { get { return _storeItemUIPrefab; } }
      public AttributesPanelUI AttributesPanelUI { get { return _attributesPanelUI; } }
      public StorePanelUI LeftPanelUI { get { return _leftPanelUI; } }
      public StorePanelUI RightPanelUI { get { return _rightPanelUI; } }
      public Button BuyButton { get { return _buyButton; } }
      public Button BackButton { get { return _backButton; } }
      public Button BackgroundButton { get { return _backgroundButton; } }
      public Button ResetButton { get { return _resetButton; } }
      
      //  Fields ---------------------------------------
      [Header("Prefabs")]
      [SerializeField]
      private StoreItemUI _storeItemUIPrefab = null;
      
      [Header("UI")]
      [SerializeField]
      private TMP_BufferedText _bufferedText = null;

      [SerializeField]
      private AttributesPanelUI _attributesPanelUI = null;
      
      [SerializeField]
      private StorePanelUI _leftPanelUI = null;

      [SerializeField]
      private StorePanelUI _rightPanelUI = null;

      [SerializeField]
      private Button _buyButton = null;
      
      [SerializeField]
      private Button _resetButton = null;

      [SerializeField]
      private Button _backButton = null;
     
      [SerializeField]
      private Button _backgroundButton = null;


      //  Unity Methods   ------------------------------
      public void Start()
      {
         CanvasGroupsDoFadeIn();
      }
   }
}
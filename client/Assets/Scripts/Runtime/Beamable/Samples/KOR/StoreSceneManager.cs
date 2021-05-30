using Beamable.Api.Payments;
using Beamable.Common.Api.Inventory;
using Beamable.Samples.KOR.Data;
using Beamable.Samples.KOR.UI;
using Beamable.Samples.KOR.Views;
using UnityEngine;

namespace Beamable.Samples.KOR
{
   /// <summary>
   /// Handles the main scene logic: Lobby
   /// </summary>
   public class StoreSceneManager : MonoBehaviour
   {
      //  Fields ---------------------------------------
      [SerializeField]
      private Configuration _configuration = null;

      [SerializeField]
      private StoreUIView _storeUIView = null;

      private IBeamableAPI _beamableAPI = null;
      private InventoryView _inventoryView = null;
      private PlayerStoreView _playerStoreView = null;

      //  Unity Methods   ------------------------------
      protected void Start()
      {
         _storeUIView.BuyButton.onClick.AddListener(BuyButton_OnClicked);
         _storeUIView.BackButton.onClick.AddListener(BackButton_OnClicked);
         
         SetupBeamable();
      }

      //  Other Methods   ------------------------------
      private async void SetupBeamable()
      {
         _storeUIView.BufferedText.SetText(KORConstants.StoreUIView_Loading_Store, 
            TMP_BufferedText.BufferedTextMode.Queue);

         _beamableAPI= await Beamable.API.Instance;

         _beamableAPI.InventoryService.Subscribe(InventoryService_OnChanged);
         _beamableAPI.CommerceService.Subscribe(CommerceService_OnChanged);
         LoadServices();
         
      }
      
      private void DebugLog(string message)
      {
         if (_configuration.IsDebugLog)
         {
            Debug.Log(message);
         }
      }
      
      private async void LoadServices()
      {
         _inventoryView = null;
         _playerStoreView = null;
         
         // Set loading text
         _storeUIView.BufferedText.SetText(KORConstants.StoreUIView_Loading_Store, 
            TMP_BufferedText.BufferedTextMode.Immediate);
         _storeUIView.InventoryPanelUIView.BodyText.text = KORConstants.StoreUIView_Loading_Store;
         _storeUIView.StorePanelUIView.BodyText.text = KORConstants.StoreUIView_Loading_Store;

         // Reload the services
         InventoryView inventoryView = await _beamableAPI.InventoryService.GetCurrent();
         InventoryService_OnChanged(inventoryView);
         
         PlayerStoreView playerStoreView = await _beamableAPI.CommerceService.GetCurrent();
         CommerceService_OnChanged(playerStoreView);
      }
      
      private void CheckLoadServicesStatus()
      {
         if (_inventoryView != null && _playerStoreView != null)
         {
            //
            _storeUIView.BufferedText.SetText(KORConstants.StoreUIView_Instructions, 
               TMP_BufferedText.BufferedTextMode.Queue);
         }
      }

      //  Event Handlers -------------------------------
      private void InventoryService_OnChanged(InventoryView inventoryView)
      {
         _inventoryView = inventoryView;
         Debug.Log($"InventoryService_OnChanged() _inventoryView = {_inventoryView}");
         CheckLoadServicesStatus();
      }

      private void CommerceService_OnChanged(PlayerStoreView playerStoreView)
      {
         _playerStoreView = playerStoreView;
         Debug.Log($"CommerceService_OnChanged() _playerStoreView = {_playerStoreView}");
         CheckLoadServicesStatus();
      }
      
      private void BuyButton_OnClicked()
      {
         Debug.Log("TODO: Enable this on item selection. Disable after purchase.");
      }
      
      private void BackButton_OnClicked()
      {
         StartCoroutine(KORHelper.LoadScene_Coroutine(_configuration.IntroSceneName,
            _configuration.DelayBeforeLoadScene));
      }
   }
}

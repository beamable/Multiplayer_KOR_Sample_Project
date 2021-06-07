using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Beamable.Api.Payments;
using Beamable.Common.Api;
using Beamable.Common.Api.Inventory;
using Beamable.Common.Shop;
using Beamable.Extensions;
using Beamable.Samples.Core;
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
      private List<string> _inventoryItems = new List<string>();
      private List<string> _storeItems = new List<string>();
      private StoreContent _storeContent = null;
      private int _currencyAmount = 0;
      
      
      //  Unity Methods   ------------------------------
      protected void Start()
      {
         _storeUIView.BuyButton.onClick.AddListener(BuyButton_OnClicked);
         _storeUIView.ResetButton.onClick.AddListener(ResetButton_OnClicked);
         _storeUIView.BackButton.onClick.AddListener(BackButton_OnClicked);
         SetupBeamable();
      }

      
      //  Other Methods   ------------------------------
      private void DebugLog(string message)
      {
         // Respects Configuration.IsDebugLog Checkbox
         Configuration.Debugger.Log(message);
      }
      
      
      private async void SetupBeamable()
      {
         // Immediately: Show default text
         _storeUIView.BufferedText.SetText(KORConstants.StoreUIView_Loading_Store, 
            TMP_BufferedText.BufferedTextMode.Queue);

         _storeUIView.AttributesPanelUI.BodyText.text = "";

         // Slowly: Load beamable data
         _beamableAPI= await Beamable.API.Instance;
         
         // Do this after calling "Beamable.API.Instance" for smoother UI
         _storeUIView.CanvasGroupsDoFadeIn();

         // Show the player's attributes in the UI of this scene
         ReloadAttributes();
         
         //
         _storeContent = await _configuration.StoreRef.Resolve();
         LoadServices();
         
      }

      
      /// <summary>
      /// Get sum total of pay-to-play attributes to impact gameplay
      /// </summary>
      /// <returns></returns>
      private async void ReloadAttributes()
      {
         // Show the player's attributes in the UI of this scene
         Attributes attributes = await RuntimeDataStorage.Instance.CharacterManager.GetChosenPlayerAttributes();
         _storeUIView.AttributesPanelUI.Attributes = attributes;

      }


      private void LoadServices()
      {
         _inventoryView = null;
         _playerStoreView = null;
         
         // Set loading text
         _storeUIView.BufferedText.SetText(KORConstants.StoreUIView_Loading_Store, 
            TMP_BufferedText.BufferedTextMode.Immediate);
         _storeUIView.RightPanelUI.BodyText.text = KORConstants.StoreUIView_Loading_Inventory;
         _storeUIView.LeftPanelUI.BodyText.text = KORConstants.StoreUIView_Loading_Store;

         // Reload the services
         _beamableAPI.InventoryService.Subscribe(KORConstants.ItemContentType, Inventory_OnChanged);
         _beamableAPI.InventoryService.Subscribe(KORConstants.CurrencyContentType, Currency_OnChanged);
         _beamableAPI.CommerceService.Subscribe(_storeContent.Id, CommerceService_OnChanged);

      }

      
      private void CheckLoadServicesStatus()
      {
         ReloadAttributes();
            
         string instructions = string.Format(KORConstants.StoreUIView_Instructions, _currencyAmount, 
            KORConstants.StoreUIView_CurrencyName);
         _storeUIView.BufferedText.SetText(instructions, 
            TMP_BufferedText.BufferedTextMode.Immediate);
         
         // RENDER #1 - INVENTORY
         _storeUIView.LeftPanelUI.BodyText.text = "You have";
         _storeUIView.LeftPanelUI.VerticalLayoutGroup.transform.ClearChildren();
         foreach (var item in _inventoryItems)
         {
            StoreItemUI inventoryItem = GameObject.Instantiate<StoreItemUI>(_storeUIView.StoreItemUIPrefab,
               _storeUIView.LeftPanelUI.VerticalLayoutGroup.transform);
            inventoryItem.transform.SetAsLastSibling();
            inventoryItem.TitleText.text = item;
            
            inventoryItem.Button.onClick.AddListener(() =>
            {
               Debug.Log("Clicked i: " + inventoryItem.TitleText.text);
            });
         }
         
         
         // RENDER #2 - STORE
         _storeUIView.RightPanelUI.BodyText.text = "You can buy";
         _storeUIView.RightPanelUI.VerticalLayoutGroup.transform.ClearChildren();
         foreach (var item in _storeItems)
         {
            StoreItemUI storeItemUI = GameObject.Instantiate<StoreItemUI>(_storeUIView.StoreItemUIPrefab,
               _storeUIView.RightPanelUI.VerticalLayoutGroup.transform);
            storeItemUI.transform.SetAsLastSibling();
            storeItemUI.TitleText.text = item;
            
            storeItemUI.Button.onClick.AddListener(() =>
            {
               Debug.Log("Clicked s: " + storeItemUI.TitleText.text);
            });
            
         }  
         
      }
      
      
      private async void BuySelectedStoreItem()
      {
         var storeSymbol = _storeContent.Id;
         var listingSymbol = _playerStoreView.listings[0].symbol;
         await _beamableAPI.CommerceService.Purchase(storeSymbol, listingSymbol);
      }


      //  Event Handlers -------------------------------
      private void Inventory_OnChanged(InventoryView inventoryView)
      {
         _inventoryView = inventoryView;
         
         _inventoryItems.Clear();
         foreach (KeyValuePair<string, List<ItemView>> kvp in _inventoryView.items)
         {
            string contentId = kvp.Key;
            string itemName = KORHelper.GetKORItemDisplayNameFromContentId(contentId);
            int itemCount = kvp.Value.Count;
            string itemDisplayName = $"\t{itemName} x {itemCount}";
               
            //TODO: Replace List<string> with List<blah> to hold more data?
            _inventoryItems.Add(itemDisplayName);
         }
         
         DebugLog($"InventoryService_OnChanged() items.Count = {_inventoryView.items.Count}");
         CheckLoadServicesStatus();
      }

      
      private void Currency_OnChanged(InventoryView inventoryViewForCurrencies)
      {
         _currencyAmount = 0;
         foreach (KeyValuePair<string, long> kvp  in inventoryViewForCurrencies.currencies)
         {
            _currencyAmount = (int)kvp.Value;
            Configuration.Debugger.Log($"Currency_OnChanged() CurrencyAmount = {_currencyAmount}");
            break;
         }
         
         CheckLoadServicesStatus();
      }
      
      
      private void CommerceService_OnChanged(PlayerStoreView playerStoreView)
      {
         _playerStoreView = playerStoreView;
         _storeItems.Clear();
         
         foreach (PlayerListingView playerListingView in playerStoreView.listings)
         {
            var contentId = playerListingView.offer.obtainItems[0].contentId;
            
            int price = playerListingView.offer.price.amount;
            string itemName = KORHelper.GetKORItemDisplayNameFromContentId(contentId);
            string itemDisplayName = $"\t{itemName} ({price} {KORConstants.StoreUIView_CurrencyName})";
               
            //TODO: Replace List<string> with List<blah> to hold more data?
            _storeItems.Add(itemDisplayName);
         }
         
         DebugLog($"CommerceService_OnChanged() listings.Count = {_playerStoreView.listings.Count}");
         CheckLoadServicesStatus();
      }
      
      
      private void BuyButton_OnClicked()
      {
         KORHelper.PlayAudioForUIClick();

         BuySelectedStoreItem();
      }

      private void ResetButton_OnClicked()
      {
         KORHelper.PlayAudioForUIClick();

         Configuration.Debugger.Log(
            "Reset! This deletes the local player, " +
            "creates a new one, and restarts the game. " +
            "Do not use this in production.");
         
         ExampleProjectHacks.ClearDeviceUsersAndReloadGame();
      }
      
      
      private void BackButton_OnClicked()
      {
         KORHelper.PlayAudioForUIClick();
         
         StartCoroutine(KORHelper.LoadScene_Coroutine(_configuration.IntroSceneName,
            _configuration.DelayBeforeLoadScene));
      }
   }
}

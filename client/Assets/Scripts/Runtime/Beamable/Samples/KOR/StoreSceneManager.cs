using System.Collections.Generic;
using System.Text;
using Beamable.Api.Payments;
using Beamable.Common.Api.Inventory;
using Beamable.Common.Shop;
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
      //  Consts ---------------------------------------
      private const string ItemContentType = "items";
      private const string CurrencyContentType = "currency";
      
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
         _storeUIView.BufferedText.SetText(KORConstants.StoreUIView_Loading_Store, 
            TMP_BufferedText.BufferedTextMode.Queue);

         _beamableAPI= await Beamable.API.Instance;
         
         // Do this after calling "Beamable.API.Instance" for smoother UI
         _storeUIView.CanvasGroupsDoFadeIn();
         
         _storeContent = await _configuration.StoreRef.Resolve();
         DebugLog($"Store Scene, dbid = {_beamableAPI.User.id}");
         DebugLog($"StoreContent, listings.Count = {_storeContent.listings.Count}");
         
         LoadServices();
         
      }
      
      
      private void LoadServices()
      {
         _inventoryView = null;
         _playerStoreView = null;
         
         // Set loading text
         _storeUIView.BufferedText.SetText(KORConstants.StoreUIView_Loading_Store, 
            TMP_BufferedText.BufferedTextMode.Immediate);
         _storeUIView.InventoryPanelUI.BodyText.text = KORConstants.StoreUIView_Loading_Inventory;
         _storeUIView.StorePanelUI.BodyText.text = KORConstants.StoreUIView_Loading_Store;

         // Reload the services
         _beamableAPI.InventoryService.Subscribe(ItemContentType, Inventory_OnChanged);
         _beamableAPI.InventoryService.Subscribe(CurrencyContentType, Currency_OnChanged);
         _beamableAPI.CommerceService.Subscribe(_storeContent.Id, CommerceService_OnChanged);

      }

      private void CheckLoadServicesStatus()
      {
         if (_inventoryView == null || _playerStoreView == null)
         {
            //TODO: Wait for both to load
            //return;
         }

         string instructions = string.Format(KORConstants.StoreUIView_Instructions, _currencyAmount, 
            KORConstants.StoreUIView_CurrencyName);
         _storeUIView.BufferedText.SetText(instructions, 
            TMP_BufferedText.BufferedTextMode.Immediate);

         // Render inventory
         StringBuilder inventoryStringBuilder = new StringBuilder();
         inventoryStringBuilder.AppendLine();
         foreach (var item in _inventoryItems)
         {
            inventoryStringBuilder.AppendLine($"•{item}");
         }
         _storeUIView.InventoryPanelUI.BodyText.text = inventoryStringBuilder.ToString();

         // Render store
         StringBuilder storeStringBuilder = new StringBuilder();
         storeStringBuilder.AppendLine();
         foreach (var item in _storeItems)
         {
            storeStringBuilder.AppendLine($"•{item}");
         }  
         _storeUIView.StorePanelUI.BodyText.text = storeStringBuilder.ToString();
      }
      
      
      private async void BuySelectedStoreItem()
      {
         Debug.Log( ("buy item"));
         var storeSymbol = _storeContent.Id;
         var listingSymbol = _playerStoreView.listings[0].symbol;
         var x = await _beamableAPI.CommerceService.Purchase(storeSymbol, listingSymbol);
         
         Debug.Log( ("buy item2"));
      }


      //  Event Handlers -------------------------------
      private void Inventory_OnChanged(InventoryView inventoryView)
      {
         _inventoryView = inventoryView;
         
         _inventoryItems.Clear();
         foreach (KeyValuePair<string, List<ItemView>> kvp in _inventoryView.items)
         {
            // User can have multiple of EACH item. Show simpler info here
            foreach (ItemView itemView in kvp.Value)
            {
               int itemCount = kvp.Value.Count;
               string itemName = kvp.Key.Replace("items.", "");
               string itemDisplayName = $"\t{itemName} x {itemCount}";
               
               //TODO: Replace List<string> with List<blah> to hold more data?
               _inventoryItems.Add(itemDisplayName);
            }
         }
         
         DebugLog($"InventoryService_OnChanged() _inventoryView = {_inventoryView}");
         CheckLoadServicesStatus();
      }

      
      private void Currency_OnChanged(InventoryView inventoryViewForCurrencies)
      {
        
         _currencyAmount = 0;
         foreach (KeyValuePair<string, long> kvp  in inventoryViewForCurrencies.currencies)
         {
            _currencyAmount = (int)kvp.Value;
            Debug.Log(("Currency_OnChanged() CurrencyAmount: " + _currencyAmount));
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
            var title = playerListingView.offer.obtainItems[0].contentId;
            
            int price = playerListingView.offer.price.amount;
            string itemName = title.Replace("items.", "");
            string itemDisplayName = $"\t{itemName} ({price} {KORConstants.StoreUIView_CurrencyName})";
               
            //TODO: Replace List<string> with List<blah> to hold more data?
            _storeItems.Add(itemDisplayName);
         }
         
         DebugLog($"CommerceService_OnChanged() _playerStoreView = {_playerStoreView}");
         CheckLoadServicesStatus();
      }
      
      
      private void BuyButton_OnClicked()
      {
         KORHelper.PlayAudioForUIClick();

         BuySelectedStoreItem();
      }




      private void BackButton_OnClicked()
      {
         KORHelper.PlayAudioForUIClick();
         
         StartCoroutine(KORHelper.LoadScene_Coroutine(_configuration.IntroSceneName,
            _configuration.DelayBeforeLoadScene));
      }
   }
}

using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Beamable.Api.Payments;
using Beamable.Common.Api;
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
      //  Consts ---------------------------------------
      private const string ContentType = "items";
      
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
         LoadServices();
         
      }
      
      private void LoadServices()
      {
         _inventoryView = null;
         _playerStoreView = null;
         
         // Set loading text
         _storeUIView.BufferedText.SetText(KORConstants.StoreUIView_Loading_Store, 
            TMP_BufferedText.BufferedTextMode.Immediate);
         _storeUIView.InventoryPanelUIView.BodyText.text = KORConstants.StoreUIView_Loading_Inventory;
         _storeUIView.StorePanelUIView.BodyText.text = KORConstants.StoreUIView_Loading_Store;

         // Reload the services
         _beamableAPI.InventoryService.Subscribe(ContentType, InventoryService_OnChanged);
         _beamableAPI.CommerceService.Subscribe(CommerceService_OnChanged);

      }
      
      private void CheckLoadServicesStatus()
      {
         if (_inventoryView == null || _playerStoreView == null)
         {
            //TODO: Wait for both to load
            //return;
         }
         
         _storeUIView.BufferedText.SetText(KORConstants.StoreUIView_Instructions, 
            TMP_BufferedText.BufferedTextMode.Queue);

         // Render inventory
         StringBuilder inventoryStringBuilder = new StringBuilder();
         foreach (var item in _inventoryItems)
         {
            inventoryStringBuilder.AppendLine($"•{item}").AppendLine();
         }
         _storeUIView.InventoryPanelUIView.BodyText.text = inventoryStringBuilder.ToString();

         // Render store
         StringBuilder storeStringBuilder = new StringBuilder();
         foreach (var item in _inventoryItems)
         {
            storeStringBuilder.AppendLine($"•{item}").AppendLine();
         }  
         _storeUIView.StorePanelUIView.BodyText.text = storeStringBuilder.ToString();
      }

      private void DebugLog(string message)
      {
         if (_configuration.IsDebugLog)
         {
            Debug.Log(message);
         }
      }
      //  Event Handlers -------------------------------
      private void InventoryService_OnChanged(InventoryView inventoryView)
      {
         _inventoryView = inventoryView;
         
         foreach (KeyValuePair<string, List<ItemView>> kvp in _inventoryView.items)
         {
            // User can have multiple of EACH item. Show simpler info here
            foreach (ItemView itemView in kvp.Value)
            {
               int itemCount = kvp.Value.Count;
               string itemName = kvp.Key.Replace("items.", "");
               string itemDisplayName = $"\t{itemName} ({itemCount})";
               
               //TODO: Replace List<string> with List<blah> to hold more data?
               _inventoryItems.Add(itemDisplayName);
            }
         }
         
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

using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Api.Payments;
using Beamable.Common.Api.Inventory;
using Beamable.Common.Shop;
using Beamable.Extensions;
using Beamable.Samples.Core;
using Beamable.Samples.KOR.CustomContent;
using Beamable.Samples.KOR.Data;
using Beamable.Samples.KOR.UI;
using Beamable.Samples.KOR.Views;
using UnityEngine;

#pragma warning disable 4014 //CS4014: Because this call is not awaited, execution...

namespace Beamable.Samples.KOR
{
    /// <summary>
    /// Handles the main scene logic: Lobby
    /// </summary>
    public class StoreSceneManager : MonoBehaviour
    {
        //  Fields ---------------------------------------
        private StoreItemData SelectedStoreItemData
        {
            get
            {
                return __selectedStoreItemData;
            }
            set
            {
                __selectedStoreItemData = value;
                _storeUIView.BuyButton.interactable = __selectedStoreItemData != null && CanAffordSelectedStoreItemData;
            }
        }

        private bool CanAffordSelectedStoreItemData
        {
            get
            {
                return _currencyAmount >= __selectedStoreItemData.PlayerListingView.offer.price.amount; ;
            }
        }

        //  Fields ---------------------------------------
        [SerializeField]
        private Configuration _configuration = null;

        [SerializeField]
        private StoreUIView _storeUIView = null;

        private IBeamableAPI _beamableAPI = null;
        private InventoryView _inventoryView = null;
        private PlayerStoreView _playerStoreView = null;
        private List<StoreItemData> _inventoryItemDatas = new List<StoreItemData>();
        private List<StoreItemData> _storeItemDatas = new List<StoreItemData>();
        private StoreContent _storeContent = null;
        private int _currencyAmount = 0;

        // "__" means don't set/get directly
        private StoreItemData __selectedStoreItemData = null;

        //  Unity Methods   ------------------------------
        protected void Start()
        {
            _storeUIView.BuyButton.onClick.AddListener(BuyButton_OnClicked);
            _storeUIView.ResetButton.onClick.AddListener(ResetButton_OnClicked);
            _storeUIView.BackButton.onClick.AddListener(BackButton_OnClicked);
            _storeUIView.BackgroundButton.onClick.AddListener(BackgroundButton_OnClicked);

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
            _beamableAPI = await Beamable.API.Instance;

            // Do this after calling "Beamable.API.Instance" for smoother UI
            _storeUIView.CanvasGroupsDoFadeIn();

            // Show the player's attributes in the UI of this scene
            ReloadAndRenderAttributes();

            //
            _storeContent = await _configuration.StoreRef.Resolve();
            LoadServices();
        }

        /// <summary>
        /// Get sum total of pay-to-play attributes to impact gameplay
        /// </summary>
        /// <returns></returns>
        private async Task<Attributes> ReloadAndRenderAttributes()
        {
            // Show the player's attributes in the UI of this scene
            await RuntimeDataStorage.Instance.CharacterManager.BootstrapTask;
            Attributes attributes = await RuntimeDataStorage.Instance.CharacterManager.GetChosenPlayerAttributes();
            _storeUIView.AttributesPanelUI.Attributes = attributes;
            return attributes;
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
            ReloadAndRenderAttributes();

            // Set Default Instructions
            BackgroundButton_OnClicked();

            // RENDER #1 - INVENTORY

            _storeUIView.LeftPanelUI.VerticalLayoutGroup.transform.ClearChildren();
            foreach (var itemData in _inventoryItemDatas)
            {
                StoreItemUI inventoryItem = GameObject.Instantiate<StoreItemUI>(_storeUIView.StoreItemUIPrefab,
                   _storeUIView.LeftPanelUI.VerticalLayoutGroup.transform);
                inventoryItem.transform.SetAsLastSibling();
                inventoryItem.StoreItemData = itemData;

                inventoryItem.Button.onClick.AddListener(() =>
                {
                    // The user has clicked an INVENTORY item
                    // So null out the selection
                    SelectedStoreItemData = null;

                    string message = string.Format(KORConstants.StoreUIView_SelectStoreInventory,
                   itemData.KORItemContent.ChargeSpeed,
                   itemData.KORItemContent.MovementSpeed);

                    _storeUIView.BufferedText.SetText(message,
                   TMP_BufferedText.BufferedTextMode.Immediate);
                });
            }

            // RENDER #2 - STORE
            _storeUIView.RightPanelUI.VerticalLayoutGroup.transform.ClearChildren();
            foreach (var itemData in _storeItemDatas)
            {
                StoreItemUI storeItemUI = GameObject.Instantiate<StoreItemUI>(_storeUIView.StoreItemUIPrefab,
                   _storeUIView.RightPanelUI.VerticalLayoutGroup.transform);
                storeItemUI.transform.SetAsLastSibling();
                storeItemUI.StoreItemData = itemData;

                storeItemUI.Button.onClick.AddListener(() =>
                {
                    // The user has clicked a STORE item
                    // so store the selection
                    SelectedStoreItemData = itemData;

                    //Show "you can buy and here are the details about it..."
                    string afford = "";
                    if (!CanAffordSelectedStoreItemData)
                    {
                        afford = KORConstants.StoreUIView_CannotAfford;
                    }
                    string message = string.Format(KORConstants.StoreUIView_SelectStoreItem,
                   itemData.KORItemContent.ChargeSpeed,
                   itemData.KORItemContent.MovementSpeed,
                   afford);

                    _storeUIView.BufferedText.SetText(message,
                   TMP_BufferedText.BufferedTextMode.Immediate);
                });
            }
        }

        private async void BuySelectedStoreItem()
        {
            if (SelectedStoreItemData == null)
            {
                Debug.LogError($"BuySelectedStoreItem() failed because __selectedStoreItemData = {__selectedStoreItemData}.");
                return;
            }

            if (!CanAffordSelectedStoreItemData)
            {
                Debug.LogError($"BuySelectedStoreItem() failed because CanAffordSelectedStoreItemData = {CanAffordSelectedStoreItemData}.");
                return;
            }

            await _beamableAPI.CommerceService.Purchase(_storeContent.Id,
               SelectedStoreItemData.PlayerListingView.symbol);
        }

        //  Event Handlers -------------------------------

        private async void Inventory_OnChanged(InventoryView inventoryView)
        {
            _inventoryView = inventoryView;

            _inventoryItemDatas.Clear();
            foreach (KeyValuePair<string, List<ItemView>> kvp in _inventoryView.items)
            {
                string contentId = kvp.Key;
                string itemName = KORHelper.GetKORItemDisplayNameFromContentId(contentId);
                KORItemContent korItemContent = await KORHelper.GetKORItemContentById(_beamableAPI, kvp.Key);

                string title = $"\t{itemName} x {kvp.Value.Count}";
                StoreItemData itemData = new StoreItemData(title, korItemContent, null);
                _inventoryItemDatas.Add(itemData);
            }

            DebugLog($"InventoryService_OnChanged() items.Count = {_inventoryView.items.Count}");
            CheckLoadServicesStatus();
        }

        private void Currency_OnChanged(InventoryView inventoryViewForCurrencies)
        {
            _currencyAmount = 0;
            foreach (KeyValuePair<string, long> kvp in inventoryViewForCurrencies.currencies)
            {
                _currencyAmount = (int)kvp.Value;
                Configuration.Debugger.Log($"Currency_OnChanged() CurrencyAmount = {_currencyAmount}");
                break;
            }

            CheckLoadServicesStatus();
        }

        private async void CommerceService_OnChanged(PlayerStoreView playerStoreView)
        {
            _playerStoreView = playerStoreView;
            _storeItemDatas.Clear();

            foreach (PlayerListingView playerListingView in playerStoreView.listings)
            {
                int price = playerListingView.offer.price.amount;
                string contentId = playerListingView.offer.obtainItems[0].contentId;
                string itemName = KORHelper.GetKORItemDisplayNameFromContentId(contentId);
                KORItemContent korItemContent = await KORHelper.GetKORItemContentById(_beamableAPI, contentId);

                string title = $"\t{itemName} ({price} {KORConstants.StoreUIView_CurrencyName})";
                StoreItemData itemData = new StoreItemData(title, korItemContent, playerListingView);
                _storeItemDatas.Add(itemData);
            }

            DebugLog($"CommerceService_OnChanged() listings.Count = {_playerStoreView.listings.Count}");
            CheckLoadServicesStatus();
        }

        /// <summary>
        /// This handles 'did user click ANYWHERE that is NOT a button?'
        /// </summary>
        public void BackgroundButton_OnClicked()
        {
            // The user has clicked NO ITEM
            // So null out the selection
            SelectedStoreItemData = null;

            // Text above inventory panel
            _storeUIView.LeftPanelUI.BodyText.text = string.Format(KORConstants.StoreUIView_InventoryTip,
               _currencyAmount, KORConstants.StoreUIView_CurrencyName, KORHelper.GetPluralization(_currencyAmount));

            // Text above store panel
            _storeUIView.RightPanelUI.BodyText.text = KORConstants.StoreUIView_StoreTip;

            // This is the default instructions
            _storeUIView.BufferedText.SetText(KORConstants.StoreUIView_Instructions,
               TMP_BufferedText.BufferedTextMode.Immediate);
        }

        private void BuyButton_OnClicked()
        {
            KORHelper.PlayAudioForUIClickPrimary();

            BuySelectedStoreItem();
        }

        private void ResetButton_OnClicked()
        {
            KORHelper.PlayAudioForUIClickPrimary();

            Configuration.Debugger.Log(
               "Reset! This deletes the local player, " +
               "creates a new one, and restarts the game. " +
               "Do not use this in production.");

            ExampleProjectHacks.ClearDeviceUsersAndReloadGame();
        }

        private void BackButton_OnClicked()
        {
            KORHelper.PlayAudioForUIClickPrimary();

            StartCoroutine(KORHelper.LoadScene_Coroutine(_configuration.IntroSceneName,
               _configuration.DelayBeforeLoadScene));
        }
    }
}
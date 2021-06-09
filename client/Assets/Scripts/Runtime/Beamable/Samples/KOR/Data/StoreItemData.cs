using Beamable.Api.Payments;
using Beamable.Samples.KOR.CustomContent;

namespace Beamable.Samples.KOR.Data
{
    /// <summary>
    /// Renders one item row of the store.
    /// </summary>
    public class StoreItemData
    {
        //  Properties -----------------------------------
        public string Title { get { return _title; } }
        public PlayerListingView PlayerListingView { get { return _playerListingView; } }
        public KORItemContent KORItemContent { get { return _korItemContent; } }

        //  Fields ---------------------------------------
        private KORItemContent _korItemContent = null;
        private string _title = null;
        private PlayerListingView _playerListingView = null;
        
        //  Constructor ---------------------------------
        public StoreItemData (string title, KORItemContent korItemContent, PlayerListingView playerListingView) 
        {
            _title = title;
            _korItemContent = korItemContent;
            
            //This is only for STORE items not for INVENTORY items
            _playerListingView = playerListingView;
            
        }
    }
}
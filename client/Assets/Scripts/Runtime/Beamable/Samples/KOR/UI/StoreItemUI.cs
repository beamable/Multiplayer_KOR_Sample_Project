using Beamable.Samples.KOR.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Samples.KOR.UI
{
    /// <summary>
    /// Renders one item row of the store.
    /// </summary>
    public class StoreItemUI : MonoBehaviour
    {
        //  Properties -----------------------------------
        public StoreItemData StoreItemData { get { return _storeItemData; } set { _storeItemData = value; Render();}}
        public Button Button { get { return _button; }}

        //  Fields ---------------------------------------
        private StoreItemData _storeItemData = null;

        [SerializeField]
        private TMP_Text _titleText = null;
        
        [SerializeField]
        private Button _button = null;
        
        [SerializeField]
        private Image _iconImage = null;
        
        //  Unity Methods ---------------------------------
        public void Render()
        {
            _titleText.text = _storeItemData.Title;
            
            //Hide image, load texture, show image
            KORHelper.AddressablesLoadAssetAsync<Texture2D>(_storeItemData.KORItemContent.icon, _iconImage);
        }
    }
}
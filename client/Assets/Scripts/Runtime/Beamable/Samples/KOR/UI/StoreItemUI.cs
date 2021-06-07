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
        public TMP_Text TitleText { get { return _titleText; } set { _titleText = value; }}
        public Button Button { get { return _button; }}
        public Image IconIconImage { get { return _iconImage; } set { _iconImage = value; Render();}}

        //  Fields ---------------------------------------
        [SerializeField]
        private TMP_Text _titleText = null;
        
        [SerializeField]
        private Button _button = null;
        
        [SerializeField]
        private Image _iconImage = null;
        
        //  Unity Methods ---------------------------------
        public void Render()
        { 
        }
    }
}
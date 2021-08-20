using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Samples.Core.UI.DialogSystem
{
    /// <summary>
    ///  Renders a button in dialog
    /// </summary>
    public class DialogButtonUI : MonoBehaviour
    {
        //  Properties ---------------------------------------
        public TMP_Text Text { get { return _text;  }}
        public Button Button { get { return _button; }}
        
        //  Fields ---------------------------------------
        
        [SerializeField]
        private TMP_Text _text = null;
        
        [SerializeField]
        private Button _button;
        
        //  Fields ---------------------------------------
        
        //  Unity Methods --------------------------------

        //  Other Methods --------------------------------
    }
}
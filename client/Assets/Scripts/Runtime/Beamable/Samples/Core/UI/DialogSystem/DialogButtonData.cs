using UnityEngine.Events;

namespace Beamable.Samples.Core.UI.DialogSystem
{
    /// <summary>
    ///  Defines each button in the dialog
    /// </summary>
    public class DialogButtonData
    {
        //  Properties ---------------------------------------
        public string Text { get { return _text;  }}
        public UnityAction OnButtonClicked { get { return _onButtonClicked; }}
        
        //  Fields ---------------------------------------
        private readonly string _text = "";
        private readonly UnityAction _onButtonClicked = null;

        //  Constructor ---------------------------------------

        public DialogButtonData(string text, UnityAction onButtonClicked)
        {
            _text = text;
            _onButtonClicked = onButtonClicked;
        }
        
        //  Other Methods ---------------------------------------
    }
}
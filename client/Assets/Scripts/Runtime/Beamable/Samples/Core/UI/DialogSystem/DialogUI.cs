using System.Collections.Generic;
using Beamable.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Samples.Core.UI.DialogSystem
{
    /// <summary>
    ///  Renders one dialog
    /// </summary>
    public class DialogUI : MonoBehaviour
    {
        //  Properties -----------------------------------
        public List<DialogButtonData> DialogButtonDatas { get { return _dialogButtonDatas; } set { _dialogButtonDatas = value; Render(); }}
        public TMP_Text TitleText { get { return _titleText; } }
        public TMP_Text BodyText { get { return _bodyText; } }
        public DialogButtonUI DialogButtonUIPrefab { get { return _dialogButtonUIPrefab; }}
        
        //  Fields ---------------------------------------
        private List<DialogButtonData> _dialogButtonDatas = null;
        
        [Header("Base Fields")]
        [SerializeField]
        private DialogButtonUI _dialogButtonUIPrefab;
                
        [SerializeField]
        private Image _backgroundImage = null;

        [SerializeField]
        private TMP_Text _titleText = null;

        [SerializeField]
        private TMP_Text _bodyText = null;

        [SerializeField]
        private HorizontalLayoutGroup _buttonsHorizontalLayoutGroup = null;

        [Header("Cosmetics")]
        [SerializeField]
        private Color _backgroundColor = new Color(0, 0, 0, 20);
        

        //  Unity Methods --------------------------------
        private void OnValidate()
        {
            if (_backgroundImage == null)
            {
                return;
            }

            _backgroundImage.color = _backgroundColor;
        }

        //  Other Methods --------------------------------
        private void Render()
        {
            if ((_dialogButtonDatas == null || _dialogButtonDatas.Count == 0) &&
                _dialogButtonUIPrefab == null)
            {
                Debug.LogError($"Render() failed. Arguments invalid.");
                return;
            }

            // There may be some some buttons remaining for readability at edit time
            // Clear them out
            _buttonsHorizontalLayoutGroup.transform.ClearChildren();
            
            //Attach only children based on the prefab chosen and the data present
            foreach (DialogButtonData dialogButtonData in _dialogButtonDatas)
            {
                DialogButtonUI dialogButtonUI = Instantiate(_dialogButtonUIPrefab, _buttonsHorizontalLayoutGroup.transform);
                dialogButtonUI.transform.SetAsLastSibling();
                dialogButtonUI.Button.onClick.AddListener(dialogButtonData.OnButtonClicked);
                dialogButtonUI.Text.text = dialogButtonData.Text;
            }
        }
    }
}
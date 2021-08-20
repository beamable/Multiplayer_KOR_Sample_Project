using System;
using System.Collections.Generic;
using UnityEngine;

namespace Beamable.Samples.Core.UI.DialogSystem
{
    /// <summary>
    ///  Manages the dialogs
    ///
    /// NOTE: This allows for only 0...1 dialog at a time. 
    /// </summary>
    [Serializable]
    public class DialogSystem
    {
        //  Properties ---------------------------------------
        public DialogUI DialogUIPrefab { get { return _dialogUIPrefab; }}
        
        //  Fields ---------------------------------------
        [SerializeField] 
        private GameObject _dialogParent = null;
        
        [SerializeField] 
        public DialogUI _dialogUIPrefab = null;

        private DialogUI _currentDialogUI = null;

        //  Constructor ---------------------------------------
        
        //  Other Methods ---------------------------------------
        public void ShowDialogBox<T>(T dialogUIPrefab, 
            string titleText,
            string bodyText,
            List<DialogButtonData> dialogButtonDatas) where  T : DialogUI
        {
            _currentDialogUI = GameObject.Instantiate<T>(dialogUIPrefab, _dialogParent.transform);
            _currentDialogUI.DialogButtonDatas = dialogButtonDatas;
            _currentDialogUI.TitleText.text = titleText;
            _currentDialogUI.BodyText.text = bodyText;
        }

        public void HideDialogBox()
        {
            if (_currentDialogUI == null)
            {
                return;
            }
            
            GameObject.Destroy(_currentDialogUI.gameObject);
            _currentDialogUI = null;
        }
    }
}
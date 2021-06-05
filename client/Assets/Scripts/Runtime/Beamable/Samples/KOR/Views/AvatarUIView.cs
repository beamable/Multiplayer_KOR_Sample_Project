using Beamable.Samples.KOR.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Samples.KOR.Views
{
    /// <summary>
    /// Handles the audio/graphics rendering logic: Avatar UI
    /// </summary>
    public class AvatarUIView : MonoBehaviour
    {
        //  Properties -----------------------------------
        public AvatarData AvatarData { set { _avatarData = value; Render(); } get { return _avatarData; } }

        public AvatarSlotData AvatarSlotData { set { _avatarSlotData = value; Render(); } get { return _avatarSlotData; } }

        public bool IsLocalPlayer { set { _isLocalPlayer = value; Render(); } get { return _isLocalPlayer; } }
        public bool IsInGame { set { _isInGame = value; Render(); } get { return _isInGame; } }
        public string Name { set { _name = value; Render(); } get { return _name; } }
        public int Health { set { _health = value; Render(); } get { return _health; } }

        //  Fields ---------------------------------------
        [SerializeField]
        private Image _backgroundImage = null;

        [SerializeField]
        private TMP_Text _text = null;

        private AvatarData _avatarData = null;
        private AvatarSlotData _avatarSlotData = null;
        private string _name = "";
        private int _health = 0;
        private bool _isLocalPlayer = false;
        private bool _isInGame = false;

        //  Other Methods   ------------------------------
        public void Render()
        {
            if (_avatarSlotData != null)
                _backgroundImage.color = _avatarSlotData.Color;

            string location = KORConstants.AvatarUIView_Remote;
            if (_isLocalPlayer)
            {
                location = KORConstants.AvatarUIView_Local;
            }

            if (_isInGame)
            {
                _text.text = $"{Name} ({_health}%)\n{location}";
            }
            else
            {
                _text.text = $"{Name}\n{KORConstants.AvatarUIView_Offline}";
            }
        }
    }
}
using System;
using Beamable.Examples.Features.Multiplayer.Core;
using Beamable.Samples.KOR.Behaviours;
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

        public AvatarSlotData AvatarSlotData { set { _avatarSlotData = value; Render(); } get { return _avatarSlotData; } }

        public Player Player => _player;
        public SpawnablePlayer SpawnablePlayer => _spawnablePlayer;

        //  Fields ---------------------------------------
        [SerializeField]
        private Image _backgroundImage = null;

        [SerializeField]
        private TMP_Text _text = null;

        [SerializeField]
        private Player _player;

        [SerializeField]
        private SpawnablePlayer _spawnablePlayer;

        private AvatarData _avatarData = null;
        private AvatarSlotData _avatarSlotData = null;
        private string _name = "";
        private bool _isLocalPlayer = false;
        private bool _isInGame = false;

        // Unity Methods   -------------------------------
        private void Update()
        {
            if (!_player) return;

            Render();
        }

        //  Other Methods   ------------------------------
        public void Set(Player player, SpawnablePlayer spawnablePlayer)
        {
            _player = player;
            _spawnablePlayer = spawnablePlayer;

            _isInGame = true;
            _isLocalPlayer = _spawnablePlayer.DBID == NetworkController.Instance.LocalDbid;
            _name = _spawnablePlayer.PlayerAlias;
        }

        public void Render()
        {
            if (_avatarSlotData != null)
                _backgroundImage.color = _avatarSlotData.Color;

            string location = KORConstants.AvatarUIView_Remote;
            if (_isLocalPlayer)
            {
                location = KORConstants.AvatarUIView_Local;
            }

            const int maxNameLength = 10;
            const string ellipsis = "...";
            string truncatedName = _name.Length > maxNameLength ? _name.Substring(0, maxNameLength - ellipsis.Length) + ellipsis : _name;

            if (_isInGame)
            {
                _text.text = $"{truncatedName} ({_player.HealthBehaviour.HealthPercentage}%)\n{location}";
            }
            else
            {
                _text.text = $"{truncatedName}\n{KORConstants.AvatarUIView_Offline}";
            }
        }
    }
}
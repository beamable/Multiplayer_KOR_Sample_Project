using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beamable.Examples.Features.Multiplayer.Core;
using Beamable.Samples.Core;
using Beamable.Samples.KOR.Behaviours;
using Beamable.Core.Debugging;
using Beamable.Samples.KOR.Data;
using Beamable.Samples.KOR.Multiplayer;
using Beamable.Samples.KOR.Multiplayer.Events;
using Beamable.Samples.KOR.UI;
using Beamable.Samples.KOR.Views;
using UnityEngine;
using static Beamable.Samples.KOR.CharacterManager;
using Beamable.Samples.KOR.Animation;

namespace Beamable.Samples.KOR
{
    /// <summary>
    /// Handles the main scene logic: Game
    /// </summary>
    public class GameSceneManager : SingletonMonobehavior<GameSceneManager>
    {
        //  Properties -----------------------------------
        public GameUIView GameUIView { get { return _gameUIView; } }

        public Configuration Configuration { get { return _configuration; } }

        public List<SpawnPointBehaviour> AvailableSpawnPoints;

        //  Fields ---------------------------------------
        private IBeamableAPI _beamableAPI = null;

        [SerializeField]
        private Configuration _configuration = null;

        [SerializeField]
        private GameUIView _gameUIView = null;

        private Attributes _ownAttributes = null;

        private List<SpawnablePlayer> _spawnablePlayers = new List<SpawnablePlayer>();
        private List<SpawnPointBehaviour> _unusedSpawnPoints = new List<SpawnPointBehaviour>();
        private HashSet<long> _dbidReadyReceived = new HashSet<long>();
        private bool _hasSpawned = false;

        //  Unity Methods   ------------------------------
        protected void Start()
        {
            for (int i = 0; i < 6; i++)
                _gameUIView.AvatarUIViews[i].GetComponent<CanvasGroup>().alpha = 0.0f;

            _gameUIView.BackButton.onClick.AddListener(BackButton_OnClicked);
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
            _beamableAPI = await Beamable.API.Instance;
            await RuntimeDataStorage.Instance.CharacterManager.BootstrapTask;
            _ownAttributes = await RuntimeDataStorage.Instance.CharacterManager.GetChosenPlayerAttributes();

            // Do this after calling "Beamable.API.Instance" for smoother UI
            _gameUIView.CanvasGroupsDoFadeIn();

            // Set defaults if scene was loaded directly
            if (RuntimeDataStorage.Instance.TargetPlayerCount == KORConstants.UnsetValue)
            {
                DebugLog(KORHelper.GetSceneLoadingMessage(gameObject.scene.name, true));
                RuntimeDataStorage.Instance.TargetPlayerCount = 1;
                RuntimeDataStorage.Instance.CurrentPlayerCount = 1;
                RuntimeDataStorage.Instance.LocalPlayerDbid = _beamableAPI.User.id;
                RuntimeDataStorage.Instance.RoomId = KORMatchmaking.GetRandomRoomId();
            }
            else
            {
                DebugLog(KORHelper.GetSceneLoadingMessage(gameObject.scene.name, false));
            }

            // Set the ActiveSimGameType. This happens in 2+ spots to handle direct scene loading
            if (RuntimeDataStorage.Instance.IsSinglePlayerMode)
                RuntimeDataStorage.Instance.ActiveSimGameType = await _configuration.SimGameType01Ref.Resolve();
            else
                RuntimeDataStorage.Instance.ActiveSimGameType = await _configuration.SimGameType02Ref.Resolve();

            // Initialize ECS
            SystemManager.StartGameSystems();

            // Show the player's attributes in the UI of this scene

            _gameUIView.AttributesPanelUI.Attributes = _ownAttributes;

            // Initialize Networking
            await NetworkController.Instance.Init();

            // Set Available Spawns
            _unusedSpawnPoints = AvailableSpawnPoints.ToList();

            NetworkController.Instance.Log.CreateNewConsumer(HandleNetworkUpdate);
            // Optional: Stuff to use later when player moves are incoming
            long tbdIncomingPlayerDbid = _beamableAPI.User.id; // test value;
            DebugLog($"MinPlayerCount = {RuntimeDataStorage.Instance.MinPlayerCount}");
            DebugLog($"MaxPlayerCount = {RuntimeDataStorage.Instance.MaxPlayerCount}");
            DebugLog($"CurrentPlayerCount = {RuntimeDataStorage.Instance.CurrentPlayerCount}");
            DebugLog($"LocalPlayerDbid = {RuntimeDataStorage.Instance.LocalPlayerDbid}");
            DebugLog($"IsLocalPlayerDbid = {RuntimeDataStorage.Instance.IsLocalPlayerDbid(tbdIncomingPlayerDbid)}");
            DebugLog($"IsSinglePlayerMode = {RuntimeDataStorage.Instance.IsSinglePlayerMode}");

            // Optional: Show queueable status text onscreen
            SetStatusText(KORConstants.GameUIView_Playing, TMP_BufferedText.BufferedTextMode.Immediate);

            // Optional: Add easily configurable delays
            await Task.Delay(TimeSpan.FromSeconds(_configuration.DelayGameBeforeMove));

            // Optional: Play sound
            //SoundManager.Instance.PlayAudioClip(SoundConstants.Click01);

            // Optional: Render color and text of avatar ui
            _gameUIView.AvatarViews.Clear();
        }

        private class SpawnablePlayer
        {
            public SpawnablePlayer(long dbid, SpawnPointBehaviour spawnPointBehaviour)
            {
                _dbid = dbid;
                _spawnPointBehaviour = spawnPointBehaviour;
            }

            public long DBID { get { return _dbid; } }
            public SpawnPointBehaviour SpawnPointBehaviour { get { return _spawnPointBehaviour; } }

            public Character ChosenCharacter { get; set; }

            public Attributes Attributes { get { return _attributes; } set { _attributes = value; } }

            public string PlayerAlias { get { return _playerAlias; } set { _playerAlias = value; } }

            private long _dbid = -1;
            private SpawnPointBehaviour _spawnPointBehaviour;
            private Attributes _attributes;
            private string _playerAlias;
        }

        public async void OnPlayerJoined(PlayerJoinedEvent joinEvent)
        {
            if (_spawnablePlayers.Find(i => i.DBID == joinEvent.PlayerDbid) != null)
                return;

            var spawnIndex = NetworkController.Instance.rand.Next(0, _unusedSpawnPoints.Count);
            var spawnPoint = _unusedSpawnPoints[spawnIndex];
            _unusedSpawnPoints.Remove(spawnPoint);

            SpawnablePlayer newPlayer = new SpawnablePlayer(joinEvent.PlayerDbid, spawnPoint);
            _spawnablePlayers.Add(newPlayer);
            await RuntimeDataStorage.Instance.CharacterManager.BootstrapTask;
            newPlayer.ChosenCharacter = await RuntimeDataStorage.Instance.CharacterManager.GetChosenCharacterByDBID(joinEvent.PlayerDbid);
            string alias = await RuntimeDataStorage.Instance.CharacterManager.GetPlayerAliasByDBID(joinEvent.PlayerDbid);

            Debug.Log($"alias from joinEvent dbid={joinEvent.PlayerDbid} alias={alias}");

            if (joinEvent.PlayerDbid == NetworkController.Instance.LocalDbid)
                NetworkController.Instance.SendNetworkMessage(new ReadyEvent(_ownAttributes, alias));
        }

        private void OnPlayerReady(ReadyEvent readyEvt)
        {
            Configuration.Debugger.Log($"Getting ready for dbid={readyEvt.PlayerDbid}"
                + $" attributes move/charge={readyEvt.aggregateMovementSpeed}/{readyEvt.aggregateChargeSpeed}", DebugLogLevel.Verbose);

            _dbidReadyReceived.Add(readyEvt.PlayerDbid);

            SpawnablePlayer sp = _spawnablePlayers.Find(i => i.DBID == readyEvt.PlayerDbid);
            sp.Attributes = new Attributes(readyEvt.aggregateChargeSpeed, readyEvt.aggregateMovementSpeed);
            sp.PlayerAlias = readyEvt.playerAlias;

            Debug.Log($"alias from readyEvt dbid={readyEvt.PlayerDbid} alias={sp.PlayerAlias}");

            Configuration.Debugger.Log($"OnPlayerReady Players={_dbidReadyReceived.Count}/{RuntimeDataStorage.Instance.CurrentPlayerCount}", DebugLogLevel.Verbose);
            if (!_hasSpawned && _dbidReadyReceived.Count == RuntimeDataStorage.Instance.CurrentPlayerCount)
            {
                _hasSpawned = true;
                SpawnAllPlayersAtOnce();
            }
        }

        private void SpawnAllPlayersAtOnce()
        {
            List<CanvasGroup> avatarUiCanvasGroups = new List<CanvasGroup>();

            for (int p = 0; p < _spawnablePlayers.Count; p++)
            {
                SpawnablePlayer sp = _spawnablePlayers[p];

                Configuration.Debugger.Log($"DBID={sp.DBID} Spawning character={sp.ChosenCharacter.CharacterContentObject.ContentName}"
                    + $" attributes move/charge={sp.Attributes.MovementSpeed}/{sp.Attributes.ChargeSpeed}", DebugLogLevel.Verbose);

                Debug.Log($"playerAlias={sp.PlayerAlias}");

                AvatarView avatarView = GameObject.Instantiate<AvatarView>(sp.ChosenCharacter.AvatarViewPrefab);
                avatarView.transform.SetPhysicsPosition(sp.SpawnPointBehaviour.transform.position);

                Player player = avatarView.gameObject.GetComponent<Player>();
                player.SetAlias(sp.PlayerAlias);

                avatarView.SetForPlayer(sp.DBID);
                _gameUIView.AvatarViews.Add(avatarView);

                if (sp.DBID == NetworkController.Instance.LocalDbid)
                    avatarView.gameObject.GetComponent<AvatarMotionBehaviour>().PreviewBehaviour = null;
                else
                    avatarView.gameObject.GetComponent<PlayerInputBehaviour>().enabled = false;

                AvatarMotionBehaviour amb = avatarView.gameObject.GetComponent<AvatarMotionBehaviour>();
                amb.Attributes = sp.Attributes;

                _gameUIView.AvatarUIViews[p].Health = 100;
                _gameUIView.AvatarUIViews[p].IsInGame = true;
                _gameUIView.AvatarUIViews[p].Name = sp.PlayerAlias;
                _gameUIView.AvatarUIViews[p].IsLocalPlayer = sp.DBID == NetworkController.Instance.LocalDbid;
                _gameUIView.AvatarUIViews[p].Render();
                avatarUiCanvasGroups.Add(_gameUIView.AvatarUIViews[p].GetComponent<CanvasGroup>());

                player.SetHealth(70);
            }

            TweenHelper.CanvasGroupsDoFade(avatarUiCanvasGroups, 0.0f, 1.0f, 1.0f, 0.0f, 0.0f);
        }

        public void HandleNetworkUpdate(TimeUpdate update)
        {
            foreach (var evt in update.Events)
            {
                HandleNetworkEvent(evt);
            }
        }
        
        
        
        /// <summary>
        /// TODO: Remove this method
        /// </summary>
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("TODO: Remove this method");
                _gameUIView.CinemachineImpulseSource.GenerateImpulse();
            }
        }

        public void HandleNetworkEvent(KOREvent korEvent)
        {
            switch (korEvent)
            {
                case ReadyEvent readyEvt:
                    OnPlayerReady(readyEvt);
                    break;

                case PlayerJoinedEvent joinEvt:
                    OnPlayerJoined(joinEvt);
                    break;
            }
        }

        /// <summary>
        /// Render UI text
        /// </summary>
        /// <param name="message"></param>
        /// <param name="statusTextMode"></param>
        public void SetStatusText(string message, TMP_BufferedText.BufferedTextMode statusTextMode)
        {
            _gameUIView.BufferedText.SetText(message, statusTextMode);
        }

        //  Event Handlers -------------------------------
        private void BackButton_OnClicked()
        {
            KORHelper.PlayAudioForUIClickPrimary();

            // Clean up manager
            _spawnablePlayers.Clear();
            _unusedSpawnPoints.Clear();
            _dbidReadyReceived.Clear();
            _hasSpawned = false;
            NetworkController.Instance.Cleanup();

            // Destroy ECS
            SystemManager.DestroyGameSystems();

            // Change scenes
            StartCoroutine(KORHelper.LoadScene_Coroutine(_configuration.IntroSceneName,
               _configuration.DelayBeforeLoadScene));
        }
    }
}
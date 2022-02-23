using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beamable.Examples.Features.Multiplayer.Core;
using Beamable.Samples.KOR.Behaviours;
using Beamable.Samples.KOR.Data;
using Beamable.Samples.KOR.Multiplayer;
using Beamable.Samples.KOR.Multiplayer.Events;
using Beamable.Samples.KOR.Views;
using UnityEngine;
using Beamable.Experimental.Api.Sim;
using Beamable.Samples.Core.Debugging;
using Beamable.Samples.Core.UI;
using Beamable.Samples.Core.UI.DialogSystem;
using Beamable.Samples.Core.Utilities;

namespace Beamable.Samples.KOR
{
    /// <summary>
    /// Handles the main scene logic: Game
    /// </summary>
    public class GameSceneManager : MonoBehaviour
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
            await RuntimeDataStorage.Instance.CharacterManager.Initialize();
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
                RuntimeDataStorage.Instance.MatchId = KORMatchmaking.GetRandomMatchId();
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

        public async void OnPlayerJoined(PlayerJoinedEvent joinEvent)
        {
            if (_spawnablePlayers.Find(i => i.DBID == joinEvent.PlayerDbid) != null)
                return;

            var spawnIndex = NetworkController.Instance.rand.Next(0, _unusedSpawnPoints.Count);
            var spawnPoint = _unusedSpawnPoints[spawnIndex];
            _unusedSpawnPoints.Remove(spawnPoint);

            SpawnablePlayer newPlayer = new SpawnablePlayer(joinEvent.PlayerDbid, spawnPoint);
            _spawnablePlayers.Add(newPlayer);
            await RuntimeDataStorage.Instance.CharacterManager.Initialize();
            newPlayer.ChosenCharacter = await RuntimeDataStorage.Instance.CharacterManager.GetChosenCharacterByDBID(joinEvent.PlayerDbid);
            string alias = await RuntimeDataStorage.Instance.CharacterManager.GetPlayerAliasByDBID(joinEvent.PlayerDbid);

            DebugLog($"alias from joinEvent dbid={joinEvent.PlayerDbid} alias={alias}");

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

            Configuration.Debugger.Log($"alias from readyEvt dbid={readyEvt.PlayerDbid} alias={sp.PlayerAlias}");

            Configuration.Debugger.Log($"OnPlayerReady Players={_dbidReadyReceived.Count}/{RuntimeDataStorage.Instance.CurrentPlayerCount}", DebugLogLevel.Verbose);
            if (!_hasSpawned && _dbidReadyReceived.Count == RuntimeDataStorage.Instance.CurrentPlayerCount)
            {
                _hasSpawned = true;
                SpawnAllPlayersAtOnce();
                StartGameTimer();
            }
        }

        private void StartGameTimer()
        {
            GameUIView.GameTimerBehaviour.StartMatch();
            GameUIView.GameTimerBehaviour.OnGameOver += async () =>
            {
                // TODO: score the players, and end the game.
                Debug.Log("Game over!");

                var uis = FindObjectsOfType<AvatarUIView>();
                var validUis = uis.Where(ui => ui.Player).ToList();
                validUis.Sort((a, b) => a.SpawnablePlayer.DBID > b.SpawnablePlayer.DBID ? 1 : -1);
                validUis.Sort((a, b) => a.Player.HealthBehaviour.Health > b.Player.HealthBehaviour.Health ? -1 : 1);

                var scores = validUis.Select(ui => new PlayerResult
                {
                    playerId = ui.SpawnablePlayer.DBID,
                    score = ui.Player.HealthBehaviour.Health,
                }).ToArray();
                var selfRank = 0;
                var selfScore = scores[0];
                for (var i = 0; i < scores.Length; i++)
                {
                    scores[i].rank = i;
                    if (scores[i].playerId == NetworkController.Instance.LocalDbid)
                    {
                        selfRank = i;
                        selfScore = scores[i];
                    }
                }

                foreach (var motionBehaviour in FindObjectsOfType<AvatarMotionBehaviour>())
                {
                    motionBehaviour.Stop();
                    motionBehaviour.enabled = false;
                }

                foreach (var inputBehaviour in FindObjectsOfType<PlayerInputBehaviour>())
                {
                    inputBehaviour.enabled = false;
                }

                var results = await NetworkController.Instance.ReportResults(scores);

                var isWinner = selfRank == 0;
                var earnings = string.Join(",", results.currenciesGranted.Select(grant => $"{grant.amount}x{grant.symbol}"));
                var earningsBody = string.IsNullOrWhiteSpace(earnings)
                    ? "nothing"
                    : earnings;
                var body = "You came in place: " + (selfRank + 1) + ". You earned " + earningsBody;
                _gameUIView.DialogSystem.ShowDialogBox<DialogUI>(

                    // Renders this prefab. DUPLICATE this prefab and drag
                    // into _storeUIView to change layout
                    _gameUIView.DialogSystem.DialogUIPrefab,

                    // Set Text
                    isWinner
                        ? KORConstants.Dialog_GameOver_Victory
                        : KORConstants.Dialog_GameOver_Defeat,
                    body,

                    // Create zero or more buttons
                    new List<DialogButtonData>
                    {
                        new DialogButtonData(KORConstants.Dialog_Ok, () =>
                        {
                            KORHelper.PlayAudioForUIClickPrimary();
                            _gameUIView.DialogSystem.HideDialogBox();

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
                        })
                    });
            };
        }

        private void SpawnAllPlayersAtOnce()
        {
            List<CanvasGroup> avatarUiCanvasGroups = new List<CanvasGroup>();

            for (int p = 0; p < _spawnablePlayers.Count; p++)
            {
                SpawnablePlayer sp = _spawnablePlayers[p];

                Configuration.Debugger.Log($"DBID={sp.DBID} Spawning character={sp.ChosenCharacter.CharacterContentObject.ContentName}"
                                           + $" attributes move/charge={sp.Attributes.MovementSpeed}/{sp.Attributes.ChargeSpeed}", DebugLogLevel.Verbose);

                DebugLog($"playerAlias={sp.PlayerAlias}");

                AvatarView avatarView = GameObject.Instantiate<AvatarView>(sp.ChosenCharacter.AvatarViewPrefab);
                avatarView.transform.SetPhysicsPosition(sp.SpawnPointBehaviour.transform.position);

                var player = avatarView.gameObject.GetComponent<Behaviours.Player>();
                player.SetAlias(sp.PlayerAlias);

                avatarView.SetForPlayer(sp.DBID);
                _gameUIView.AvatarViews.Add(avatarView);

                if (sp.DBID == NetworkController.Instance.LocalDbid)
                    avatarView.gameObject.GetComponent<AvatarMotionBehaviour>().PreviewBehaviour = null;
                else
                    avatarView.gameObject.GetComponent<PlayerInputBehaviour>().enabled = false;

                AvatarMotionBehaviour amb = avatarView.gameObject.GetComponent<AvatarMotionBehaviour>();
                amb.Attributes = sp.Attributes;

                _gameUIView.AvatarUIViews[p].Set(player, sp);
                _gameUIView.AvatarUIViews[p].Render();
                avatarUiCanvasGroups.Add(_gameUIView.AvatarUIViews[p].GetComponent<CanvasGroup>());
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
            KORHelper.PlayAudioForUIClickBack();

            _gameUIView.DialogSystem.ShowDialogBox<DialogUI>(

                // Renders this prefab. DUPLICATE this prefab and drag
                // into _storeUIView to change layout
                _gameUIView.DialogSystem.DialogUIPrefab,

                // Set Text
                KORConstants.Dialog_AreYouSure,
                "This will end your game.",

                // Create zero or more buttons
                new List<DialogButtonData>
                {
                    new DialogButtonData(KORConstants.Dialog_Ok, () =>
                    {
                        KORHelper.PlayAudioForUIClickPrimary();
                        _gameUIView.DialogSystem.HideDialogBox();

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
                    }),
                    new DialogButtonData(KORConstants.Dialog_Cancel, () =>
                    {
                        KORHelper.PlayAudioForUIClickSecondary();
                        _gameUIView.DialogSystem.HideDialogBox();
                    })
                });
        }
    }
}
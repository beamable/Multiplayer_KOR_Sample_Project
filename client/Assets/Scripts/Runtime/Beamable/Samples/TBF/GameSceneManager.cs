using Beamable.Samples.TBF.Audio;
using Beamable.Samples.TBF.Data;
using Beamable.Samples.TBF.Multiplayer;
using Beamable.Samples.TBF.Multiplayer.Events;
using Beamable.Samples.TBF.Views;
using System;
using UnityEngine;
using static Beamable.Samples.TBF.UI.TMP_BufferedText;

namespace Beamable.Samples.TBF
{

   /// <summary>
   /// List of all users' moves
   /// </summary>
   public enum GameMoveType
   {
      Null,
      High,     // Like "Rock"
      Mid,   // Like "Paper"
      Low       // Like "Scissors"
   }

   /// <summary>
   /// Handles the main scene logic: Game
   /// </summary>
   public class GameSceneManager : MonoBehaviour
   {
      //  Properties -----------------------------------
      public GameUIView GameUIView { get { return _gameUIView; } }
      public GameProgressData GameProgressData { get { return _gameProgressData; } set { _gameProgressData = value; } }
      public Configuration Configuration { get { return _configuration; } }
      public TBFMultiplayerSession MultiplayerSession { get { return _multiplayerSession; } }
      public RemotePlayerAI RemotePlayerAI { get { return _remotePlayerAI; } set { _remotePlayerAI = value; } }

      //  Fields ---------------------------------------

      [SerializeField]
      private Configuration _configuration = null;

      [SerializeField]
      private GameUIView _gameUIView = null;

      private IBeamableAPI _beamableAPI = null;
      private TBFMultiplayerSession _multiplayerSession;
      private GameProgressData _gameProgressData;
      private RemotePlayerAI _remotePlayerAI;
      private GameStateHandler _gameStateHandler;


      //  Unity Methods   ------------------------------
      protected void Start()
      {
         _gameUIView.BackButton.onClick.AddListener(BackButton_OnClicked);
         _gameUIView.MoveButton_01.onClick.AddListener(MoveButton_01_OnClicked);
         _gameUIView.MoveButton_02.onClick.AddListener(MoveButton_02_OnClicked);
         _gameUIView.MoveButton_03.onClick.AddListener(MoveButton_03_OnClicked);

         foreach (AvatarUIView avatarUIView in _gameUIView.AvatarUIViews)
         {
            avatarUIView.HealthBarView.OnValueChanged += HealthBarView_OnValueChanged;
         }

         _gameUIView.AvatarUIViews[TBFConstants.PlayerIndexLocal].HealthBarView.Value = 100;
         _gameUIView.AvatarUIViews[TBFConstants.PlayerIndexRemote].HealthBarView.Value = 100;

         //
         _gameStateHandler = new GameStateHandler(this);
         SetupBeamable();
      }



      protected void Update()
      {
         _multiplayerSession?.Update();
      }

      //  Other Methods  -----------------------------
      private void DebugLog(string message)
      {
         if (TBFConstants.IsDebugLogging)
         {
            Debug.Log(message);
         }
      }


      private async void SetupBeamable()
      {
         await _gameStateHandler.SetGameState(GameState.Loading);

         await Beamable.API.Instance.Then(async de =>
         {
            await _gameStateHandler.SetGameState (GameState.Loaded);

            try
            {
               _beamableAPI = de;

               if (!RuntimeDataStorage.Instance.IsMatchmakingComplete)
               {
                  DebugLog($"Scene '{gameObject.scene.name}' was loaded directly. That is ok. Setting defaults.");
                  RuntimeDataStorage.Instance.LocalPlayerDbid = _beamableAPI.User.id;
                  RuntimeDataStorage.Instance.TargetPlayerCount = 1;
                  RuntimeDataStorage.Instance.RoomId = TBFMatchmaking.GetRandomRoomId();
               }

               _multiplayerSession = new TBFMultiplayerSession(
                  RuntimeDataStorage.Instance.LocalPlayerDbid,
                  RuntimeDataStorage.Instance.TargetPlayerCount,
                  RuntimeDataStorage.Instance.RoomId) ;

               await _gameStateHandler.SetGameState(GameState.Initializing);

               _multiplayerSession.OnInit += MultiplayerSession_OnInit;
               _multiplayerSession.OnConnect += MultiplayerSession_OnConnect;
               _multiplayerSession.OnDisconnect += MultiplayerSession_OnDisconnect;
               _multiplayerSession.Initialize();

            }
            catch (Exception)
            {
               SetStatusText(TBFHelper.InternetOfflineInstructionsText, BufferedTextMode.Immediate);
            }
         });
      }

      /// <summary>
      /// Render UI text
      /// </summary>
      /// <param name="message"></param>
      /// <param name="statusTextMode"></param>
      public void SetStatusText(string message, BufferedTextMode statusTextMode)
      {
         _gameUIView.BufferedText.SetText(message, statusTextMode);
      }

      /// <summary>
      /// Render UI text
      /// </summary>
      /// <param name="message"></param>
      public void SetRoundText(int roundNumber)
      {
         _gameUIView.RoundText.text = string.Format(TBFConstants.RoundText, roundNumber, _configuration.GameRoundsTotal);
      }


      private void BindPlayerDbidToEvents(long playerDbid, bool isBinding)
      {
         if (isBinding)
         {
            string origin = playerDbid.ToString();
            _multiplayerSession.On<GameStartEvent>(origin, MultiplayerSession_OnGameStartEvent);
            _multiplayerSession.On<GameMoveEvent>(origin, MultiplayerSession_OnGameMoveEvent);
         }
         else
         {
            _multiplayerSession.Remove<GameStartEvent>(MultiplayerSession_OnGameStartEvent);
            _multiplayerSession.Remove<GameMoveEvent>(MultiplayerSession_OnGameMoveEvent);
         }
      }

      private void SendGameMoveEventSave(GameMoveType gameMoveType)
      {
         if (_gameStateHandler.GameState == GameState.RoundPlayerMoving)
         {
            _gameUIView.MoveButtonsCanvasGroup.interactable = false;
            SoundManager.Instance.PlayAudioClip(SoundConstants.Click02);

            _multiplayerSession.SendEvent<GameMoveEvent>(
               new GameMoveEvent(gameMoveType));
         }
      }

      //  Event Handlers -------------------------------
      private void HealthBarView_OnValueChanged(int oldValue, int newValue)
      {
         if (newValue < oldValue)
         {
            // Play "damage" sound
            SoundManager.Instance.PlayAudioClip(SoundConstants.HealthBarDecrement);
         }
      }

      private void BackButton_OnClicked()
      {
         //Change scenes
         StartCoroutine(TBFHelper.LoadScene_Coroutine(_configuration.IntroSceneName,
            _configuration.DelayBeforeLoadScene));
      }


      private void MoveButton_01_OnClicked()
      {
         SendGameMoveEventSave(GameMoveType.High);
      }


      private void MoveButton_02_OnClicked()
      {
         SendGameMoveEventSave(GameMoveType.Mid);
      }


      private void MoveButton_03_OnClicked()
      {
         SendGameMoveEventSave(GameMoveType.Low);
      }


      private async void MultiplayerSession_OnInit(System.Random random)
      {
         await _gameStateHandler.SetGameState(GameState.Initialized);
      }


      private async void MultiplayerSession_OnConnect(long playerDbid)
      {
         BindPlayerDbidToEvents(playerDbid, true);

         Debug.Log($"CHECK {_multiplayerSession.PlayerDbidsCount} < {_multiplayerSession.TargetPlayerCount}");
         if (_multiplayerSession.PlayerDbidsCount < _multiplayerSession.TargetPlayerCount)
         {
            await _gameStateHandler.SetGameState (GameState.Connecting);

         }
         else
         {
            await _gameStateHandler.SetGameState (GameState.Connected);

            _multiplayerSession.SendEvent<GameStartEvent>(new GameStartEvent());
         }
      }


      private async void MultiplayerSession_OnDisconnect(long playerDbid)
      {
         BindPlayerDbidToEvents(playerDbid, false);

         SetStatusText(string.Format(TBFConstants.StatusText_Multiplayer_OnDisconnect,
            _multiplayerSession.PlayerDbidsCount.ToString(),
            _multiplayerSession.TargetPlayerCount), BufferedTextMode.Immediate);

         await _gameStateHandler.SetGameState(GameState.GameEnded);
      }


      private async void MultiplayerSession_OnGameStartEvent(GameStartEvent gameStartEvent)
      {
         DebugLog($"OnGameStartEvent() by {gameStartEvent.PlayerDbid}");

         if (_gameStateHandler.GameState == GameState.GameStarting)
         {
            _gameProgressData.GameStartEventsBucket.Add(gameStartEvent);

            DebugLog($"GameStartEventBucket.Count = {_gameProgressData.GameStartEventsBucket.Count}");

            if (_gameProgressData.GameStartEventsBucket.Count == _multiplayerSession.TargetPlayerCount)
            {
               await _gameStateHandler.SetGameState(GameState.GameStarted);
            }
         }
      }


      private async void MultiplayerSession_OnGameMoveEvent(GameMoveEvent gameMoveEvent)
      {
         DebugLog($"OnGameMoveEvent() of {gameMoveEvent.GameMoveType} by {gameMoveEvent.PlayerDbid}");

         if (_gameStateHandler.GameState == GameState.RoundPlayerMoving)
         {
            _gameProgressData.GameMoveEventsThisRoundBucket.Add(gameMoveEvent);

            DebugLog($"GameMoveEventsThisRoundBucket.Count = {_gameProgressData.GameMoveEventsThisRoundBucket.Count}");

            if (_gameProgressData.GameMoveEventsThisRoundBucket.Count == _multiplayerSession.TargetPlayerCount)
            {
               await _gameStateHandler.SetGameState(GameState.RoundPlayerMoved);
            }
            
         }

      }
   }
}
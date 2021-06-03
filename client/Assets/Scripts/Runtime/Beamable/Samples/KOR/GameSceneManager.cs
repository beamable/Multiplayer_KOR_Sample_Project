using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Examples.Features.Multiplayer.Core;
using Beamable.Samples.Core;
using Beamable.Samples.KOR.Audio;
using Beamable.Samples.KOR.Behaviours;
using Beamable.Samples.KOR.Data;
using Beamable.Samples.KOR.Multiplayer;
using Beamable.Samples.KOR.Multiplayer.Events;
using Beamable.Samples.KOR.UI;
using Beamable.Samples.KOR.Views;
using UnityEngine;

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


      //  Unity Methods   ------------------------------
      protected void Start()
      {
         _gameUIView.BackButton.onClick.AddListener(BackButton_OnClicked);
         SetupBeamable();
      }


      //  Other Methods  -----------------------------
      private void DebugLog(string message)
      {
         if (_configuration.IsDebugLog)
         {
            Debug.Log(message);
         }
      }


      private async void SetupBeamable()
      {
         _beamableAPI = await Beamable.API.Instance;

         // Set defaults if scene was loaded directly
         if (RuntimeDataStorage.Instance.TargetPlayerCount == KORConstants.UnsetValue)
         {
            DebugLog(KORHelper.GetSceneLoadingMessage(gameObject.scene.name, true));
            RuntimeDataStorage.Instance.TargetPlayerCount = 1;
            RuntimeDataStorage.Instance.LocalPlayerDbid = _beamableAPI.User.id;
            RuntimeDataStorage.Instance.RoomId = KORMatchmaking.GetRandomRoomId();
         }
         else
         {
            DebugLog(KORHelper.GetSceneLoadingMessage(gameObject.scene.name, false));
         }

         // Set the ActiveSimGameType. This happens in 2+ spots to handle direct scene loading
         if (RuntimeDataStorage.Instance.IsSinglePlayerMode)
         {
            RuntimeDataStorage.Instance.ActiveSimGameType = await _configuration.SimGameType01Ref.Resolve();
         }
         else
         {
            RuntimeDataStorage.Instance.ActiveSimGameType = await _configuration.SimGameType02Ref.Resolve();
         }

         // Initialize ECS
         SystemManager.StartGameSystems();

         // Initialize Networking
         await NetworkController.Instance.Init();

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

         // Optional: Play "damage" sound
         SoundManager.Instance.PlayAudioClip(SoundConstants.HealthBarDecrement);

         // Optional: Render color and text of avatar ui
         _gameUIView.AvatarViews.Clear();

         // TODO: Spawn the HUD from player join messages...
         for (int i = 0; i < RuntimeDataStorage.Instance.MaxPlayerCount; i++)
         {
            // AvatarData avatarData = _configuration.AvatarDatas[i];
            _gameUIView.AvatarUIViews[i].AvatarData = _configuration.LocalAvatar; // TODO: This is incorrect, now. We need to spawn huds based on who is in the game
            _gameUIView.AvatarUIViews[i].Health = 100;
            _gameUIView.AvatarUIViews[i].IsInGame = i < RuntimeDataStorage.Instance.MinPlayerCount;
            _gameUIView.AvatarUIViews[i].Name = $"Player {(i + 1):00}"; // "Player 01"
            _gameUIView.AvatarUIViews[i].IsLocalPlayer = i == 0; //Todo: check dbid

            if (i < RuntimeDataStorage.Instance.MinPlayerCount)
            {
               // AvatarView avatarView = GameObject.Instantiate<AvatarView>(avatarData.AvatarViewPrefab);
               // _gameUIView.AvatarViews.Add(avatarView);

               //Optional: Play animations. All are working properly
               //avatarView.PlayAnimationAttack01();
               //avatarView.PlayAnimationAttack02();
               //avatarView.PlayAnimationDie();
               //avatarView.PlayAnimationRunForward();
               //avatarView.PlayAnimationTakeDamage();
               //avatarView.PlayAnimationWalkForward();
               // avatarView.PlayAnimationIdle();
            }
         }
      }

      public void AddPlayer(PlayerJoinedEvent joinEvent)
      {
         // get random spawn point...
         var spawnIndex = NetworkController.Instance.rand.Next(0, AvailableSpawnPoints.Count);
         var spawnPoint = AvailableSpawnPoints[spawnIndex];

         var isLocal = joinEvent.PlayerDbid == NetworkController.Instance.LocalDbid;
         var avatarData = isLocal
            ? _configuration.LocalAvatar
            : _configuration.RemoteAvatar;

         var avatarView = GameObject.Instantiate<AvatarView>(avatarData.AvatarViewPrefab);
         avatarView.transform.SetPhysicsPosition(spawnPoint.transform.position);

         avatarView.SetForPlayer(joinEvent.PlayerDbid);
         _gameUIView.AvatarViews.Add(avatarView);

         // clean up spawn point so no one else can use it...
         AvailableSpawnPoints.Remove(spawnPoint);
      }

      public void HandleNetworkEvent(KOREvent korEvent)
      {
         switch (korEvent)
         {
            case PlayerMoveStartedEvent moveEvt:
               // moveEvt.Consume();
               // _gameUIView.GetAvatarViewForDbid(moveEvt.PlayerDbid);
               break;
            case PlayerJoinedEvent joinEvt:
               joinEvt.Consume();
               AddPlayer(joinEvt);
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
         // Destroy ECS
         SystemManager.DestroyGameSystems();

         // Change scenes
         StartCoroutine(KORHelper.LoadScene_Coroutine(_configuration.IntroSceneName,
            _configuration.DelayBeforeLoadScene));
      }
   }
}
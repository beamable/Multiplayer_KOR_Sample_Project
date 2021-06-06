﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beamable.Common.Content;
using Beamable.Common.Inventory;
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

      private Dictionary<long, AvatarView> _dbidToAvatar = new Dictionary<long, AvatarView>();
      private List<SpawnPointBehaviour> _unusedSpawnPoints = new List<SpawnPointBehaviour>();

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

         // Do this after calling "Beamable.API.Instance" for smoother UI
         _gameUIView.CanvasGroupsDoFadeIn();

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

         // TODO: Spawn the HUD from player join messages...
         for (int i = 0; i < 5; i++)
         {
            // AvatarData avatarData = _configuration.AvatarDatas[i];
            // TODO: Spawn Avatars according to chosen character per DBID
            // For now, just use a fixed prefab for everyone.
            // TODO: Only show HUD for players that are actually in the game.
            _gameUIView.AvatarUIViews[i].AvatarData = _configuration.LocalAvatar;
            _gameUIView.AvatarUIViews[i].Health = 100;
            _gameUIView.AvatarUIViews[i].IsInGame = i < RuntimeDataStorage.Instance.MinPlayerCount;
            _gameUIView.AvatarUIViews[i].Name = $"Player {(i + 1):00}"; // "Player 01"
            _gameUIView.AvatarUIViews[i].IsLocalPlayer = i == 0; //Todo: check dbid
            _gameUIView.AvatarUIViews[i].Render();

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
         // don't respawn a player if they had already joined.
         if (_dbidToAvatar.ContainsKey(joinEvent.PlayerDbid))
         {
            return;
         }

         // get random spawn point...
         var spawnIndex = NetworkController.Instance.rand.Next(0, _unusedSpawnPoints.Count);
         var spawnPoint = _unusedSpawnPoints[spawnIndex];

         var isLocal = joinEvent.PlayerDbid == NetworkController.Instance.LocalDbid;
         var avatarData = isLocal
            ? _configuration.LocalAvatar
            : _configuration.RemoteAvatar;

         var avatarView = GameObject.Instantiate<AvatarView>(avatarData.AvatarViewPrefab);
         avatarView.transform.SetPhysicsPosition(spawnPoint.transform.position);
         _dbidToAvatar.Add(joinEvent.PlayerDbid, avatarView);

         avatarView.SetForPlayer(joinEvent.PlayerDbid);
         _gameUIView.AvatarViews.Add(avatarView);

         // clean up spawn point so no one else can use it...
         _unusedSpawnPoints.Remove(spawnPoint);
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
            case PlayerJoinedEvent joinEvt:
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
         KORHelper.PlayAudioForUIClick();

         // Clean up manager
         _dbidToAvatar.Clear();
         _unusedSpawnPoints.Clear();
         NetworkController.Instance.Cleanup();

         // Destroy ECS
         SystemManager.DestroyGameSystems();

         // Change scenes
         StartCoroutine(KORHelper.LoadScene_Coroutine(_configuration.IntroSceneName,
            _configuration.DelayBeforeLoadScene));
      }
   }
}
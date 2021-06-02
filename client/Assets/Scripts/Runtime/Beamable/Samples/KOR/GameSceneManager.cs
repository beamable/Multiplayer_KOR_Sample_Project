using System;
using System.Threading.Tasks;
using Beamable.Examples.Features.Multiplayer.Core;
using Beamable.Samples.KOR.Audio;
using Beamable.Samples.KOR.Data;
using Beamable.Samples.KOR.Multiplayer;
using Beamable.Samples.KOR.UI;
using Beamable.Samples.KOR.Views;
using UnityEngine;

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
            RuntimeDataStorage.Instance.LocalPlayerDbid = _beamableAPI.User.id;
            RuntimeDataStorage.Instance.CurrentPlayerCount = 1;
            RuntimeDataStorage.Instance.RoomId = KORMatchmaking.GetRandomRoomId();
         }
         else
         {
            DebugLog(KORHelper.GetSceneLoadingMessage(gameObject.scene.name, false));
         }

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
         for (int i = 0; i < RuntimeDataStorage.Instance.MaxPlayerCount; i++)
         {
            AvatarData avatarData = _configuration.AvatarDatas[i];
            _gameUIView.AvatarUIViews[i].AvatarData = avatarData;
            _gameUIView.AvatarUIViews[i].Health = 100;
            _gameUIView.AvatarUIViews[i].IsInGame = i < RuntimeDataStorage.Instance.MinPlayerCount;
            _gameUIView.AvatarUIViews[i].Name = $"Player {(i + 1):00}"; // "Player 01"
            _gameUIView.AvatarUIViews[i].IsLocalPlayer = i == 0; //Todo: check dbid

            if (i < RuntimeDataStorage.Instance.MinPlayerCount)
            {
               AvatarView avatarView = GameObject.Instantiate<AvatarView>(avatarData.AvatarViewPrefab);
               _gameUIView.AvatarViews.Add(avatarView);

               //Optional: Play animations. All are working properly
               //avatarView.PlayAnimationAttack01();
               //avatarView.PlayAnimationAttack02();
               //avatarView.PlayAnimationDie();
               //avatarView.PlayAnimationRunForward();
               //avatarView.PlayAnimationTakeDamage();
               //avatarView.PlayAnimationWalkForward();
               avatarView.PlayAnimationIdle();
            }
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
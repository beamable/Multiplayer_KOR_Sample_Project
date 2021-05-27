using Beamable.Samples.TBF.Data;
using Beamable.Samples.TBF.Multiplayer;
using Beamable.Samples.TBF.Views;
using System;
using System.Collections;
using Beamable.Common.Content;
using Beamable.Examples.Features.Multiplayer.Core;
using UnityEngine;
using static Beamable.Samples.TBF.UI.TMP_BufferedText;
using SimGameTypeRef = Beamable.Common.Content.SimGameTypeRef;

namespace Beamable.Samples.TBF
{
   /// <summary>
   /// Handles the main scene logic: Lobby
   /// </summary>
   public class LobbySceneManager : MonoBehaviour
   {
      //  Fields ---------------------------------------
      [SerializeField]
      private Configuration _configuration = null;

      [SerializeField]
      private LobbyUIView _lobbyUIView = null;

      /// <summary>
      /// This defines the matchmaking criteria including "NumberOfPlayers"
      /// </summary>
      [SerializeField]
      private SimGameTypeRef _onePlayerSimGameTypeRef;

      /// <summary>
      /// This defines the matchmaking criteria including "NumberOfPlayers"
      /// </summary>
      [SerializeField]
      private SimGameTypeRef _twoPlayerSimGameTypeRef;

      private IBeamableAPI _beamableAPI;
      private TBFMatchmaking matchmaking;

      //  Unity Methods   ------------------------------
      protected void Start()
      {
         _lobbyUIView.BackButton.onClick.AddListener(BackButton_OnClicked);

         if (RuntimeDataStorage.Instance.TargetPlayerCount == RuntimeDataStorage.UnsetPlayerCount)
         {
            DebugLog($"Scene '{gameObject.scene.name}' was loaded directly. That is ok. Setting defaults.");
            RuntimeDataStorage.Instance.TargetPlayerCount = 1;
         }

         var text = string.Format(TBFConstants.StatusText_Joining, 0,
            RuntimeDataStorage.Instance.TargetPlayerCount);

         _lobbyUIView.BufferedText.SetText(text, BufferedTextMode.Immediate);

         SetupBeamable();
      }

      private Action _onDestroy;

      public void OnDestroy()
      {
         _onDestroy?.Invoke();
      }

      //  Other Methods   ------------------------------
      private async void SetupBeamable()
      {
         SimGameType simGameType;

         if (RuntimeDataStorage.Instance.TargetPlayerCount == 1)
         {
            simGameType = await _onePlayerSimGameTypeRef.Resolve();
         }
         else if (RuntimeDataStorage.Instance.TargetPlayerCount == 2)
         {
            simGameType = await _twoPlayerSimGameTypeRef.Resolve();
         }
         else
         {
            throw new Exception("Codepath is never intended.");
         }

         var beamable = await Beamable.API.Instance;
         _beamableAPI = beamable;
         RuntimeDataStorage.Instance.IsMatchmakingComplete = false;

         matchmaking = new TBFMatchmaking(beamable.Experimental.MatchmakingService, simGameType,
            _beamableAPI.User.id);
         matchmaking.OnProgress += MyMatchmaking_OnProgress;
         matchmaking.OnComplete += MyMatchmaking_OnComplete;
         _onDestroy = matchmaking.Stop;

         try
         {
            await matchmaking.Start();
         }
         catch (Exception)
         {
            _lobbyUIView.BufferedText.SetText(TBFHelper.InternetOfflineInstructionsText,
               BufferedTextMode.Queue);
         }
      }

      private void DebugLog(string message)
      {
         if (TBFConstants.IsDebugLogging)
         {
            Debug.Log(message);
         }
      }


      //  Event Handlers -------------------------------
      private void BackButton_OnClicked()
      {
         matchmaking?.Stop();

         StartCoroutine(TBFHelper.LoadScene_Coroutine(_configuration.IntroSceneName,
            _configuration.DelayBeforeLoadScene));
      }


      private void MyMatchmaking_OnProgress(MyMatchmakingResult myMatchmakingResult)
      {
         DebugLog($"MyMatchmaking_OnProgress() " +
            $"Players={myMatchmakingResult.Players.Count}/{myMatchmakingResult.TargetPlayerCount} " +
            $"RoomId={myMatchmakingResult.RoomId}");

         string text = string.Format(TBFConstants.StatusText_Joining,
            myMatchmakingResult.Players.Count,
            myMatchmakingResult.TargetPlayerCount);

         _lobbyUIView.BufferedText.SetText(text, BufferedTextMode.Queue);
      }


      private void MyMatchmaking_OnComplete(MyMatchmakingResult myMatchmakingResult)
      {
         if (!RuntimeDataStorage.Instance.IsMatchmakingComplete)
         {
            string text = string.Format(TBFConstants.StatusText_Joined,
               myMatchmakingResult.Players.Count,
               myMatchmakingResult.TargetPlayerCount);

            _lobbyUIView.BufferedText.SetText(text, BufferedTextMode.Queue);

            DebugLog($"MyMatchmaking_OnComplete() " +
               $"Players={myMatchmakingResult.Players.Count}/{myMatchmakingResult.TargetPlayerCount} " +
               $"RoomId={myMatchmakingResult.RoomId}");

            //Store successful info here for use in another scene
            RuntimeDataStorage.Instance.IsMatchmakingComplete = true;
            RuntimeDataStorage.Instance.LocalPlayerDbid = myMatchmakingResult.LocalPlayerDbid;
            RuntimeDataStorage.Instance.RoomId = myMatchmakingResult.RoomId;

            StartCoroutine(LoadScene_Coroutine());

         }
         else
         {
            throw new Exception("Codepath is never intended.");
         }
      }

      private IEnumerator LoadScene_Coroutine()
      {
         //Wait for old messages to pass before changing scenes
         while (_lobbyUIView.BufferedText.HasRemainingQueueText)
         {
            yield return new WaitForEndOfFrame();
         }

         //Show final status message a little longer
         yield return new WaitForSeconds(0.5f);

         //Load another scene
         StartCoroutine(TBFHelper.LoadScene_Coroutine(_configuration.GameSceneName,
            _configuration.DelayBeforeLoadScene));
      }
   }
}

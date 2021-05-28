using System;
using System.Collections;
using Beamable.Common.Content;
using Beamable.Examples.Features.Multiplayer.Core;
using Beamable.Samples.KOR.Data;
using Beamable.Samples.KOR.Multiplayer;
using Beamable.Samples.KOR.UI;
using Beamable.Samples.KOR.Views;
using UnityEngine;
using SimGameTypeRef = Beamable.Common.Content.SimGameTypeRef;

namespace Beamable.Samples.KOR
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

      private IBeamableAPI _beamableAPI;
      private KORMatchmaking matchmaking;

      //  Unity Methods   ------------------------------
      protected void Start()
      {
         _lobbyUIView.BackButton.onClick.AddListener(BackButton_OnClicked);

         if (RuntimeDataStorage.Instance.CurrentPlayerCount == RuntimeDataStorage.UnsetPlayerCount)
         {
            DebugLog($"Scene '{gameObject.scene.name}' was loaded directly. That is ok. Setting defaults.");
            RuntimeDataStorage.Instance.CurrentPlayerCount = 1;
         }

         var text = string.Format(KORConstants.StatusText_Joining, 0,
            RuntimeDataStorage.Instance.CurrentPlayerCount);

         _lobbyUIView.BufferedText.SetText(text, TMP_BufferedText.BufferedTextMode.Immediate);

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
         var beamable = await Beamable.API.Instance;
         _beamableAPI = beamable;
         RuntimeDataStorage.Instance.IsMatchmakingComplete = false;
         RuntimeDataStorage.Instance.SimGameType = await _configuration.SimGameTypeRef.Resolve();
            
         matchmaking = new KORMatchmaking(beamable.Experimental.MatchmakingService, 
            RuntimeDataStorage.Instance.SimGameType,
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
            _lobbyUIView.BufferedText.SetText(KORHelper.InternetOfflineInstructionsText,
               TMP_BufferedText.BufferedTextMode.Queue);
         }
      }

      private void DebugLog(string message)
      {
         if (KORConstants.IsDebugLogging)
         {
            Debug.Log(message);
         }
      }


      //  Event Handlers -------------------------------
      private void BackButton_OnClicked()
      {
         matchmaking?.Stop();

         StartCoroutine(KORHelper.LoadScene_Coroutine(_configuration.IntroSceneName,
            _configuration.DelayBeforeLoadScene));
      }


      private void MyMatchmaking_OnProgress(MyMatchmakingResult result)
      {
         DebugLog($"MyMatchmaking_OnProgress() " +
            $"Players={result.CurrentPlayerDbidList.Count}/{result.TargetPlayerCount} " +
            $"RoomId={result.RoomId}");

         string text = string.Format(KORConstants.StatusText_Joining,
            result.CurrentPlayerDbidList.Count,
            result.TargetPlayerCount);

         _lobbyUIView.BufferedText.SetText(text, TMP_BufferedText.BufferedTextMode.Queue);
      }


      private void MyMatchmaking_OnComplete(MyMatchmakingResult result)
      {
         if (!RuntimeDataStorage.Instance.IsMatchmakingComplete)
         {
            string text = string.Format(KORConstants.StatusText_Joined,
               result.CurrentPlayerDbidList.Count,
               result.TargetPlayerCount);

            _lobbyUIView.BufferedText.SetText(text, TMP_BufferedText.BufferedTextMode.Queue);

            DebugLog($"MyMatchmaking_OnComplete() " +
               $"Players={result.CurrentPlayerDbidList.Count}/{result.TargetPlayerCount} " +
               $"RoomId={result.RoomId}");

            //Store successful info here for use in another scene
            RuntimeDataStorage.Instance.IsMatchmakingComplete = true;
            RuntimeDataStorage.Instance.LocalPlayerDbid = result.LocalPlayerDbid;
            RuntimeDataStorage.Instance.CurrentPlayerCount = result.CurrentPlayerDbidList.Count;
            RuntimeDataStorage.Instance.RoomId = result.RoomId;

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
         StartCoroutine(KORHelper.LoadScene_Coroutine(_configuration.GameSceneName,
            _configuration.DelayBeforeLoadScene));
      }
   }
}

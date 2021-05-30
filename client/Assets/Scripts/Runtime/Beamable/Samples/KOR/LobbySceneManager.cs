using System;
using System.Collections;
using Beamable.Examples.Features.Multiplayer.Core;
using Beamable.Samples.KOR.Data;
using Beamable.Samples.KOR.Multiplayer;
using Beamable.Samples.KOR.UI;
using Beamable.Samples.KOR.Views;
using TMPro;
using UnityEngine;

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
      private KORMatchmaking _korMatchmaking;

      //  Unity Methods   ------------------------------
      protected void Start()
      {
         _lobbyUIView.BackButton.onClick.AddListener(BackButton_OnClicked);
         _lobbyUIView.StartGameButton.onClick.AddListener(StartGameButton_OnClicked);
         MyKorMatchmakingOnProgress(null);

         if (RuntimeDataStorage.Instance.CurrentPlayerCount == RuntimeDataStorage.UnsetPlayerCount)
         {
            DebugLog($"Scene '{gameObject.scene.name}' was loaded directly. That is ok. Setting defaults.");
            RuntimeDataStorage.Instance.CurrentPlayerCount = 1;
         }

         var text = string.Format(KORConstants.LobbyUIView_Joining, 0,
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
         
         // Put in storage to offer several convenience getters
         RuntimeDataStorage.Instance.SimGameType = await _configuration.SimGameTypeRef.Resolve();
            
         // Do matchmaking
         _korMatchmaking = new KORMatchmaking(beamable.Experimental.MatchmakingService, 
            RuntimeDataStorage.Instance.SimGameType,
            _beamableAPI.User.id);
         _korMatchmaking.OnProgress += MyKorMatchmakingOnProgress;
         _korMatchmaking.OnComplete += MyKorMatchmakingOnComplete;
         _onDestroy = _korMatchmaking.Stop;

         try
         {
            await _korMatchmaking.Start();
         }
         catch (Exception)
         {
            _lobbyUIView.BufferedText.SetText(KORHelper.InternetOfflineInstructionsText,
               TMP_BufferedText.BufferedTextMode.Queue);
         }
      }

      private void DebugLog(string message)
      {
         if (_configuration.IsDebugLog)
         {
            Debug.Log(message);
         }
      }


      //  Event Handlers -------------------------------
      private void StartGameButton_OnClicked()
      {
         Debug.Log("TODO: Properly end the matchmaking so the game can be playable without max players");
         _korMatchmaking?.Stop();

         StartCoroutine(KORHelper.LoadScene_Coroutine(_configuration.GameSceneName,
            _configuration.DelayBeforeLoadScene));
      }
      
      private void BackButton_OnClicked()
      {
         _korMatchmaking?.Stop();

         StartCoroutine(KORHelper.LoadScene_Coroutine(_configuration.IntroSceneName,
            _configuration.DelayBeforeLoadScene));
      }

      private void MyKorMatchmakingOnProgress(MyMatchmakingResult result)
      {
         int currentPlayersCount = 0;
         if (result != null)
         {
            currentPlayersCount = result.CurrentPlayerDbidList.Count;
            DebugLog($"MyMatchmaking_OnProgress() " +
                     $"Players={currentPlayersCount}/{result.TargetPlayerCount} " +
                     $"RoomId={result.RoomId}");

            string text = string.Format(KORConstants.LobbyUIView_Joining,
               result.CurrentPlayerDbidList.Count,
               result.TargetPlayerCount);

            _lobbyUIView.BufferedText.SetText(text, TMP_BufferedText.BufferedTextMode.Queue);
         }

         _lobbyUIView.StartGameButton.interactable = currentPlayersCount > 0;
         _lobbyUIView.StartGameButton.GetComponentInChildren<TMP_Text>().text = $"Start Game\n({currentPlayersCount} Players)";

      }


      private void MyKorMatchmakingOnComplete(MyMatchmakingResult result)
      {
         if (!RuntimeDataStorage.Instance.IsMatchmakingComplete)
         {
            string text = string.Format(KORConstants.LobbyUIView_Joined,
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

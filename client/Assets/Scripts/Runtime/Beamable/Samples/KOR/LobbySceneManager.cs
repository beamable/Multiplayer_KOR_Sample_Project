using System;
using System.Collections;
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
    /// Handles the main scene logic: Lobby
    /// </summary>
    public class LobbySceneManager : MonoBehaviour
    {
        //  Events ---------------------------------------
        private Action _onDestroy;

        //  Fields ---------------------------------------
        [SerializeField]
        private Configuration _configuration = null;

        [SerializeField]
        private LobbyUIView _lobbyUIView = null;

        private IBeamableAPI _beamableAPI;
        private KORMatchmaking _korMatchmaking;

        private int _lastProgressPlayerCount = 0;

        //  Unity Methods   ------------------------------
        protected void Start()
        {
            _lastProgressPlayerCount = 0;

            _lobbyUIView.BackButton.onClick.AddListener(BackButton_OnClicked);

            _lobbyUIView.BufferedText.SetText("",
               TMP_BufferedText.BufferedTextMode.Immediate);

            MyKorMatchmakingOnProgress(null);

            SetupBeamable();
        }

        public void OnDestroy()
        {
            _onDestroy?.Invoke();
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

            // Do this after calling "Beamable.API.Instance" for smoother UI
            _lobbyUIView.CanvasGroupsDoFadeIn();

            // Set defaults if scene was loaded directly
            if (RuntimeDataStorage.Instance.TargetPlayerCount == KORConstants.UnsetValue)
            {
                DebugLog(KORHelper.GetSceneLoadingMessage(gameObject.scene.name, true));
                RuntimeDataStorage.Instance.TargetPlayerCount = 1;
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

            RuntimeDataStorage.Instance.IsMatchmakingComplete = false;

            // Do matchmaking
            _korMatchmaking = new KORMatchmaking(_beamableAPI.Experimental.MatchmakingService,
               RuntimeDataStorage.Instance.ActiveSimGameType,
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

        //  Event Handlers -------------------------------

        private void BackButton_OnClicked()
        {
            KORHelper.PlayAudioForUIClickBack();

            _korMatchmaking?.Stop();

            StartCoroutine(KORHelper.LoadScene_Coroutine(_configuration.IntroSceneName,
               _configuration.DelayBeforeLoadScene));
        }

        private void MyKorMatchmakingOnProgress(MyMatchmakingResult result)
        {
            int currentPlayersCount = 0;
            int targetPlayerCount = 0;
            int secondsRemaining = -1;
            string roomId = "0";

            if (result != null)
            {
                currentPlayersCount = result.CurrentPlayerDbidList.Count;
                targetPlayerCount = result.TargetPlayerCount;
                roomId = result.RoomId;
                secondsRemaining = result.SecondsRemaining;
            }

            DebugLog($"MyMatchmaking_OnProgress() ...\n " +
                     $"Players={currentPlayersCount}/{targetPlayerCount}, " +
                     $"SecondsRemaining = {secondsRemaining}, " +
                     $"RoomId={roomId}");

            string text = string.Format(KORConstants.LobbyUIView_Joining,
               currentPlayersCount,
               targetPlayerCount,
               secondsRemaining);
            if (secondsRemaining == 0)
            {
                text = string.Format(KORConstants.LobbyUIView_Finalizing,
                   currentPlayersCount,
                   targetPlayerCount);
            }
            if (secondsRemaining == -1)
            {
                text = string.Format(KORConstants.LobbyUIView_Waiting,
                   currentPlayersCount,
                   targetPlayerCount);
            }

            _lobbyUIView.BufferedText.SetText(text, TMP_BufferedText.BufferedTextMode.Queue);

            const float playerCountPitchMultiplier = 0.05f;

            if (currentPlayersCount > _lastProgressPlayerCount)
                SoundManager.Instance.PlayAudioClip(SoundConstants.Chime02, 1.0f + currentPlayersCount * playerCountPitchMultiplier);
            if (currentPlayersCount < _lastProgressPlayerCount)
                SoundManager.Instance.PlayAudioClip(SoundConstants.Chime01, 1.0f - currentPlayersCount * playerCountPitchMultiplier);

            _lastProgressPlayerCount = currentPlayersCount;
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
                throw new Exception("Must not reach MyKorMatchmakingOnComplete() if already IsMatchmakingComplete == true");
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
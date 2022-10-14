using System;
using System.Collections;
using Beamable.Examples.Features.Multiplayer.Core;
using Beamable.Experimental.Api.Matchmaking;
using Beamable.Samples.Core.Audio;
using Beamable.Samples.Core.UI;
using Beamable.Samples.KOR.Data;
using Beamable.Samples.KOR.Multiplayer;
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

        private BeamContext _beam;
        private KORMatchmaking _korMatchmaking;

        private int _lastProgressPlayerCount = 0;

        //  Unity Methods   ------------------------------
        protected void Start()
        {
            _lastProgressPlayerCount = 0;

            _lobbyUIView.BackButton.onClick.AddListener(BackButton_OnClicked);

            _lobbyUIView.BufferedText.SetText("",
               TMP_BufferedText.BufferedTextMode.Immediate);

            MyKorMatchmaking_OnProgress(null);

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
            _beam = BeamContext.Default;
            await _beam.OnReady;

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
            _korMatchmaking = new KORMatchmaking(_beam.ServiceProvider.GetService<MatchmakingService>(),
               RuntimeDataStorage.Instance.ActiveSimGameType,
               _beam.PlayerId);

            _korMatchmaking.OnProgress.AddListener(MyKorMatchmaking_OnProgress);
            _korMatchmaking.OnComplete.AddListener(MyKorMatchmaking_OnComplete);
            _korMatchmaking.OnError.AddListener(MyKorMatchmaking_OnError);
            _onDestroy = async () =>
            {
                await _korMatchmaking.CancelMatchmaking();
            };

            try
            {
                await _korMatchmaking.StartMatchmaking();
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

            _korMatchmaking?.CancelMatchmaking();

            StartCoroutine(KORHelper.LoadScene_Coroutine(_configuration.IntroSceneName,
               _configuration.DelayBeforeLoadScene));
        }

        private void MyKorMatchmaking_OnProgress(MyMatchmakingResult result)
        {
            int currentPlayersCount = 0;
            int targetPlayerCount = 0;
            string matchId = "0";

            if (result != null)
            {
                currentPlayersCount = result.Players.Count;
                targetPlayerCount = result.SimGameType.maxPlayers;
                matchId = result.MatchId;
            }

            DebugLog($"MyMatchmaking_OnProgress() ...\n " +
                     $"Players={currentPlayersCount}/{targetPlayerCount}, " +
                     $"MatchId={matchId}");

            string text = string.Format(KORConstants.LobbyUIView_Joining,
               currentPlayersCount,
               targetPlayerCount );
            
            //TODO REmove
            //text = string.Format(KORConstants.LobbyUIView_Finalizing,
            //text = string.Format(KORConstants.LobbyUIView_Waiting,

            _lobbyUIView.BufferedText.SetText(text, TMP_BufferedText.BufferedTextMode.Queue);

            const float playerCountPitchMultiplier = 0.05f;

            int playerCountDiff = currentPlayersCount - _lastProgressPlayerCount;

            if (playerCountDiff > 0)
            {
                for (int p = 0; p < playerCountDiff; p++)
                    SoundManager.Instance.PlayAudioClipDelayed(SoundConstants.Chime02, p * 0.5f,
                        1.0f + ((_lastProgressPlayerCount + p) * playerCountPitchMultiplier));
            }

            if (playerCountDiff < 0)
            {
                for (int p = 0; p < -playerCountDiff; p++)
                    SoundManager.Instance.PlayAudioClipDelayed(SoundConstants.Chime02, p * 0.5f,
                        1.0f - ((_lastProgressPlayerCount - p) * playerCountPitchMultiplier));
            }

            _lastProgressPlayerCount = currentPlayersCount;
        }

        private void MyKorMatchmaking_OnComplete(MyMatchmakingResult result)
        {
            if (!RuntimeDataStorage.Instance.IsMatchmakingComplete)
            {
                string text = string.Format(KORConstants.LobbyUIView_Joined,
                   result.Players.Count,
                   result.SimGameType.maxPlayers);

                _lobbyUIView.BufferedText.SetText(text, TMP_BufferedText.BufferedTextMode.Queue);

                DebugLog($"MyMatchmaking_OnComplete() " +
                   $"Players = {result.Players.Count}/{result.SimGameType.maxPlayers} " +
                   $"MatchId = {result.MatchId}");

                //Store successful info here for use in another scene
                RuntimeDataStorage.Instance.IsMatchmakingComplete = true;
                RuntimeDataStorage.Instance.LocalPlayerDbid = result.LocalPlayer;
                RuntimeDataStorage.Instance.CurrentPlayerCount = result.Players.Count;
                RuntimeDataStorage.Instance.MatchId = result.MatchId;

                StartCoroutine(LoadScene_Coroutine());
            }
            else
            {
                throw new Exception("Must not reach MyKorMatchmaking_OnComplete() if already IsMatchmakingComplete == true");
            }
        }
        
        private void MyKorMatchmaking_OnError(MyMatchmakingResult result)
        {
            string text = string.Format(KORConstants.LobbyUIView_Error,
                result.PlayerCountMin,
                result.PlayerCountMax,
                result.ErrorMessage);
            
            DebugLog($"{text}");

            _lobbyUIView.BufferedText.SetText(text, TMP_BufferedText.BufferedTextMode.Immediate);
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
using System;
using Beamable.Common.Api.Leaderboards;
using Beamable.Common.Leaderboards;
using Beamable.Samples.KOR.Data;
using Beamable.Samples.KOR.Views;
using TMPro;
using UnityEngine;

namespace Beamable.Samples.KOR
{
   /// <summary>
   /// Handles the main scene logic: Intro
   /// </summary>
   public class IntroSceneManager : MonoBehaviour
   {
      //  Fields ---------------------------------------

      [SerializeField]
      private IntroUIView _introUIView = null;

      [SerializeField]
      private Configuration _configuration = null;

      private IBeamableAPI _beamableAPI = null;
      private bool _isConnected = false;
      private bool _isBeamableSDKInstalled = false;
      private string _isBeamableSDKInstalledErrorMessage = "";

      //  Unity Methods   ------------------------------
      protected void Start()
      {
         _introUIView.AboutBodyText = "";
         
         _introUIView.StartGame01Button.onClick.AddListener(StartGame01Button_OnClicked);
         _introUIView.StartGame02Button.onClick.AddListener(StartGame02Button_OnClicked);
         _introUIView.LeaderboardButton.onClick.AddListener(LeaderboardButton_OnClicked);
         _introUIView.StoreButton.onClick.AddListener(StoreButton_OnClicked);
         _introUIView.QuitButton.onClick.AddListener(QuitButton_OnClicked);
         SetupBeamable();
      }


      protected void OnDestroy()
      {
         Beamable.API.Instance.Then(de =>
         {
            _beamableAPI = null;
            de.ConnectivityService.OnConnectivityChanged -= ConnectivityService_OnConnectivityChanged;
         });
      }


      //  Other Methods --------------------------------

      /// <summary>
      /// Login with Beamable and fetch user/session information
      /// </summary>
      private async void SetupBeamable()
      {
         // Attempt Connection to Beamable
         _beamableAPI = await Beamable.API.Instance;
         
         // Do this after calling "Beamable.API.Instance" for smoother UI
         _introUIView.CanvasGroupsDoFade();
         
         try
         {
            _isBeamableSDKInstalled = true;

            // Handle any changes to the internet connectivity
            _beamableAPI.ConnectivityService.OnConnectivityChanged += ConnectivityService_OnConnectivityChanged;
            ConnectivityService_OnConnectivityChanged(_beamableAPI.ConnectivityService.HasConnectivity);
               
            // Populate the leaderboard with mock values for cosmetics
            if (!RuntimeDataStorage.Instance.HasPopulatedLeaderboard)
            {
               LeaderboardContent leaderboardContent = await _configuration.LeaderboardRef.Resolve();
               LeaderBoardView leaderBoardView = await MockDataCreator.PopulateLeaderboardWithMockData(_beamableAPI, 
                  leaderboardContent, 
                  _configuration.LeaderboardMinRowCount, 
                  _configuration.LeaderboardMockScoreMin, 
                  _configuration.LeaderboardMockScoreMax);

               // No need to check again during this Unity playmode session
               RuntimeDataStorage.Instance.HasPopulatedLeaderboard = 
                  leaderBoardView.rankings.Count >= _configuration.LeaderboardMinRowCount;
            }
         }
         catch (Exception e)
         {
            // Failed to connect (e.g. not logged in)
            _isBeamableSDKInstalled = false;
            _isBeamableSDKInstalledErrorMessage = e.Message;
            ConnectivityService_OnConnectivityChanged(false);
         }
      }


      /// <summary>
      /// Render the user-facing text with success or helpful errors.
      /// </summary>
      private void RenderUI()
      {
         long dbid = 0;
         if (_isConnected)
         {
            dbid = _beamableAPI.User.id;
         }

         string aboutBodyText = KORHelper.GetIntroAboutBodyText(
            _isConnected, 
            dbid, 
            _isBeamableSDKInstalled, 
            _isBeamableSDKInstalledErrorMessage);

         _introUIView.AboutBodyText = aboutBodyText;
         _introUIView.ButtonsCanvasGroup.interactable = _isConnected;
      }


      private void StartGame(int targetPlayerCount)
      {
         _introUIView.ButtonsCanvasGroup.interactable = false;
         
         // Stores the "UI wants this player count"
         RuntimeDataStorage.Instance.TargetPlayerCount = targetPlayerCount;
         
         StartCoroutine(KORHelper.LoadScene_Coroutine(_configuration.LobbySceneName,
            _configuration.DelayBeforeLoadScene));
      }

      //  Event Handlers -------------------------------
      private void ConnectivityService_OnConnectivityChanged(bool isConnected)
      {
         _isConnected = isConnected;
         RenderUI();
      }


      private void StartGame01Button_OnClicked()
      {
         KORHelper.PlayAudioForUIClick();
         StartGame(1);
      }

      
      private void StartGame02Button_OnClicked()
      {
         KORHelper.PlayAudioForUIClick();
         StartGame(6);
      }
      
      private void LeaderboardButton_OnClicked()
      {
         KORHelper.PlayAudioForUIClick();
         StartCoroutine(KORHelper.LoadScene_Coroutine(_configuration.LeaderboardSceneName,
            _configuration.DelayBeforeLoadScene));
      }
      
      private void StoreButton_OnClicked()
      {
         KORHelper.PlayAudioForUIClick();
         StartCoroutine(KORHelper.LoadScene_Coroutine(_configuration.StoreSceneName,
            _configuration.DelayBeforeLoadScene));
      }

      private void QuitButton_OnClicked()
      {
         KORHelper.PlayAudioForUIClick();
         
         if (Application.isEditor)
         {
            //In the Unity Editor, mimic the user clicking 'Stop' to stop.
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
         }
         else
         {
            //In the build, mimic the user clicking 'X' to quit.
            Application.Quit();
         }
      }
   }
}
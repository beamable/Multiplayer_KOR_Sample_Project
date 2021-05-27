using Beamable.Samples.TBF.Data;
using Beamable.Samples.TBF.Views;
using System;
using UnityEngine;

namespace Beamable.Samples.TBF
{
   /// <summary>
   /// Handles the main scene logic: Intro
   /// </summary>
   public class IntroSceneManager : MonoBehaviour
   {
      //  Fields ---------------------------------------

      /// <summary>
      /// Determines if we are demo mode. Demo mode does several operations
      /// which are not recommended in a production project including 
      /// creating mock data for the game.
      /// </summary>
      private static bool IsDemoMode = true;

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
         _introUIView.StartGameOnePlayerButton.onClick.AddListener(StartGameOnePlayerButton_OnClicked);
         _introUIView.StartGameTwoPlayerButton.onClick.AddListener(StartGameTwoPlayerButton_OnClicked);
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
      private void SetupBeamable()
      {
         // Attempt Connection to Beamable
         Beamable.API.Instance.Then(de =>
         {
            try
            {
               _beamableAPI = de;
               _isBeamableSDKInstalled = true;

               // Handle any changes to the internet connectivity
               _beamableAPI.ConnectivityService.OnConnectivityChanged += ConnectivityService_OnConnectivityChanged;
               ConnectivityService_OnConnectivityChanged(_beamableAPI.ConnectivityService.HasConnectivity);

               if (IsDemoMode)
               {
                  //Set my player's name
                  //MockDataCreator.SetCurrentUserAlias(_beamableAPI.Stats, "This_is_you:)");
               }
            }
            catch (Exception e)
            {
               // Failed to connect (e.g. not logged in)
               _isBeamableSDKInstalled = false;
               _isBeamableSDKInstalledErrorMessage = e.Message;
               ConnectivityService_OnConnectivityChanged(false);
            }
         });
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

         string aboutBodyText = TBFHelper.GetIntroAboutBodyText(
            _isConnected, 
            dbid, 
            _isBeamableSDKInstalled, 
            _isBeamableSDKInstalledErrorMessage);

         _introUIView.AboutBodyText = aboutBodyText;
         _introUIView.ButtonsCanvasGroup.interactable = _isConnected;
      }


      private void StartGame(int targetPlayerCount)
      {
         RuntimeDataStorage.Instance.TargetPlayerCount = targetPlayerCount;

         _introUIView.ButtonsCanvasGroup.interactable = false;

         StartCoroutine(TBFHelper.LoadScene_Coroutine(_configuration.LobbySceneName,
            _configuration.DelayBeforeLoadScene));
      }

      //  Event Handlers -------------------------------
      private void ConnectivityService_OnConnectivityChanged(bool isConnected)
      {
         _isConnected = isConnected;
         RenderUI();
      }


      private void StartGameOnePlayerButton_OnClicked()
      {
         StartGame(1);
      }


      private void StartGameTwoPlayerButton_OnClicked()
      {
         StartGame(2);
      }

      private void QuitButton_OnClicked()
      {
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
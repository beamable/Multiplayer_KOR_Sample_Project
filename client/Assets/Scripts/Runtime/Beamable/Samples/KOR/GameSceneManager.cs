using System;
using System.Threading.Tasks;
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

      protected void Update()
      {
         //_multiplayerSession?.Update();
      }

      //  Other Methods  -----------------------------
      private void DebugLog(string message)
      {
         if (KORConstants.IsDebugLogging)
         {
            Debug.Log(message);
         }
      }

      private void SetupBeamable()
      {
         Beamable.API.Instance.Then(async beamableAPI =>
         {
            try
            {
               _beamableAPI = beamableAPI;

               if (!RuntimeDataStorage.Instance.IsMatchmakingComplete)
               {
                  DebugLog($"Scene '{gameObject.scene.name}' was loaded directly. That is ok. Setting defaults.");
                  RuntimeDataStorage.Instance.LocalPlayerDbid = _beamableAPI.User.id;
                  RuntimeDataStorage.Instance.TargetPlayerCount = 1;
                  RuntimeDataStorage.Instance.RoomId = KORMatchmaking.GetRandomRoomId();
               }
               else
               {
                  DebugLog($"Scene '{gameObject.scene.name}' was loaded from lobby per production.");
               }

               // Optional: Stuff to use later when player moves incoming
               long tbdIncomingPlayerDbid = 0;
               DebugLog($"LocalPlayerDbid = {RuntimeDataStorage.Instance.LocalPlayerDbid}'");
               DebugLog($"TargetPlayerCount = {RuntimeDataStorage.Instance.TargetPlayerCount}'");
               DebugLog($"IsLocalPlayerDbid = {RuntimeDataStorage.Instance.IsLocalPlayerDbid(tbdIncomingPlayerDbid)}'");
               DebugLog($"IsSinglePlayerMode = {RuntimeDataStorage.Instance.IsSinglePlayerMode}'");
               
               // Optional: Show queueable status text onscreen
               SetStatusText(KORConstants.StatusText_GameState_Playing, TMP_BufferedText.BufferedTextMode.Immediate);

               // Optional: Add easily configurable delays
               await Task.Delay(TimeSpan.FromSeconds(_configuration.DelayGameBeforeMove));
               
               // Optional: Play "damage" sound
               SoundManager.Instance.PlayAudioClip(SoundConstants.HealthBarDecrement);
            }
            catch (Exception)
            {
               SetStatusText(KORHelper.InternetOfflineInstructionsText, TMP_BufferedText.BufferedTextMode.Immediate);
            }
         });
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
         //Change scenes
         StartCoroutine(KORHelper.LoadScene_Coroutine(_configuration.IntroSceneName,
            _configuration.DelayBeforeLoadScene));
      }
   }
}
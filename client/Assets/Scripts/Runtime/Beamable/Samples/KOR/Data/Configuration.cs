﻿using Beamable.Common.Content;
using Beamable.Common.Leaderboards;
using Beamable.Common.Shop;
using Beamable.Samples.Core.Debugging;
using UnityEngine;
using static Beamable.Common.Constants.MenuItems.Assets;

namespace Beamable.Samples.KOR.Data
{
   /// <summary>
   /// Store the common configuration for easy editing ats
   /// EditTime and RuntTime with the Unity Inspector Window.
   /// </summary>
   [CreateAssetMenu(
      fileName = Title,
      menuName = Paths.MENU_ITEM_PATH_ASSETS_BEAMABLE_SAMPLES + "/" +
                 "Multiplayer/Create New " + Title,
      order = Orders.MENU_ITEM_PATH_ASSETS_BEAMABLE_ORDER_1)]
   public class Configuration : ScriptableObject
   {
      
      //  Constants  -----------------------------------
      private const string Title = "KOR Configuration";

      //  Properties -----------------------------------
      public bool IsDemoMode { get { return _isDemoMode; } }
      public DebugLogLevel DebugLogLevel { get { return _debugLogLevel; } }
      public bool IsAudioMuted { get { return _isAudioMuted; } }
      
      /// <summary>
      /// This defines the matchmaking criteria including "NumberOfPlayers"
      /// </summary>
      public SimGameTypeRef SimGameType01Ref { get { return _simGameType01Ref; } }

      /// <summary>
      /// This defines the matchmaking criteria including "NumberOfPlayers"
      /// </summary>
      public SimGameTypeRef SimGameType02Ref { get { return _simGameType02Ref; } }

      /// <summary>
      /// This defines the leaderboard, shared across game players
      /// </summary>
      public LeaderboardRef LeaderboardRef { get { return _leaderboardRef; } }

      /// <summary>
      /// This defines the store, for IPA by game players
      /// </summary>
      public StoreRef StoreRef { get { return _storeRef; } }

      public string IntroSceneName { get { return _introSceneName; } }

      public string LobbySceneName { get { return _lobbySceneName; } }

      public string GameSceneName { get { return _gameSceneName; } }

      public string StoreSceneName { get { return _storeSceneName; } }

      public string LeaderboardSceneName { get { return _leaderboardSceneName; } }

      public float DelayBeforeLoadScene { get { return _delayBeforeLoadScene; } }
      public float DelayBetweenFadeInUI { get { return delayBetweenBetweenFadeInUI; } }
      public float DelayBeforeFadeInUI { get { return _delayBeforeFadeInUI; } }
      public float DelayGameBeforeMove { get { return _delayGameBeforeMove; } }
      public AvatarData LocalAvatar { get { return _localAvatar; } }
      public AvatarData RemoteAvatar { get { return _remoteAvatar; } }
      public int LeaderboardMinRowCount { get { return _leaderboardMinRowCount; } }
      public int LeaderboardMockScoreMin { get { return _leaderboardMockScoreMin; } }
      public int LeaderboardMockScoreMax { get { return _leaderboardMockScoreMax; } }

      /// <summary>
      /// Duration in seconds
      /// </summary>
      public float StatusMessageMinDuration { get { return _statusMessageMinDuration; } }

      public Material CameraSkyboxMaterial { get { return _cameraSkyboxMaterial; } }
      public float CameraSkyboxRotationSpeed { get { return _cameraSkyboxRotationSpeed; } }
      
      public bool IsRotatingCameraSkybox { get { return _isRotatingCameraSkybox; } }

      //  Fields ---------------------------------------

      /// <summary>
      /// Determines if we are demo mode. Demo mode does several operations
      /// which are not recommended in a production project including
      /// creating mock data for the game.
      /// </summary>
      [Header("Debug")]
      [SerializeField]
      private DebugLogLevel _debugLogLevel = DebugLogLevel.Disabled;

      [SerializeField]
      private bool _isDemoMode = true;
      
      [SerializeField]
      private bool _isAudioMuted = false;

      [Header("Scene Names")]
      [SerializeField]
      private string _introSceneName = "";

      [SerializeField]
      private string _lobbySceneName = "";

      [SerializeField]
      private string _gameSceneName = "";

      [SerializeField]
      private string _storeSceneName = "";

      [SerializeField]
      private string _leaderboardSceneName = "";

      [Header("Game Data")]
      [SerializeField]
      private float _delayGameBeforeMove = 1;

      [Header("Game Content")]
      [SerializeField]
      private SimGameTypeRef _simGameType01Ref = null;

      [SerializeField]
      private SimGameTypeRef _simGameType02Ref = null;

      [SerializeField]
      private LeaderboardRef _leaderboardRef = null;

      [SerializeField]
      private StoreRef _storeRef = null;

      [Header("Game Visuals")]
      [SerializeField]
      private AvatarData _localAvatar;

      [SerializeField]
      private AvatarData _remoteAvatar;

      [SerializeField] 
      private Material _cameraSkyboxMaterial = null;

      [SerializeField]
      private bool _isRotatingCameraSkybox = false;

      [SerializeField]
      private float _cameraSkyboxRotationSpeed = 1;

      [Header("Cosmetic Delays")]
      [SerializeField]
      private float _delayBeforeLoadScene = 0;

      [Range (0,3)]
      [SerializeField]
      private float _statusMessageMinDuration = 3000;

      [SerializeField]
      private float delayBetweenBetweenFadeInUI = 0.25f;

      [SerializeField]
      private float _delayBeforeFadeInUI = 0.25f;

      [Header("Mock Data")]
      [SerializeField]
      private int _leaderboardMinRowCount = 10;

      [SerializeField]
      private int _leaderboardMockScoreMin = 1;

      [SerializeField]
      private int _leaderboardMockScoreMax = 10;

      //  Unity Methods ---------------------------------------
      protected void OnEnable()
      {
         SetInstance();
      }

      protected void OnDisable()
      {
         ClearInstance();
      }

      protected void OnValidate()
      {
         // Example validation, remove as needed
         _leaderboardMinRowCount = Mathf.Clamp(_leaderboardMinRowCount, 0, 20);
         _leaderboardMockScoreMin = Mathf.Max(_leaderboardMockScoreMin, 0);
         _leaderboardMockScoreMax = Mathf.Max(_leaderboardMockScoreMin, _leaderboardMockScoreMax);
         
         SetInstance();
      }
      
      
      //  Subsystem -------------------------------------------
#region DebugLog

      // KEEP AS PRIVATE : Not intended as full Singleton implementation.
      // This is just to ease global use of Configuration.Debugger.Log("Hello World").
      private static Configuration _instance = null;
      
      public static Debugger Debugger { get { return _debugger;} }

      private static Debugger _debugger = null;
      
      private void SetInstance()
      {
         // When OnEnable (Always) and also...
         // When OnValidate (only when app is running)
         // Do update the debugging
         if (!Application.isPlaying)
         {
            //return;
         }
         _instance = this;
         _debugger = new Debugger(_instance.DebugLogLevel);
      }
      private void ClearInstance()
      {
         _instance = null;
         _debugger = null;
      }
#endregion

   }
}
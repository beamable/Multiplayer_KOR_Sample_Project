using System.Collections.Generic;
using Beamable.Common.Content;
using UnityEngine;

namespace Beamable.Samples.KOR.Data
{
   /// <summary>
   /// Store the common configuration for easy editing ats
   /// EditTime and RuntTime with the Unity Inspector Window.
   /// </summary>
   [CreateAssetMenu(
      fileName = Title,
      menuName = BeamableConstants.MENU_ITEM_PATH_ASSETS_BEAMABLE_SAMPLES + "/" +
      "Multiplayer/Create New " + Title,
      order = BeamableConstants.MENU_ITEM_PATH_ASSETS_BEAMABLE_ORDER_1)]
   public class Configuration : ScriptableObject
   {
      //  Constants  -----------------------------------
      private const string Title = "TFB Configuration";

      //  Properties -----------------------------------
      public bool IsDemoMode { get { return _isDemoMode; } }
      
      /// <summary>
      /// This defines the matchmaking criteria including "NumberOfPlayers"
      /// </summary>
      public SimGameTypeRef SimGameTypeRef { get { return _simGameTypeRef; } }
      
      public string IntroSceneName { get { return _introSceneName; } }
      
      public string LobbySceneName { get { return _lobbySceneName; } }

      public string GameSceneName { get { return _gameSceneName; } }

      public string StoreSceneName { get { return _storeSceneName; } }

      public string LeaderboardSceneName { get { return _leaderboardSceneName; } }
      
      public float DelayBeforeLoadScene { get { return _delayBeforeLoadScene; } }
      public float DelayFadeInUI { get { return _delayFadeInUI; } }
      public float DelayGameBeforeMove { get { return _delayGameBeforeMove; } }
      public List<AvatarData> AvatarDatas { get { return _avatarDatas; } }
      public int LeaderboardMinRowCount { get { return _leaderboardMinRowCount; } }
      public int LeaderboardMockScoreMin { get { return _leaderboardMockScoreMin; } }
      public int LeaderboardMockScoreMax { get { return _leaderboardMockScoreMax; } }

         
      /// <summary>
      /// Duration in seconds
      /// </summary>
      public float StatusMessageMinDuration { get { return _statusMessageMinDuration; } }
      

      //  Fields ---------------------------------------
      
      /// <summary>
      /// Determines if we are demo mode. Demo mode does several operations
      /// which are not recommended in a production project including 
      /// creating mock data for the game.
      /// </summary>
      [Header("Debug")]
      [SerializeField]
      private bool _isDemoMode = true;
      
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
      private SimGameTypeRef _simGameTypeRef = null;

      [Header("Game Visuals")]
      [SerializeField]
      private List<AvatarData> _avatarDatas = null;

      [Header("Cosmetic Delays")]
      [SerializeField]
      private float _delayBeforeLoadScene = 0;

      [Range (0,3)]
      [SerializeField]
      private float _statusMessageMinDuration = 3000;

      [SerializeField]
      private float _delayFadeInUI = 0.25f;

      [Header("Cosmetic Data")] 
      [SerializeField]
      private int _leaderboardMinRowCount = 10;
         
      [SerializeField]
      private int _leaderboardMockScoreMin = 1;

      [SerializeField]
      private int _leaderboardMockScoreMax = 10;

      //  Unity Methods ---------------------------------------
      protected void OnValidate()
      {
         // Example validation, remove as needed
         _leaderboardMinRowCount = Mathf.Clamp(_leaderboardMinRowCount, 0, 20);
         _leaderboardMockScoreMin = Mathf.Max(_leaderboardMockScoreMin, 0);
         _leaderboardMockScoreMax = Mathf.Min(_leaderboardMockScoreMax, _leaderboardMockScoreMin);
      }
   }
}

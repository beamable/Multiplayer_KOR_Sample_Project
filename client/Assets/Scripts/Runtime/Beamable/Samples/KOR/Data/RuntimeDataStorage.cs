using System;
using System.Collections.Concurrent;
using Beamable.Common.Content;
using Beamable.Samples.Core;
using Beamable.Samples.Core.Components;

namespace Beamable.Samples.KOR.Data
{
    /// <summary>
    /// Store game-related data which survives across scenes
    /// </summary>
    public class RuntimeDataStorage : SingletonMonobehavior<RuntimeDataStorage>
    {
        //  Properties  ----------------------------------
        public CharacterManager CharacterManager { get { return _characterManager; } }

        public GameServices GameServices { get { return _gameServices; } }

        public bool IsMatchmakingComplete { get { return _isMatchmakingComplete; } set { _isMatchmakingComplete = value; } }

        public int TargetPlayerCount { get { return _targetPlayerCount; } set { _targetPlayerCount = value; } }
        public int CurrentPlayerCount { get { return _currentPlayerCount; } set { _currentPlayerCount = value; } }

        public int MinPlayerCount
        {
            get
            {
                if (_activeSimGameType == null)
                {
                    throw new Exception("Must set ActiveSimGameType before getting MinPlayerCount");
                }
                int playerCountMin = 0;
                foreach (TeamContent teamContent in _activeSimGameType.teams)
                {
                    if (teamContent.minPlayers.HasValue)
                    {
                        playerCountMin += teamContent.minPlayers.Value;
                    }
                }
                return playerCountMin;
            }
        }

        public int MaxPlayerCount
        {
            get
            {
                if (_activeSimGameType == null)
                {
                    throw new Exception("Must set ActiveSimGameType before getting MaxPlayerCount");
                }
                return _activeSimGameType.CalculateMaxPlayers();
            }
        }

        public string MatchId { 
            get => _matchId;
            set => _matchId = value;
        }
        public long LocalPlayerDbid {
            get => _localPlayerDbid;
            set => _localPlayerDbid = value;
        }

        public bool IsLocalPlayerDbid(long dbid) => LocalPlayerDbid == dbid;

        public bool IsSinglePlayerMode
        {
            get
            {
                if (_targetPlayerCount == KORConstants.UnsetValue)
                {
                    throw new Exception("Must set TargetPlayerCount before getting IsSinglePlayerMode");
                }
                return _targetPlayerCount == 1;
            }
        }

        public SimGameType ActiveSimGameType { 
            get => _activeSimGameType;
            set => _activeSimGameType = value;
        }
        public bool HasPopulatedLeaderboard { 
            get => _hasPopulatedLeaderboard;
            set => _hasPopulatedLeaderboard = value;
        }

        //  Fields  --------------------------------------
        private bool _isMatchmakingComplete;

        private bool _hasPopulatedLeaderboard;
        private long _localPlayerDbid;
        private string _matchId;
        private int _currentPlayerCount;
        private int _targetPlayerCount;
        private SimGameType _activeSimGameType;
        private CharacterManager _characterManager;
        private GameServices _gameServices;

        //  Unity Methods  --------------------------------
        protected override void Awake()
        {
            base.Awake();
            ClearData();
        }

        private void FixedUpdate()
        {
            GameServices.Tick();
        }

        //  Other Methods  --------------------------------
        /// <summary>
        /// Demonstrates that the lifecycle of data is for runtime only
        /// </summary>
        private void ClearData()
        {
            _isMatchmakingComplete = false;
            _hasPopulatedLeaderboard = false;
            _localPlayerDbid = KORConstants.UnsetValue;
            _matchId = "";
            _currentPlayerCount = KORConstants.UnsetValue;
            _targetPlayerCount = KORConstants.UnsetValue;
            _activeSimGameType = null;
            _characterManager = new CharacterManager();
            _gameServices = new GameServices(); 
        }
    }
}
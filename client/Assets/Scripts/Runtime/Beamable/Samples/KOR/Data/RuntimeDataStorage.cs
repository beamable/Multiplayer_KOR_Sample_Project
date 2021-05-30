using Beamable.Common.Content;
using Beamable.Samples.Core;
using UnityEngine;

namespace Beamable.Samples.KOR.Data
{
	/// <summary>
	/// Store game-related data which survives across scenes
	/// </summary>
	public class RuntimeDataStorage : SingletonMonobehavior<RuntimeDataStorage>
	{
		//  Properties  ----------------------------------
		public bool IsMatchmakingComplete { get { return _isMatchmakingComplete; } set { _isMatchmakingComplete = value; } }
		public int CurrentPlayerCount { get { return _currentPlayerCount; } set { _currentPlayerCount = value; } }
		public int MinPlayerCount { get { return _simGameType.minPlayersToStart.Value; } }
		public int MaxPlayerCount { get { return _simGameType.maxPlayers; } }
		public string RoomId { get { return _roomId; } set { _roomId = value; } }
		//
		public long LocalPlayerDbid { get { return _localPlayerDbid; } set { _localPlayerDbid = value; } }
		public bool IsLocalPlayerDbid (long dbid) { return LocalPlayerDbid == dbid; }
		public bool IsSinglePlayerMode { get { return CurrentPlayerCount == 1; } }
		
		public SimGameType SimGameType { get { return _simGameType; } set { _simGameType = value; } }
		public bool HasPopulatedLeaderboard { get { return _hasPopulatedLeaderboard; } set { _hasPopulatedLeaderboard = value; } }

		//  Fields  --------------------------------------
		public const int UnsetPlayerCount = -1;
		private bool _isMatchmakingComplete;
		private bool _hasPopulatedLeaderboard = false;
		private long _localPlayerDbid;
		private string _roomId;
		private int _currentPlayerCount;
		private SimGameType _simGameType = null;


		//  Unity Methods  --------------------------------
		protected override void Awake()
		{
			base.Awake();
			ClearData();
		}

		//  Other Methods  --------------------------------
		/// <summary>
		/// Demonstrates that the lifecycle of data is for runtime only
		/// </summary>
		private void ClearData()
		{
			_isMatchmakingComplete = false;
			_localPlayerDbid = 0;
			_roomId = "";
			_currentPlayerCount = UnsetPlayerCount;
		}
   }
}

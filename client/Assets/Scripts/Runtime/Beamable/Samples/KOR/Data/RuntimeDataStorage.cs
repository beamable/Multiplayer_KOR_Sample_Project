using System;
using Beamable.Common.Content;
using Beamable.Samples.Core;

namespace Beamable.Samples.KOR.Data
{
	/// <summary>
	/// Store game-related data which survives across scenes
	/// </summary>
	public class RuntimeDataStorage : SingletonMonobehavior<RuntimeDataStorage>
	{
		//  Properties  ----------------------------------
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
				return _activeSimGameType.minPlayersToStart.Value;
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
				return _activeSimGameType.maxPlayers;
			}
		}
		public string RoomId { get { return _roomId; } set { _roomId = value; } }
		public long LocalPlayerDbid { get { return _localPlayerDbid; } set { _localPlayerDbid = value; } }
		public bool IsLocalPlayerDbid (long dbid) { return LocalPlayerDbid == dbid; }

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

		public SimGameType ActiveSimGameType { get { return _activeSimGameType; } set { _activeSimGameType = value; } }
		public bool HasPopulatedLeaderboard { get { return _hasPopulatedLeaderboard; } set { _hasPopulatedLeaderboard = value; } }

		
		//  Fields  --------------------------------------
		private bool _isMatchmakingComplete;
		private bool _hasPopulatedLeaderboard;
		private long _localPlayerDbid;
		private string _roomId;
		private int _currentPlayerCount;
		private int _targetPlayerCount;
		private SimGameType _activeSimGameType;


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
			 _hasPopulatedLeaderboard = false;
			_localPlayerDbid= KORConstants.UnsetValue;
			_roomId = "";
			_currentPlayerCount = KORConstants.UnsetValue;
			_targetPlayerCount = KORConstants.UnsetValue;
			_activeSimGameType = null;
		}
   }
}


using Beamable.Samples.Core;

namespace Beamable.Samples.TBF.Data
{
	/// <summary>
	/// Store game-related data which survives across scenes
	/// </summary>
	public class RuntimeDataStorage : SingletonMonobehavior<RuntimeDataStorage>
	{

		//  Properties  ----------------------------------
		public long LocalPlayerDbid { get { return _localPlayerDbid; } set { _localPlayerDbid = value; } }
		public string RoomId { get { return _roomId; } set { _roomId = value; } }
		public int TargetPlayerCount { get { return _targetPlayerCount; } set { _targetPlayerCount = value; } }
      public bool IsMatchmakingComplete { get { return _isMatchmakingComplete; } set { _isMatchmakingComplete = value; } }

		//  Fields  --------------------------------------
		public const int UnsetPlayerCount = -1;
		private bool _isMatchmakingComplete;
		private long _localPlayerDbid;
		private string _roomId;
		private int _targetPlayerCount;

		//  Unity Methods  --------------------------------

		protected override void Awake()
		{
			base.Awake();
			ClearData();
		}

		//  Other Methods  --------------------------------

		/// <summary>
		/// Demonstrates that the lifecycle of data is runtime only
		/// </summary>
		private void ClearData()
      {
			_isMatchmakingComplete = false;
			_localPlayerDbid = 0;
			_roomId = "";
			_targetPlayerCount = UnsetPlayerCount;
		}
   }
}

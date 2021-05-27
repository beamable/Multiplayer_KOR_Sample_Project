using static Beamable.Samples.TBF.Multiplayer.RemotePlayerAI;

namespace Beamable.Samples.TBF
{
   /// <summary>
   /// Store commonly used static values
   /// </summary>
   public static class TBFConstants
   {

      //  Fields ---------------------------------------

      /// <summary>
      /// Determines if using Unity debug log statements.
      /// </summary>
      public static bool IsDebugLogging = true;

      /// <summary>
      /// Toggle various modes for AI testing
      /// </summary>
      public static AIMode AIMode = AIMode.Production;

      /// <summary>
      /// Used as a 'null' value.
      /// </summary>
      public const int UnsetValue = -1;

      /// <summary>
      /// The index for the LOCAL player in <see cref="System.Collections.Generic.List{T}"/>s.
      /// </summary>
      public const int PlayerIndexLocal = 0;

      /// <summary>
      /// The index for the REMOTE player in <see cref="System.Collections.Generic.List{T}"/>s.
      /// </summary>
      public const int PlayerIndexRemote = 1;

      // Round
      public static string RoundText = "<b>Round</b><br><size=20>{0} of {1}</size>";

      // Status
      public const string StatusText_GameState_Loading = "Beamable Loading ...";
      public const string StatusText_GameState_Loaded = "Beamable Loaded ...";
      public const string StatusText_GameState_Initializing = "Beamable Initializing ...";
      public const string StatusText_GameState_Initialized = "Beamable Initialized ...";
      public const string StatusText_GameState_Connecting = "Connecting. Players <b>{0}</b>/<b>{1}</b> ...";
      public const string StatusText_Multiplayer_OnDisconnect = "Disconnecting. Players Remaining <b>{0}</b>/<b>{1}</b>. Game Over.";
      public const string StatusText_GameState_PlayerMoving = "Waiting For Moves ...";
      public const string StatusText_GameState_PlayerMoved = "<b>{0}</b> Player Moves <b>{1}</b> ...";
      public const string StatusText_GameState_PlayersAllMoved = "All Moves Complete ...";
      public const string StatusText_GameState_EvaluatedWinner = "Round <b>{0}</b> Over. Round Winner <b>{1}</b> Player ...";
      public const string StatusText_GameState_EvaluatedTie = "Round <b>{0}</b> Over. <b>Tie!</b> Repeat Round {0}...";
      public const string StatusText_GameState_Ending = "Round <b>{0}</b> Over. Game Over. Game Winner <b>{1}</b> ...";
      //
      public static string Avatar_Idle = "Idle"; //start here
      public static string Avatar_Attack_01 = "Attack_01";
      public static string Avatar_Attack_02 = "Attack_02";
      public static string Avatar_Attack_03 = "Attack_03";
      public static string Avatar_Death = "Death"; //end here

      //Lobby
      public static string StatusText_Joining = "Player {0}/{1} joined. Waiting ...";
      public static string StatusText_Joined = "Player {0}/{1} joined. Ready!";



   }
}
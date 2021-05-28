namespace Beamable.Samples.KOR
{
   /// <summary>
   /// Store commonly used static values
   /// </summary>
   public static class KORConstants
   {

      //  Fields ---------------------------------------

      /// <summary>
      /// Determines if using Unity debug log statements.
      /// </summary>
      public static bool IsDebugLogging = true;

      /// <summary>
      /// Used as a 'null' value.
      /// </summary>
      public const int UnsetValue = -1;

      // Round
      public static string RoundText = "<b>Round</b><br><size=20>{0} of {1}</size>";

      // Status
      public const string StatusText_GameState_Playing = "Playing ...";
      
      // Animations
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
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

      // Display Text
      public static string RoundText = "<b>Round</b><br><size=20>{0} of {1}</size>";
      public static string AvatarUIView_Remote = "Remote";
      public static string AvatarUIView_Local = "Local";
      public static string AvatarUIView_Offline = "Offline";
         

      // Status
      public const string StatusText_GameState_Playing = "Playing ...";
      
      // Animations
      public static string Avatar_WalkForward = "Walk Forward"; //start here
      public static string Avatar_RunForward = "Run Forward";
      public static string Avatar_Attack01 = "Attack 01";
      public static string Avatar_Attack02 = "Attack 02";
      public static string Avatar_TakeDamage = "Take Damage";
      public static string Avatar_Die = "Die";

      //Lobby
      public static string StatusText_Joining = "Player {0}/{1} joined. Waiting ...";
      public static string StatusText_Joined = "Player {0}/{1} joined. Ready!";

   }
}
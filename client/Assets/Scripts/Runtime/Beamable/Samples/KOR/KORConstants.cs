namespace Beamable.Samples.KOR
{
   /// <summary>
   /// Store commonly used static values
   /// </summary>
   public static class KORConstants
   {
      
      //  Fields ---------------------------------------

      /// <summary>
      /// Used as a 'null' value.
      /// </summary>
      public const int UnsetValue = -1;
      
      // Animations
      public const string Avatar_WalkForward = "Walk Forward"; //start here
      public const string Avatar_RunForward = "Run Forward";
      public const string Avatar_Attack01 = "Attack 01";
      public const string Avatar_Attack02 = "Attack 02";
      public const string Avatar_TakeDamage = "Take Damage";
      public const string Avatar_Die = "Die";
      
      // Display Text
      public const string AvatarUIView_Remote = "Remote";
      public const string AvatarUIView_Local = "Local";
      public const string AvatarUIView_Offline = "Offline";
      public const string GameUIView_Playing = "Playing ...";
      public const string LobbyUIView_Joining = "Player {0}/{1} joined. Waiting {2}...";
      public const string LobbyUIView_Joined = "Player {0}/{1} joined. Ready!";
      public const string StoreUIView_Loading_Store = "Loading Store...";
      public const string StoreUIView_Loading_Inventory = "Loading Inventory ...";
      public const string StoreUIView_Instructions = "You have {0} {1}s. Select item and 'Buy'!";
      public const string StoreUIView_CurrencyName = "Coin";
   }
}
namespace Beamable.Samples.KOR
{
   /// <summary>
   /// Store commonly used static values
   /// </summary>
   public static class KORConstants
   {

      //  Consts ---------------------------------------
      public const string ItemContentType = "items";
      public const string CurrencyContentType = "currency";

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
      public const string LobbyUIView_Finalizing = "Player {0}/{1} joined. Finalizing...";
      public const string LobbyUIView_Waiting = "Player {0}/{1} Acquiring Match...";
      public const string LobbyUIView_Joined = "Player {0}/{1} joined. Ready!";
      public const string StoreUIView_Loading_Store = "Loading Store...";
      public const string StoreUIView_Loading_Inventory = "Loading Inventory ...";
      public const string StoreUIView_InventoryTip = "You Have {0} {1}{2}.";
      public const string StoreUIView_StoreTip = "Today's Specials...";
      public const string StoreUIView_Instructions = "Select an item and 'Buy'!";
      public const string StoreUIView_SelectStoreInventory = "Gives bonus " + Player_Attributes;
      public const string StoreUIView_CannotAfford = "Can't Afford. ";
      public const string StoreUIView_SelectStoreItem = "{2}Gives bonus " + Player_Attributes;
      public const string StoreUIView_CurrencyName = "Coin";
      public const string Player_Attributes = "C:{0:000} M:{1:000}";
      public const string Dialog_AreYouSure = "Are you sure?";
      public const string Dialog_GameOver_Victory = "Game Over - Victory!!";
      public const string Dialog_GameOver_Defeat = "Game Over";
      public static string Dialog_Ok = "Ok";
      public static string Dialog_Cancel = "Cancel";


   }
}
using Beamable.Samples.KOR.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Samples.KOR.Views
{
   /// <summary>
   /// Handles the audio/graphics rendering logic: Leaderboard UI
   /// </summary>
   public class LeaderboardUIView : BaseUIView
   {
      //  Properties -----------------------------------
      public TMP_BufferedText BufferedText { get { return _bufferedText; } }
      public Button BackButton { get { return _backButton; } }
      public KORLeaderboardItem KORLeaderboardItem { get { return _korLeaderboardItem; }}
      public KORLeaderboardMainMenu KORLeaderboardMainMenu { get { return _korLeaderboardMainMenu; }}
      
      //  Fields ---------------------------------------
      [SerializeField]
      private TMP_BufferedText _bufferedText = null;

      [SerializeField]
      private Button _backButton = null;
      
      [SerializeField]
      private KORLeaderboardMainMenu _korLeaderboardMainMenu = null;
        
      [Header ("Cosmetics")]
      [SerializeField]
      private KORLeaderboardItem _korLeaderboardItem = null;

      
      //  Unity Methods   ------------------------------
      public void Start()
      {
         CanvasGroupsDoFadeIn();
      }
   }
}
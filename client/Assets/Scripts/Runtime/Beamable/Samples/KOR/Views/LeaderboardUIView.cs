using Beamable.Samples.KOR.UI;
using UnityEngine;

namespace Beamable.Samples.KOR.Views
{
   /// <summary>
   /// Handles the audio/graphics rendering logic: Lobby UI
   /// </summary>
   public class LeaderboardUIView : BaseUIView
   {
      //  Properties -----------------------------------
      public KORLeaderboardItem KORLeaderboardItem { get { return _korLeaderboardRowUIPrefab; }}


      //  Fields ---------------------------------------
      [Header ("Cosmetics")]
      [SerializeField]
      private KORLeaderboardItem _korLeaderboardRowUIPrefab = null;

      
      //  Unity Methods   ------------------------------
      public void Start()
      {
         CanvasGroupsDoFadeIn();
      }
   }
}
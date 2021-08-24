using System.Threading.Tasks;
using Beamable.Api;
using Beamable.Common;
using Beamable.Common.Api.Leaderboards;
using Beamable.Leaderboards;
using Beamable.Samples.KOR.Data;
using Beamable.UI.Scripts;
using UnityEngine;
using UnityEngine.Events;

namespace Beamable.Samples.KOR.UI
{
    /// <summary>
    /// Replace native Beamable class with subclass of
    /// additional functionality.
    ///
    /// The Beamable "KORLeaderboardMainMenu" was duplicated and
    /// pasted below. Modifications added.
    /// </summary>
    public class KORLeaderboardMainMenu : MenuBase
    {
       //  Events ---------------------------------------
       public UnityEvent OnRendered = new UnityEvent();

       //  Fields ---------------------------------------
      public KORLeaderboardItem KORLeaderboardItem;
      public LeaderboardBehavior LeaderboardBehavior;
      public Transform LeaderboardEntries;
      public bool willAutoRender = true;
      private const int maxRetries = 5;
      
      //  Unity Methods ---------------------------------
      protected void OnEnable()
      {
         if (willAutoRender)
         {
            Render();
         }
      }

      //  Other Methods   ------------------------------
      private void DebugLog(string message)
      {
         // Respects Configuration.IsDebugLog Checkbox
         Configuration.Debugger.Log(message);
      }
      
      
      public async void Render()
      {
         var de = await API.Instance;

         // There is a timing issue the first time a leaderboard is accessed for a particular realm.
         // This is a small bandage to fix that issue.
         var retry = 0;
         while (retry < maxRetries)
         {
            try
            {
               await FetchBoard(de);
               return;
            }
            catch (PlatformRequesterException e)
            {
               if (e.Status == 404)
               {
                  await Task.Delay(500);
                  retry++;
                  if (retry == maxRetries)
                  {
                     DebugLog(e.Message);
                  }
               }
               else
               {
                  throw;
               }
            }
         }

         DebugLog("Unable to load. Please check LeaderboardFlow GameObject has Leaderboard field set.");
      }


      private Promise<LeaderBoardView> FetchBoard(IBeamableAPI de)
      {
         return de.LeaderboardService.GetBoard(LeaderboardBehavior.Leaderboard.Id, 0, 50)
            .Error(err =>
            {
               // Explicitly do nothing to prevent an error from being logged.
            })
            .Then(HandleBoard);
      }

      private void HandleBoard(LeaderBoardView board)
      {
         // Clear all data
         for (var i = 0; i < LeaderboardEntries.childCount; i++)
         {
            Destroy(LeaderboardEntries.GetChild(i).gameObject);
         }

         // Populate lines
         foreach (var rank in board.rankings)
         {
            var leaderboardItem = Instantiate(KORLeaderboardItem, LeaderboardEntries);
            leaderboardItem.Apply(rank);
         }
         
         OnRendered.Invoke();
      }
   }
}
using System.Collections.Generic;
using System.Linq;
using Beamable.Api.Auth;
using Beamable.Api.Leaderboard;
using Beamable.Api.Stats;
using Beamable.Common.Api.Leaderboards;
using Beamable.Common.Leaderboards;
using UnityEngine;
using Random = System.Random;

namespace Beamable.Samples.KOR.Data
{
   /// <summary>
   /// Create mock data. This is appropriate for a sample project, but not for
   /// production.
   /// </summary>
   public static class MockDataCreator
   {
      //  Other Methods --------------------------------

      /// <summary>
      /// Because this game is NOT a production game with real users, it is helpful
      /// to populate the Leaderboard with some mock users scores for
      /// cosmetic reasons
      /// </summary>
      /// <param name="beamableAPI"></param>
      /// <param name="leaderboardContent"></param>
      /// <param name="configuration"></param>
      public static async void PopulateLeaderboardWithMockData(IBeamableAPI beamableAPI,
         LeaderboardContent leaderboardContent, Configuration configuration)
      {
         LeaderboardService leaderboardService = beamableAPI.LeaderboardService;
         StatsService statsService = beamableAPI.StatsService;
         IAuthService authService = beamableAPI.AuthService;

         // Capture current user
         var localDbid = beamableAPI.User.id;

         // Check Leaderboard
         LeaderBoardView leaderboardView = await leaderboardService.GetBoard(leaderboardContent.Id, 0, 100);

         // Not enough data in the leaderboard? Create users with mock scores
         int currentRowCount = leaderboardView.rankings.Count;
         int targetRowCount = configuration.LeaderboardMinRowCount;

         Debug.Log($"PopulateLeaderboardWithMockData() BEFORE, rowCount={currentRowCount}");

         if (currentRowCount < targetRowCount)
         {
            int itemsToCreate = targetRowCount - currentRowCount;
            for (int i = 0; i < itemsToCreate; i++)
            {
               // Create NEW user
               // Login as NEW user (Required before using "SetScore")
               await authService.CreateUser().FlatMap(beamableAPI.ApplyToken);

               // Rename NEW user
               string alias = MockDataCreator.CreateNewRandomAlias("Player");
               MockDataCreator.SetCurrentUserAlias(statsService, alias);
           
               // Submit mock score for NEW user
               double mockScore = UnityEngine.Random.Range(configuration.LeaderboardMockScoreMin, 
                  configuration.LeaderboardMockScoreMax);
               
               mockScore = KORHelper.GetRoundedScore(mockScore);
               await leaderboardService.SetScore(leaderboardContent.Id, mockScore);

               Debug.Log($"PopulateLeaderboardWithMockData() Created Mock User. Alias={alias}, score:{mockScore}.");

            }
         }

         LeaderBoardView leaderboardViewAfter = await leaderboardService.GetBoard(leaderboardContent.Id, 0, 100);
         int currentRowCountAfter = leaderboardViewAfter.rankings.Count;
         Debug.Log($"PopulateLeaderboardWithMockData() AFTER, rowCount={currentRowCountAfter}");

         // Login again as local user
         var deviceUsers = await beamableAPI.GetDeviceUsers();
         var user = deviceUsers.First(bundle => bundle.User.id == localDbid);
         await beamableAPI.ApplyToken(user.Token);
      }

      /// <summary>
      /// The the user alias which is visible in the Leaderboard Scene
      /// </summary>
      /// <param name="statsService"></param>
      /// <param name="alias"></param>
      public static async void SetCurrentUserAlias(StatsService statsService, string alias)
      {
         await statsService.SetStats("public", new Dictionary<string, string>()
            {
               { "alias", alias },
            });
      }


      /// <summary>
      /// Inspired by http://developer.qbapi.com/Generate-a-Random-Username.aspx
      /// </summary>
      private static string CreateNewRandomAlias(string prependName)
      {
         string alias = prependName;

         char[] lowers = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'j', 'k', 'm', 'n', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
         char[] uppers = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
         char[] numbers = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

         int l = lowers.Length;
         int u = uppers.Length;
         int n = numbers.Length;

         Random random = new Random();
         alias += "_";
         //
         alias += lowers[random.Next(0, l)].ToString();
         //
         alias += uppers[random.Next(0, u)].ToString();
         //
         alias += "_";
         //
         alias += numbers[random.Next(0, n)].ToString();
         alias += numbers[random.Next(0, n)].ToString();
         alias += numbers[random.Next(0, n)].ToString();

         return alias;
      }
   }
}
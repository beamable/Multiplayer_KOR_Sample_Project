using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using Beamable.Samples.TBF.Audio;
using UnityEngine.SceneManagement;

namespace Beamable.Samples.TBF
{
   /// <summary>
   /// Store commonly reused functionality for concerns: General
   /// </summary>
   public static class TBFHelper
   {

      //  Other Methods --------------------------------
      public static IEnumerator LoadScene_Coroutine(string sceneName, float delayBeforeLoading)
      {
         SoundManager.Instance.PlayAudioClip(SoundConstants.Click01);

         if (TBFConstants.IsDebugLogging)
         {
            Debug.Log($"LoadScene() to {sceneName}");
         }

         yield return new WaitForSeconds(delayBeforeLoading);
         SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
      }


      /// <summary>
      /// Return the intro menu text. This serves as a welcome to the game plot and game instructions.
      /// If error, help text is shown.
      /// </summary>
      public static string GetIntroAboutBodyText(bool isConnected, long dbid,
         bool isBeamableSDKInstalled, string isBeamableSDKInstalledErrorMessage)
      {
         string text = "";

         // Is Beamable SDK Properly Installed In Unity?
         if (isBeamableSDKInstalled)
         {
            // Is Game Properly Connected To Internet?
            if (isConnected)
            {
               text += TBFHelper.GameInstructionsText;
               text += TBFHelper.GetBulletList("Status", new List<string>
               {
                  $"Connected: {true}",
                  $"DBID: {dbid}", 
               });
            }
            else
            {
               // Error
               text += TBFHelper.InternetOfflineInstructionsText;
            }
         }
         else
         {
            // Error
            text += $"_isBeamableSDKInstalled = {isBeamableSDKInstalled}." + "\n\n";
            text += $"_isBeamableSDKInstalledErrorMessage = {isBeamableSDKInstalledErrorMessage}" + "\n\n";
            text += TBFHelper.BeamableSDKInstallInstructionsText;
         }

         return text;
      }


      /// <summary>
      /// Return a random item from the list. 
      /// This provides cosmetic variety.
      /// </summary>
      public static string GetRandomString(List<string> items)
      {
         int index = UnityEngine.Random.Range(0, items.Count);
         return items[index];
      }


      public static float GetAudioPitchByGrowthPercentage(float growthPercentage)
      {
         //Range from 0.5 to 1.5
         return 0.5f + Mathf.Clamp01(growthPercentage);
      }


      public static string InternetOfflineInstructionsText
      {
         get
         {
            string text = "";
            text += "<color=#ff0000>You are currently offline.</color>" + "\n\n";
            text += "<align=left>";
            text += TBFHelper.GetBulletList("Suggestions\n", new List<string> {
               "Stop the Scene in the Unity Editor",
               "Connect to the internet",
               "Play the Scene in the Unity Editor",
               "Enjoy!"
            }); ;
            text += "</align>";
            return text;
         }
      }


      /// <summary>
      /// Convert the <see cref="float"/> to a <see cref="string"/>
      /// with rounding like "10.1";
      /// </summary>
      public static string GetRoundedTime(float value)
      {
         return string.Format("{0:0.0}", value);
      }


      /// <summary>
      /// Convert the <see cref="double"/> to a whole number like an <see cref="int"/>.
      /// </summary>
      public static double GetRoundedScore(double score)
      {
         return (int)score;
      }


      /// <summary>
      /// Convert the <see cref="string"/> to a whole number like an <see cref="int"/>.
      /// </summary>
      public static double GetRoundedScore(string score)
      {
         return GetRoundedScore(Double.Parse(score));
      }


      private static string GameInstructionsText
      {
         get
         {
            string text = "";

            text += "Choose strategic attacks to win! Only the bravest knight survives." + "\n\n";

            text += "This sample project demonstrates Beamable's Multiplayer feature.\n\n";

            text += TBFHelper.GetBulletList("Resources", new List<string>
            {
               "Overview: <u><link=https://docs.beamable.com/docs/multiplayer-sample>Multiplayer Sample</u>",
               "Feature: <u><link=https://docs.beamable.com/docs/multiplayer>Multiplayer</u>",
            });


            return text;
         }
      }


      private static string BeamableSDKInstallInstructionsText
      {
         get
         {
            string text = "";
            text += "<color=#ff0000>";
            text += TBFHelper.GetBulletList("Todo", new List<string> {
               "Download & Install <u><link=http://docs.beamable.com>Beamable SDK</link></u>",
               "Open the Beamable Toolbox Window in Unity",
               "Register or Sign In"
            });
            text += "</color>";
            return text;
         }
      }

      private static string GetBulletList(string title, List<string> items)
      {
         string text = "";
         text += $"{title}" + "\n";
         text += "<indent=5%>";
         foreach (string item in items)
         {
            text += $"• {item}" + "\n";
         }
         text += "</indent>" + "\n";
         return text;
      }

   }
}
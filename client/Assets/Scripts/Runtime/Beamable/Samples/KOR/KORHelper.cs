using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using System.Threading.Tasks;
using Beamable.Samples.KOR.Animation;
using Beamable.Samples.KOR.Audio;
using Beamable.Samples.KOR.CustomContent;
using Beamable.UI.Scripts;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Beamable.Samples.KOR
{
   /// <summary>
   /// Store commonly reused functionality for concerns: General
   /// </summary>
   public static class KORHelper
   {

      //  Other Methods --------------------------------
      public static IEnumerator LoadScene_Coroutine(string sceneName, float delayBeforeLoading)
      {
         yield return new WaitForSeconds(delayBeforeLoading);
         SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
      }
      
      /// <summary>
      /// Changing scenes and other 'big' things
      /// </summary>
      public static void PlayAudioForUIClickPrimary()
      {
         SoundManager.Instance.PlayAudioClip(SoundConstants.Chime02);
      }
      
      /// <summary>
      /// Changing settings and other 'small' things
      /// </summary>
      public static void PlayAudioForUIClickSecondary()
      {
         SoundManager.Instance.PlayAudioClip(SoundConstants.Click01);
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
               text += KORHelper.GameInstructionsText;
               text += KORHelper.GetBulletList("Status", new List<string>
               {
                  $"Connected: {true}",
                  $"DBID: {dbid}", 
               });
            }
            else
            {
               // Error
               text += KORHelper.InternetOfflineInstructionsText;
            }
         }
         else
         {
            // Error
            text += $"_isBeamableSDKInstalled = {isBeamableSDKInstalled}." + "\n\n";
            text += $"_isBeamableSDKInstalledErrorMessage = {isBeamableSDKInstalledErrorMessage}" + "\n\n";
            text += KORHelper.BeamableSDKInstallInstructionsText;
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


      public static string InternetOfflineInstructionsText
      {
         get
         {
            string text = "";
            text += "<color=#ff0000>You are currently offline.</color>" + "\n\n";
            text += "<align=left>";
            text += KORHelper.GetBulletList("Suggestions\n", new List<string> {
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

            text += "Attack with power & dodge with speed! Last player alive, wins." + "\n\n";

            text += "This sample project demonstrates Beamable's Multiplayer feature.\n\n";
            
            text += KORHelper.GetBulletList("Resources", new List<string>
            {
               "Overview: <u><link=https://docs.beamable.com/docs/multiplayer-kor-sample>Multiplayer (KOR) Sample</link></u>",
               "Feature: <u><link=https://docs.beamable.com/docs/multiplayer-feature>Multiplayer</link></u>",
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
            text += KORHelper.GetBulletList("Todo", new List<string> {
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

      public static string GetSceneLoadingMessage(string sceneName, bool wasLoadedDirectly)
      {
         if (wasLoadedDirectly)
         {
           return $"Scene '{sceneName}' loaded directly per debugging. That is ok. Setting defaults.";
         }
         else
         {
            return $"Scene '{sceneName}' loaded indirectly per production.";
         }
      }

      public static string GetKORItemDisplayNameFromContentId(string contentId)
      {
         //Change "items.KORItems.Blah" to "Blah"
         var tokens = contentId.Split('.');
         return tokens[tokens.Length - 1];
      }

      public static void SetSkyboxRotation(float rotation)
      {
         RenderSettings.skybox.SetFloat("_Rotation", rotation);
      }

      public static async Task<KORItemContent> GetKORItemContentById(IBeamableAPI _beamableAPI, string id)
      {
        return  await _beamableAPI.ContentService.GetContent(id, typeof(KORItemContent)) as KORItemContent;
      }

      /// <summary>
      /// Return pluralization or not
      /// </summary>
      /// <param name="count"></param>
      /// <returns></returns>
      public static string GetPluralization(int count)
      {
         return count > 1 ? "s" : "";
      }

      /// <summary>
      /// Lazily load a AssetReferenceTexture2D into an Image
      /// </summary>
      /// <param name="assetReferenceTexture2D"></param>
      /// <param name="destinationImage"></param>
      /// <typeparam name="T"></typeparam>
      public static void AddressablesLoadAssetAsync<T>(AssetReferenceTexture2D assetReferenceTexture2D, Image destinationImage)
      {
         // Hide it
         TweenHelper.ImageDoFade(destinationImage, 0, 0, 0, 0);
         
         AsyncOperationHandle<Texture2D> asyncOperationHandle1 = Addressables.LoadAssetAsync<Texture2D>(
            assetReferenceTexture2D);

         asyncOperationHandle1.Completed += (AsyncOperationHandle<Texture2D> asyncOperationHandle2) =>
         {
            Texture2D texture2D = asyncOperationHandle2.Result;
            destinationImage.sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height),
               new Vector2(0.5f, 0.5f));

            // Show it
            TweenHelper.ImageDoFade(destinationImage, 0, 1, 0.25f, 0);
         };
      }
      
      public static async void AddressablesLoadAssetAsync<T>(AssetReferenceSprite assetReferenceSprite, Image destinationImage)
      {
         // Check before await
         if (destinationImage == null || assetReferenceSprite == null)
         {
            return;
         }
         
         // Hide it
         TweenHelper.ImageDoFade(destinationImage, 0, 0, 0, 0);

         Sprite sprite = await AddressableSpriteLoader.LoadSprite(assetReferenceSprite);

         // Check after await
         if (destinationImage == null || assetReferenceSprite == null)
         {
            return;
         }
         
         if (sprite != null)
         {
            destinationImage.sprite = sprite;
            
            // Show it
            TweenHelper.ImageDoFade(destinationImage, 0, 1, 0.25f, 0);
         }
      }
   }
}
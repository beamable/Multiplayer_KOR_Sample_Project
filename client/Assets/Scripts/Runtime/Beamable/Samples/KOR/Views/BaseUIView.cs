using System.Collections.Generic;
using Beamable.Samples.KOR.Animation;
using Beamable.Samples.KOR.Data;
using UnityEngine;

namespace Beamable.Samples.KOR.Views
{
   /// <summary>
   /// Handles the audio/graphics rendering logic: Shared for various UI Views
   /// </summary>
   public class BaseUIView : MonoBehaviour
   {
      //  Properties -----------------------------------
      public Camera Camera { get { return _camera; } }
      public bool WillRecolorCamera { get { return _willRecolorCamera; } }
      public Configuration Configuration { get { return _configuration; } }
      public List<CanvasGroup> CanvasGroups { get { return _canvasGroups; } }

      //  Fields ---------------------------------------
      [Header ("Base Properties")]
      [SerializeField]
      private Camera _camera = null;

      [SerializeField]
      private bool _willRecolorCamera = true;

      [SerializeField]
      private Configuration _configuration = null;

      [SerializeField]
      private List<CanvasGroup> _canvasGroups = null;


      //  Unity Methods   ------------------------------
      protected void OnValidate()
      {
         if (_camera == null || !_willRecolorCamera)
         {
            return;
         }
         
         if (_camera.backgroundColor != _configuration.CameraBackgroundColor)
         {
            _camera.backgroundColor = _configuration.CameraBackgroundColor;
         }
      }
      
      //  Other Methods   ------------------------------
      public void CanvasGroupsDoFadeOut()
      {
         // Instantly fade to 0
         TweenHelper.CanvasGroupsDoFade(_canvasGroups, 0, 0, 0, 0, 0);
      }
      
      public void CanvasGroupsDoFadeIn()
      {
         // Slowly fade to 1
         TweenHelper.CanvasGroupsDoFade(_canvasGroups, 0, 1, 1, Configuration.DelayBeforeFadeInUI, Configuration.DelayBetweenFadeInUI);
      }
   }
}
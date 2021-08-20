using System.Collections.Generic;
using Beamable.Samples.Core.UI.DialogSystem;
using Beamable.Samples.Core.Utilities;
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
      public Configuration Configuration { get { return _configuration; } }
      public List<CanvasGroup> CanvasGroups { get { return _canvasGroups; } }
      
      public DialogSystem DialogSystem { get { return _dialogSystem; } }
      

      //  Fields ---------------------------------------
      [Header ("Base Fields")]
      [SerializeField]
      private Camera _camera = null;

      [SerializeField]
      private Configuration _configuration = null;

      [SerializeField] 
      private DialogSystem _dialogSystem = new DialogSystem();

      [SerializeField]
      private List<CanvasGroup> _canvasGroups = null;


      //  Unity Methods   ------------------------------
      protected void OnValidate()
      {
         if (RenderSettings.skybox != _configuration.CameraSkyboxMaterial)
         {
            RenderSettings.skybox = _configuration.CameraSkyboxMaterial;
         }
      }

      protected virtual void Update()
      {
         if (_configuration.IsRotatingCameraSkybox)
         {
            KORHelper.SetSkyboxRotation(Time.time * _configuration.CameraSkyboxRotationSpeed);

         }
      }

      protected void OnDestroy()
      {
         // Simply done to prevent randomly dirtying git version control on the material
         KORHelper.SetSkyboxRotation(0);
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
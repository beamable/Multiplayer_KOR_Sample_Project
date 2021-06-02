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

      //  Fields ---------------------------------------
      [Header ("Base Properties")]
      [SerializeField]
      private Camera _camera = null;

      [SerializeField]
      private Configuration _configuration = null;

      //  Unity Methods   ------------------------------
      protected void OnValidate()
      {
         if (_camera == null)
         {
            return;
         }
         
         if (_camera.backgroundColor != _configuration.CameraBackgroundColor)
         {
            _camera.backgroundColor = _configuration.CameraBackgroundColor;
         }
      }
   }
}
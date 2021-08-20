using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Samples.Core.Utilities
{
   /// <summary>
   /// Store commonly reused functionality 
   /// for programmatic animation (tweening)
   /// </summary>
   public static class TweenHelper
   {
      //  Other Methods --------------------------------

      /// <summary>
      /// Fades opacity of a list of 2D objects over time, in series.
      /// </summary>
      public static void CanvasGroupsDoFade(List<CanvasGroup> canvasGroups,
               float fromAlpha, float toAlpha, float duration, float delayStart, float delayDelta)
      {

         float delay = delayStart;

         foreach (CanvasGroup canvasGroup in canvasGroups)
         {
            // Fade out immediately
            canvasGroup.DOFade(fromAlpha, 0);

            // Fade in slowly
            canvasGroup.DOFade(toAlpha, duration).SetDelay(delay);

            delay += delayDelta;
         }
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="spriteRenderer"></param>
      /// <param name="fromAlpha"></param>
      /// <param name="toAlpha"></param>
      /// <param name="duration"></param>
      /// <param name="delayStart"></param>
      public static void SpriteRendererDoFade(SpriteRenderer spriteRenderer,
         float fromAlpha, float toAlpha, float duration, float delayStart)
      {
            // Fade out immediately
            spriteRenderer.DOFade(fromAlpha, 0);
            
            // Fade in slowly
            spriteRenderer.DOFade(toAlpha, duration).SetDelay(delayStart);
      }
      
      
      /// <summary>
      /// 
      /// </summary>
      /// <param name="image"></param>
      /// <param name="fromAlpha"></param>
      /// <param name="toAlpha"></param>
      /// <param name="duration"></param>
      /// <param name="delayStart"></param>
      public static void ImageDoFade(Image image,
         float fromAlpha, float toAlpha, float duration, float delayStart)
      {
         // Fade out immediately
         image.DOFade(fromAlpha, 0);
         
         // Fade in slowly
         image.DOFade(toAlpha, duration).SetDelay(delayStart);
      }
      
      /// <summary>
      /// Fades opacity of a 3D object over time.
      /// </summary>
      public static void RenderersDoFade(List<Renderer> renderers, float fromAlpha, float toAlpha, float delay, float duration)
      {
         foreach (Renderer r in renderers)
         {
            foreach (Material m in r.materials)
            {
               m.DOFade(0, 3).SetDelay(delay);
            }
         }
      }

      /// <summary>
      /// Changes color of a 3D object temporarily. 
      /// (E.g. Flicker red to indicate taking damage)
      /// </summary>
      public static void RenderersDoColorFlicker(List<Renderer> renderers, Color color, float duration)
      {
         foreach (Renderer r in renderers)
         {
            foreach (Material m in r.materials)
            {
               Color oldColor = m.color;
               m.DOColor(color, duration).OnComplete(() =>
               {
                  m.color = oldColor;
               });
            }
         }
      }

      /// <summary>
      /// Move a <see cref="GameObject"/>
      /// </summary>
      /// <param name="targetGo"></param>
      /// <param name="fromPosition"></param>
      /// <param name="toPosition"></param>
      /// <param name="duration"></param>
      public static void TransformDOBlendableMoveBy(GameObject targetGo, Vector3 fromPosition, Vector3 toPosition, float duration)
      {
         targetGo.transform.DOBlendableMoveBy(fromPosition, 0);
         targetGo.transform.DOBlendableMoveBy(toPosition, duration).
            SetDelay(0.01f).
            SetEase(Ease.InBounce);
      }

      /// <summary>
      /// Scale a <see cref="GameObject"/>
      /// </summary>
      /// <param name="targetGo"></param>
      /// <param name="fromScale"></param>
      /// <param name="toScale"></param>
      /// <param name="duration"></param>
      public static void TransformDoScale(GameObject targetGo, Vector3 fromScale, Vector3 toScale, float duration)
      {
         targetGo.transform.DOScale(fromScale, 0);
         targetGo.transform.DOScale(toScale, duration).
            SetDelay(0.01f).
            SetEase(Ease.InBounce);
      }
   }
}
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Beamable.Samples.TBF.Views
{
   /// <summary>
   /// Handles the view concerns for UI elements related
   /// to the points of score or damage
   /// </summary>
   public class PointsView : MonoBehaviour
   {
      //  Consts ---------------------------------------
      private const float DeltaYMove = 0.2f;
      private const float DurationTime = 0.5f;
      
      //  Properties -----------------------------------
      public int Points
      {
         set { RenderUI($"{value:00}!"); }
      }

      public string Text
      {
         set { RenderUI(value); }
      }

      //  Fields ---------------------------------------
      [SerializeField]
      private TextMeshProUGUI _text = null;

      private Tween _tween = null;

      //  Unity Methods   ------------------------------
      protected void Awake ()
      {
         _text.text = "";
      }

      protected void OnDestroy()
      {
         if (_tween != null)
         {
            _tween.Kill();
         }
      }

      //  Other Methods --------------------------------
      private void RenderUI(string message)
      {
         _text.text = message;

         _tween = gameObject.transform.DOLocalMoveY(gameObject.transform.position.y + DeltaYMove, 
            DurationTime, false)
            .OnComplete<Tween>(DoLocalMoveY_OnCompleted);
      }


      //  Event Handlers -------------------------------
      private void DoLocalMoveY_OnCompleted()
      {
         Destroy(gameObject);
      }
   }
}
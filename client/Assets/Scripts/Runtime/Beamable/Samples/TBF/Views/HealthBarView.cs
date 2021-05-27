using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Samples.TBF.Views
{
   /// <summary>
   /// Handles the view concerns for UI elements related
   /// to health bar for a game character
   /// </summary>
   public class HealthBarView : MonoBehaviour
   {
      //  Events ---------------------------------------
      public event Action<int, int> OnValueChanged;

      //  Properties -----------------------------------
      public int Value 
      { 
         set 
         {
            int oldValue = _value;
            _value = Mathf.Clamp(value, MinValue, MaxValue); 
            Render();
            OnValueChanged?.Invoke(oldValue, _value);
         } 
         get 
         { 
            return _value; 
         } 
      }

      public string Title { set { _title = value; Render(); } get { return _title; } }
      public Color BackgroundColor { set { _backgroundColor = value; Render(); } get { return _backgroundColor; } }


      //  Fields ---------------------------------------
      public const int MaxValue = 100;
      public const int MinValue = 0;

      [SerializeField]
      private string _title = "";

      [SerializeField]
      private Color _backgroundColor = Color.white;

      [SerializeField]
      private Image _backgroundImage = null;

      [SerializeField]
      private Slider _slider = null;

      [SerializeField]
      private TMP_Text _text = null;

      [SerializeField]
      private bool _isAlignedLeft = false;

      [SerializeField]
      private int _value = 100;

      private Tween _tween = null;


      //  Unity Methods   ------------------------------
      protected void OnValidate()
      {
         //cap values
         Value = _value;

         //Debug the rendering in edit mode
         //as the inspector values are manually changed
         Render();
      }


      //  Other Methods   ------------------------------
      private void Render()
      {
         if (_backgroundImage != null)
         {
            _backgroundImage.color = _backgroundColor;
         }

         SetFillImageWidthPercent(_value);

         if (_isAlignedLeft)
         {
            _text.text = $"{_title} {_value}%";
         }
         else
         {
            _text.text = $"{_value}% {_title}";
         }
      }


      private void SetFillImageWidthPercent(float targetPercent)
      {
         if (_tween != null)
         {
            _tween.Kill();
         }

         //DOTween works only at runtime
         if (Application.isPlaying)
         {
            _tween = DOTween.To(nextWidth =>
            {
               _slider.value = nextWidth;
            }, _slider.value, targetPercent, 0.5f);
         }
         else
         {
            _slider.value = targetPercent;
         }
      }
   }
}
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Samples.KOR.UI
{
   /// <summary>
   /// Handles the view concerns for panels with texts
   /// </summary>
   public class StorePanelUI : PanelUI
   {
      //  Properties -----------------------------------
      public VerticalLayoutGroup VerticalLayoutGroup { get { return _verticalLayoutGroup; } }
      
      //  Fields ---------------------------------------
      [SerializeField]
      private VerticalLayoutGroup _verticalLayoutGroup = null;
   }
}
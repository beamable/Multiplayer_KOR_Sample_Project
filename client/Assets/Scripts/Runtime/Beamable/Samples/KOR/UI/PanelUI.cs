﻿using TMPro;
using UnityEngine;

namespace Beamable.Samples.KOR.UI
{
   /// <summary>
   /// Handles the view concerns for panels with texts
   /// </summary>
   public class PanelUI : MonoBehaviour
   {
      //  Properties -----------------------------------
      public TMP_Text TitleText { get { return _titleText; } }
      public TMP_Text BodyText { get { return _bodyText; } }
      
      //  Fields ---------------------------------------
      [SerializeField]
      private TMP_Text _titleText = null;
      
      [SerializeField]
      private TMP_Text _bodyText = null;

      //  Unity Methods   ------------------------------
   }
}
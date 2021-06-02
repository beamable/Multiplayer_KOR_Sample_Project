﻿using System.Collections.Generic;
using Beamable.Samples.KOR.Animation;
using Beamable.Samples.KOR.Data;
using Beamable.Samples.KOR.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Samples.KOR.Views
{
   /// <summary>
   /// Handles the audio/graphics rendering logic: Game
   /// </summary>
   public class GameUIView : BaseUIView
   {
      //  Properties -----------------------------------
      public RingView RingView { get { return _ringView; } }
      public Button BackButton { get { return _backButton; } }

      public TMP_BufferedText BufferedText { get { return _bufferedText; } }
      public List<AvatarView> AvatarViews { get { return _avatarViews; } }
      public List<AvatarUIView> AvatarUIViews { get { return _avatarUIViews; } }

      //  Fields ---------------------------------------
      [SerializeField]
      private TMP_BufferedText _bufferedText = null;

      [SerializeField]
      private Button _backButton = null;

      [SerializeField]
      private RingView _ringView = null;
      
      [SerializeField]
      private List<AvatarUIView> _avatarUIViews = null;


      [Header("Populates at Runtime")]
      [SerializeField]
      private List<AvatarView> _avatarViews = new List<AvatarView>();

   }
}
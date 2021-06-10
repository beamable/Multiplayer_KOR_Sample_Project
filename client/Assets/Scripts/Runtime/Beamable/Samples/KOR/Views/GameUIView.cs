using System.Collections.Generic;
using System.Linq;
using Beamable.Samples.KOR.UI;
using Cinemachine;
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
      public AttributesPanelUI AttributesPanelUI { get { return _attributesPanelUI; } }
      public RingView RingView { get { return _ringView; } }
      public Button BackButton { get { return _backButton; } }

      public TMP_BufferedText BufferedText { get { return _bufferedText; } }
      public List<AvatarView> AvatarViews { get { return _avatarViews; } }
      public List<AvatarUIView> AvatarUIViews { get { return _avatarUIViews; } }
      public CinemachineImpulseSource CinemachineImpulseSource { get { return _cinemachineImpulseSource; } }

      //  Fields ---------------------------------------
      [SerializeField]
      private CinemachineImpulseSource _cinemachineImpulseSource = null;

      [SerializeField]
      private AttributesPanelUI _attributesPanelUI = null;

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

      public AvatarView GetAvatarViewForDbid(long dbid)
      {
         return AvatarViews.FirstOrDefault(view => view.playerDbid == dbid);
      }
   }
}
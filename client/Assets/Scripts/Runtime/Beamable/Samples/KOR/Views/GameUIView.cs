using System.Collections.Generic;
using System.Linq;
using Beamable.Samples.KOR.Behaviours;
using Beamable.Samples.KOR.UI;
using TMPro;
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
      public AttributesPanelUI AttributesPanelUI => _attributesPanelUI;
      public RingView RingView => _ringView;
      public Button BackButton => _backButton;

      public TMP_BufferedText BufferedText => _bufferedText;
      public List<AvatarView> AvatarViews => _avatarViews;
      public List<AvatarUIView> AvatarUIViews => _avatarUIViews;
      public GameTimerBehaviour GameTimerBehaviour => _timerBehaviour;
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

      [SerializeField]
      private TextMeshProUGUI _timeRemainingValue;

      [SerializeField]
      private GameTimerBehaviour _timerBehaviour;

      [Header("Populates at Runtime")]
      [SerializeField]
      private List<AvatarView> _avatarViews = new List<AvatarView>();

      public AvatarView GetAvatarViewForDbid(long dbid)
      {
         return AvatarViews.FirstOrDefault(view => view.playerDbid == dbid);
      }


      protected override void Update()
      {
         base.Update();
         _timeRemainingValue.text = $"<mspace=mspace=500>{_timerBehaviour.SecondsRemaining.ToString("00")}</mspace>";
      }

   }
}
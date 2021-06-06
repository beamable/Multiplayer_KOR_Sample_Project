using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Samples.KOR.Views
{
    /// <summary>
    /// Handles the audio/graphics rendering logic: Intro UI
    /// </summary>
    public class IntroUIView : BaseUIView
    {
        //  Properties -----------------------------------
        public string AboutBodyText { set { _aboutBodyText.text = value; } }

        public Texture2D CharacterImage { set { _characterImage.texture = value; } }

        public string CharacterInfoText { set { _characterInfoText.text = value; } }

        public Button PreviousCharacterButton { get { return previousCharacterButton; } }
        public Button NextCharacterButton { get { return nextCharacterButton; } }
        public Button StartGame01Button { get { return startGame01Button; } }
        public Button StartGame02Button { get { return startGame02Button; } }
        public Button LeaderboardButton { get { return _leaderboardButton; } }
        public Button StoreButton { get { return _storeButton; } }
        public Button QuitButton { get { return _quitButton; } }
        public CanvasGroup ButtonsCanvasGroup { get { return _buttonsCanvasGroup; } }

        //  Fields ---------------------------------------
        [Header("UI")]
        [SerializeField]
        private TMP_Text _characterInfoText = null;

        [SerializeField]
        private Button previousCharacterButton = null;

        [SerializeField]
        private Button nextCharacterButton = null;

        [SerializeField]
        private Button startGame01Button = null;

        [SerializeField]
        private Button startGame02Button = null;

        [SerializeField]
        private Button _leaderboardButton = null;

        [SerializeField]
        private Button _storeButton = null;

        [SerializeField]
        private Button _quitButton = null;

        [SerializeField]
        private TMP_Text _aboutBodyText = null;

        [SerializeField]
        private CanvasGroup _buttonsCanvasGroup = null;

        [SerializeField]
        private RawImage _characterImage = null;

        //  Unity Methods   ------------------------------
        public void Awake()
        {
            _characterInfoText.text = "";
        }

        public void Start()
        {
            CanvasGroupsDoFadeOut();
        }

        //  Other Methods   ------------------------------
        public void CanvasGroupsDoFade()
        {
            CanvasGroupsDoFadeIn();
        }
    }
}
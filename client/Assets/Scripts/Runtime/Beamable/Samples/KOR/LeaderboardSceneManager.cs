using Beamable.Samples.KOR.Data;
using Beamable.Samples.KOR.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Samples.KOR
{
    /// <summary>
    /// Handles the main scene logic: Leaderboard
    /// </summary>
    public class LeaderboardSceneManager : MonoBehaviour
    {
        //  Fields ---------------------------------------
        [SerializeField]
        private Configuration _configuration = null;

        [SerializeField]
        private LeaderboardUIView _leaderboardUIView = null;

        [SerializeField]
        private Button _closeButton = null;


        //  Unity Methods   ------------------------------
        protected void Start()
        {
            _closeButton.onClick.AddListener(CloseButton_OnClicked);
            
            _leaderboardUIView.CanvasGroupsDoFadeIn();
        }

        //  Event Handlers -------------------------------
        private void CloseButton_OnClicked()
        {
            KORHelper.PlayAudioForUIClick();
            
            StartCoroutine(KORHelper.LoadScene_Coroutine(_configuration.IntroSceneName,
                _configuration.DelayBeforeLoadScene));
        }
    }
}
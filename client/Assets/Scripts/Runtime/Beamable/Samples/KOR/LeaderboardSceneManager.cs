using Beamable.Samples.KOR.UI;
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
        private LeaderboardUIView _leaderboardUIView = null;

        [SerializeField]
        private KORLeaderboardMainMenu _korLeaderboardMainMenu = null;
        
        [SerializeField]
        private Button _closeButton = null;


        //  Unity Methods   ------------------------------
        protected void Start()
        {
            _closeButton.onClick.AddListener(CloseButton_OnClicked);

            _korLeaderboardMainMenu.OnRendered.AddListener(KORLeaderboardMainMenu_OnRendered);
            
            // For KOR, use a custom UI for the leaderboard rows
            _korLeaderboardMainMenu.KORLeaderboardItem = _leaderboardUIView.KORLeaderboardItem;
            
            Debug.Log("Calling refresh");
            _korLeaderboardMainMenu.Render();
            
        }

        //  Event Handlers -------------------------------
        private void CloseButton_OnClicked()
        {
            KORHelper.PlayAudioForUIClick();
            
            StartCoroutine(KORHelper.LoadScene_Coroutine(_leaderboardUIView.Configuration.IntroSceneName,
                _leaderboardUIView.Configuration.DelayBeforeLoadScene));
        }
        
        protected void KORLeaderboardMainMenu_OnRendered()
        {
            _leaderboardUIView.CanvasGroupsDoFadeIn();
        }
    }
}
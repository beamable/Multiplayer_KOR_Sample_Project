using Beamable.Samples.KOR.Views;
using UnityEngine;

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

        //  Unity Methods   ------------------------------
        protected void Start()
        {
            _leaderboardUIView.BackButton.onClick.AddListener(BackButton_OnClicked);

            _leaderboardUIView.KORLeaderboardMainMenu.OnRendered.AddListener(KORLeaderboardMainMenu_OnRendered);
            
            // For KOR, use a custom UI for the leaderboard rows
            _leaderboardUIView.KORLeaderboardMainMenu.KORLeaderboardItem = _leaderboardUIView.KORLeaderboardItem;
            
            _leaderboardUIView.KORLeaderboardMainMenu.Render();
            
        }

        //  Event Handlers -------------------------------
        private void BackButton_OnClicked()
        {
            KORHelper.PlayAudioForUIClickPrimary();
            
            StartCoroutine(KORHelper.LoadScene_Coroutine(_leaderboardUIView.Configuration.IntroSceneName,
                _leaderboardUIView.Configuration.DelayBeforeLoadScene));
        }
        
        protected void KORLeaderboardMainMenu_OnRendered()
        {
            _leaderboardUIView.CanvasGroupsDoFadeIn();
        }
    }
}
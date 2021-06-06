using Beamable.Common.Api.Leaderboards;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Samples.KOR.UI
{
    /// <summary>
    /// Renders one item row of the leaderboard.
    ///
    /// NOTE: Its also used for the header atop the leaderboard
    /// 
    /// </summary>
    public class KORLeaderboardItem : MonoBehaviour
    {
        //  Fields ---------------------------------------
        [SerializeField]
        private TextMeshProUGUI _aliasText;
        
        [SerializeField]
        private TextMeshProUGUI _rankText;
        
        [SerializeField]
        private TextMeshProUGUI _scoreText;
        
        [SerializeField]
        private Image _iconImage;
        
        [SerializeField]
        private bool _isHeader = false;
        
        
        //  Unity Methods ---------------------------------
        public void Start()
        { 
            _iconImage.enabled = !_isHeader;
        }
        
        //  Other Methods ---------------------------------
        public async void Apply(RankEntry entry)
        {
            var beamableAPI = await API.Instance;
            
            // TODO: load icon. via stat?
            string path = $"Sprites/temp_image_0{UnityEngine.Random.Range(1,3)}";
            _iconImage.sprite = Resources.Load<Sprite>(path);
            _iconImage.color = new Color(0, 0, 0, 0);//temporarily set alpha to 0

            //
            var stats = await beamableAPI.StatsService.GetStats("client", "public", "player", entry.gt);
            string alias;
            stats.TryGetValue("alias", out alias);
            _aliasText.text = alias;
            _rankText.text = entry.rank.ToString("000");
            _scoreText.text = entry.score.ToString("000");
         
        }
    }
}
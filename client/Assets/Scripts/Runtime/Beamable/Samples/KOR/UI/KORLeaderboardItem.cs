using System.Threading.Tasks;
using Beamable.Common.Api;
using Beamable.Common.Api.Leaderboards;
using Beamable.Samples.KOR.Animation;
using Beamable.Samples.KOR.CustomContent;
using Beamable.Samples.KOR.Data;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
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

            // Load text
            var stats = await beamableAPI.StatsService.GetStats("client", "public", "player", entry.gt);
            string alias;
            stats.TryGetValue("alias", out alias);
            _aliasText.text = alias;
            _rankText.text = entry.rank.ToString("000");
            _scoreText.text = entry.score.ToString("000");
            
            // Load icon
            await LoadIconForDbid(entry.gt);
         
        }

        
        private async Task<EmptyResponse> LoadIconForDbid(long dbid)
        {
            TweenHelper.ImageDoFade(_iconImage, 0, 0, 0, 0); //temporarily hide icon
            CharacterContentObject chosenCharacterByDbid = await
                RuntimeDataStorage.Instance.CharacterManager.GetChosenCharacterByDBID(dbid);

            if (chosenCharacterByDbid == null)
            {
               Configuration.Debugger.Log($"No CharacterContentObject for {dbid}.");
            }
            else
            {
                AsyncOperationHandle<Texture2D> asyncOperationHandle = Addressables.LoadAssetAsync<Texture2D>(
                    chosenCharacterByDbid.bigIcon);

                asyncOperationHandle.Completed += AsyncOperationHandle_OnCompleted;
            }
      
            return new EmptyResponse();
        }


        //  Event Handlers  ---------------------------------
        public void AsyncOperationHandle_OnCompleted (AsyncOperationHandle<Texture2D> asyncOperationHandle)
        {
            Texture2D texture2D = asyncOperationHandle.Result;
            _iconImage.sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height),
                new Vector2(0.5f, 0.5f));

            TweenHelper.ImageDoFade(_iconImage, 0, 1, 0.25f, 0);
        }
    }
}
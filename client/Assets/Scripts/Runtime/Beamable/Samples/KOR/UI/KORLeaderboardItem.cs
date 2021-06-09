using System.Collections.Generic;
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
            await API.Instance;

            // Load text
            string alias = await RuntimeDataStorage.Instance.CharacterManager.GetPlayerAliasByDBID(entry.gt);
            _aliasText.text = alias;
            _rankText.text = entry.rank.ToString("000");
            _scoreText.text = entry.score.ToString("000");

            // Load icon
            await LoadIconForDbid(entry.gt);
        }

        private async Task<EmptyResponse> LoadIconForDbid(long dbid)
        {
            TweenHelper.ImageDoFade(_iconImage, 0, 0, 0, 0); //temporarily hide icon
            CharacterContentObject chosenCharacterByDbid = (await
                RuntimeDataStorage.Instance.CharacterManager.GetChosenCharacterByDBID(dbid)).CharacterContentObject;

            if (chosenCharacterByDbid == null)
            {
                Configuration.Debugger.Log($"No CharacterContentObject for {dbid}.");
            }
            else
            {
                //Hide image, load texture, show image
                KORHelper.AddressablesLoadAssetAsync<Texture2D>(chosenCharacterByDbid.bigIcon, _iconImage);
            }

            return new EmptyResponse();
        }

        //  Event Handlers  ---------------------------------
    }
}
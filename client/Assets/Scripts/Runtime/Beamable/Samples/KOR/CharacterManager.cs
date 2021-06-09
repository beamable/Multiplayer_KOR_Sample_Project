using Beamable.Common;
using Beamable.Common.Api;
using Beamable.Common.Api.Inventory;
using Beamable.Common.Content;
using Beamable.Core.Debugging;
using Beamable.Samples.KOR.CustomContent;
using Beamable.Samples.KOR.Data;
using Beamable.Samples.KOR.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Beamable.Samples.KOR
{
    public class CharacterManager
    {
        public class Character
        {
            public Character(CharacterContentObject characterContentObject, AvatarView avatarView)
            {
                CharacterContentObject = characterContentObject;
                AvatarViewPrefab = avatarView;
            }

            public CharacterContentObject CharacterContentObject { get; }
            public AvatarView AvatarViewPrefab { get; }
        }

        public const string ChosenCharacterStatKey = "ChosenCharacterContentID";
        public const string PlayerAliasStatKey = "alias";
        public const string DefaultPlayerAliasPrefix = "Player";

        private List<Character> _allCharacters = null;

        private IBeamableAPI _beamableAPI = null;

        private Dictionary<string, Character> _mapCharacterObjectNameToCharacter = new Dictionary<string, Character>();
        private Character _currentlyChosenCharacter = null;

        public List<Character> AllCharacters { get { return _allCharacters; } }

        public Character CurrentlyChosenCharacter { get { return _currentlyChosenCharacter; } }

        public event Action OnChoiceHasBeenMade;

        public CharacterManager()
        {
            SetupBeamable();
        }

        private async void SetupBeamable()
        {
            _beamableAPI = await Beamable.API.Instance;

            List<CharacterContentObject> allCCOs = await GetAllCharacterContentObjects();

            if (allCCOs.Count == 0)
            {
                Debug.LogError("No characters found in manifest. Did you forget to add some? This will break.");
                return;
            }

            _allCharacters = new List<Character>();

            foreach (var cco in allCCOs)
            {
                Configuration.Debugger.Log($"CharacterContentObject name={cco.ContentName} " +
                                           $"type={cco.ContentType} id={cco.Id} readable name={cco.ReadableName}",
                    DebugLogLevel.Verbose);

                GameObject viewPrefab = await Addressables.LoadAssetAsync<GameObject>(cco.avatarViewPrefab).Task;
                AvatarView view = viewPrefab.GetComponent<AvatarView>();

                Character newCharacter = new Character(cco, view);

                _allCharacters.Add(newCharacter);
                _mapCharacterObjectNameToCharacter.Add(cco.ContentName, newCharacter);
            }

            Character chosenCharacter = await GetChosenCharacterByDBID(_beamableAPI.User.id);

            if (chosenCharacter == null)
                await ChooseCharacter(_allCharacters[0]);
            else
            {
                _currentlyChosenCharacter = _allCharacters.Where(c => c.CharacterContentObject == chosenCharacter.CharacterContentObject).FirstOrDefault();
                OnChoiceHasBeenMade?.Invoke();
            }
        }

        public async Task<Character> GetChosenCharacterByDBID(long dbid)
        {
            string chosenCharacterName = await GetStatKeyByDBID(ChosenCharacterStatKey, dbid);
            if (chosenCharacterName == null)
                return null;

            Character character;
            if (!_mapCharacterObjectNameToCharacter.TryGetValue(chosenCharacterName, out character))
            {
                Debug.LogError($"Chosen character name={chosenCharacterName} by dbid={dbid} from stats does not refer to an existing character content object. " +
                    "You've probably removed/renamed this recently.");
                return null;
            }

            return character;
        }

        public async Task<string> GetPlayerAliasByDBID(long dbid)
        {
            return await GetStatKeyByDBID(PlayerAliasStatKey, dbid);
        }

        private async Task<string> GetStatKeyByDBID(string statKey, long dbid)
        {
            Dictionary<string, string> allCurrentUserStats = await _beamableAPI.StatsService.GetStats("client", "public", "player", dbid);

            Configuration.Debugger.Log($"Stats count={allCurrentUserStats.Count} for dbid={dbid}", DebugLogLevel.Verbose);
            foreach (KeyValuePair<string, string> entry in allCurrentUserStats)
                Configuration.Debugger.Log($"Stat dbid={dbid} key={entry.Key} value={entry.Value}", DebugLogLevel.Verbose);

            string value;
            if (!allCurrentUserStats.TryGetValue(statKey, out value))
            {
                Configuration.Debugger.Log($"dbid={dbid} has no key for stat={statKey}!", DebugLogLevel.Verbose);
                return null;
            }

            Configuration.Debugger.Log($"dbid={dbid} has value={value} for stat key={statKey}", DebugLogLevel.Verbose);

            return value;
        }

        public int GetChosenCharacterIndex()
        {
            if (_allCharacters == null || _currentlyChosenCharacter == null)
                return -1;

            return _allCharacters.IndexOf(_allCharacters.Where(c => c.CharacterContentObject == _currentlyChosenCharacter.CharacterContentObject).FirstOrDefault());
        }

        public async Task<EmptyResponse> ChooseCharacter(Character newlyChosenCharacter)
        {
            _currentlyChosenCharacter = newlyChosenCharacter;
            await SetStatsKeyForCurrentUser(ChosenCharacterStatKey, newlyChosenCharacter.CharacterContentObject.ContentName);
            OnChoiceHasBeenMade?.Invoke();

            return null;
        }

        public Promise<EmptyResponse> SetCurrentPlayerAlias(string newPlayerAlias)
        {
            return SetStatsKeyForCurrentUser(PlayerAliasStatKey, newPlayerAlias);
        }

        private Promise<EmptyResponse> SetStatsKeyForCurrentUser(string statsKey, string newValue)
        {
            return _beamableAPI.StatsService.SetStats("public", new Dictionary<string, string>() { { statsKey, newValue } });
        }

        private async Task<List<CharacterContentObject>> GetAllCharacterContentObjects()
        {
            var filteredManifest = await _beamableAPI.ContentService.GetManifest(new ContentQuery
            {
                TypeConstraints = new HashSet<Type> { typeof(CharacterContentObject) }
            });
            var results = await filteredManifest.ResolveAll();
            return results.Cast<CharacterContentObject>().ToList();
        }

        /// <summary>
        /// Get sum total of pay-to-play attributes to impact gameplay
        /// </summary>
        /// <param name="dbid"></param>
        /// <returns></returns>
        public async Task<Attributes> GetChosenPlayerAttributes()
        {
            CharacterContentObject characterContentObject =
                (await GetChosenCharacterByDBID(_beamableAPI.User.id)).CharacterContentObject;

            // Very early in play session, this may not be ready
            if (characterContentObject == null)
            {
                return new Attributes(0, 0);
            }
            else
            {
                var chargeSpeed = characterContentObject.ChargeSpeed;
                var movementSpeed = characterContentObject.MovementSpeed;

                // #1 GET ALL ITEMS
                InventoryView inventoryView =
                    await _beamableAPI.InventoryService.GetCurrent(KORConstants.ItemContentType);

                foreach (KeyValuePair<string, List<ItemView>> kvp in inventoryView.items)
                {
                    int itemCount = kvp.Value.Count;
                    KORItemContent y = await _beamableAPI.ContentService.GetContent(kvp.Key, typeof(KORItemContent)) as KORItemContent;

                    //Reward user for each TYPE and COUNT of Inventory
                    chargeSpeed += (y.ChargeSpeed * itemCount);
                    movementSpeed += (y.MovementSpeed * itemCount);
                }

                // #2 RETURN FINAL VALUES
                return new Attributes(chargeSpeed, movementSpeed);
            }
        }
    }
}
using Beamable.Common;
using Beamable.Common.Api;
using Beamable.Common.Api.Inventory;
using Beamable.Common.Content;
using Beamable.Samples.KOR.CustomContent;
using Beamable.Samples.KOR.Data;
using Beamable.Samples.KOR.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beamable.Samples.Core.Debugging;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Beamable.Samples.KOR
{
    public class CharacterManager
    {
        public const string ChosenCharacterStatKey = "ChosenCharacterContentID";
        public const string PlayerAliasStatKey = "alias";
        public const string DefaultPlayerAliasPrefix = "Plyr";

        private bool _isInitialized = false;
        private List<Character> _allCharacters = null;

        private IBeamableAPI _beamableAPI = null;

        private Dictionary<string, Character> _mapCharacterObjectNameToCharacter = new Dictionary<string, Character>();

        private Character _currentlyChosenCharacter = null;

        public event Action OnChoiceHasBeenMade;

        public List<Character> AllCharacters { get { return _allCharacters; } }

        public Character CurrentlyChosenCharacter { get { return _currentlyChosenCharacter; } }

        public class Character
        {
            public CharacterContentObject CharacterContentObject { get; }

            public AvatarView AvatarViewPrefab { get; }

            public Character(CharacterContentObject characterContentObject, AvatarView avatarView)
            {
                CharacterContentObject = characterContentObject;
                AvatarViewPrefab = avatarView;
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

        /// <summary>
        /// Get sum total of pay-to-play attributes to impact gameplay
        /// </summary>
        /// <param name="dbid"></param>
        /// <returns></returns>
        public async Task<Attributes> GetChosenPlayerAttributes()
        {
            var character = await GetChosenCharacterByDBID(_beamableAPI.User.id);

            if (character == null)
            {
                Configuration.Debugger.Log($"No character for {_beamableAPI.User.id}.");
                return new Attributes(0, 0);
            }

            CharacterContentObject characterContentObject = character.CharacterContentObject;

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
                    KORItemContent korItemContent = await KORHelper.GetKORItemContentById(_beamableAPI, kvp.Key);

                    //Reward user for each TYPE and COUNT of Inventory
                    chargeSpeed += (korItemContent.ChargeSpeed * itemCount);
                    movementSpeed += (korItemContent.MovementSpeed * itemCount);
                }

                // #2 RETURN FINAL VALUES
                return new Attributes(chargeSpeed, movementSpeed);
            }
        }

        public async Task Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            try
            {
                // Immediately set true to prevent race conditions
                // from concurrent calls to this method
                _isInitialized = true;
                
                
                _beamableAPI = await Beamable.API.Instance;

                List<CharacterContentObject> allCCOs = await GetAllCharacterContentObjects();

                if (allCCOs.Count == 0)
                {
                    throw new Exception("No characters found in manifest. Did you forget to add some? This will break.");
                }
            
                _allCharacters = new List<Character>();
                _mapCharacterObjectNameToCharacter.Clear();
                foreach (var cco in allCCOs)
                {
                    GameObject viewPrefab = await Addressables.LoadAssetAsync<GameObject>(cco.avatarViewPrefab).Task;
                
                    if (viewPrefab == null)
                    {
                        throw new Exception($"Can't find addressable for {cco.avatarViewPrefab}. " +
                                            $"Build Unity Addressables before running project. " +
                                            $"See https://docs.unity3d.com/Packages/com.unity.addressables@1.3/manual/AddressableAssetsDevelopmentCycle.html");
                    }
                
                    AvatarView view = viewPrefab.GetComponent<AvatarView>();

                    Character newCharacter = new Character(cco, view);
                
                    _allCharacters.Add(newCharacter);
                    _mapCharacterObjectNameToCharacter.Add(cco.ContentName, newCharacter);
                }

                Character chosenCharacter = await GetChosenCharacterByDBID(_beamableAPI.User.id);
            
                if (chosenCharacter == null)
                {
                    await ChooseCharacter(_allCharacters[0]);
                }
                else
                {
                    _currentlyChosenCharacter = _allCharacters.Where(c => c.CharacterContentObject == chosenCharacter.CharacterContentObject).FirstOrDefault();
                    OnChoiceHasBeenMade?.Invoke();
                }

            }
            catch (Exception e)
            {
                _isInitialized = false;
                Debug.LogError(e);
            }
            
        }

        private async Task<string> GetStatKeyByDBID(string statKey, long dbid)
        {
            await Initialize();
            
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
    }
}
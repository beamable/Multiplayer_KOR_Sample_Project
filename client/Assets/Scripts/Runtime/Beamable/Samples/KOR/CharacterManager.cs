using Beamable.Api.Stats;
using Beamable.Common;
using Beamable.Common.Api;
using Beamable.Common.Content;
using Beamable.Samples.Core;
using Beamable.Samples.KOR.CustomContent;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beamable.Common.Api.Inventory;
using Beamable.Core.Debugging;
using Beamable.Samples.KOR.Data;
using UnityEngine;

namespace Beamable.Samples.KOR
{
    public class CharacterManager
    {
        private IBeamableAPI _beamableAPI = null;
        private List<CharacterContentObject> _allCharacterContentObjects = null;
        private Dictionary<string, CharacterContentObject> _mapCharacterObjectNameToContent = new Dictionary<string, CharacterContentObject>();
        public const string ChosenCharacterStatKey = "ChosenCharacterContentID";
        public const string PlayerAliasStatKey = "alias";
        public const string DefaultPlayerAliasPrefix = "Player";
        private CharacterContentObject _currentlyChosenCharacter = null;

        public List<CharacterContentObject> AllCharacterContentObjects { get { return _allCharacterContentObjects; } }

        public CharacterContentObject CurrentlyChosenCharacter { get { return _currentlyChosenCharacter; } }

        public event Action OnChoiceHasBeenMade;

        public CharacterManager()
        {
            SetupBeamable();
        }

        private async void SetupBeamable()
        {
            _beamableAPI = await Beamable.API.Instance;

            _allCharacterContentObjects = await GetAllCharacterContentObjects();

            if (_allCharacterContentObjects.Count == 0)
            {
                Debug.LogError("No characters found in manifest. Did you forget to add some? This will break.");
                return;
            }

            foreach (var cco in _allCharacterContentObjects)
            {
                Configuration.Debugger.Log($"CharacterContentObject name={cco.ContentName} " +
                                           $"type={cco.ContentType} id={cco.Id} something={cco.ReadableName}",
                    DebugLogLevel.Verbose);

                _mapCharacterObjectNameToContent.Add(cco.ContentName, cco);
            }

            CharacterContentObject chosenCCO = await GetChosenCharacterByDBID(_beamableAPI.User.id);

            if (chosenCCO == null)
                await ChooseCharacter(_allCharacterContentObjects[0]);
            else
            {
                _currentlyChosenCharacter = chosenCCO;
                OnChoiceHasBeenMade?.Invoke();
            }
        }

        public async Task<CharacterContentObject> GetChosenCharacterByDBID(long dbid)
        {
            Dictionary<string, string> allCurrentUserStats = await _beamableAPI.StatsService.GetStats("client", "public", "player", dbid);

            string chosenCharacterName = await GetStatKeyByDBID(ChosenCharacterStatKey, dbid);
            if (chosenCharacterName == null)
                return null;

            CharacterContentObject cco;
            if (!_mapCharacterObjectNameToContent.TryGetValue(chosenCharacterName, out cco))
            {
                Debug.LogError($"Chosen character name={ chosenCharacterName} by dbid={dbid} from stats does not refer to an existing character content object. " +
                    "You've probably removed/renamed this recently.");
                return null;
            }

            return cco;
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
            if (_allCharacterContentObjects == null || _currentlyChosenCharacter == null)
                return -1;

            return _allCharacterContentObjects.IndexOf(_currentlyChosenCharacter);
        }

        public async Task<EmptyResponse> ChooseCharacter(CharacterContentObject newlyChosenCharacter)
        {
            _currentlyChosenCharacter = newlyChosenCharacter;
            SetStatsKeyForCurrentUser(ChosenCharacterStatKey, newlyChosenCharacter.ContentName);
            OnChoiceHasBeenMade?.Invoke();
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
                await GetChosenCharacterByDBID(_beamableAPI.User.id);

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
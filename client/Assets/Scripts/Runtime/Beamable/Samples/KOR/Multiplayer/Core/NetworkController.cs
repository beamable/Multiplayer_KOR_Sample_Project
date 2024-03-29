using System;
using System.Globalization;
using System.Threading.Tasks;
using Beamable.Experimental.Api.Sim;
using Beamable.Samples.Core;
using Beamable.Samples.Core.Components;
using Beamable.Samples.KOR;
using Beamable.Samples.KOR.Data;
using Beamable.Samples.KOR.Multiplayer.Events;
using Beamable.Service;
using UnityEngine;

namespace Beamable.Examples.Features.Multiplayer.Core
{
    public class NetworkController : SingletonMonobehavior<NetworkController>
    {
        public const int NetworkFramesPerSecond = 20; // TODO: Un-hardcode this at the server level
        public static long HighestSeenNetworkFrame;
        public static bool NetworkInitialized;
        private SimClient _sim;

        public SimulationLog Log = new SimulationLog();
        public long LocalDbid;

        public System.Random rand;
        private string _roomId;

        public string RandomSeed { get; private set; }

        public async Task Init()
        {
            HighestSeenNetworkFrame = 0;
            NetworkInitialized = false;
            // roomId = string.IsNullOrEmpty(roomIdOverride) ? roomId : roomIdOverride;

            var beam = BeamContext.Default;
            await beam.OnReady;

            _roomId = RuntimeDataStorage.Instance.MatchId;
            LocalDbid = beam.AuthorizedUser.Value.id;
            _sim = new SimClient(new FastNetworkEventStream(_roomId, LocalDbid.ToString(CultureInfo.InvariantCulture), beam.ServiceProvider),
                                 NetworkFramesPerSecond, 1);
            _sim.OnInit(HandleOnInit);
            _sim.OnConnect(HandleOnConnect);
            _sim.OnDisconnect(HandleOnDisconnect);
            _sim.OnTick(HandleOnTick);
        }

        public void Cleanup()
        {
            Log = new SimulationLog();
            LocalDbid = 0;
            _sim = null;
        }

        private void HandleOnInit(string seed)
        {
            RandomSeed = seed;
            rand = new System.Random(seed.GetHashCode());
            NetworkInitialized = true;
        }

        private void HandleOnTick(long tick)
        {
            HighestSeenNetworkFrame = tick;
            Log.RecordEvent(tick, new TickEvent(tick));
        }

        private void HandleOnConnect(string dbid)
        {
            Configuration.Debugger.Log("Sim client has connection from " + dbid);

            // listen for messages from this player...
            var dbidNumber = long.Parse(dbid);

            ListenForEventFrom<PlayerMoveStartedEvent>(dbid);
            ListenForEventFrom<PlayerMoveEndEvent>(dbid);
            ListenForEventFrom<PlayerMoveProgressEvent>(dbid);
            ListenForEventFrom<ReadyEvent>(dbid);
            _sim.On<ChecksumEvent>(nameof(ChecksumEvent), dbid, hashCheck =>
            {
                hashCheck.SetPlayerDbid(dbidNumber);
                if (dbidNumber == LocalDbid) return;
                Configuration.Debugger.Log("Validating hash from " + dbid + " for tick " + hashCheck.ForTick);
                Log.EnqueueHashAssertion(hashCheck.ForTick, hashCheck.Hash);
            });

            var joinMsg = new PlayerJoinedEvent();
            joinMsg.SetPlayerDbid(dbidNumber);

            Log.RecordEvent(joinMsg);
        }

        private void HandleOnDisconnect(string dbid)
        {
            Debug.Log("Sim client has disconnection from " + dbid);
        }

        private void FixedUpdate()
        {
            _sim?.Update();
        }

        public void SendNetworkMessage(KOREvent message)
        {
            _sim?.SendEvent(message.GetType().Name, message);
        }

        public async Task<GameResults> ReportResults(PlayerResult[] results)
        {
            var beamable = await Beamable.API.Instance;
            return await beamable.Experimental.GameRelayService.ReportResults(_roomId, results);
        }

        private SimClient.EventCallback<string> ListenForEventFrom<T>(string origin)
            where T : KOREvent
        {
            var dbid = long.Parse(origin);
            return _sim.On<T>(typeof(T).Name, origin, evt =>
            {
                evt.SetPlayerDbid(dbid);
                Log.RecordEvent(evt);
            });
        }
    }
}
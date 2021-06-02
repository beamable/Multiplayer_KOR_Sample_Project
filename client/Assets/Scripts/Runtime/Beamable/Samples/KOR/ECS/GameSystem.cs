using System.Collections.Generic;
using System.Linq;
using Beamable.Examples.Features.Multiplayer.Core;
using Beamable.Samples.KOR.Data;
using Beamable.Samples.KOR.Multiplayer.Events;
using Unity.Entities;
using UnityEngine;

namespace Beamable.Samples.KOR.Multiplayer
{
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
   [DisableAutoCreation]
   public class GameSystem : SystemBase
   {
      // TODO: Factor this state out of the system
      // private Dictionary<long, PlayerBehaviour> _dbidToPlayer = new Dictionary<long,  PlayerBehaviour>();

      protected override void OnCreate()
      {
         base.OnCreate();
         // var network = NetworkController.Instance; // TODO: Factor this initialization into a centralized initialiation
         // var _ = network.Init();
      }

      protected override void OnUpdate()
      {
         // get the tick this represents...
         var network = NetworkController.Instance;
         var time = World.Time.ElapsedTime;
         var frame = (long) (time * NetworkController.NetworkFramesPerSecond);

         var messages = network.Log.GetMessagesForTick(frame).ToList();

         network.Log.NotifyConsumers((float)time, (float)World.Time.DeltaTime);

         foreach (var message in messages)
         {
            switch (message)
            {
               // case PlayerInputMessage inputMessage:
               //    var effectIn = (sfloat)time + sfloat.FromRaw(inputMessage.ForcedLagTime);
               //    var startTime = sfloat.FromRaw(inputMessage.StartWorldTime);
               //    var inputLag = (sfloat)time - startTime;
               //    Debug.Log("Got input message with lag of " + inputLag );
               //    inputMessage.Consume();
               //    break;
               case PlayerJoinedEvent join:
                  HandlePlayerJoin(join);
                  join.Consume();
                  break;
               // case PlayerSpawnCubeMessage cube:
               //    var shootPlr = _dbidToPlayer[message.PlayerDbid];
               //    shootPlr.ShootCube();
               //    cube.Consume();
               //    break;
               // case PlayerDestroyAllMessage destroy:
               //    var destroyPlr = _dbidToPlayer[message.PlayerDbid];
               //    destroyPlr.DestroyAllCubes();
               //    destroy.Consume();
               //    break;
               default:
                  break;
            }
         }


      }

      void HandlePlayerJoin(PlayerJoinedEvent joinedMessage)
      {
         // if (_dbidToPlayer.ContainsKey(joinedMessage.PlayerDbid))
         // {
         //    return;
         // }

         Debug.Log("A player has joined!");

         // UnityS.Physics.Material material = UnityS.Physics.Material.Default;
         // material.Friction = (sfloat)0.05f;
         //
         // PhysicsParams physicsParams = PhysicsParams.Default;
         // physicsParams.isDynamic = true;
         // physicsParams.startingLinearVelocity = float3.zero;
         // physicsParams.mass = (sfloat).25f;
         //
         //
         // var playerData = GameController.Instance.CreateBoxColliderObject(ResourceManager.Instance.PlayerPrefab, new float3((sfloat)0, (sfloat)10, (sfloat)0),
         //    new float3((sfloat)1f, (sfloat)1f, (sfloat)1f), quaternion.identity, material, physicsParams);

         // TODO: Bind to a player model, and maybe play a cool "join" animatino?

         // var player = GameObject.Instantiate(GameResourceManager.Instance.PlayerPrefab);
         // player.Setup(joinedMessage.PlayerDbid);
         // _dbidToPlayer.Add(joinedMessage.PlayerDbid, player);
      }
   }
}
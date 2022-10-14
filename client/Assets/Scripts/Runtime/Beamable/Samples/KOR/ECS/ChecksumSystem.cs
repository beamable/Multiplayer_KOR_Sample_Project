using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Beamable.Examples.Features.Multiplayer.Core;
using Beamable.Samples.KOR.Multiplayer.Events;
using Unity.Entities;
using UnityS.Transforms;

namespace Beamable.Samples.KOR.Multiplayer
{
   [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
   [DisableAutoCreation]
   public class ChecksumSystem : SystemBase
   {
      private NetworkController _networkController;
      private const int FramesPerSecond = NetworkController.NetworkFramesPerSecond;
      readonly StringBuilder _sBuilder = new StringBuilder(512);
      protected override void OnCreate()
      {
         _networkController = NetworkController.Instance;
         base.OnCreate();
      }
      protected override void OnUpdate()
      {
         if (!NetworkController.NetworkInitialized) return;

         var time = World.Time.ElapsedTime;
         var tick = (long) (time * FramesPerSecond);

         if (_networkController.Log.HasHashForTick(tick)) return; // don't do anything...

         var list = new List<uint>();
         var entities = EntityManager.GetAllEntities();
         for (var i = 0; i < entities.Length; i++) // TODO: There is (probably) a better way to iterate over these entities using ECS
         {
            var entity = entities[i];
            if (!EntityManager.HasComponent<Translation>(entity)) continue;
            var translation = EntityManager.GetComponentData<Translation>(entity);
            list.Add((translation.Value.x).RawValue);
            list.Add((translation.Value.y).RawValue);
            list.Add((translation.Value.z).RawValue);
         }

         var hash = GetMD5Checksum(list);
         _networkController.Log.ReportHashForTick(tick, hash);
         if (tick % 40 == 0) // every 2 seconds ish...
         {
            _networkController.SendNetworkMessage(new ChecksumEvent(hash, tick));
         }
      }

      private string GetMD5Checksum(IList<uint> list)
      {
         using var md5 = MD5.Create();

         var buffer = new byte[list.Count * 4];
         for (var i = 0; i < list.Count; i++)
         {
            var bytes = BitConverter.GetBytes(list[i]);
            buffer[(i * 4) + 0] = bytes[0];
            buffer[(i * 4) + 1] = bytes[1];
            buffer[(i * 4) + 2] = bytes[2];
            buffer[(i * 4) + 3] = bytes[3];
         }

         var hash = md5.ComputeHash(buffer);
         _sBuilder.Clear();
         for (int i = 0; i < hash.Length; i++)
         {
            _sBuilder.Append(hash[i].ToString("x2"));
         }
         return _sBuilder.ToString();
      }

   }
}
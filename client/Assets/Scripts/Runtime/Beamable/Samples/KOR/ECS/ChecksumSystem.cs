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
      protected override void OnUpdate()
      {
         var network = NetworkController.Instance;
         if (!NetworkController.NetworkInitialized) return;

         var time = World.Time.ElapsedTime;
         var tick = (long) (time * NetworkController.NetworkFramesPerSecond);

         if (network.Log.HasHashForTick(tick)) return; // don't do anything...

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
         network.Log.ReportHashForTick(tick, hash);
         if (tick % 40 == 0) // every 2 seconds ish...
         {
            network.SendNetworkMessage(new ChecksumEvent(hash, tick));
         }
      }

      private string GetMD5Checksum(List<uint> list)
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
         var sBuilder = new StringBuilder();
         for (int i = 0; i < hash.Length; i++)
         {
            sBuilder.Append(hash[i].ToString("x2"));
         }
         return sBuilder.ToString();
      }

   }
}
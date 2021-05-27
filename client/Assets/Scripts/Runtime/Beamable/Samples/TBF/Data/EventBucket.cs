using Beamable.Samples.TBF.Multiplayer.Events;
using System;
using System.Collections.Generic;

namespace Beamable.Samples.TBF.Data
{
   /// <summary>
   /// A collection of <see cref="TBFEvent"/> objects by dbid.
   /// 
   /// EXAMPLE: This is especially helpful for: 
   /// When you have 2 players in a game 
   /// and want to wait until 2 players send an event before 
   /// proceeding the game state? Use this!
   /// </summary>
   /// <typeparam name="T"></typeparam>
   public class EventBucket<T> where T : TBFEvent
   {
      public int Count { get { return _eventsByPlayerDbid.Count;} }

      public Dictionary<long, T>.ValueCollection Values { get { return _eventsByPlayerDbid.Values; } }

      private Dictionary<long, T> _eventsByPlayerDbid = new Dictionary<long, T>();

      public void Add(T e)
      {
         if (_eventsByPlayerDbid.ContainsKey(e.PlayerDbid))
         {
            throw new ArgumentException("EventBucket.AddEvent() This event has already been added.");
         }
         _eventsByPlayerDbid[e.PlayerDbid] =e;
      }

      public T GetByPlayerDbid(long playerDbid)
      {
         T e;
         _eventsByPlayerDbid.TryGetValue(playerDbid, out e);
         return e;
      }

      public void Clear()
      {
         _eventsByPlayerDbid.Clear();
      }
   }
}
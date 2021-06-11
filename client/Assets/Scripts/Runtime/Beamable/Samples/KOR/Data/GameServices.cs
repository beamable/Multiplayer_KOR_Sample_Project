using Cinemachine;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace Beamable.Samples.KOR.Data
{
    public class GameServices
    {
        private CinemachineImpulseSource _gameCameraCinemachineImpulseSource = null;

        public CinemachineImpulseSource CinemachineImpulseSource
        {
            get
            {
                if (_gameCameraCinemachineImpulseSource == null)
                    _gameCameraCinemachineImpulseSource = GameObject.FindObjectOfType<CinemachineImpulseSource>();

                return _gameCameraCinemachineImpulseSource;
            }
        }

        public void ShakeCamera()
        {
            CinemachineImpulseSource.GenerateImpulse();
        }

        private ConcurrentQueue<Action> _concurrentQueue = new ConcurrentQueue<Action>();

        public void EnqueueConcurrent(Action action)
        {
            _concurrentQueue.Enqueue(action);
        }

        public void Tick()
        {
            Action newAction;
            if (_concurrentQueue.TryDequeue(out newAction))
                newAction();
        }
    }
}
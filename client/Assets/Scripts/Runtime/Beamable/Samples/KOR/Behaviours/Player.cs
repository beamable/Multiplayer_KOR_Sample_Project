using System;
using Beamable.Samples.KOR.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beamable.Samples.KOR.Behaviours
{
    public class Player : MonoBehaviour
    {
        // Properties
        public HealthBehaviour HealthBehaviour => health;


        // Fields
        [SerializeField]
        private AvatarHUD avatarHUD;

        [SerializeField]
        private HealthBehaviour health;

        public void SetAlias(string newAlias)
        {
            avatarHUD.SetAlias(newAlias);
        }

        private void Update()
        {
            avatarHUD.SetHealth(health.HealthRatio);
        }
    }
}
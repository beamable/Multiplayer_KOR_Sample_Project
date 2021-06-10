using Beamable.Samples.KOR.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beamable.Samples.KOR.Behaviours
{
    public class Player : MonoBehaviour
    {
        [SerializeField]
        private AvatarHUD avatarHUD;

        public void SetAlias(string newAlias)
        {
            avatarHUD.SetAlias(newAlias);
        }

        public void SetHealth(int newHealth)
        {
            avatarHUD.SetHealth(newHealth);
        }
    }
}
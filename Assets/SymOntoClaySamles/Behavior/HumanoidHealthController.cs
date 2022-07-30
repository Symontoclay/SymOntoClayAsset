﻿using SymOntoClay;
using SymOntoClay.UnityAsset.Samles.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SymOntoClay.UnityAsset.Samles.Behavior
{
    [AddComponentMenu("SymOntoClaySamles/HumanoidHealthController")]
    public class HumanoidHealthController: MonoBehaviour, IHumanoidHealth, ITargetOfDamage
    {
        private IDieProvider _dieProvider;

        public int Health = 10;

        void Start()
        {
            _dieProvider = GetComponent<IDieProvider>();
        }

        public void SetHit(RaycastHit shootHit, int damagePerShot)
        {
            Health -= damagePerShot;

            if(Health < 0)
            {
                Health = 0;
                _dieProvider.Die();
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace SymOntoClay.UnityAsset.Helpers
{
    public class NavHelper
    {
        public NavHelper(Transform transform, NavMeshAgent navMeshAgent)
        {
            _transform = transform;
            _navMeshAgent = navMeshAgent;
        }

        private readonly Transform _transform;
        private readonly NavMeshAgent _navMeshAgent;
    }
}

using Assets.SymOntoClaySamles.Spawners;
using SymOntoClay.UnityAsset.Components;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SymOntoClay.UnityAsset.Samles.Spawners
{
    [AddComponentMenu("SymOntoClay Samles/SimpleSpawner")]
    public class SimpleSpawner : BaseSpawner
    {
        // Use this for initialization
        void Start()
        {
            ProcessInstantiate(transform.position);
        }
    }
}

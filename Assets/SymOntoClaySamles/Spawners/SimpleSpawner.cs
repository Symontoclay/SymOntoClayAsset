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
#if UNITY_EDITOR
            UnityEngine.Debug.Log("SimpleSpawner Start");
#endif

            ProcessInstantiate(transform.position);
        }
    }
}

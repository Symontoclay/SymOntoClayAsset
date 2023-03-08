using Assets.SymOntoClaySamles.Spawners;
using SymOntoClay.UnityAsset.Components;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
            //UnityEngine.Debug.Log("SimpleSpawner Start");

            //var thread = Thread.CurrentThread;
            //UnityEngine.Debug.Log($"SimpleSpawner Start thread.ManagedThreadId = {thread.ManagedThreadId}");
#endif

            if (IsEmptySpawner())
            {
                return;
            }

            ProcessInstantiate(transform.position);
        }
    }
}

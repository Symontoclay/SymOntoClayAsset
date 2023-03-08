using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.SymOntoClaySamles.Spawners
{
    [AddComponentMenu("SymOntoClay Samles/Spawn Group")]
    public class SpawnGroup : MonoBehaviour
    {
        public GameObject Prefab;
        public List<GameObject> PropsToSpawn;
    }
}
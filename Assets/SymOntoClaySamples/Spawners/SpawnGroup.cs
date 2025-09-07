using System.Collections.Generic;
using UnityEngine;

namespace Assets.SymOntoClaySamples.Spawners
{
    [AddComponentMenu("SymOntoClay Samples/Spawn Group")]
    public class SpawnGroup : MonoBehaviour
    {
#if UNITY_EDITOR
        public KindOfCharacter KindOfCharacter;
#endif

        public GameObject Prefab;
        public List<GameObject> PropsToSpawn;
    }
}
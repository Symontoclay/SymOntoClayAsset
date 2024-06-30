using System.Collections.Generic;
using UnityEngine;

namespace Assets.SymOntoClaySamles.Spawners
{
    [AddComponentMenu("SymOntoClay Samles/Spawn Group")]
    public class SpawnGroup : MonoBehaviour
    {
#if UNITY_EDITOR
        public KindOfCharacter KindOfCharacter;
#endif

        public GameObject Prefab;
        public List<GameObject> PropsToSpawn;
    }
}
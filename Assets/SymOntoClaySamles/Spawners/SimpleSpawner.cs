using SymOntoClay.UnityAsset.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SymOntoClay.UnityAsset.Samles.Spawners
{
    [AddComponentMenu("SymOntoClay Samles/SimpleSpawner")]
    public class SimpleSpawner : MonoBehaviour
    {
        public GameObject Prefab;
        public List<GameObject> PropsToSpawn;

        // Use this for initialization
        void Start()
        {
            var instance = Instantiate(Prefab, transform.position, transform.rotation);

            var npc = instance.GetComponent<HumanoidNPC>();

#if DEBUG
            Debug.Log($"SimpleSpawner Start npc == null = {npc == null}");
#endif
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

using SymOntoClay.UnityAsset.Components;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

            var humanoidNPC = instance.GetComponent<HumanoidNPC>();

#if DEBUG
            Debug.Log($"SimpleSpawner Start humanoidNPC == null = {humanoidNPC == null}");
#endif

#if DEBUG
            Debug.Log($"SimpleSpawner Start PropsToSpawn?.Count = {PropsToSpawn?.Count}");
#endif

            if(PropsToSpawn.Any())
            {
                foreach(var propItem in PropsToSpawn)
                {
                    var propInstance = Instantiate(propItem, transform.position, transform.rotation);

                    var handThing = propInstance.GetComponent<HandThing>();

#if DEBUG
                    Debug.Log($"SimpleSpawner Start handThing == null = {handThing == null}");
#endif
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

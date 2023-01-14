using SymOntoClay.UnityAsset.Components;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.SymOntoClaySamles.Spawners
{
    public class BaseSpawner : MonoBehaviour
    {
        public GameObject Prefab;
        public List<GameObject> PropsToSpawn;

        protected void ProcessInstantiate(Vector3 position)
        {
            var instance = Instantiate(Prefab, position, transform.rotation);

            var humanoidNPC = instance.GetComponent<HumanoidNPC>();

#if DEBUG
            Debug.Log($"BaseSpawner Start humanoidNPC == null = {humanoidNPC == null}");
#endif

#if DEBUG
            Debug.Log($"BaseSpawner Start PropsToSpawn?.Count = {PropsToSpawn?.Count}");
#endif

            if (PropsToSpawn.Any())
            {
                foreach (var propItem in PropsToSpawn)
                {
                    var propInstance = Instantiate(propItem, position, transform.rotation);

                    var handThing = propInstance.GetComponent<HandThing>();

#if DEBUG
                    Debug.Log($"BaseSpawner Start handThing == null = {handThing == null}");
#endif
                }
            }
        }
    }
}
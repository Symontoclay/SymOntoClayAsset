using SymOntoClay.UnityAsset.Components;
using SymOntoClay.UnityAsset.Samles.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.SymOntoClaySamles.Spawners
{
    public class BaseSpawner : MonoBehaviour
    {
        public SpawnGroup SpawnGroup;

        public GameObject Prefab;
        public List<GameObject> PropsToSpawn;        

        protected bool IsEmptySpawner()
        {
            if(Prefab == null && (SpawnGroup == null || SpawnGroup.Prefab == null))
            {
                return true;
            }

            return false;
        }

        protected void ProcessInstantiate(Vector3 position)
        {
            var instance = Instantiate(GetTargetPrefab(), position, transform.rotation);

            var humanoidNPC = instance.GetComponent<IBipedHumanoidCustomBehavior>();

#if DEBUG
            //Debug.Log($"BaseSpawner Start humanoidNPC == null = {humanoidNPC == null}");
#endif

#if DEBUG
            //Debug.Log($"BaseSpawner Start PropsToSpawn?.Count = {PropsToSpawn?.Count}");
#endif

            var targetPropsToSpawn = GetTargetPropsToSpawn();

            if (targetPropsToSpawn.Any())
            {
                foreach (var propItem in targetPropsToSpawn)
                {
                    var propInstance = Instantiate(propItem, position, transform.rotation);

                    var handThing = propInstance.GetComponent<IHandThingCustomBehavior>();

#if DEBUG
                    //Debug.Log($"BaseSpawner Start handThing == null = {handThing == null}");
#endif

                    humanoidNPC.Take(handThing);
                }
            }
        }

        private GameObject GetTargetPrefab()
        {
            if(Prefab == null)
            {
                if(SpawnGroup == null)
                {
                    return null;
                }

                if(SpawnGroup.Prefab == null)
                {
                    return null;
                }

                return SpawnGroup.Prefab;
            }

            return Prefab;
        }

        private static List<GameObject> EmptyTargetPropsToSpawn = new List<GameObject>();

        private List<GameObject> GetTargetPropsToSpawn()
        {
#if DEBUG
            Debug.Log($"BaseSpawner GetTargetPropsToSpawn PropsToSpawn?.Count = {PropsToSpawn?.Count}");
            Debug.Log($"BaseSpawner GetTargetPropsToSpawn SpawnGroup.PropsToSpawn?.Count = {SpawnGroup.PropsToSpawn?.Count}");
#endif

            if(PropsToSpawn.Any())
            {
                return PropsToSpawn;
            }
 
            if(SpawnGroup?.PropsToSpawn?.Any()??false)
            {
                return SpawnGroup.PropsToSpawn;
            }

            return EmptyTargetPropsToSpawn;
        }
    }
}
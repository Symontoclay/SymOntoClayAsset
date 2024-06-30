using SymOntoClay.UnityAsset.GizmoDecorators;
using SymOntoClay.UnityAsset.Samles.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.SymOntoClaySamles.Spawners
{
    public class BaseSpawner : MonoBehaviour
    {
#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            var size = GetComponent<Renderer>().bounds.size;

            DrawGizmosHelper.DrawGizmos(transform, size, SpawnGroup == null ? GetColor(KindOfCharacter) : GetColor(SpawnGroup.KindOfCharacter), false, null, false, null, true);
        }

        private static Color GetColor(KindOfCharacter kindOfCharacter)
        {
            return kindOfCharacter switch
            {
                KindOfCharacter.Blue => Color.blue,
                KindOfCharacter.Neutral => Color.green,
                KindOfCharacter.Red => Color.red,
                _ => throw new ArgumentOutOfRangeException(nameof(kindOfCharacter), kindOfCharacter, null)
            };
        }
#endif
        public KindOfCharacter KindOfCharacter;

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
            var targetPrefab = GetTargetPrefab();

            if(targetPrefab == null)
            {
                return;
            }

            var instance = Instantiate(targetPrefab, position, transform.rotation);

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

                    humanoidNPC.Take(null, handThing);
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
            Debug.Log($"BaseSpawner GetTargetPropsToSpawn SpawnGroup.PropsToSpawn?.Count = {SpawnGroup?.PropsToSpawn?.Count}");
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
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
#if UNITY_EDITOR
        

        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;

            var scaleX = transform.localScale.x / 2;

            var rightPoint = transform.position + transform.right.normalized * scaleX;

            var rX = rightPoint.x;

            var leftPoint = transform.position + transform.right.normalized * scaleX * -1;

            var lX = leftPoint.x;

            //Gizmos.DrawLine(transform.position, rightPoint);

            var scaleZ = transform.localScale.z / 2;

            var forvardPoint = transform.position + transform.forward.normalized * scaleZ;

            var fZ = forvardPoint.z;

            var backPoint = transform.position + transform.forward.normalized * scaleZ * -1;

            var bZ = backPoint.z;

            //Gizmos.DrawLine(transform.position, forvardPoint);

            var scaleY = transform.localScale.y / 2;

            var upPoint = transform.position + transform.up.normalized * scaleY;

            var uY = upPoint.y;

            var downPoint = transform.position + transform.up.normalized * scaleY * -1;

            var dY = downPoint.y;

            //Gizmos.DrawLine(transform.position, upPoint);

            var ufrPoint = new Vector3(rX, uY, fZ);
            var ubrPoint = new Vector3(rX, uY, bZ);
            var uflPoint = new Vector3(lX, uY, fZ);
            var ublPoint = new Vector3(lX, uY, bZ);

            Gizmos.DrawLine(ufrPoint, ubrPoint);
            Gizmos.DrawLine(ufrPoint, uflPoint);
            Gizmos.DrawLine(ublPoint, uflPoint);
            Gizmos.DrawLine(ublPoint, ubrPoint);

            var dfrPoint = new Vector3(rX, dY, fZ);
            var dbrPoint = new Vector3(rX, dY, bZ);
            var dflPoint = new Vector3(lX, dY, fZ);
            var dblPoint = new Vector3(lX, dY, bZ);

            Gizmos.DrawLine(dfrPoint, dbrPoint);
            Gizmos.DrawLine(dfrPoint, dflPoint);
            Gizmos.DrawLine(dblPoint, dflPoint);
            Gizmos.DrawLine(dblPoint, dbrPoint);

            Gizmos.DrawLine(dfrPoint, ufrPoint);
            Gizmos.DrawLine(dbrPoint, ubrPoint);
            Gizmos.DrawLine(dflPoint, uflPoint);
            Gizmos.DrawLine(dblPoint, ublPoint);
        }
#endif

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
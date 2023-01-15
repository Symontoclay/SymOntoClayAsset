using Assets.SymOntoClaySamles.Spawners;
using SymOntoClay.UnityAsset.Components;
using SymOntoClay.UnityAsset.Core;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace SymOntoClay.UnityAsset.Samles.Spawners
{
    [AddComponentMenu("SymOntoClay Samles/GridSpawner")]
    public class GridSpawner : BaseSpawner
    {
        public int CellHeight = 1;
        public int CellWidth = 1;
        public int Count = 1;

        // Use this for initialization
        void Start()
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Log("GridSpawner Start");

            var thread = Thread.CurrentThread;
            UnityEngine.Debug.Log($"GridSpawner Start thread.ManagedThreadId = {thread.ManagedThreadId}");
#endif

            var meshFilter = GetComponent<MeshFilter>();

            var mesh = meshFilter.mesh;

            var bounds = mesh.bounds;

#if DEBUG
            //Debug.Log($"GridSpawner Start bounds.size.x = {bounds.size.x}");
            //Debug.Log($"GridSpawner Start bounds.size.y = {bounds.size.y}");
            //Debug.Log($"GridSpawner Start bounds.size.z = {bounds.size.z}");
            //Debug.Log($"GridSpawner Start bounds.min = {bounds.min}");
            //Debug.Log($"GridSpawner Start bounds.max = {bounds.max}");
#endif

            var bottomLeft = bounds.min;
            var topRight = bounds.max;

            var bottomRight = new Vector3(topRight.x, 0, bottomLeft.z);
            var topLeft = new Vector3(bottomLeft.x, 0, topRight.z);

            Vector3 initLeftVector;
            Vector3 initRightVector;
            Vector3 leftVector;
            Vector3 rightVector;
            Vector3 initRowVector;
            Vector3 rowVector;

            initLeftVector = bottomLeft;
            initRightVector = bottomRight;

            var spawnPoints = new List<Vector3>();

            while(true)
            {
                leftVector = Vector3.MoveTowards(initLeftVector, topLeft, CellHeight);

                if(leftVector == topLeft)
                {
                    break;
                }

                initLeftVector = leftVector;

                rightVector = Vector3.MoveTowards(initRightVector, topRight, CellHeight);
                initRightVector = rightVector;

                initRowVector = leftVector;

                while(true)
                {
                    rowVector = Vector3.MoveTowards(initRowVector, rightVector, CellWidth);

                    if(rowVector == rightVector)
                    {
                        break;
                    }

                    initRowVector = rowVector;

                    spawnPoints.Add(transform.TransformPoint(rowVector));
                }
            }

            var n = 0;

            foreach(var spawnPoint in spawnPoints)
            {
                n++;

                if(n > Count)
                {
                    break;
                }

                ProcessInstantiate(spawnPoint);
            }
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}
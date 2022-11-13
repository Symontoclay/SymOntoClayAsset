using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SymOntoClay.UnityAsset.Samles.Spawners
{
    public class SimpleSpawner : MonoBehaviour
    {
        public GameObject Prefab;
        public List<GameObject> PropsToSpawn;

        // Use this for initialization
        void Start()
        {
            Instantiate(Prefab, transform.position, transform.rotation);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
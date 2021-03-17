using UnityEngine;

namespace RPG.Core
{
    public class PersistenObjectSpawner : MonoBehaviour
    {
        [SerializeField] GameObject persistentObjectPrefab;

        static bool hasSpawn = false;

        private void Awake()
        {
            if (hasSpawn) return;

            SpawnPersistenObjects();

            hasSpawn = true;
        }

        private void SpawnPersistenObjects()
        {
            GameObject persistentObject = Instantiate(persistentObjectPrefab);
            DontDestroyOnLoad(persistentObject);
        }
    }
}


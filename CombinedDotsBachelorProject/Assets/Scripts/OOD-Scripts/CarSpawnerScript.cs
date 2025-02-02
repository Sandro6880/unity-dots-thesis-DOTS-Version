using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace Assets.Scripts.OOD_Scripts
{
    public class CarSpawnerScript : MonoBehaviour
    {
        [SerializeField] private GameObject carPrefab;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private float spawnInterval = 0.1f;
        [SerializeField] private List<Transform> checkpoints;
        public int spawnCount = 0;

        private void Start()
        {
            StartCoroutine(SpawnCars());
        }

        private IEnumerator SpawnCars()
        {
            while (spawnCount < 20000)
            {
                GameObject car = Instantiate(carPrefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);

                // Assign checkpoints
                CarAI carAI = car.GetComponent<CarAI>();
                if (carAI != null)
                {
                    spawnCount++;
                    carAI.checkpoints = new List<Transform>(checkpoints);
                }

                yield return new WaitForSeconds(spawnInterval);
            }
        }
    }
}
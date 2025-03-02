using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Profiling;

namespace Assets.Scripts.OOD_Scripts
{
    public class CarSpawnerScript : MonoBehaviour
    {
        [SerializeField] private GameObject carPrefab;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private float spawnInterval = 0.01f;
        [SerializeField] private List<Transform> checkpoints;
        public int spawnCount = 0;
        //private float _timer;
        private void Start()
        {
            spawnInterval = 0.01f;
            StartCoroutine(SpawnCars());
        }
        /*private void Update()
        {
            SpawnCars2();
        }*/

        private IEnumerator SpawnCars()
        {
            while (spawnCount < 200000)
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
        /*private void SpawnCars2()
        {
            _timer += Time.deltaTime;
            if (_timer < spawnInterval) return; // Adjust spawn time
            _timer = 0f;
            while (spawnCount < 200000)
            {
                GameObject car = Instantiate(carPrefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);

                // Assign checkpoints
                CarAI carAI = car.GetComponent<CarAI>();
                if (carAI != null)
                {
                    spawnCount++;
                    carAI.checkpoints = new List<Transform>(checkpoints);
                }
            }
        }*/
    }
}
using Assets.Scripts.DOD_Scripts;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Profiling;
using UnityEngine;
using System.Linq;
using System.Globalization;
using Unity.Rendering;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.DOD_Scripts
{
    [BurstCompile]
    public partial struct CarSpawningSystem : ISystem
    {
        private EntityQuery _spawnerQuery;
        private EntityQuery _checkpointQuery;
        private float _timer;
        public int amountOfObjects;
        Entity checkpointEntity;
        EntityCommandBuffer ecb;
        private int lastSceneIndex;
        public void OnCreate(ref SystemState state)
        {
            // Require CarSpawnerComponent and at least one CarCheckpointBuffer
            state.RequireForUpdate<CarSpawnerComponent>();
            state.RequireForUpdate<CarCheckpointBuffer>();

            _spawnerQuery = SystemAPI.QueryBuilder().WithAll<CarSpawnerComponent>().Build();
            _checkpointQuery = SystemAPI.QueryBuilder().WithAll<CarCheckpointBuffer>().Build();
            amountOfObjects = 0;
            //Debug.Log($"CarSpawningSystem: SpawnerQuery Count: {_spawnerQuery.CalculateEntityCount()}");
            //Debug.Log($"CarSpawningSystem: CheckpointQuery Count: {_checkpointQuery.CalculateEntityCount()}");

        }
        public void OnUpdate(ref SystemState state)
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

            // Reset counter if we detect a scene change
            if (currentSceneIndex != lastSceneIndex)
            {
                lastSceneIndex = currentSceneIndex;
                amountOfObjects = 0;
            }

            _timer += SystemAPI.Time.DeltaTime;
            if (_timer < 0.01 || amountOfObjects > 10005) return; // Adjust spawn time
            _timer = 0f;

            if (_checkpointQuery.IsEmpty)
            {
                _checkpointQuery = SystemAPI.QueryBuilder().WithAll<CarCheckpointBuffer>().Build();
                checkpointEntity = _checkpointQuery.GetSingletonEntity();
            }
            ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var spawner in SystemAPI.Query<RefRW<CarSpawnerComponent>>())
            {
                // Spawn a new car
                Entity newCar = state.EntityManager.Instantiate(spawner.ValueRO.carPrefab);
                amountOfObjects++;
                //Debug.Log($"Spawned Car: {newCar}");
                // Set Position

                /*state.EntityManager.SetComponentData(newCar, new LocalTransform
                {
                    Position = spawner.ValueRO.spawnPosition,   ----> NEVER USE THIS ***, fucks up with the spawning
                });*/
                //ecb.AddComponent(newCar, new CarCheckpointReference { checkpointEntity = checkpointEntity });

            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        
        }
    }
}
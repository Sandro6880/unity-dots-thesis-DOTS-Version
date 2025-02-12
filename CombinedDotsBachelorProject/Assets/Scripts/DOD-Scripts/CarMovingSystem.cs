using Assets.Scripts.DOD_Scripts;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.DOD_Scripts
{
    [BurstCompile]
    public partial struct CarMovementSystem : ISystem
    {
        private EntityQuery _checkpointQuery;
        private Entity checkpointEntity;
        private DynamicBuffer<CarCheckpointBuffer> checkpoints;
        private bool startSystem; 
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CheckpointSingletonTag>();
            _checkpointQuery = SystemAPI.QueryBuilder().WithAll<CheckpointSingletonTag>().Build();
            startSystem = false;
        }

        public void OnUpdate(ref SystemState state)
        {
            if (_checkpointQuery.IsEmpty) return;

            // Get the checkpoint entity and buffer (read-only)
            var checkpointEntity = _checkpointQuery.GetSingletonEntity();
            var checkpoints = state.EntityManager.GetBuffer<CarCheckpointBuffer>(checkpointEntity).AsNativeArray();

            if (checkpoints.Length == 0) return;

            // **Schedule a Parallel Job**
            var job = new CarMovementJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
                Checkpoints = checkpoints
            };
            state.Dependency = job.ScheduleParallel(state.Dependency);
            // Get the singleton checkpoint entity
            /*if (_checkpointQuery.IsEmpty)
            {
                _checkpointQuery = SystemAPI.QueryBuilder().WithAll<CarCheckpointBuffer>().Build();
                checkpointEntity = _checkpointQuery.GetSingletonEntity();
                checkpoints = state.EntityManager.GetBuffer<CarCheckpointBuffer>(checkpointEntity);
                startSystem = true;
            }
            else
            {
                checkpointEntity = _checkpointQuery.GetSingletonEntity();
                checkpoints = state.EntityManager.GetBuffer<CarCheckpointBuffer>(checkpointEntity);
                startSystem = true;
            }

            if (startSystem)
            {
                foreach (var (carTransform, carData) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<CarComponent>>())
                {
                    if (checkpoints.Length == 0) return; //has correct size of 8

                    // Move towards the next checkpoint
                    var targetCheckpoint = checkpoints[carData.ValueRO.currentCheckpointIndex];
                    float3 direction = math.normalize(targetCheckpoint.position - carTransform.ValueRO.Position);

                    // Move the car
                    float speed = carData.ValueRO.speed;
                    carTransform.ValueRW.Position += direction * speed * SystemAPI.Time.DeltaTime;

                    if (!math.all(direction == float3.zero)) // Avoid NaN rotations
                    {
                        quaternion targetRotation = quaternion.LookRotationSafe(direction, math.up());
                        carTransform.ValueRW.Rotation = math.slerp(carTransform.ValueRO.Rotation, targetRotation, SystemAPI.Time.DeltaTime * 5f);
                    }

                    // Check if car has reached the checkpoint (within a certain threshold)
                    if (math.distance(carTransform.ValueRO.Position, targetCheckpoint.position) <= 2)
                    {
                        // Wait for a bit or immediately move to the next checkpoint
                        carData.ValueRW.waiting = true;
                        carData.ValueRW.currentCheckpointIndex = (carData.ValueRO.currentCheckpointIndex + 1) % checkpoints.Length; // Loop to next checkpoint
                    }

                    // If the car is done waiting, allow it to move again
                    if (carData.ValueRO.waiting)
                    {
                        carData.ValueRW.waitTime += SystemAPI.Time.DeltaTime;
                        if (carData.ValueRO.waitTime >= carData.ValueRO.waitDuration)
                        {
                            carData.ValueRW.waiting = false;
                            carData.ValueRW.waitTime = 0f;
                        }
                    }
                }
            }*/
        }
    }
    [BurstCompile]
    public partial struct CarMovementJob : IJobEntity
    {
        public float DeltaTime;
        [ReadOnly] public NativeArray<CarCheckpointBuffer> Checkpoints;

        public void Execute(ref LocalTransform carTransform, ref CarComponent carData)
        {
            if (Checkpoints.Length == 0) return;

            // Get target checkpoint
            var targetCheckpoint = Checkpoints[carData.currentCheckpointIndex];
            float3 direction = math.normalize(targetCheckpoint.position - carTransform.Position);

            // Move the car
            float speed = carData.speed;
            carTransform.Position += direction * speed * DeltaTime;

            // Rotate smoothly towards direction
            if (!math.all(direction == float3.zero))
            {
                quaternion targetRotation = quaternion.LookRotationSafe(direction, math.up());
                carTransform.Rotation = math.slerp(carTransform.Rotation, targetRotation, DeltaTime * 5f);
            }

            // Check if car has reached checkpoint
            if (math.distance(carTransform.Position, targetCheckpoint.position) <= 2)
            {
                carData.waiting = true;
                carData.currentCheckpointIndex = (carData.currentCheckpointIndex + 1) % Checkpoints.Length; // Loop checkpoints
            }

            // Handle waiting logic
            if (carData.waiting)
            {
                carData.waitTime += DeltaTime;
                if (carData.waitTime >= carData.waitDuration)
                {
                    carData.waiting = false;
                    carData.waitTime = 0f;
                }
            }
        }
    }
}
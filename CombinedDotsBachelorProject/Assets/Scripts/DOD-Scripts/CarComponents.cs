using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Assets.Scripts.DOD_Scripts
{
    public struct CarComponent : IComponentData
    {
        public float speed;
        public int currentCheckpointIndex;
        public bool waiting;                // Whether the car is waiting at the checkpoint
        public float waitTime;              // Timer for waiting
        public float waitDuration;          // Time to wait at the checkpoint
        public float stoppingDistance;
    }
    
    [InternalBufferCapacity(10)] // Optimize for most cases (adjust as needed)
    public struct CarCheckpointBuffer : IBufferElementData
    {
        public float3 position;
    }
    public struct CarSpawnerComponent : IComponentData
    {
        public Entity carPrefab;
        public float3 spawnPosition;
        public float spawnFrequency;
        public FixedList512Bytes<float3> checkpoints;
    }
    public struct CheckpointSingletonTag : IComponentData { }

    public struct CarCheckpointReference : IComponentData
    {
        public Entity checkpointEntity;
    }
}


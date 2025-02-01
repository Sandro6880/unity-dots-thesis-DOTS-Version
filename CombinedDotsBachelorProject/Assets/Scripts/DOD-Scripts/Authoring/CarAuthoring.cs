using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.DOD_Scripts.Authoring
{
    public class CarAuthoring : MonoBehaviour
    {
        public float speed = 10f;
        public class Baker : Baker<CarAuthoring>
        {
            public override void Bake(CarAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new CarComponent 
                { 
                    speed = authoring.speed, 
                    currentCheckpointIndex = 0,
                    waiting = false,
                    waitTime = 0f,
                    waitDuration = 2f,  
                    stoppingDistance = 1f
                });
                AddComponent(entity, new LocalTransform
                {
                    Position = authoring.transform.position,
                    Rotation = authoring.transform.rotation
                });
            }
        }
    }
}

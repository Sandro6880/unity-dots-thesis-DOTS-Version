using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.DOD_Scripts.Authoring
{
    public class CheckpointAuthoring : MonoBehaviour
    {
        public Transform[] checkpoints;

        public class Baker : Baker<CheckpointAuthoring>
        {
            public override void Bake(CheckpointAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                //Debug.Log($"[CheckpointBaker] Baking {authoring.checkpoints.Length} checkpoints to Entity {entity.Index}");
                var buffer = AddBuffer<CarCheckpointBuffer>(entity);

                foreach (var checkpoint in authoring.checkpoints)
                {
                    buffer.Add(new CarCheckpointBuffer { position = checkpoint.position });
                }

                AddComponent<CheckpointSingletonTag>(entity);
            }
        }
    }
}

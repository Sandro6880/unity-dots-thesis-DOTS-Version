using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.DOD_Scripts.Authoring
{
    public class CarSpawnerAuthoring : MonoBehaviour
    {
        public GameObject carPrefab;
        public Transform spawnPoint;
        public float spawnFrequency = 1f;

        public class Baker : Baker<CarSpawnerAuthoring>
        {
            public override void Bake(CarSpawnerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                //Debug.Log($"[CarSpawnerAuthoring] Baking {authoring.carPrefab} carPrefab to Entity {entity.Index}");
                AddComponent(entity, new CarSpawnerComponent
                {
                    carPrefab = GetEntity(authoring.carPrefab, TransformUsageFlags.Dynamic),
                    spawnPosition = authoring.spawnPoint.position,
                    spawnFrequency = authoring.spawnFrequency
                });
            }
        }
    }
}

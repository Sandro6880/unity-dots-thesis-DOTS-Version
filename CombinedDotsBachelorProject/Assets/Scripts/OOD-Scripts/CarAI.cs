using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.OOD_Scripts
{
    public class CarAI : MonoBehaviour
    {
        public List<Transform> checkpoints;
        public float speed = 10f;
        public float turnSpeed = 5f;
        private int currentCheckpointIndex = 0;

        private void Update()
        {
            if (checkpoints.Count == 0) return;

            MoveTowardsCheckpoint();
        }
    
        private void MoveTowardsCheckpoint()
        {
            Transform targetCheckpoint = checkpoints[currentCheckpointIndex];
            Vector3 direction = (targetCheckpoint.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetCheckpoint.position) < 1f)
            {
                currentCheckpointIndex = (currentCheckpointIndex + 1) % checkpoints.Count;
            }
        }
    } 
}

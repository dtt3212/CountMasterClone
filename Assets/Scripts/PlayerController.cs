using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using System.Collections.Generic;

namespace CountMasterClone
{
    public class PlayerController : MonoBehaviour
    {
        public const int LayerNumber = 6;

        [SerializeField]
        private float verticalMoveSpeedPerSec = 0.5f;

        [SerializeField]
        private float horizontalMoveSpeedPerSec = 10.0f;

        [SerializeField]
        private float cameraDistance = 10.0f;

        [SerializeField]
        private Camera gameCamera;

        private ClonableGroupController groupController;

        private void Awake()
        {
            groupController = GetComponent<ClonableGroupController>();
        }

        private void OnMove(InputValue value)
        {
            Vector2 pos = value.Get<Vector2>();
            Vector3 realWorldPos = gameCamera.ScreenToWorldPoint(new Vector3(pos.x, pos.y, cameraDistance));

            Vector3 direction = (realWorldPos - transform.position);
            direction.Scale(Vector3.right);

            transform.position += direction.normalized * horizontalMoveSpeedPerSec * Time.deltaTime; 
        }

        private void SpawnNewClones(GateController gate)
        {
            if (gate == null)
            {
                return;
            }
 
            int addNumber = 0;

            switch (gate.GateType)
            {
                case GateType.Add:
                    {
                        addNumber = gate.Value;
                        break;
                    }

                case GateType.Multiplication:
                    {
                        addNumber = groupController.CloneCount * (gate.Value - 1);
                        break;
                    }
            }

            if (addNumber == 0)
            {
                return;
            }

            groupController.Clone(addNumber);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == GateController.LayerNumber)
            {
                SpawnNewClones(other.gameObject.GetComponent<GateController>());
            }
        }

        private void Update()
        {
            transform.position += new Vector3(0, 0, verticalMoveSpeedPerSec * Time.deltaTime);
        }
    }
}

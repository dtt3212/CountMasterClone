using UnityEngine;
using UnityEngine.InputSystem;

namespace CountMasterClone
{
    public class PlayerController : GroupController
    {
        [SerializeField]
        private float verticalMoveSpeedPerSec = 0.5f;

        [SerializeField]
        private float horizontalMoveSpeedPerSec = 10.0f;

        [SerializeField]
        private float attackSpeed = 20.0f;

        [SerializeField]
        private float cameraDistance = 10.0f;

        [SerializeField]
        private Camera gameCamera;

        private GroupManager clonableGroupManager;

        private void Start()
        {
            clonableGroupManager = GetComponent<GroupManager>();
            TargetChanged += OnTargetChanged;
        }

        private void OnMove(InputValue value)
        {
            if (AggressiveMode)
            {
                return;
            }

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
                        addNumber = clonableGroupManager.CloneCount * (gate.Value - 1);
                        break;
                    }
            }

            if (addNumber == 0)
            {
                return;
            }

            clonableGroupManager.Clone(addNumber);
        }

        private void OnTargetChanged()
        {
            if (Target != null)
            {
                GroupManager targetGroupManager = Target.GetComponent<GroupManager>();
                if (targetGroupManager != null)
                {
                    targetGroupManager.Disbanded += () =>
                    {
                        AggressiveMode = false;
                        ClearTarget();
                    };
                }
            }
        }

        protected override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);

            if (other.gameObject.layer == (int)EntityLayer.Gate)
            {
                SpawnNewClones(other.gameObject.GetComponent<GateController>());
            }
            else if (other.gameObject.layer == (int)EntityLayer.EnemiesCage)
            {
                // The first clone enter the cage will make the group go into this aggressive mode
                AggressiveMode = true;
            }
        }

        private void Update()
        {
            transform.position += (AggressiveMode ? MoveDirection * attackSpeed : new Vector3(0, 0, verticalMoveSpeedPerSec)) * Time.deltaTime;
        }
    }
}

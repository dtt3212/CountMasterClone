using UnityEngine;
using UnityEngine.InputSystem;

namespace CountMasterClone
{
    public class PlayerGroupController : GroupController
    {
        [SerializeField]
        private float attackSpeed = 4.0f;

        private GroupManager clonableGroupManager;

        private void Start()
        {
            clonableGroupManager = GetComponent<GroupManager>();
            TargetChanged += OnTargetChanged;
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
            if (AggressiveMode)
            {
                transform.localPosition += MoveDirection * attackSpeed * Time.deltaTime;
            }
        }
    }
}

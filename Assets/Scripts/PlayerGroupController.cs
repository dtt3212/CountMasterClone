using UnityEngine;

namespace CountMasterClone
{
    public class PlayerGroupController : GroupController
    {
        [SerializeField]
        private float attackSpeed = 4.0f;

        private PlayerGroupManager clonableGroupManager;

        private FinishDestinationType destinationType = FinishDestinationType.None;

        public bool ReachedFinish => destinationType != FinishDestinationType.None;
        public FinishDestinationType FinishDestinationType => destinationType;

        private void Start()
        {
            clonableGroupManager = GetComponent<PlayerGroupManager>();
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

            switch ((EntityLayer)other.gameObject.layer)
            {
                case EntityLayer.Gate:
                    SpawnNewClones(other.gameObject.GetComponent<GateController>());
                    break;

                case EntityLayer.EnemiesCage:
                    // The first clone enter the cage will make the group go into this aggressive mode
                    AggressiveMode = true;
                    break;

                case EntityLayer.Finish:
                    {
                        FinishLineInfo info = other.gameObject.GetComponent<FinishLineInfo>();
                        if (info != null)
                        {
                            destinationType = info.DestinationType;
                        }

                        clonableGroupManager.StartBuildingTower();
                        break;
                    }

                case EntityLayer.Stair:
                    clonableGroupManager.StepOnStair(other.gameObject.GetComponent<MultiplierStairsDestController>());
                    break;
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

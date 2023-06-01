using UnityEngine;

namespace CountMasterClone
{
    public class MultiplierStairsDestController : MonoBehaviour
    {
        [SerializeField]
        private GameObject stepPrefab;

        [SerializeField]
        private Material[] stairMaterials;

        private int stairCount;

        private void Initialize(int stairCount)
        {
            this.stairCount = stairCount;

            Vector3 putPosition = Vector3.zero;
            float currentMulitplier = 1.0f;

            for (int i = 0; i < stairCount; i++)
            {
                GameObject step = Instantiate(stepPrefab, transform, false);

                if (putPosition == Vector3.zero)
                {
                    step.transform.position = transform.position;
                }
                else
                {
                    step.transform.position = putPosition;
                }

                StairStepManager stepManager = step.GetComponent<StairStepManager>();
                stepManager.Initialize(stairMaterials[i % stairMaterials.Length], currentMulitplier);

                putPosition = stepManager.NextStepPoint;
                currentMulitplier += 0.1f;
            }
        }

        public int StairCount => stairCount;

        public Transform GetStairCloneRester(int position)
        {
            if (position >= stairCount)
            {
                return null;
            }

            return transform.GetChild(position).GetComponent<StairStepManager>().CloneRester;
        }

        public float GetStairMultiplier(int position)
        {
            if (position >= stairCount)
            {
                throw new System.ArgumentException("Invalid stair index!");
            }

            return transform.GetChild(position).GetComponent<StairStepManager>().Multiplier;
        }

        private void Start()
        {
            Initialize(10);
        }
    }
}
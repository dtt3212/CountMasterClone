using UnityEngine;

namespace CountMasterClone
{
    public class MultiplierStairsDestController : MonoBehaviour
    {
        [SerializeField]
        private GameObject stepPrefab;

        [SerializeField]
        private GameObject destinationPlatformPrefab;

        [SerializeField]
        private Material[] stairMaterials;

        private StairDestinationInfo destinationInfo;

        private int stairCount;

        public StairDestinationInfo DestinationInfo => destinationInfo;

        public void Initialize(float maxMultiplier)
        {
            this.stairCount = (int)((maxMultiplier - 1.0f) / 0.2f + 2);

            Vector3 putPosition = Vector3.zero;
            Vector3 platformPosition = Vector3.zero;
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
                platformPosition = stepManager.DestPlatformPoint;

                currentMulitplier += 0.2f;
            }

            GameObject platform = Instantiate(destinationPlatformPrefab, transform, false);
            platform.transform.position = platformPosition;

            destinationInfo = platform.GetComponent<StairDestinationInfo>();
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
        }
    }
}
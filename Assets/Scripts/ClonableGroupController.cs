using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace CountMasterClone
{
    public class ClonableGroupController : MonoBehaviour
    {
        [SerializeField]
        private GameObject clonePrefab;

        [SerializeField]
        private float distanceBetweenSpawn = 0.7f;

        [SerializeField]
        private float unevenDistance = 0.2f;

        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float unevenIntensity = 0.4f;

        [SerializeField]
        private int perColumnCloneEstimate = 7;

        [SerializeField]
        private bool spawnInCircleLayers = false;

        [SerializeField]
        [Range(0.0f, 10.0f)]
        private float moveToNewPositonDuration = 0.3f;

        public int CloneCount => transform.childCount;

        private void OnCloneDead()
        {
        }

        private void Awake()
        {
            DOTween.Init();
        }

        private void Start()
        {
            Transform containerTransform = transform;

            for (int i = 0; i < transform.childCount; i++)
            {
                DamagableController controller = containerTransform.GetChild(i).GetComponent<DamagableController>();
                if (controller != null)
                {
                    controller.Died += OnCloneDead;
                }
            }
        }

        public void Clone(int additionNumber)
        {
            for (int i = 0; i < additionNumber; i++)
            {
                GameObject clone = Instantiate(clonePrefab, transform, false);
                DamagableController controller = clone.GetComponent<DamagableController>();

                controller.Died += OnCloneDead;
            }

            RepositionClones();
        }

        public void RepositionClones()
        {
            Transform containerTransform = transform;
            float shrunkenDistance = (containerTransform.childCount >= 100) ? distanceBetweenSpawn / Mathf.Log(containerTransform.childCount, 110) : distanceBetweenSpawn;

            if (spawnInCircleLayers)
            {
                int currentLayer = 1;
                int currentObjectInLayer = 0;
                float anglePerObject = 360.0f;

                for (int i = 0; i < containerTransform.childCount; i++)
                {
                    Transform stickmanTransfrom = containerTransform.GetChild(i);

                    float x = shrunkenDistance * (currentLayer - 1) * Mathf.Cos(anglePerObject * currentObjectInLayer);
                    float z = shrunkenDistance * (currentLayer - 1) * Mathf.Sin(anglePerObject * currentObjectInLayer);

                    stickmanTransfrom.DOLocalMove(new Vector3(x, 0, z), moveToNewPositonDuration)
                        .SetEase(Ease.OutBack);

                    if (++currentObjectInLayer >= currentLayer * currentLayer)
                    {
                        currentLayer++;
                        currentObjectInLayer = 0;

                        anglePerObject = Mathf.PI * 2 / (currentLayer * currentLayer);
                    }
                }
            }
            else
            {
                int avgPerCol = Mathf.FloorToInt(Mathf.Sqrt(containerTransform.childCount));

                List<int> colCounts = new List<int>();
                int totalCalced = 0;

                int dividedByTwoCol = avgPerCol;
                int positionStartRng = 0;

                while ((dividedByTwoCol >= 2) && (totalCalced < containerTransform.childCount))
                {
                    int takeThisTime = Mathf.Min(containerTransform.childCount - totalCalced, dividedByTwoCol / 2);
                    colCounts.Insert(0, takeThisTime);

                    totalCalced += takeThisTime;
                    if (totalCalced >= containerTransform.childCount)
                    {
                        break;
                    }

                    takeThisTime = Mathf.Min(containerTransform.childCount - totalCalced, dividedByTwoCol / 2);
                    colCounts.Add(takeThisTime);

                    totalCalced += takeThisTime;

                    if (totalCalced >= containerTransform.childCount)
                    {
                        break;
                    }

                    positionStartRng++;
                    dividedByTwoCol /= 2;
                }

                while (totalCalced < containerTransform.childCount)
                {
                    int columnCloneCount = Mathf.Min(Random.Range(avgPerCol - 1, avgPerCol + 2), containerTransform.childCount - totalCalced);

                    colCounts.Insert(positionStartRng++, columnCloneCount);
                    totalCalced += columnCloneCount;
                }

                int startingColumn = -(colCounts.Count / 2);
                int currentStickman = 0;

                for (int i = 0; i < colCounts.Count; i++)
                {
                    int column = startingColumn + i;
                    int cloneColumnCount = colCounts[i];
                    int startingRow = -(cloneColumnCount / 2);

                    for (int j = 0; j < cloneColumnCount; j++)
                    {
                        float x = shrunkenDistance * column + (Mathf.Cos(startingRow + j) * unevenDistance * unevenIntensity);
                        float z = shrunkenDistance * (startingRow + j) + (Mathf.Sin(column) * unevenDistance * unevenIntensity);

                        Transform stickmanTransfrom = containerTransform.GetChild(currentStickman++);

                        stickmanTransfrom.DOLocalMove(new Vector3(x, 0, z), moveToNewPositonDuration)
                            .SetEase(Ease.OutBack);
                    }
                }
            }
        }

    }
}
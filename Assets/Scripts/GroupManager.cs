using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace CountMasterClone
{
    public class GroupManager : MonoBehaviour
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
        [Range(0.0f, 10.0f)]
        private float moveToNewPositonDuration = 0.3f;

        [SerializeField]
        [Range(0.0f, 10.0f)]
        private float clonePopOffDuration = 0.3f;

        [SerializeField]
        private bool repositionable = false;

        private bool groupMembersChanged = false;

        public int CloneCount => transform.childCount;
        public event System.Action Disbanded;

        protected virtual void OnDisbanding()
        {
            Destroy(gameObject);
        }

        private void OnCloneDead(GameObject unfortunateClone, HitReason reason)
        {
            unfortunateClone.transform.DOComplete();
            groupMembersChanged = true;
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

        private void RepositionClones(bool moveNearestToTarget = false)
        {
            Transform containerTransform = transform;
            float shrunkenDistance = (containerTransform.childCount >= 100) ? distanceBetweenSpawn / Mathf.Log(containerTransform.childCount, 110) : distanceBetweenSpawn;

            int avgPerCol = Mathf.FloorToInt(Mathf.Sqrt(containerTransform.childCount));

            List<int> colCounts = new List<int>();
            int totalCalced = 0;

            int dividedByTwoCol = avgPerCol;
            int positionStartRng = 0;

            while ((dividedByTwoCol >= 4) && (totalCalced < containerTransform.childCount))
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

            List<Transform> unsearchedTransforms = null;
            if (moveNearestToTarget)
            {
                unsearchedTransforms = new();
                for (int i = 0; i < transform.childCount; i++)
                {
                    unsearchedTransforms.Add(transform.GetChild(i));
                }
            }

            for (int i = 0; i < colCounts.Count; i++)
            {
                int column = startingColumn + i;
                int cloneColumnCount = colCounts[i];
                int startingRow = -(cloneColumnCount / 2);

                for (int j = 0; j < cloneColumnCount; j++)
                {
                    float x = shrunkenDistance * column + (Mathf.Cos(startingRow + j) * unevenDistance * unevenIntensity);
                    float z = shrunkenDistance * (startingRow + j) + (Mathf.Sin(column) * unevenDistance * unevenIntensity);

                    Transform stickmanTransfrom = null;
                    Vector3 newPos = new Vector3(x, 0, z);

                    if (!moveNearestToTarget)
                    {
                        stickmanTransfrom = containerTransform.GetChild(currentStickman++);
                    }
                    else
                    {
                        float minDistNeg = float.MaxValue;
                        float minDistPos = float.MaxValue;

                        int transformIndexNeg = -1;
                        int transformIndexPos = -1;

                        for (int k = 0; k < unsearchedTransforms.Count; k++)
                        {
                            float dist = Vector3.Distance(unsearchedTransforms[k].localPosition, newPos);
                            bool zDirTowards = (unsearchedTransforms[k].localPosition.z < newPos.z); 

                            if (zDirTowards)
                            {
                                if (dist < minDistPos)
                                {
                                    transformIndexPos = k;
                                    minDistPos = dist;
                                }
                            }
                            else
                            {
                                if (dist < minDistNeg)
                                {
                                    transformIndexNeg = k;
                                    minDistNeg = dist;
                                }
                            }
                        }

                        int transformIndex = 0;
                        if (transformIndexNeg < 0)
                        {
                            transformIndex = transformIndexPos;
                        }
                        else
                        {
                            if ((minDistPos < minDistNeg) || (Mathf.Abs(minDistPos - minDistNeg) <= 0.3f))
                            {
                                transformIndex = transformIndexPos;
                            }
                            else
                            {
                                transformIndex = transformIndexNeg;
                            }
                        }

                        if (transformIndex < 0)
                        {
                            transformIndex = 0;
                        }

                        stickmanTransfrom = unsearchedTransforms[transformIndex];
                        unsearchedTransforms.RemoveAt(transformIndex);
                    }

                    stickmanTransfrom.DOComplete();
                    stickmanTransfrom.DOLocalMove(newPos, moveNearestToTarget ? moveToNewPositonDuration : clonePopOffDuration)
                        .SetEase(moveNearestToTarget ? Ease.Linear : Ease.OutBack);
                }
            }
        }

        private void Update()
        {
            if (groupMembersChanged)
            {
                if (transform.childCount == 0)
                {
                    Disbanded?.Invoke();
                    OnDisbanding();
                }
                else if (repositionable)
                {
                    RepositionClones(moveNearestToTarget: true);
                }

                groupMembersChanged = false;
            }
        }
    }
}
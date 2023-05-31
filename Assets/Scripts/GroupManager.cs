using DG.Tweening;
using System.Collections.Generic;
using Unity.VisualScripting;
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

        private Transform[,] childMap;

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
            if (repositionable)
            {
                RepositionToReplaceFallen(unfortunateClone);
            }
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

        private Vector3 RetriveMemberPosition(int x, int y)
        {
            int startingColumn = -(childMap.GetLength(0) / 2);
            int zeroYRow = childMap.GetLength(1) / 2;

            int columnReal = startingColumn + x;
            int rowReal = -(y - zeroYRow);

            float finalX = distanceBetweenSpawn * columnReal + (Mathf.Cos(rowReal) * unevenDistance * unevenIntensity);
            float finalZ = distanceBetweenSpawn * rowReal + (Mathf.Sin(columnReal) * unevenDistance * unevenIntensity);

            return new Vector3(finalX, 0, finalZ);
        }

        private void TweenToPosition(int gridX, int gridY)
        {
            childMap[gridX, gridY].DOLocalMove(RetriveMemberPosition(gridX, gridY), moveToNewPositonDuration)
                .SetEase(Ease.Linear);
        }

        private void RepositionToFillSpace(int destX, int destY, bool skipCurrentCol = false)
        {
            // Find a near alternative to go in. Check what side it's by
            int middleColumnIndex = childMap.GetLength(0) / 2;

            bool rightSide = (destX > middleColumnIndex);

            int y = destY;
            int yInc = 1;

            // First, look on the right side to see if anything can be taken
            while (y > 0)
            {
                for (int x = skipCurrentCol ? (rightSide ? destX + 1 : destX - 1) : destX; rightSide ? (x < childMap.GetLength(0)) : (x >= 0); x += (rightSide ? 1 : -1))
                {
                    if (childMap[x, y] != null)
                    {
                        // Take first and move the previous column to its place
                        childMap[destX, destY] = childMap[x, y];
                        childMap[x, y] = null;

                        TweenToPosition(destX, destY);

                        int yAlt = y + 1;
                        bool needFillSpace = false;

                        for (yAlt = y + 1; yAlt < childMap.GetLength(1); yAlt++)
                        {
                            childMap[x, yAlt - 1] = childMap[x, yAlt];
                            childMap[x, yAlt] = null;

                            if (childMap[x, yAlt - 1] == null)
                            {
                                needFillSpace = true;
                                break;
                            }

                            TweenToPosition(x, yAlt - 1);
                        }

                        if (needFillSpace)
                        {
                            RepositionToFillSpace(x, yAlt - 1, skipCurrentCol: true);
                        }

                        return;
                    }
                }

                y += yInc;

                if (y >= childMap.GetLength(1))
                {
                    yInc = -1;
                    y = destY - 1;
                }
            }
        }

        private void RepositionToReplaceFallen(GameObject fallenSolider)
        {
            GroupMemberInfo fallenInfo = fallenSolider.GetComponent<GroupMemberInfo>();
            childMap[fallenInfo.X, fallenInfo.Y] = null;

            RepositionToFillSpace(fallenInfo.X, fallenInfo.Y);
        }

        private void RepositionClones(bool moveNearestToTarget = false)
        {
            Transform containerTransform = transform;
            int avgPerCol = Mathf.FloorToInt(Mathf.Sqrt(containerTransform.childCount));

            List<int> colCounts = new List<int>();
            int totalCalced = 0;

            int dividedByTwoCol = avgPerCol;
            int positionStartRng = 0;

            int maxColumnCount = 0;

            while ((dividedByTwoCol >= 4) && (totalCalced < containerTransform.childCount))
            {
                int takeThisTime = Mathf.Min(containerTransform.childCount - totalCalced, dividedByTwoCol / 2);
                maxColumnCount = Mathf.Max(takeThisTime, maxColumnCount);

                colCounts.Insert(0, takeThisTime);

                totalCalced += takeThisTime;
                if (totalCalced >= containerTransform.childCount)
                {
                    break;
                }

                takeThisTime = Mathf.Min(containerTransform.childCount - totalCalced, dividedByTwoCol / 2);
                maxColumnCount = Mathf.Max(takeThisTime, maxColumnCount);

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
                maxColumnCount = Mathf.Max(columnCloneCount, maxColumnCount);

                colCounts.Insert(positionStartRng++, columnCloneCount);
                totalCalced += columnCloneCount;
            }

            int startingColumn = -(colCounts.Count / 2);
            int currentStickman = 0;

            childMap = new Transform[colCounts.Count, maxColumnCount];

            for (int i = 0; i < colCounts.Count; i++)
            {
                int column = startingColumn + i;
                int cloneColumnCount = colCounts[i];
                int startingRow = -(cloneColumnCount / 2);
                int startingRowInChildMap = maxColumnCount / 2 + cloneColumnCount / 2 - ((maxColumnCount % 2 == 0) ? 1 : 0);

                for (int j = 0; j < cloneColumnCount; j++)
                {
                    float x = distanceBetweenSpawn * column + (Mathf.Cos(startingRow + j) * unevenDistance * unevenIntensity);
                    float z = distanceBetweenSpawn * (startingRow + j) + (Mathf.Sin(column) * unevenDistance * unevenIntensity);

                    Transform stickmanTransfrom = containerTransform.GetChild(currentStickman++);
                    Vector3 newPos = new Vector3(x, 0, z);

                    childMap[i, startingRowInChildMap - j] = stickmanTransfrom;

                    GroupMemberInfo childInfo = stickmanTransfrom.GetOrAddComponent<GroupMemberInfo>();
                    childInfo.X = i;
                    childInfo.Y = startingRowInChildMap - j;

                    stickmanTransfrom.DOComplete();
                    stickmanTransfrom.DOLocalMove(newPos, clonePopOffDuration).SetEase(Ease.OutBack);
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

                groupMembersChanged = false;
            }
        }
    }
}
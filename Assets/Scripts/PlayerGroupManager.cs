using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using System.Linq;

namespace CountMasterClone
{
    public class PlayerGroupManager : GroupManager
    {
        private List<List<Transform>> towerLayers;

        [SerializeField]
        private float towerLayerHeight = 0.5f;

        [SerializeField]
        private float timeBuildupTower = 0.8f;

        [SerializeField]
        private float timeBetweenStairStepping = 0.3f;

        [SerializeField]
        private int maxPerLayer = 10;

        private bool isBuilding = false;
        private bool isSteppingOnStair = false;

        public void StartBuildingTower()
        {
            if (isBuilding)
            {
                return;
            }

            isBuilding = true;
            StartCoroutine(BuildTower());
        }

        public void StepOnStair(MultiplierStairsDestController stepManager)
        {
            if (isSteppingOnStair)
            {
                return;
            }

            isSteppingOnStair = true;
            StartCoroutine(StepOnStairCoroutine(stepManager));
        }

        private IEnumerator StepOnStairCoroutine(MultiplierStairsDestController stepManager)
        {
            WaitForSeconds timeStepBreak = new WaitForSeconds(timeBetweenStairStepping);
            int stairMax = Mathf.Max(stepManager.StairCount, towerLayers.Count);

            float heightDecrease = 0.0f;

            for (int i = 0; i < stairMax; i++)
            {
                int layerIndex = towerLayers.Count - i - 1;

                Transform rester = stepManager.GetStairCloneRester(i);
                transform.position = rester.position - Vector3.up * heightDecrease;

                // Detach the current layer's transforms from parent
                foreach (Transform transform in towerLayers[layerIndex])
                {
                    transform.SetParent(rester, true);
                }

                heightDecrease += towerLayerHeight;
                yield return timeStepBreak;
            }

            yield break;
        }

        private IEnumerator BuildTower()
        {
            towerLayers = new();

            int left = transform.childCount;

            // Index 0 will say how many 1-clone layer there is
            // Index 1 will say how many 2-clone layer there is
            List<int> layerIndiciesCount = new();
            while (left > 0)
            {
                int maxLayerCurrent = 1;
                while (maxLayerCurrent <= maxPerLayer)
                {
                    if ((maxLayerCurrent + 2) * (maxLayerCurrent + 1) / 2 > left)
                    {
                        break;
                    }

                    maxLayerCurrent++;
                }

                for (int i = 0; i < maxLayerCurrent; i++)
                {
                    if (layerIndiciesCount.Count < i + 1)
                    {
                        layerIndiciesCount.Add(1);
                    }
                    else
                    {
                        layerIndiciesCount[i]++;
                    }
                }

                left -= (maxLayerCurrent + 1) * maxLayerCurrent / 2;
            }

            int layerCount = layerIndiciesCount.Sum();
            float timePerLayer = timeBuildupTower / layerCount;

            WaitForSeconds waitBuildup = new WaitForSeconds(timePerLayer);

            Queue<Vector2> searching = new();
            List<Vector2> closed = new();
            
            searching.Enqueue(new Vector2(childMap.GetLength(0) / 2, childMap.GetLength(1) / 2));
            closed.Add(searching.Peek());

            for (int layerCountIndex = 0; layerCountIndex < layerIndiciesCount.Count; layerCountIndex++)
            {
                for (int repeatCount = 0; repeatCount < layerIndiciesCount[layerCountIndex]; repeatCount++)
                {
                    int toTake = layerCountIndex + 1;

                    List<Transform> layer = new();

                    while (toTake > 0)
                    {
                        Vector2 positionTake = searching.Dequeue();
                        Transform childTransformCurrent = childMap[(int)positionTake.x, (int)positionTake.y];

                        if (childTransformCurrent != null)
                        {
                            layer.Add(childTransformCurrent);
                            toTake--;
                        }

                        for (int i = -1; i <= 1; i++)
                        {
                            for (int j = -1; j <= 1; j++)
                            {
                                Vector2 checkPosition = positionTake + new Vector2(i, j);
                                if ((checkPosition.x < 0) || (checkPosition.y < 0) || (checkPosition.x >= childMap.GetLength(0)) || (checkPosition.y >= childMap.GetLength(1)))
                                {
                                    continue;
                                }

                                if (!closed.Contains(checkPosition))
                                {
                                    searching.Enqueue(checkPosition);
                                    closed.Add(checkPosition);
                                }
                            }
                        }
                    }

                    towerLayers.Add(layer);

                    float currentY = 0;

                    for (int i = towerLayers.Count - 1; i >= 0; i--)
                    {
                        int currentLayerCount = towerLayers[i].Count;
                        float middleIndex = currentLayerCount / 2 - ((currentLayerCount % 2 == 0) ? 0.5f : 0);

                        for (int j = 0; j < currentLayerCount; j++)
                        {
                            float x = (j - middleIndex) * distanceBetweenSpawn;

                            towerLayers[i][j].transform.DOComplete();
                            towerLayers[i][j].transform.DOLocalMove(new Vector3(x, currentY, 0), timePerLayer / 2);
                        }

                        currentY += towerLayerHeight;
                    }

                    yield return waitBuildup;
                }
            }
        }
    }
}
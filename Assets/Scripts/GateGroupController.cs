using System;
using System.Collections.Generic;
using UnityEngine;

namespace CountMasterClone
{
    public class GateGroupController: MonoBehaviour
    {
        public void DisableGates()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                BoxCollider collider = transform.GetChild(i).GetComponent<BoxCollider>();

                if (collider != null)
                {
                    collider.enabled = false;
                }
            }
        }

        public int GetMaxPossibleCloneAfterPassing(int currentCloneCount)
        {
            int maxCount = 0;

            for (int i = 0; i < transform.childCount; i++)
            {
                GateController info = transform.GetChild(i).GetComponent<GateController>();
                switch (info.GateType)
                {
                    case GateType.Add:
                        maxCount = Mathf.Max(maxCount, currentCloneCount + info.Value);
                        break;

                    case GateType.Multiplication:
                        maxCount = Mathf.Max(maxCount, currentCloneCount * info.Value);
                        break;
                }
            }

            return maxCount;
        }

        public void Initialize(int addUnitMax, int multiplierMax, bool leastOneAdd = false, int addUnitMin = -1, int mulSpawnRate = 30)
        {
            List<Tuple<GateType, int>> generated = new();

            for (int i = 0; i < transform.childCount; i++)
            {
                GateController info = transform.GetChild(i).GetComponent<GateController>();
                if (info != null)
                {
                    GateType type;
                    int value = 0;

                    while (true)
                    {
                        type = (((i == 0) && leastOneAdd) || (multiplierMax < 0)) ? GateType.Add : ((UnityEngine.Random.Range(0, 101) < mulSpawnRate) ? GateType.Multiplication : GateType.Add);

                        switch (type)
                        {
                            case GateType.Add:
                                {
                                    value = UnityEngine.Random.Range(addUnitMin > 0 ? addUnitMin : 1, addUnitMax + 1) * 5;
                                    break;
                                }

                            case GateType.Multiplication:
                                {
                                    value = UnityEngine.Random.Range(2, multiplierMax + 1);
                                    break;
                                }
                        }

                        var entry = new Tuple<GateType, int>(type, value);

                        if (generated.Contains(entry))
                        {
                            continue;
                        }
                        else
                        {
                            generated.Add(entry);
                            break;
                        }
                    }

                    info.Initialize(type, value);
                }
            }
        }
    };
}
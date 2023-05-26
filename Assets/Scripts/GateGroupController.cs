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

        private void Start()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                GateController info = transform.GetChild(i).GetComponent<GateController>();
                if (info != null)
                {
                    GateType type = (GateType)Random.Range(0, 2);
                    int value = 0;

                    switch (type)
                    {
                        case GateType.Add:
                            {
                                value = Random.Range(1, 21) * 5;
                                break;
                            }

                        case GateType.Multiplication:
                            {
                                value = Random.Range(2, 6);
                                break;
                            }
                    }

                    info.Initialize(type, value);
                }
            }
        }
    };
}
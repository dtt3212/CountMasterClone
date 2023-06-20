using System.Collections.Generic;
using System.IO;
using UniRx;
using UnityEngine;

namespace CountMasterClone
{
    [CreateAssetMenu(fileName = "ValuableState", menuName = "ScriptableObjects/ValuableState")]
    public class ValuableState : SerializableState
    {
        public int money = 350;

        public ReactiveProperty<int> activeStickman = new ReactiveProperty<int>(0);

        public List<int> ownedStickmans = new();

        public event System.Action<int> ActiveStickmanChanged;

        protected override string SaveName => "valuableState.json";

        public bool Purchase(int cost)
        {
            if (money < cost)
            {
                return false;
            }

            money -= cost;
            return true;
        }
    }
}
using System.Collections.Generic;
using System.IO;

using UnityEngine;

namespace CountMasterClone
{
    [CreateAssetMenu(fileName = "PlayState", menuName = "ScriptableObjects/PlayState")]
    public class PlayState : SerializableState
    {
        public int level = 1;

        protected override string SaveName => "PlayState.json";
    }
}
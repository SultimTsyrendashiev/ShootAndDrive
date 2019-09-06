using UnityEngine;

namespace SD.Background
{
    [CreateAssetMenu(menuName = "Background Data", order = 51)]
    class BackgroundData : ScriptableObject
    {
        [SerializeField]
        string[] blockNames;

        [SerializeField]
        string cutsceneBlockName;

        public string[] Blocks => blockNames;

        public string CutsceneBlockName => cutsceneBlockName;
    }
}

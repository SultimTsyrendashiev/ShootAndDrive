using UnityEngine;

namespace SD.Background
{
    [CreateAssetMenu(menuName = "Background Data", order = 51)]
    class BackgroundData : ScriptableObject
    {
        [SerializeField]
        string[] blockNames;

        public string[] Blocks => blockNames;
    }
}

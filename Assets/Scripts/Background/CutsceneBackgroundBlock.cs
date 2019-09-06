using UnityEngine;

namespace SD.Background
{
    class CutsceneBackgroundBlock : BackgroundBlock
    {
        public override int AmountInPool => 1;
        public override BackgroundBlockType BlockType => BackgroundBlockType.Cutscene;
    }
}

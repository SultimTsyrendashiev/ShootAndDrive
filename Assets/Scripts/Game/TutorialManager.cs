using System;
using UnityEngine;

namespace SD.Game
{
    class TutorialManager : MonoBehaviour
    {
        public void StartTutorial(Action onTutorialEnd)
        {
            Debug.Log("Tutorial invoked", this);

            // TODO

            onTutorialEnd?.Invoke();
        }
    }
}

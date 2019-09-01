using UnityEngine;
using UnityEngine.EventSystems;

namespace SD.UI.Controls
{
    class ButtonAudio : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        AudioClip audioClip;

        AudioSource audioSource;

        void Start()
        {
            audioSource = GetComponentInParent<AudioSource>();

            if (!audioSource.ignoreListenerPause)
            {
                audioSource.ignoreListenerPause = true;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (audioSource != null && audioClip != null)
            {
                audioSource.PlayOneShot(audioClip);
            }
        }
    }
}

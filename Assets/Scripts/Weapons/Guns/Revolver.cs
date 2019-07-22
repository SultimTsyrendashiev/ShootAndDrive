using System.Collections;
using UnityEngine;

namespace SD.Weapons
{
    /// <summary>
    /// There is a small delay before hitscan
    /// </summary>
    class Revolver : Gun
    {
        [SerializeField]
        float triggerTime = 0.083f;

        protected override void PrimaryAttack()
        {
            StartCoroutine(WaitForTrigger());
        }

        IEnumerator WaitForTrigger()
        {
            yield return new WaitForSeconds(triggerTime);
            base.PrimaryAttack();
        }
    }
}

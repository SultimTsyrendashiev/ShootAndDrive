using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Pivot for a hand that holds a weapon
/// </summary>
public class HandPivot : MonoBehaviour
{
    static Transform hand;
    static Animation handAnim;

    bool    poseExist;
    string  poseAnimName;

    /// <summary>
    /// Set animation name for hand pose
    /// </summary>
    public void Init(string animName)
    {
        poseAnimName = animName;
        poseExist = false;

        if (hand == null && handAnim == null)
        {
            hand = HandsController.Instance.RightHand;
            handAnim = hand.GetComponentInChildren<Animation>();
        }

        foreach (AnimationState a in handAnim)
        {
            if (a.name == poseAnimName)
            {
                poseExist = true;
                return;
            }
        }
    }

    /// <summary>
    /// Should be called on weapon enable
    /// </summary>
    public void PoseHand()
    {
        HandsController.Instance.RenderRightHand = poseExist;
        if (!poseExist)
        {
            return;
        }

        // set hand's parent
        // so hand now is attached to a weapon
        hand.SetParent(transform, true);

        // play animation to place hand on weapon
        handAnim.Play(poseAnimName);
    }
}

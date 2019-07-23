using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SD.Enemies
{
    //[RequireComponent(typeof(Rigidbody))]
    class VanDoor : MonoBehaviour
    {
        EnemyVan van;
        Animation doorAnim;
        Quaternion startLocalRotation;

        public void Init(EnemyVan van)
        {
            this.van = van;

            doorAnim = GetComponent<Animation>();
            startLocalRotation = transform.localRotation;
        }

        public void Open()
        {
            doorAnim.Play();
        }

        public void Close()
        {
            transform.localRotation = startLocalRotation;
        }

        //[SerializeField]
        //float       openForce = 5.0f;

        //[SerializeField]
        //int         coordinate; // what component of euler angles to use
        //[SerializeField]
        //Vector2     bounds;

        //Quaternion localStartRotation;
        //Rigidbody rb;

        //bool isOpened;

        //public void Init()
        //{
        //    isOpened = false;

        //    rb = GetComponent<Rigidbody>();
        //    rb.isKinematic = true;

        //    localStartRotation = transform.localRotation;
        //}

        //public void Open()
        //{
        //    // if opened, or already openning
        //    if (isOpened || rb.isKinematic == false)
        //    {
        //        return;
        //    }

        //    rb.isKinematic = false;

        //    rb.AddTorque(openForce * transform.up, ForceMode.Impulse);

        //    // wait for opening and only then constraint rotation
        //    StartCoroutine(WaitForOpening());
        //}

        //IEnumerator WaitForOpening()
        //{
        //    while (true)
        //    {
        //        Vector3 euler = transform.localEulerAngles;

        //        if (euler[coordinate] < bounds[0] || euler[coordinate] > bounds[1])
        //        {
        //            isOpened = true; // now door's rotation will be clamped
        //            yield break;
        //        }

        //        yield return new WaitForFixedUpdate();
        //    }
        //}

        //public void Close()
        //{
        //    if (!isOpened)
        //    {
        //        return;
        //    }

        //    isOpened = false;
        //    rb.isKinematic = true;
        //    transform.localRotation = localStartRotation;
        //}

        //void FixedUpdate()
        //{
        //    // constraint rotation
        //    if (isOpened)
        //    {
        //        Vector3 euler = transform.localEulerAngles;

        //        if (euler[coordinate] < bounds[0])
        //        {
        //            euler[coordinate] = bounds[0];
        //            transform.localEulerAngles = euler;
        //        }
        //        else if (euler[coordinate] > bounds[1])
        //        {
        //            euler[coordinate] = bounds[1];
        //            transform.localEulerAngles = euler;
        //        }
        //    }
        //}
    }
}

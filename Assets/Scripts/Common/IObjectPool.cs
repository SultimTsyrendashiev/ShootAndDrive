using UnityEngine;

namespace SD
{
    interface IObjectPool
    {
        GameObject GetObject(string name);
        GameObject GetObject(string name, Vector3 position);
        GameObject GetObject(string name, Vector3 position, Vector3 direction);
        GameObject GetObject(string name, Vector3 position, Quaternion rotation);
    }
}

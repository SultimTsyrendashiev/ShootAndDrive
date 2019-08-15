using System.Collections.Generic;
using UnityEngine;

namespace SD.Background
{
    /// <summary>
    /// Creates and deletes background blocks, based on camera position.
    /// Assume, that camera moves only in FORWARD direction (0,0,1)
    /// </summary>
    class BackgroundController : MonoBehaviour, IBackgroundController
    {
        /// <summary>
        /// Blocks must be to this distance
        /// </summary>
        [SerializeField]
        float           desiredDistance = 300.0f;

        /// <summary>
        /// Names of block prefabs in object pool
        /// </summary>
        [SerializeField]
        BackgroundData  data;

        string[]        blockPrefabs;

        /// <summary>
        /// Holds all active blocks in current scene
        /// </summary>
        LinkedList<IBackgroundBlock> blocks;

        /// <summary>
        /// Camera to track
        /// </summary>
        Transform       target;

        /// <summary>
        /// Length of all active blocks
        /// Must be >= 'distance'
        /// </summary>
        public float    CurrentLength { get; private set; }


        public void Awake()
        {
            blockPrefabs = data.Blocks;

            Debug.Assert(blockPrefabs.Length > 0, "Block prefabs amount must be > 0", this);

            blocks = new LinkedList<IBackgroundBlock>();
            CurrentLength = 0.0f;

            // delete all child block components in this scene
            BackgroundBlock[] inScene = GetComponentsInChildren<BackgroundBlock>();
            foreach (var b in inScene)
            {
                Destroy(b.gameObject);
            }
        }

        public void Reinit()
        {

        }

        #region creating blocks
        /// <summary>
        /// Creates block at the end of the last one
        /// </summary>
        void CreateBlock()
        {
            int index = GetNextBlockIndex();

            GameObject newBlockObj = ObjectPool.Instance.GetObject(blockPrefabs[index]);

            IBackgroundBlock newBlock = newBlockObj.GetComponent<IBackgroundBlock>();
            Debug.Assert(newBlock != null, "Block must contain 'IBackgroundBlock' component", newBlockObj);

            if (blocks.Count > 0)
            {
                IBackgroundBlock last = blocks.Last.Value;

                Vector3 newPosition = last.Center;
                newPosition.z += (newBlock.Length + last.Length) / 2;
                newBlockObj.transform.position = newPosition;
            }
            else
            {
                newBlockObj.transform.position = Vector3.zero;
            }

            CurrentLength += newBlock.Length;
            blocks.AddLast(newBlock);
        }

        void DeleteOldestBlock()
        {
            // first is the oldest
            IBackgroundBlock first = blocks.First.Value;

            // return to pool
            first.CurrentObject.SetActive(false);
            CurrentLength -= first.Length;
            
            blocks.RemoveFirst();
        }

        /// <summary>
        /// Get next block type according to previous ones
        /// </summary>
        int GetNextBlockIndex()
        {
            // temporary, random
            return Random.Range(0, blockPrefabs.Length);
        }
        #endregion

        public Vector2 GetBlockBounds(Vector3 position)
        {
            // 'position' is often player's position,
            // so  it's more possible that
            // needed block is in the beginning of the list

            LinkedListNode<IBackgroundBlock> current = blocks.First;

            if (current == null)
            {
                return Vector2.zero;
            }

            // iterate through list
            while (!current.Value.Contains(position))
            {
                if (current.Next == null)
                {
                    Debug.Log("Not enough blocks to check current", this);
                    break;
                }

                current = current.Next;
            }

            return current.Value.GetHorizontalBounds();
        }

        public bool IsOut(Vector3 min, Vector3 max)
        {
            // as blocks are aligned to z axis,
            // then just check it
            return max.z < blocks.First.Value.GetMinZ();
        }

        #region updating camera
        void Update()
        {
            if (target != null)
            {
                UpdateCameraPosition(target.position);
            }
        }

        public void SetTarget(Transform target)
        {
            this.target = target;
        }

        public void UpdateCameraPosition(Vector3 cameraPosition)
        {
            // create if needed
            while (CurrentLength < desiredDistance)
            {
                CreateBlock();
            }

            // delete invisible blocks for camera;
            // first is the oldest;
            // assume, that all blocks are created along z axis
            while (!blocks.First.Value.Contains(cameraPosition))
            {
                DeleteOldestBlock();
            }
        }
        #endregion
    }
}

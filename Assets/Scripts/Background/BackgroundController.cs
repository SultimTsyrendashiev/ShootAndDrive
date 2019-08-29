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
            DeleteAll();
        }

        #region creating blocks
        /// <summary>
        /// Creates block at undefined position
        /// </summary>
        IBackgroundBlock CreateBlock()
        {
            int index = GetNextBlockIndex();

            GameObject newBlockObj = ObjectPool.Instance.GetObject(blockPrefabs[index]);

            IBackgroundBlock newBlock = newBlockObj.GetComponent<IBackgroundBlock>();
            Debug.Assert(newBlock != null, "Block must contain 'IBackgroundBlock' component", newBlockObj);

            //if (blocks.Count > 0)
            //{
            //    IBackgroundBlock last = blocks.Last.Value;

            //    Vector3 newPosition = last.Center;
            //    newPosition.z += (newBlock.Length + last.Length) / 2;
            //    newBlockObj.transform.position = newPosition;
            //}
            //else
            //{
            //    newBlockObj.transform.position = target.position;
            //}

            CurrentLength += newBlock.Length;
            blocks.AddLast(newBlock);

            return newBlock;
        }

        void DeleteBlock(LinkedListNode<IBackgroundBlock> node)
        {
            // first is the oldest
            IBackgroundBlock first = node.Value;

            // return to pool
            first.CurrentObject.SetActive(false);
            CurrentLength -= first.Length;

            blocks.Remove(node);
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
                UpdateTargetPosition(target.position);
            }
        }

        public void SetTarget(Transform target)
        {
            this.target = target;
        }

        public void UpdateTargetPosition(Vector3 targetPosition)
        {
            DeleteBeginning();
            DeleteEnding();

            AddBeginning();
            AddEnding();
        }

        /// <summary>
        /// Delete blocks behind target
        /// </summary>
        void DeleteBeginning()
        {
            if (blocks.Count == 0)
            {
                return;
            }

            float targetz = target.position.z;

            // scan from the beginning
            while (blocks.First.Value.GetMaxZ() < targetz)
            {
                DeleteBlock(blocks.First);

                if (blocks.Count == 0)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Delete blocks that are too far from target
        /// </summary>
        void DeleteEnding()
        {
            if (blocks.Count == 0)
            {
                return;
            }

            float farTargetZ = target.position.z + desiredDistance;

            // scan from the end
            while (blocks.Last.Value.GetMinZ() > farTargetZ)
            {
                DeleteBlock(blocks.Last);

                if (blocks.Count == 0)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Remove all blocks
        /// </summary>
        void DeleteAll()
        {
            while (blocks.Count != 0)
            {
                DeleteBlock(blocks.First);
            }
        }

        /// <summary>
        /// If there are no blocks, adds block to the begginning
        /// </summary>
        void AddBeginning()
        {
            if (blocks.Count == 0)
            {
                // if there are no blocks, add around target
                var b = CreateBlock();
                b.Center = target.position;

                blocks.AddFirst(b);
            }
        }

        /// <summary>
        /// Add blocks up to desired distance
        /// </summary>
        void AddEnding()
        {
            if (blocks.Count == 0)
            {
                Debug.Log("'AddBeginning' must be called before 'AddEnding'", this);
                return;
            }

            float desiredZ = target.position.z + desiredDistance;

            while (blocks.Last.Value.GetMaxZ() < desiredZ)
            {
                var last = blocks.Last.Value;

                var b = CreateBlock();
                b.Center = (last.GetMaxZ() + b.Length / 2) * Vector3.forward;

                blocks.AddLast(b);
            }
        }
        #endregion
    }
}

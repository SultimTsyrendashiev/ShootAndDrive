using System.Collections.Generic;
using UnityEngine;

namespace SD.Background
{
    class BackgroundController : MonoBehaviour, IBackgroundController
    {
        /// <summary>
        /// Blocks must be to this distance
        /// </summary>
        [SerializeField]
        float distance = 150.0f;

        /// <summary>
        /// Names of block prefabs in object pool
        /// </summary>
        [SerializeField]
        string[] blockPrefabs;
        
        // holds all active blocks in current scene
        LinkedList<IBackgroundBlock> blocks;
        // length of all blocks
        // must be >= 'distance'
        float currentLength;

        static IBackgroundController instance;
        public static IBackgroundController Instance => instance;

        void Awake()
        {
            Debug.Assert(instance == null, "Several singletons", this);
            instance = this;
        }

        void Start()
        {
            Debug.Assert(blockPrefabs.Length > 0, "Not enough block prefabs", this);

            blocks = new LinkedList<IBackgroundBlock>();
            currentLength = 0.0f;

            // delete all child block objects in this scene
            BackgroundBlock[] inScene = GetComponentsInChildren<BackgroundBlock>();
            foreach (var b in inScene)
            {
                Destroy(b.gameObject);
            }

            // create one for init
            CreateBlock();
        }

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

            currentLength += newBlock.Length;
            blocks.AddLast(newBlock);
        }

        void DeleteOldestBlock()
        {
            // first is the oldest
            IBackgroundBlock first = blocks.First.Value;

            // return to pool
            first.CurrentObject.SetActive(false);
            currentLength -= first.Length;
            
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

        public Vector2 GetCurrentBounds(Vector3 position)
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

        public void UpdateCameraPosition(Vector3 cameraPosition)
        {
            // create if needed
            while (currentLength < distance)
            {
                CreateBlock();
            }

            // delete invisible blocks for camera;
            // first is oldest;
            // assume, that all blocks are created along z axis
            while (!blocks.First.Value.Contains(cameraPosition))
            {
                DeleteOldestBlock();
            }
        }
    }
}

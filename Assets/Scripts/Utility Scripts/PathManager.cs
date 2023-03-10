using System;
using System.Collections.Generic;
using UnityEngine;
using Utility_Scripts.Grid;

namespace Utility_Scripts
{
    public class PathManager : MonoBehaviour
    {
        private enum Direction
        {
            Up,
            Down,
            Right,
            Forward,
            Left,
            Backward
        };

        public Enum[] DirectionBias;
        public List<Transform> pathNodes;
        [SerializeField] private GameObject pathStart;
        [SerializeField] private bool reverseDirection;
        private GameObject _pathEnd;

        // Typing will change when enemies are added
        private List<GameObject> _enemiesOnPath;

        private void Awake()
        {
            /*DirectionBias = (Enum[]) Enum.GetValues(typeof(Direction));
            foreach (Direction d in DirectionBias)
            {
                Debug.Log(d);
            }
            
            pathNodes.Add(pathStart.transform);
            var pathFound = true;

            var currPosition = pathStart.transform.position;

            while (pathFound)
            {
                pathFound = false;

                var cardinalRight = currPosition;
                cardinalRight.x += 1;
                if (GridManager.Instance.CheckPosition(cardinalRight, 0).CompareTag("Path"))
                {

                }

                var cardinalUp = currPosition;
                cardinalUp.z += 1;
                if (GridManager.Instance.CheckPosition(cardinalUp, 0).CompareTag("Path"))
                {
                    
                }
                
                var cardinalLeft = currPosition;
                cardinalLeft.x -= 1;
                if (GridManager.Instance.CheckPosition(cardinalLeft, 0).CompareTag("Path"))
                {
                    
                }

                var cardinalDown = currPosition;
                cardinalDown.z -= 1;
                if (GridManager.Instance.CheckPosition(cardinalDown, 0).CompareTag("Path"))
                {
                    
                }
            }
            var startNode = pathStart.GetComponent<PathNode>();
              
            var currNode = startNode;
            while (currNode.nextNode != null)
            {
                pathNodes.Add(currNode.nextNode.transform);
                currNode = currNode.nextNode.GetComponent<PathNode>();
            }

            _pathEnd = currNode.gameObject;
        }*/
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}

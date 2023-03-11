using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility_Scripts.Grid;

namespace Utility_Scripts
{
    public class PathManager : MonoBehaviour
    {
        public static PathManager Instance { get; private set; }

        // The variables serve to hold the current levels pertinent path information
        [SerializeField] private GameObject pathStart;
        private GameObject _pathEnd;
        public List<Vector3> pathVectors;
        
        // Use a dictionary for efficient, O(1) look ups while building the vector array
        // and a boolean for situations when we come back on an old tile
        private Dictionary<Vector3, int> _pathNodes;
        private bool _alreadyTraversed;

        // All the enemies currently on the path
        private List<GameObject> _enemiesOnPath;
        
        // The directions we check for our next path block. The order is used to make
        // path generation deterministic and is exposed to allow per level altering.
        private enum Direction { East, North, West, South }

        private enum Height { Up, Down, Equal}

        [SerializeField] private string[] directionBias = {"East", "North", "West", "South"};
        [SerializeField] private string[] heightBias = {"Up", "Down", "Equal"};
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else 
                Instance = this;
            
            _pathNodes = new Dictionary<Vector3, int>();
            
            GeneratePath();
        }

        private void GeneratePath()
        {
            // The first node (pathStart) is given to us
            var pathFound = true;
            var pathIndex = 1;
            var currPosition = pathStart.transform.position;
            _pathNodes.Add(currPosition, pathIndex);
            pathVectors.Add(currPosition);
            
            while (pathFound)
            {
                // Reset path found and re-instantiate the visited list every iteration
                pathFound = false;
                var alreadyVisited = new List<Vector3>();

                foreach (var height in heightBias)
                {
                    foreach (var direction in directionBias)
                    {
                        // Check the current direction for a block tagged "Path"
                        var nextNode = LookForPath(currPosition, direction, height, out pathFound);
                        if (!pathFound) continue;
                    
                        // If we find one, add its position to the dictionary if we haven't seen it yet. If we
                        // have seen it already, hold onto it until every direction is checked for a fresh one
                        var nextPosition = nextNode.transform.position;
                        if (_pathNodes.ContainsKey(nextPosition))
                        {
                            _alreadyTraversed = true;
                        
                            alreadyVisited.Add(nextPosition);
                            pathFound = false;
                        }
                        else
                        {
                            _alreadyTraversed = false;
                        
                            pathIndex++;
                            nextPosition = nextNode.transform.position;
                            currPosition = nextPosition;
                        
                            _pathNodes.Add(nextPosition, pathIndex);
                            pathVectors.Add(nextPosition);
                            break;
                        }
                    }
                    if (pathFound) break;
                }

                // If we made it here and multiple were already traversed, we have to determine which to
                // move to. If only one was traversed, it means we're at the end since that's the only
                // block other than the start that only touches one other block
                if (!_alreadyTraversed) continue;
                if (alreadyVisited.Count == 1) break;

                _alreadyTraversed = false;
                
                // Determine the block that has been visited the least recently
                // This one ensures we never come back where we came from or get stuck
                var leastRecentPosition = new Vector3();
                var blocksSinceLastVisit = 0;
                foreach (var vector in alreadyVisited)
                {
                    if (!_pathNodes.TryGetValue(vector, out var index)) continue;
                    if (pathIndex - index <= blocksSinceLastVisit) continue;
                    
                    leastRecentPosition = vector;
                    blocksSinceLastVisit = pathIndex - index;
                }
                
                // Add it to the vector list, but only 'update' the dictionary to reflect how
                // recently we've visited this particular location
                pathIndex++;
                pathFound = true;
                _pathNodes[leastRecentPosition] = pathIndex;
                pathVectors.Add(leastRecentPosition);
                currPosition = leastRecentPosition;

            }
            // We always break out of the loop at the end block, store it for safe keeping
            _pathEnd = GridManager.Instance.GetCellInColumn(currPosition, 0);
        }
        
        // Check the given direction for a block and return whether or not it is a block or a path
        private GameObject LookForPath(Vector3 currPosition, string direction, string height, out bool pathFound)
        {
            var directionPosition = currPosition;
            switch (direction)
            {
                // Up and down case will need to be reworked to work properly.
                // Currently thinking a diagonal transition will be required.
                case "East":
                {
                    directionPosition.x += 1;
                    break;
                }
                case "North":
                {
                    directionPosition.z += 1;
                    break;
                }
                case "West":
                {
                    directionPosition.x -= 1;
                    break;
                }
                case "South":
                {
                    directionPosition.z -= 1;
                    break;
                }
            }

            switch (height)
            {
                case "Up":
                {
                    directionPosition.y += 1;
                    break;
                }
                case "Down":
                {
                    directionPosition.y -= 1;
                    break;
                }
                case "Equal":
                {
                    directionPosition.y += 0;
                    break;
                }
            }
            
            var nextNode = GridManager.Instance.GetCellInColumn(directionPosition, 0);
            if (nextNode)
            {
                if (nextNode.CompareTag("Path"))
                {
                    pathFound = true;
                    return nextNode;
                }
            }

            pathFound = false;
            return null;
        }
    }
}

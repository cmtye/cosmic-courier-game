using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Utility_Scripts.Grid
{
    public class GridManager : MonoBehaviour
    {
        // Minimum and maximum heights of the tile maps attached to this manager
        private float _minTileY;
        private float _maxTileY;

        private static GridManager _instance;
        
        public static GridManager Instance => _instance;

        [SerializeField] private bool generateDownwards;
        [SerializeField] [InspectorButton(nameof(OnButtonClicked))] private string generateNewTileLayer;
        [SerializeField] private Tilemap[] tileMaps;
        private Dictionary<int, TileLayer> _tileLayers;

        private void Awake()
        {
            if (_instance != null && _instance != this)
                Destroy(gameObject);
            else 
                _instance = this;
            
            // Make sure our tile maps are in order in the case of developer error
            tileMaps = GetComponentsInChildren<Tilemap>();
            Array.Sort(tileMaps, YLevelComparison);

            // Iterate through the tile maps at start to add any level editor added blocks to
            // their respective tile layer dictionary
            _tileLayers = new Dictionary<int, TileLayer>();
            foreach (var tileMap in tileMaps)
            {
                var tileMapHeight = Vector3Int.FloorToInt(tileMap.gameObject.transform.position).y;

                var grid = new Dictionary<Vector2, GameObject>();
                foreach (Transform child in tileMap.transform)
                {
                    var childPosition = child.position;
                    var gridPosition = new Vector2(childPosition.x, childPosition.z);
                    grid.Add(gridPosition, child.gameObject);
                }

                var tileLayer = new TileLayer(grid);
                _tileLayers.Add(tileMapHeight, tileLayer);
            }
        }

        // Try and place a given GameObject on the grid. The tile maps don't actually manage
        // the GameObjects in their cells (thanks Unity), so we add them to tile layer dictionary's
        public bool TryPlaceObject(GameObject toPlace, Vector3 worldPosition)
        {
            // If there is a block above the block we're trying to place onto, we can't
            // place there. This is redundancy as the selector handles this already.
            if (FindBlockAbove(worldPosition)) return false;
            
            // The instantiation position is Vector3 version of target position
            var instantiatePosition = worldPosition;
            instantiatePosition.y += 1;
            
            var targetPosition = new Vector2(worldPosition.x, worldPosition.z);
            var targetHeight = Vector3Int.FloorToInt(worldPosition).y + 2;

            // Return false if there is no layer to hold this height. This is also redundancy
            // since the FindBlockAbove function already generates one when needed
            if (!_tileLayers.TryGetValue(targetHeight, out var layer)) return false;

            // Instantiate our object at the appropriate position. Finds the tile map this object should belong
            // to so it can add it as a child (ensures transform correctness).
            // Finally, adds the GameObject and its position it to the tile layer dictionary
            var placedObject = Instantiate(toPlace, instantiatePosition, toPlace.transform.rotation);
            foreach (var t in tileMaps)
            {
                if ((int) t.transform.position.y == targetHeight)
                {
                    placedObject.transform.SetParent(t.transform);
                }
            }
            layer.AddTile(targetPosition, placedObject);
            return true;
        }

        // Calculates if there is a block above the one at the given position
        public GameObject FindBlockAbove(Vector3 worldPosition)
        {
            var targetedTileLayer = Vector3Int.FloorToInt(worldPosition).y + 1;
            var aboveTileLayer = targetedTileLayer + 1;
            if (_tileLayers.TryGetValue(aboveTileLayer, out var layer))
            {
                var targetPosition = new Vector2(worldPosition.x, worldPosition.z);
                var target = layer.GetTile(targetPosition);

                return target ? target : null;
            }
            // There is no layer above, generate a new one just in case
            GenerateNewLayer(false, false);
            return null;
        }
        
        // Generates a new tilemap and layer. Can be called in editor or during runtime if a new
        // layer is required to place a structure during gameplay
        private void GenerateNewLayer(bool downwards, bool inEditor)
        {
            _maxTileY = 0;
            _minTileY = 0;
            tileMaps = GetComponentsInChildren<Tilemap>();
            Array.Sort(tileMaps, YLevelComparison);
            var emptyGrid = new Dictionary<Vector2, GameObject>();
            var tileLayer = new TileLayer(emptyGrid);
            
            float newHeight;
            if (!downwards)
                newHeight = _maxTileY + 1;
            else
                newHeight = _minTileY - 1;

            if (tileMaps.Length == 0)
            {
                var newLayer = new GameObject("Tilemap (0)");
                newLayer.AddComponent<Tilemap>();
                newLayer.AddComponent<CombineMesh>();
                newLayer.transform.SetParent(transform);
            }
            else
            {
                var newLayer = new GameObject("Tilemap (" + newHeight + ")");
                newLayer.AddComponent<Tilemap>();
                newLayer.AddComponent<CombineMesh>();
                newLayer.transform.Translate(0, newHeight, 0);
                newLayer.transform.SetParent(transform);
                _tileLayers.Add((int) newHeight, tileLayer);
            }
            
            tileMaps = GetComponentsInChildren<Tilemap>();
            Array.Sort(tileMaps, YLevelComparison);

            if (inEditor) ResetHierarchyOrder();
        }

        // Reorders hierarchy so that lowest tile layer is first
        private void ResetHierarchyOrder()
        {
            for (var i = 0; i < tileMaps.Length - 1; i++)
                tileMaps[i].transform.SetSiblingIndex(i);
        }
        
        // Compares the y levels of two given objects. Used to sort logic arrays from lowest to highest
        private int YLevelComparison(Tilemap a, Tilemap b)
        {
            if (a == null) return b == null ? 0 : -1;
            if (b == null) return 1;
 
            var yA = a.transform.position.y;
            var yB = b.transform.position.y;
            
            if (_maxTileY < yA || _maxTileY < yB)
                _maxTileY = yA > yB ? yA : yB;
            
            if (_minTileY > yA || _minTileY > yB)
                _minTileY = yA < yB ? yA : yB;
            
            return yA.CompareTo(yB);
        }
        
        // Called when the inspector button is pressed. It generates a new tile layer with the
        // corresponding Y value depending on which direction it is generated. It sorts both the
        // internal array as well as the editors hierarchy
        private void OnButtonClicked()
        { 
            GenerateNewLayer(generateDownwards, true);
        }
    }
}

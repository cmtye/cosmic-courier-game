using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Utility_Scripts.Grid
{
    public class GridManager : MonoBehaviour
    {
        private float _minTileY;
        private float _maxTileY;

        private static GridManager _instance;
        
        public static GridManager Instance => _instance;

        [SerializeField] private bool generateDownwards;
        [SerializeField] [InspectorButton(nameof(OnButtonClicked))] private string generateNewTileLayer;
        [SerializeField] private Tilemap[] tileLayers;

        private void Awake()
        {
            if (_instance != null && _instance != this)
                Destroy(gameObject);
            else 
                _instance = this;
            
            tileLayers = GetComponentsInChildren<Tilemap>();
            Array.Sort(tileLayers, YLevelComparison);
        }

        // Try and place a given GameObject on the grid. The tile layers don't actually manage
        // the GameObjects in their cells (thanks Unity), but we still make an effort to sort
        // each object into it's respective layer. This many come in handy later with more
        // inclusion of vertical maps or upgrading turrets
        public bool TryPlaceObject(GameObject toPlace, Vector3 worldPosition)
        {
            // Target position is on top of the selected object, so one unit up on the Y
            var targetPosition = new Vector3(worldPosition.x, worldPosition.y + 1f, worldPosition.z);
            foreach (var t in tileLayers)
            {
                // If the floor of the target's Y plus one is equal to the tiles layer's Y, the new object
                // will be placed as a child of that layer
                if (Math.Abs(t.transform.position.y - (Vector3Int.FloorToInt(targetPosition).y + 1)) < 0.0001)
                {
                    // If any child on that layer shares the target position, the "cell" is taken
                    // This call may be expensive and unnecessary since each layer can be very large
                    if (t.transform.Cast<Transform>().Any(child => child.position == targetPosition))
                        return false;

                    var layerExists = Instantiate(toPlace, targetPosition, toPlace.transform.rotation);
                    layerExists.transform.SetParent(t.transform);
                    return true;
                }
            }
            // If a tile layer wasn't found, it means we've tried placing on the top most
            // layer since we know that we can't place on the underside of a layer.
            // We'll generate a new layer and assign the new object to it
            GenerateNewLayer(false, false);
            var layerDoesntExist = Instantiate(toPlace, targetPosition, toPlace.transform.rotation);
            layerDoesntExist.transform.SetParent(tileLayers[^1].transform);
            return true;
        }

        // Generates a new tilemap layer. Can be called in editor or during runtime if a new
        // layer is required to place an object during gameplay
        private void GenerateNewLayer(bool downwards, bool inEditor)
        {
            _maxTileY = 0;
            _minTileY = 0;
            tileLayers = GetComponentsInChildren<Tilemap>();
            Array.Sort(tileLayers, YLevelComparison);

            if (tileLayers.Length == 0)
            {
                var newLayer = new GameObject("Tilemap (0)");
                newLayer.AddComponent<Tilemap>();
                newLayer.AddComponent<CombineMesh>();
                newLayer.transform.SetParent(transform);
            }
            else if (!downwards)
            {
                var newLayer = new GameObject("Tilemap (" + (_maxTileY + 1) + ")");
                newLayer.AddComponent<Tilemap>();
                newLayer.AddComponent<CombineMesh>();
                newLayer.transform.Translate(0, _maxTileY + 1, 0);
                newLayer.transform.SetParent(transform);
            }
            else
            {
                var newLayer = new GameObject("Tilemap (" + (_minTileY - 1) + ")");
                newLayer.AddComponent<Tilemap>();
                newLayer.AddComponent<CombineMesh>();
                newLayer.transform.Translate(0, _minTileY - 1, 0);
                newLayer.transform.SetParent(transform);
            }
            
            tileLayers = GetComponentsInChildren<Tilemap>();
            Array.Sort(tileLayers, YLevelComparison);
            
            if (inEditor) ResetHierarchyOrder();
        }
        
        // Called when the inspector button is pressed. It generates a new tile layer with the
        // corresponding Y value depending on which direction it is generated. It sorts both the
        // internal array as well as the editors hierarchy
        private void OnButtonClicked()
        { 
            GenerateNewLayer(generateDownwards, true);
        }

        // Reorders hierarchy so that lowest tile layer is first
        private void ResetHierarchyOrder()
        {
            for (var i = 0; i < tileLayers.Length - 1; i++)
                tileLayers[i].transform.SetSiblingIndex(i);
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
    }
}

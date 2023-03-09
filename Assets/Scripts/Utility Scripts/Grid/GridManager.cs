using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Utility_Scripts.Grid
{
    public class GridManager : MonoBehaviour
    {
        private UnityEngine.Grid _levelGrid;
        private float _minTileY;
        private float _maxTileY;
        
        [SerializeField] private bool generateDownwards;
        [SerializeField] [InspectorButton(nameof(OnButtonClicked))] private string generateNewTileLayer;
        [SerializeField] private Tilemap[] tileLayers;

        private void Awake()
        {
            _levelGrid = GetComponent<UnityEngine.Grid>();
            
            tileLayers = GetComponentsInChildren<Tilemap>();
            Array.Sort(tileLayers, YLevelComparison);
        }

        private void Update()
        {
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

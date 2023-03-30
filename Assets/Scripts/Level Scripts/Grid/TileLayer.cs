using System.Collections.Generic;
using UnityEngine;

namespace Level_Scripts.Grid
{
    public class TileLayer
    {
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private Dictionary<Vector2, GameObject> _tileGrid;

        public TileLayer(Dictionary<Vector2, GameObject> grid)
        {
            _tileGrid = grid;
        }
        
        public void AddTile(Vector2 position, GameObject tileToAdd)
        {
            _tileGrid.Add(position, tileToAdd);
        }

        public GameObject GetTile(Vector2 position)
        {
            return _tileGrid.TryGetValue(position, out var tile) ? tile : null;
        }

        public bool RemoveTile(Vector2 position)
        {
            return _tileGrid.Remove(position);
        }
    }
}

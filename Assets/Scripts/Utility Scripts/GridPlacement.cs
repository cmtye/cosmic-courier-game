using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GridPlacement : MonoBehaviour
{
    // TODO: Temporary, will come back and look for a more efficient solution
    [SerializeField] private float placementDistance;
    [SerializeField] private GameObject temp;
    private GameObject _currHighlighted;
    private Vector3 _prevCell;

    private void Update()
    {
        FindTarget(transform);
    }

    private void FindTarget(Transform playerTransform)
    {
        // Make a vector out in front of the character and slightly downward to get the tile in front of us
        var fwd = playerTransform.TransformDirection(Vector3.forward) * placementDistance;
        fwd.y -= 1;

        // Check all layers for a hit, return if nothing
        Debug.DrawRay(playerTransform.position, fwd, Color.red);
        if (!Physics.Raycast(playerTransform.position, fwd, out var hit, 2, -1))
        {
            // Player doesn't have a cell in front of them, destroy the highlight
            if (_currHighlighted)
                Destroy(_currHighlighted);
            
            return;
        }

        // Raise target position slightly so instantiated object shows above the grid
        var cell = hit.transform.position;
        cell.y += 0.6f;
        if (cell != _prevCell)
        {
            // Move highlight to new cell, or instantiate a new one if no current cell is highlighted
            if (_currHighlighted)
            {
                // TODO: Fix this so its a smooth transition to new tiles instead of instant
                _currHighlighted.transform.position = Vector3.Lerp(_currHighlighted.transform.position, cell, 1f);
            }
            else
            {
                _currHighlighted = Instantiate(temp, cell, Quaternion.identity);
            }
        }

        _prevCell = cell;
    }
}

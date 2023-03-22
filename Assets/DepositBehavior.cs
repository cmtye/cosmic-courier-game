using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepositBehavior : MonoBehaviour
{
    private static readonly int Fill = Shader.PropertyToID("_Fill");
    private MaterialPropertyBlock _materialBlock;
    private MeshRenderer _meshRenderer;
    [SerializeField] private float currentValue = 0f;
    [SerializeField] private float maxValue = 10f;

    private void Awake() 
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _materialBlock = new MaterialPropertyBlock();
    }

    private void Update()
    {
        UpdateParams();
    }

    private void UpdateParams()
    {
        _meshRenderer.GetPropertyBlock(_materialBlock);
        _materialBlock.SetFloat(Fill, 1 - currentValue / maxValue);
        _meshRenderer.SetPropertyBlock(_materialBlock);
    }
}

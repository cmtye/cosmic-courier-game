using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class HeldItemPreview : MonoBehaviour
{
    [SerializeField] private GameObject temp;

    private Texture2D temp2d;
    // Start is called before the first frame update
    void Start()
    {
        temp2d = AssetPreview.GetAssetPreview(temp);
        gameObject.GetComponentInChildren<RawImage>().texture = temp2d;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

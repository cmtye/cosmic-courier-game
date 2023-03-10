using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility_Scripts;

public class FollowVectors : MonoBehaviour
{
    private List<Vector3> path;
    public float MoveSpeed = 8;
    Coroutine MoveIE;
    private void Start()
    {
        path = PathManager.Instance.pathVectors;
        StartCoroutine(moveObject());
        
    }
    
    IEnumerator moveObject()
    {
        for (int i = 0; i < path.Count; i++)
        {
            MoveIE = StartCoroutine(Moving(i));
            yield return MoveIE;
        }
    }
 
    IEnumerator Moving(int currentPosition)
    {
        while (transform.position != path[currentPosition])
        {
            transform.position = Vector3.MoveTowards(transform.position, path[currentPosition] , MoveSpeed * Time.deltaTime);
            yield return null;
        }
  
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;

public class GameObjectTestBehaviour : MonoBehaviour
{
    public float Val;

    internal void Move()
    {
        Debug.Log("Moving object!");
        transform.position += new Vector3(0, 0, Time.deltaTime * Val);
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour , IGridObject
{
    public float speed;
    private GridObjectType _material;
    private Vector3 destination;

    public void Awake()
    {
        destination = transform.position;
    }

    public GridObjectType gridObjectType
    {
        get { return _material; }
        set
        {
            _material = value;
            Material mat = Resources.Load(_material.ToString(), typeof(Material)) as Material;
            GetComponent<Renderer>().material = mat;
        }
    }
    

    public Vector3 size
    {
        get { return GetComponent<MeshRenderer>().bounds.size; }
    }


    public void MoveToPosition(Vector3 position)
    {
        destination = position;
        Vector3 spud = destination;
    }

    
    public void Destroy()
    {
        if (Application.isEditor)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void Update()
    {
        transform.position = Vector3.Lerp(transform.position, destination, speed*Time.deltaTime);
    }
}


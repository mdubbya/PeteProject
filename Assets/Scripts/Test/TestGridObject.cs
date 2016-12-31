using UnityEngine;
using System.Collections.Generic;

public class TestGridObject : MonoBehaviour, IGridObject
{
    Vector3 _position;
    CubeMaterials _material;
    public static List<int> DestroyCalledOn = new List<int>();

    public CubeMaterials material
    {
        get
        {
            return _material;
        }

        set
        {
            _material = value;
        }
    }

    public Vector3 size
    {
        get
        {
            return new Vector3(1, 1, 1);
        }
    }

    public void Destroy()
    {
        if (Application.isEditor)
        {
            DestroyCalledOn.Add(GetHashCode());
            DestroyImmediate(gameObject);
        }
        else
        {
            DestroyCalledOn.Add(GetHashCode());
            GameObject.Destroy(gameObject);
        }
    }

    public void MoveToPosition(Vector3 position)
    {
        _position = position;
    }
}
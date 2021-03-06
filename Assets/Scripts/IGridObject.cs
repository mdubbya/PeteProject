﻿using UnityEngine;

public interface IGridObject
{
    CubeMaterials material
    {
        get;
        set;
    }

    Vector3 size
    {
        get;
    }

    void MoveToPosition(Vector3 position);
    void Destroy();
}


using UnityEngine;
public interface IGridObject
{
    GridObjectType gridObjectType
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


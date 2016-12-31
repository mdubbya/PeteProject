using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum GridDirection { left, up,  down, right};

public class GameGrid : MonoBehaviour, IEnumerable<IGridObject>
{
    public int numberOfRows;
    public int numberOfColumns;
    public float rowSpawnDelay;
    public GameObject gridObjectPrefab;

    private Dictionary<GridIndex,IGridObject> _contents;
    private List<Vector3> _spawnPositions;
    private Dictionary<GridIndex, Vector3> _gridPositions;
    private float _gridSizeX;
    private float _gridSizeY;
    private float _gridObjectSizeX;
    private float _gridObjectSizeY;
    private float _spawnRowY;
    private Vector3 _topLeft;


    public IGridObject this[int xIndex, int yIndex]
    {
        get { return _contents[new GridIndex(xIndex,yIndex)]; }
    }


    public IGridObject this[GridIndex index]
    {
        get { return _contents[index]; }
    }


    public GridIndex IndexOf(IGridObject gridObject)
    {
        return _contents.FirstOrDefault(x => x.Value == gridObject).Key;
    }
    


    public void Start()
    {
        Initialize();
    }


    public void Initialize()
    {
        IGridObject gridObjectPrefabScript = gridObjectPrefab.GetComponent<IGridObject>();
        if(gridObjectPrefabScript == null)
        {
            throw new UnityException("Grid object prefab does not contain component matching IGridObject interface.");
        }

        _gridSizeX = gridObjectPrefabScript.size.x * numberOfColumns;
        _gridSizeY = gridObjectPrefabScript.size.y * numberOfRows;
        _gridObjectSizeX = gridObjectPrefabScript.size.x;
        _gridObjectSizeY = gridObjectPrefabScript.size.y;
        _spawnRowY = (_gridSizeY + (_gridObjectSizeY * 2)) - gridObjectPrefab.transform.position.y;
        _topLeft = new Vector3(transform.position.x - ((_gridSizeX - _gridObjectSizeX) / 2), transform.position.y + ((_gridSizeY - _gridObjectSizeY) / 2), transform.position.z);

        SpawnGridObjects();
        RandomizeGridObjects();
        StartCoroutine(PositionGridObjects());
    }


    public void SpawnGridObjects()
    {
        if (_contents != null)
        {
            if (_contents.Count > 0 && (from p in _contents where p.Value != null select p).Count() > 0)
            {
                foreach (var keyVal in _contents)
                {
                    keyVal.Value.Destroy();
                }
            }
        }

        _contents = new Dictionary<GridIndex, IGridObject>();
        _gridPositions = new Dictionary<GridIndex, Vector3>();
        _spawnPositions = new List<Vector3>();

        
        for (int i = 0; i < numberOfColumns; i++)
        {
            _spawnPositions.Add(new Vector3(_topLeft.x + (i * _gridObjectSizeX), _spawnRowY, _topLeft.z));
        }

        for (int r = numberOfRows - 1; r >= 0; r--)
        {
            for (int c = 0; c < numberOfColumns; c++)
            {
                Vector3 position = new Vector3(_topLeft.x + (c * _gridObjectSizeX), _topLeft.y - (r * _gridObjectSizeY), _topLeft.z);
                _gridPositions.Add(new GridIndex(r, c), position);
                IGridObject gridObj = ((GameObject)Instantiate(gridObjectPrefab, _spawnPositions[c], gridObjectPrefab.transform.rotation)).GetComponent<IGridObject>();
                _contents.Add(new GridIndex(r, c), gridObj);
            }
        }
    }


    public IEnumerator PositionGridObjects()
    {
        for (int r = numberOfRows - 1; r >= 0; r--)
        {
            for (int c = 0; c < numberOfColumns; c++)
            {
                IGridObject gridObj = this[r, c];
                gridObj.MoveToPosition(_gridPositions[new GridIndex(r, c)]);
            }
            yield return new WaitForSeconds(rowSpawnDelay);
        }
    }


    public IGridObject GetNeighbor(IGridObject gridObject, GridDirection dir, int index = 1)
    {
        int indexVal = index;
        if (dir == GridDirection.left || dir == GridDirection.up)
        {
            indexVal *= -1;
        }

        GridIndex gridIndex = IndexOf(gridObject);

        if ((dir == GridDirection.up) || (dir == GridDirection.down))
        {
            
            int xIndexVal = gridIndex.rowNumber + indexVal;
            if (xIndexVal >= 0 && xIndexVal < numberOfColumns)
            {
                try
                {
                    return this[xIndexVal, gridIndex.columnNumber];
                }
                catch(KeyNotFoundException ex)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        else
        {
            int yIndexVal = gridIndex.columnNumber + indexVal;
            if (yIndexVal >= 0 && yIndexVal < numberOfRows)
            {
                try
                {
                    return this[gridIndex.rowNumber, yIndexVal];
                }
                catch(KeyNotFoundException ex)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }


    public List<IGridObject> GetMatchingNeighbors(IGridObject gridObject)
    {
        List<IGridObject> retVal = new List<IGridObject>();
        foreach (GridDirection dir in Enum.GetValues(typeof(GridDirection)))
        {
            IGridObject obj = GetNeighbor(gridObject, dir);
            if (obj != null)
            {
                if (obj.material == gridObject.material)
                {
                    retVal.Add(obj);
                }
            }
        }
        return retVal;
    }

   
    private void RandomizeGridObjects()
    {
        for (int r = 0; r < numberOfRows; r++)
        {
            for (int c = 0; c < numberOfColumns; c++)
            {
                IGridObject gridObject = _contents[new GridIndex(r, c)];
                List<int> materials = new List<int>((int[])(Enum.GetValues(typeof(CubeMaterials))));

                IGridObject leftGridObject = GetNeighbor(gridObject, GridDirection.left);
                if (leftGridObject != null)
                {
                    IGridObject nextLeftGridObject = GetNeighbor(gridObject, GridDirection.left,2);
                    if (nextLeftGridObject != null)
                    {
                        if (leftGridObject.material == nextLeftGridObject.material)
                        {
                            materials.Remove((int)leftGridObject.material);
                        }
                    }
                }
                IGridObject upGridObject = GetNeighbor(gridObject, GridDirection.up);
                if (upGridObject != null)
                {
                    IGridObject nextUpGridObject = GetNeighbor(gridObject, GridDirection.up, 2);
                    if (nextUpGridObject != null)
                    {
                        if (upGridObject.material == nextUpGridObject.material)
                        {
                            materials.Remove((int)upGridObject.material);
                        }
                    }
                }

                gridObject.material = (CubeMaterials)materials[UnityEngine.Random.Range(0, materials.Count - 1)];
            }
        }
    }


    private List<List<IGridObject>> GetMatchingGroups()
    {
        List<List<IGridObject>> retVal = new List<List<IGridObject>>();
        foreach(IGridObject gridObject in this)
        {
            if ((from p in retVal where p.Contains(gridObject) select p).SingleOrDefault() == null)
            {
                List<IGridObject> matches = GetMatchingGroup(gridObject);
                if (matches.Count > 0)
                {
                    retVal.Add(matches);
                }
            }
        }
        return retVal;
    }


    private List<IGridObject> GetMatchingGroup(IGridObject gridObject = null, List<IGridObject> matching = null)
    {
        if(matching == null)
        {
            matching = new List<IGridObject>();
        }
        List<IGridObject> newMatches = (from p in GetMatchingNeighbors(gridObject) where p.material==gridObject.material && !matching.Contains(p) select p).ToList();
        if (newMatches != null && newMatches.Count > 0)
        {
            matching.AddRange(newMatches);
            foreach (IGridObject match in newMatches)
            {
                matching = GetMatchingGroup(match, matching);
            }
        }
        return matching;
    }


    public void ResolveMatches()
    {
        var toResolve = GetMatchingGroups();
        toResolve.ForEach(p => p.ForEach(q => q.Destroy()));
        List<GridIndex> removed = new List<GridIndex>();
        foreach(List<IGridObject> gridObjects in toResolve)
        {
            foreach (IGridObject gridObject in gridObjects)
            {
                GridIndex indexToRemove = IndexOf(gridObject);
                removed.Add(indexToRemove);
                _contents.Remove(indexToRemove);
            }
        }

        ShiftDownGridObjects();
        RefillColumns();
    }


    private void RefillColumns()
    {
        for(int columnIndex=0; columnIndex < numberOfColumns; columnIndex++)
        {
            List<IGridObject> existingObjects = (from p in this where IndexOf(p).columnNumber == columnIndex select p).ToList();
            //get the number of missing rows in this column
            int rowCountMissing = numberOfRows - existingObjects.Count;
            if(rowCountMissing > 0)
            {
                for(int rowIndex = rowCountMissing-1; rowIndex >= 0; rowIndex--)
                {
                    GridIndex newGridIndex = new GridIndex(rowIndex, columnIndex);
                    IGridObject gridObj = ((GameObject)Instantiate(gridObjectPrefab, _spawnPositions[columnIndex], gridObjectPrefab.transform.rotation)).GetComponent<IGridObject>();
                    _contents.Add(newGridIndex, gridObj);
                    gridObj.MoveToPosition(_gridPositions[newGridIndex]);
                }
            }
        }
    }


    private void ShiftDownGridObjects()
    {
        foreach (IGridObject gridObjectToShift in this.OrderBy(p => IndexOf(p).columnNumber).OrderByDescending(p => IndexOf(p).rowNumber))
        {
            
            int newRowIndex = 1;
            GridIndex gridIndex = IndexOf(gridObjectToShift);
            bool neighborFound = GetNeighbor(gridObjectToShift, GridDirection.down, newRowIndex) == null ? false : true;

            while (!neighborFound && gridIndex.rowNumber + newRowIndex < numberOfRows)
            {
                newRowIndex++;
                neighborFound = GetNeighbor(gridObjectToShift, GridDirection.down, newRowIndex) == null ? false : true;
            }
            //If a neighbor was never found searching all the way down the column,
            //then the gridObject must be shifted to the bottom row;
            GridIndex newGridIndex = new GridIndex(gridIndex.rowNumber + newRowIndex - 1, gridIndex.columnNumber);

            if (newRowIndex > 1)
            {
                Vector3 newPosition = _gridPositions[newGridIndex];
                gridObjectToShift.MoveToPosition(newPosition);
                if (_contents.ContainsKey(newGridIndex))
                {
                    _contents.Remove(newGridIndex);
                }
                _contents.Add(newGridIndex, gridObjectToShift);
                _contents.Remove(gridIndex);
            }
        }
    }

    
    private void RepopulateGrid()
    {

    }


    public IEnumerator<IGridObject> GetEnumerator()
    {
        return (from p in _contents select p.Value).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return (from p in _contents select p.Value).GetEnumerator();
    }
}


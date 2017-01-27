using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum GridDirection { left, up,  down, right};

namespace MatchThreeMiniGame
{
    public class GameGrid : MonoBehaviour, IEnumerable<IGridObject>
    {
        public int numberOfRows;
        public int numberOfColumns;
        public float rowSpawnDelay;
        public GameObject gridObjectPrefab;

        private Dictionary<GridIndex, IGridObject> _contents;
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
            get { return _contents[new GridIndex(xIndex, yIndex)]; }
        }


        public IGridObject this[GridIndex index]
        {
            get { return _contents[index]; }
        }


        public GridIndex IndexOf(IGridObject gridObject)
        {
            return _contents.FirstOrDefault(x => x.Value == gridObject).Key;
        }


        public IEnumerator<IGridObject> GetEnumerator()
        {
            return (from p in _contents select p.Value).GetEnumerator();
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return (from p in _contents select p.Value).GetEnumerator();
        }


        public void Start()
        {
            Initialize();
        }


        public void Initialize()
        {
            IGridObject gridObjectPrefabScript = gridObjectPrefab.GetComponent<IGridObject>();
            if (gridObjectPrefabScript == null)
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


        public void ResolveMatches(bool iterate = true)
        {
            var toResolve = GetMatchingGroups();

            if (toResolve.Count > 0)
            {
                foreach (var listOfMatches in toResolve)
                {
                    foreach (var gridObject in listOfMatches)
                    {
                        //Found that when referring to a monobehaviour by its interface, one must cast it to
                        //a monobehaviour when using the == or != operator.  These two operators are overloaded
                        //in the monobehaviour class, and the correct overloads are not called when referring
                        //to the object by its interface.  Note this is only an issue when comparing to null
                        //and the underlying object is a destroyed monobehaviour.
                        if ((MonoBehaviour)gridObject != null)
                        {
                            gridObject.Destroy();
                            GridIndex indexToRemove = IndexOf(gridObject);
                            _contents.Remove(indexToRemove);
                        }
                    }
                }
                ShiftDownGridObjects();
                RefillColumns();

                if (iterate)
                {
                    ResolveMatches();
                }
            }
        }


        public void SwapGridObjects(IGridObject firstToSwap, IGridObject secondToSwap)
        {
            GridIndex firstIndex = IndexOf(firstToSwap);
            GridIndex secondIndex = IndexOf(secondToSwap);
            int rowDiff = firstIndex.rowNumber - secondIndex.rowNumber;
            int columnDiff = firstIndex.columnNumber - secondIndex.columnNumber;
            //Only swap if gridobject is only one "grid square" away
            if ((rowDiff == 0 && Math.Abs(columnDiff) == 1) || (columnDiff == 0 && Math.Abs(rowDiff) == 1))
            {
                //Only swap if there is a possible match three (or more) to be made
                List<IGridObject> matchingInRowForFirst =
                    GetConnectedMatching(secondToSwap, firstToSwap).Where(p => IndexOf(p).rowNumber == IndexOf(secondToSwap).rowNumber).ToList();
                List<IGridObject> matchingInColumnForFirst =
                    GetConnectedMatching(secondToSwap, firstToSwap).Where(p => IndexOf(p).columnNumber == IndexOf(secondToSwap).columnNumber).ToList();
                List<IGridObject> matchingInRowForSecond =
                    GetConnectedMatching(firstToSwap, secondToSwap).Where(p => IndexOf(p).rowNumber == IndexOf(firstToSwap).rowNumber).ToList();
                List<IGridObject> matchingInColumnForSecond =
                    GetConnectedMatching(firstToSwap, secondToSwap).Where(p => IndexOf(p).columnNumber == IndexOf(firstToSwap).columnNumber).ToList();
                if (matchingInRowForFirst.Count >= 2 || matchingInRowForSecond.Count >= 2 ||
                    matchingInColumnForFirst.Count >= 2 || matchingInColumnForSecond.Count >= 2)
                {
                    //Do the swap
                    Vector3 newFirstPosition = _gridPositions[secondIndex];
                    Vector3 newSecondPosition = _gridPositions[firstIndex];
                    _contents.Remove(firstIndex);
                    _contents.Remove(secondIndex);
                    _contents.Add(firstIndex, secondToSwap);
                    _contents.Add(secondIndex, firstToSwap);
                    firstToSwap.MoveToPosition(newFirstPosition);
                    secondToSwap.MoveToPosition(newSecondPosition);
                }
            }
        }


        private void SpawnGridObject(int rowIndex, int columnIndex)
        {
            GridIndex newGridIndex = new GridIndex(rowIndex, columnIndex);
            IGridObject gridObj = ((GameObject)Instantiate(gridObjectPrefab, _spawnPositions[columnIndex], gridObjectPrefab.transform.rotation)).GetComponent<IGridObject>();
            gridObj.gridObjectType = GridObjectType.None;
            _contents.Add(newGridIndex, gridObj);
        }


        private void SpawnGridObjects()
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
                    SpawnGridObject(r, c);
                }
            }
        }


        private IEnumerator PositionGridObjects()
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


        private IGridObject GetNeighbor(IGridObject gridObject, GridDirection dir, int index = 1)
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
                    catch (KeyNotFoundException)
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
                    catch (KeyNotFoundException)
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


        private List<IGridObject> GetConnectedMatching(IGridObject gridObject, IGridObject projectedMatchingObject = null)
        {
            GridObjectType compareType = projectedMatchingObject == null ? gridObject.gridObjectType : projectedMatchingObject.gridObjectType;
            List<IGridObject> retVal = new List<IGridObject>();
            if (projectedMatchingObject == null)
            {
                retVal.Add(gridObject);
            }
            foreach (GridDirection dir in Enum.GetValues(typeof(GridDirection)))
            {
                IGridObject obj = GetNeighbor(gridObject, dir);
                while (obj != null)
                {
                    if (compareType != GridObjectType.None)
                    {
                        if (obj.gridObjectType == compareType)
                        {
                            if (obj != projectedMatchingObject)
                            {
                                retVal.Add(obj);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    obj = GetNeighbor(obj, dir);
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
                    List<int> materials = new List<int>((int[])(Enum.GetValues(typeof(GridObjectType))));

                    IGridObject leftGridObject = GetNeighbor(gridObject, GridDirection.left);
                    if (leftGridObject != null)
                    {
                        IGridObject nextLeftGridObject = GetNeighbor(gridObject, GridDirection.left, 2);
                        if (nextLeftGridObject != null)
                        {
                            if (leftGridObject.gridObjectType == nextLeftGridObject.gridObjectType)
                            {
                                materials.Remove((int)leftGridObject.gridObjectType);
                            }
                        }
                    }
                    IGridObject upGridObject = GetNeighbor(gridObject, GridDirection.up);
                    if (upGridObject != null)
                    {
                        IGridObject nextUpGridObject = GetNeighbor(gridObject, GridDirection.up, 2);
                        if (nextUpGridObject != null)
                        {
                            if (upGridObject.gridObjectType == nextUpGridObject.gridObjectType)
                            {
                                materials.Remove((int)upGridObject.gridObjectType);
                            }
                        }
                    }

                    if (gridObject.gridObjectType == GridObjectType.None)
                    {
                        gridObject.gridObjectType = (GridObjectType)materials[UnityEngine.Random.Range(0, materials.Count - 1)];
                    }
                }
            }
        }


        private List<List<IGridObject>> GetMatchingGroups()
        {
            List<List<IGridObject>> retVal = new List<List<IGridObject>>();
            foreach (IGridObject gridObject in this)
            {

                if (!((from p in retVal where p.Contains(gridObject) select p).Count() > 0))
                {
                    List<IGridObject> matches = GetConnectedMatching(gridObject);
                    List<IGridObject> rowMatches = (from p in matches where IndexOf(p).rowNumber == IndexOf(gridObject).rowNumber select p).ToList();
                    List<IGridObject> columnMatches = (from p in matches where IndexOf(p).columnNumber == IndexOf(gridObject).columnNumber select p).ToList();


                    retVal.Add(rowMatches);
                    retVal.Add(columnMatches);
                }
            }

            return retVal.Where(p => p.Count >= 3).ToList();
        }


        private void RefillColumns()
        {
            for (int columnIndex = 0; columnIndex < numberOfColumns; columnIndex++)
            {
                List<IGridObject> existingObjects = (from p in this where IndexOf(p).columnNumber == columnIndex select p).ToList();
                //get the number of missing rows in this column
                int rowCountMissing = numberOfRows - existingObjects.Count;
                if (rowCountMissing > 0)
                {
                    for (int rowIndex = rowCountMissing - 1; rowIndex >= 0; rowIndex--)
                    {
                        GridIndex newGridIndex = new GridIndex(rowIndex, columnIndex);
                        IGridObject gridObj = ((GameObject)Instantiate(gridObjectPrefab, _spawnPositions[columnIndex], gridObjectPrefab.transform.rotation)).GetComponent<IGridObject>();
                        gridObj.gridObjectType = GridObjectType.None;
                        _contents.Add(newGridIndex, gridObj);
                    }
                }
            }
            RandomizeGridObjects();
            StartCoroutine(PositionGridObjects());
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
    }
}
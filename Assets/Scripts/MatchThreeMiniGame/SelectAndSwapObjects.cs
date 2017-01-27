using UnityEngine;
namespace MatchThreeMiniGame
{
    class SelectAndSwapObjects : MonoBehaviour
    {
        private IGridObject firstSelected;
        private GameGrid grid;

        void Start()
        {
            firstSelected = null;
            grid = GetComponent<GameGrid>();
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hitInfo = new RaycastHit();
                bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);

                if (hit)
                {
                    var gridObject = hitInfo.transform.gameObject.GetComponent<IGridObject>();
                    if (gridObject != null)
                    {
                        if (firstSelected == null)
                        {
                            firstSelected = gridObject;
                        }
                        else
                        {
                            grid.SwapGridObjects(firstSelected, gridObject);
                            grid.ResolveMatches();
                            firstSelected = null;
                        }
                    }
                }
            }
        }
    }
}

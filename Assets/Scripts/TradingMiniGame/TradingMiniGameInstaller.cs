using System;
using TradingMiniGame;
using UnityEngine;
using Zenject;

namespace TradingMiniGame
{

    public class TradingMiniGameInstaller : MonoInstaller<TradingMiniGameInstaller>
    {
        public GameObject gameGrid;
        public GameObject gridObjectPrefab;

        public class GridObjectFactory : IFactory<IGridObject>
        {
            private DiContainer _container;
            private GameObject _gridObjectPrefab;

            [Inject]
            public GridObjectFactory(DiContainer container, GameObject gridObjectPrefab)
            {
                _container = container;
                _gridObjectPrefab = gridObjectPrefab;
            }

            public IGridObject Create()
            {
                return _container.InstantiatePrefabForComponent<IGridObject>(_gridObjectPrefab);
            }
        }

        public override void InstallBindings()
        {
            Container.Bind<IFactory<IGridObject>>().To<GridObjectFactory>();
            Container.Bind<IGameGrid>().To<IGameGrid>().FromInstance(gameGrid.GetComponent<IGameGrid>());
            Container.Bind<Vector3>().FromInstance(gridObjectPrefab.GetComponent<MeshRenderer>().bounds.size).WhenInjectedInto<IGameGrid>();
            Container.Bind<DiContainer>().FromInstance(Container).WhenInjectedInto<IFactory<IGridObject>>();
            Container.Bind<GameObject>().FromInstance(gridObjectPrefab).WhenInjectedInto<IFactory<IGridObject>>();

            GameGridController controller = new GameGridController();
            Container.Bind<IGameGridController>().FromInstance(controller);
            Container.Inject(controller);
        }
    }
}
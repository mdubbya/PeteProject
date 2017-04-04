using System;
using TradingSelection;
using UnityEngine;
using Zenject;

namespace TradingSelection
{

    public class TradingSelectionInstaller : MonoInstaller<TradingSelectionInstaller>
    {
        public GameObject spacePortSpawner;
        public GameObject spacePortPrefab;

        public class SpacePortFactory : IFactory<ISpacePort>
        {
            private DiContainer _container;
            private GameObject _spacePortPrefab;

            [Inject]
            public SpacePortFactory(DiContainer container, GameObject spacePortPrefab)
            {
                _container = container;
                _spacePortPrefab = spacePortPrefab;
            }

            public ISpacePort Create()
            {
                return _container.InstantiatePrefabForComponent<ISpacePort>(_spacePortPrefab);
            }
        }

        public class SpacePortControllerFactory : IFactory<ISpacePortController>
        {
            public ISpacePortController Create()
            {
                return new SpacePortController();
            }
        }

        public override void InstallBindings()
        {
            Container.Bind<IFactory<ISpacePort>>().To<SpacePortFactory>();
            Container.Bind<DiContainer>().FromInstance(Container).WhenInjectedInto<IFactory<ISpacePort>>();
            Container.Bind<GameObject>().FromInstance(spacePortPrefab).WhenInjectedInto<IFactory<ISpacePort>>();
            Container.Bind<ISpacePortController>().FromFactory<SpacePortControllerFactory>();
            Container.Bind<Vector3>().FromInstance(spacePortPrefab.GetComponent<MeshRenderer>().bounds.size).WhenInjectedInto<ISpacePortSpawner>();

            ISpacePortSpawner spawner = spacePortSpawner.GetComponent<ISpacePortSpawner>();
            Container.Bind<ISpacePortSpawner>().FromInstance(spawner);
        }
    }
}
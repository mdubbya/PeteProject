using System;
using TradingMiniGame;
using UnityEngine;
using Zenject;

public class TradingMiniGameInstaller : MonoInstaller<TradingMiniGameInstaller>
{
    public GameObject gameGrid;

    public class GridObjectFactory : IFactory<IGridObject>
    {
        public IGridObject Create()
        {
            return new HexGridObject();
        }
    }

    public override void InstallBindings()
    {
        Container.Bind<IFactory<IGridObject>>().To<GridObjectFactory>();
        Container.Bind<IGameGrid>().To<IGameGrid>().FromInstance(gameGrid.GetComponent<IGameGrid>());
        Container.Bind<IGameSetup>().To<IGameSetup>().FromInstance(gameGrid.GetComponent<IGameSetup>());
        GameGridController controller = new GameGridController();
        Container.Bind<IGameGridController>().FromInstance( controller );
        Container.Inject(controller);
    }
}
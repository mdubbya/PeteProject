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
        Container.Bind<IGameGrid>().To<GameGrid>().FromInstance(gameGrid.GetComponent<GameGrid>());
        Container.Bind<IGameGridController>().To<GameGridController>();
    }
}
using TradingMiniGame;
using UnityEngine;
using Zenject;

public class TradingMiniGameInstaller : MonoInstaller<TradingMiniGameInstaller>
{
    public GameObject gameGrid;

    public override void InstallBindings()
    {
        Container.Bind<IGameGrid>().To<GameGrid>().FromInstance(gameGrid.GetComponent<GameGrid>());
        Container.Bind<IGameGridController>().To<GameGridController>();
    }
}
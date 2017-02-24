using System.Collections.Generic;
using UnityEngine;

namespace TradingMiniGame
{
    public static class GameResources
    {

        public enum Materials
        {
            Blue,
            Green,
            Orange,
            Purple,
            Red,
            White,
            Yellow
        };

        public enum Prefabs
        {
            NumberZero,
            NumberOne,
            NumberTwo,
            NumberThree,
            NumberFour,
            NumberFive,
            NumberSix,
            NumberSeven,
            NumberEight,
            NumberNine,
            HexTile
        };

        private static Dictionary<Materials, Material> _materials = new Dictionary<Materials, Material>()
        {
            { Materials.Blue, (Material)Resources.Load("Blue") },
            { Materials.Green, (Material)Resources.Load("Green") },
            { Materials.Orange, (Material)Resources.Load("Orange") },
            { Materials.Purple, (Material)Resources.Load("Purple") },
            { Materials.Red, (Material)Resources.Load("Red") },
            { Materials.White, (Material)Resources.Load("White") },
            { Materials.Yellow, (Material)Resources.Load("Yellow") },
        };

        private static Dictionary<Prefabs, GameObject> _prefabs = new Dictionary<Prefabs, GameObject>()
        {
            { Prefabs.NumberZero, (GameObject)Resources.Load("NumberZero") },
            { Prefabs.NumberOne, (GameObject)Resources.Load("NumberOne") },
            { Prefabs.NumberTwo, (GameObject)Resources.Load("NumberTwo") },
            { Prefabs.NumberThree, (GameObject)Resources.Load("NumberThree") },
            { Prefabs.NumberFour, (GameObject)Resources.Load("NumberFour") },
            { Prefabs.NumberFive, (GameObject)Resources.Load("NumberFive") },
            { Prefabs.NumberSix, (GameObject)Resources.Load("NumberSix") },
            { Prefabs.NumberSeven, (GameObject)Resources.Load("NumberSeven") },
            { Prefabs.NumberEight, (GameObject)Resources.Load("NumberEight") },
            { Prefabs.NumberNine, (GameObject)Resources.Load("NumberNine") },
            { Prefabs.HexTile, (GameObject)Resources.Load("HexTile") }
        };

        public static Material Load(Materials materialName)
        {
            return _materials[materialName];
        }

        public static GameObject Load(Prefabs prefabName)
        {
            return _prefabs[prefabName];
        }
    }
}

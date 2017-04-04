using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace TradingSelection
{
    public class SpacePortSpawner : MonoBehaviour, ISpacePortSpawner
    {
        public Rect boundingArea;
        public int numberOfPorts;
        [Range(1, 10)]
        public int maxSplitDepth;
        public float minSpacing;

        private List<ISpacePort> _ports;
        private IFactory<ISpacePort> _spacePortFactory;
        private Vector3 _spacePortSize;

        [Inject]
        public void Init(IFactory<ISpacePort> spacePortFactory, Vector3 spacePortSize)
        {
            _spacePortFactory = spacePortFactory;
            _ports = new List<ISpacePort>();
            _spacePortSize = spacePortSize;
            SpawnSpacePorts();
        }


        public void SpawnSpacePorts()
        {
            Vector3 sizeWithPadding = new Vector3(_spacePortSize.x + minSpacing, _spacePortSize.y + minSpacing, _spacePortSize.z);
            QuadTree tree = new QuadTree(0, boundingArea, sizeWithPadding);
            tree.SplitRecursive(maxSplitDepth);
            DebugExtension.DebugBounds(new Bounds(tree.bounds.center, tree.bounds.size), Color.cyan, 1000);
            var leafNodes = tree.GetLeafNodes();
            foreach (QuadTree leaf in leafNodes)
            {
                ISpacePort port = _spacePortFactory.Create();
                port.gameObject.transform.position = leaf.bounds.center;
                DebugExtension.DebugBounds(new Bounds(leaf.bounds.center,leaf.bounds.size), Color.blue, 100000);
            }
        }
    }
}

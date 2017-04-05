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
        [Range(0, 50)]
        public int maxSplitDepth;
        public float minSectorSize;

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
            Vector3 sizeWithPadding = new Vector3(_spacePortSize.x + minSectorSize, _spacePortSize.y + minSectorSize, _spacePortSize.z);
            QuadTree tree = new QuadTree(0, boundingArea, sizeWithPadding);
            tree.SplitRecursive(maxSplitDepth);
            DebugExtension.DebugBounds(new Bounds(tree.bounds.center, tree.bounds.size), Color.cyan, 1000);
            var leafNodes = tree.GetLeafNodes();
            foreach (QuadTree leaf in leafNodes)
            {
                ISpacePort port = _spacePortFactory.Create();
                float x = Random.Range(leaf.bounds.x, leaf.bounds.x + leaf.bounds.width);
                float y = Random.Range(leaf.bounds.y, leaf.bounds.y + leaf.bounds.height);
                port.gameObject.transform.position = new Vector3(x, y, transform.position.z);
                //DebugExtension.DebugBounds(new Bounds(leaf.bounds.center,leaf.bounds.size), Color.blue, 100000);
            }
        }
    }
}

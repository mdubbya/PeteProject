using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Common
{
    public class QuadTree
    {
        private int _level;
        private Rect _bounds;
        private bool _isLeaf;

        public Rect bounds
        {
            get { return _bounds; }
        }
        private QuadTree[] _nodes;
        private Vector3 _objectSize;

        public QuadTree(int level, Rect bounds, Vector3 objectSize)
        {
            _level = level;
            _bounds = bounds;
            _objectSize = objectSize;
            _isLeaf = true;
            _nodes = new QuadTree[4];
        }


        public void SplitRecursive(int maxDepth = 0)
        {
            int splits = 0;
            if (maxDepth == 0 || _level < maxDepth)
            {
                if (_isLeaf)
                {
                    Split();
                    splits++;
                }

                foreach (QuadTree node in _nodes)
                {
                    if (node != null)
                    {
                        node.SplitRecursive(maxDepth);
                    }
                }
            }
        }


        public List<QuadTree> GetLeafNodes()
        {
            List<QuadTree> returnVal = new List<QuadTree>();
            if(_isLeaf)
            {
                returnVal.Add(this);
            }
            else
            {
                foreach(QuadTree tree in _nodes.Where(n=>n!=null))
                {
                    returnVal.AddRange(tree.GetLeafNodes());
                }
            }
            return returnVal;
        }


        private void Clear()
        {
            for(int i = 0; i < _nodes.Length; i++)
            {
                if(_nodes[i] != null)
                {
                    _nodes[i].Clear();
                    _nodes[i] = null;
                }
            }
        }

        private void Split()
        {
            float subWidth;
            float subWidth2;
            float subHeight;
            if (_bounds.width < (_objectSize.x))
            {
                subWidth = _bounds.width;
                subWidth2 = _bounds.width;
            }
            else
            {
                subWidth = Random.Range(_objectSize.x, _bounds.width - _objectSize.x);
                subWidth2 = Random.Range(_objectSize.x, _bounds.width - _objectSize.x);
            }
            if(_bounds.height < (_objectSize.y))
            {
                subHeight = _bounds.height;
            }
            else
            {
                subHeight = Random.Range(_objectSize.y, _bounds.height - _objectSize.y);
            }
            
            float x = _bounds.x;
            float y = _bounds.y;

            if ((subWidth > _objectSize.x) && (subHeight > _objectSize.y))
            {
                _nodes[0] = new QuadTree(_level + 1, new Rect(x, y, subWidth, subHeight), _objectSize);
                _isLeaf = false;
            }
            if((Math.Abs(_bounds.width - subWidth) > _objectSize.x) && (subHeight > _objectSize.y))
            {
                _nodes[1] = new QuadTree(_level + 1, new Rect(x + subWidth, y, Math.Abs(_bounds.width - subWidth), subHeight), _objectSize);
                _isLeaf = false;
            }
            if ((Math.Abs(_bounds.width - subWidth2) > _objectSize.x) && Math.Abs(_bounds.height - subHeight) > _objectSize.y)
            {
                _nodes[2] = new QuadTree(_level + 1, new Rect(x + subWidth2, y + subHeight, Math.Abs(_bounds.width - subWidth2), Math.Abs(_bounds.height - subHeight)), _objectSize);
                _isLeaf = false;
            }
            if((subWidth2 > _objectSize.x) && Math.Abs(_bounds.height - subHeight) > _objectSize.y)
            {                
                _nodes[3] = new QuadTree(_level + 1, new Rect(x, y + subHeight, subWidth2, Math.Abs(_bounds.height - subHeight)), _objectSize);
                _isLeaf = false;
            }
        }

      
    }
}

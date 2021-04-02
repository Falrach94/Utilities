using PatternUtils.Module_Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternUtils.Dependency_Graph
{
    public class DependencyGraph<T> : IDependencyGraph<T>
    {
        //  private List<T> _rootObjects = new(); //objects without dependencies

        private Dictionary<T, DependencyGraphNode<T>> _nodeDic = new();

        private List<T> _sortedList = new();

        public IReadOnlyList<T> DependingList => _sortedList;

        public IReadOnlyList<T> DependentList
        {
            get
            {
                var list = new List<T>(_sortedList);
                list.Reverse();
                return list;
            }
        }

        public void AddObject(T obj, IEnumerable<T> dependentObjects)
        {
            if (dependentObjects is null)
            {
                throw new ArgumentNullException(nameof(dependentObjects));
            }
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            foreach(var dep in dependentObjects)
            {
                if(!_nodeDic.ContainsKey(dep))
                {
                    throw new DependencyException("Dependent object not found!");
                }
            }
            if(_nodeDic.ContainsKey(obj))
            {
                throw new ArgumentException("Object was already added!");
            }
            
            var node = new DependencyGraphNode<T>(obj);
            foreach(var dep in dependentObjects)
            {
                var depNode = _nodeDic[dep];
                depNode.Dependents.Add(node);
                node.Dependencies.Add(depNode);
            }
            _nodeDic.Add(obj, node);

            var dependents = new List<T>(dependentObjects);

            int i;
            for(i = 0; i < _sortedList.Count && dependents.Any(); i++)
            {
                if(dependents.Contains(_sortedList[i]))
                {
                    dependents.Remove(_sortedList[i]);
                }
            }
            _sortedList.Insert(i, obj);
        }
        public void RemoveObject(T obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            if (!_nodeDic.ContainsKey(obj))
            {
                throw new ArgumentException("Object not found!");
            }
            var node = _nodeDic[obj];

            if (node.IsUsed)
            {
                throw new DependencyException("There are objects that depend on this object!");
            }

            foreach(var dep in node.Dependencies)
            {
                dep.Dependents.Remove(node);
            }
            _nodeDic.Remove(obj);

            _sortedList.Remove(obj);
        }

        public List<T> GetDependents(T obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            if (!_nodeDic.ContainsKey(obj))
            {
                throw new ArgumentException("Object not found!");
            }
            var node = _nodeDic[obj];

            Dictionary<DependencyGraphNode<T>, int> depthDic = new();

            node.FindDependentNodes(depthDic, 0);

            depthDic.Remove(node);

            SortedList<int, T> sortedList = new(IntComparer.Instance);

            foreach (var tuple in depthDic)
            {
                sortedList.Add(tuple.Value, tuple.Key.Object);
            }
            return sortedList.Values.Reverse().ToList();
        }
        public List<T> GetDependencies(T obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            if (!_nodeDic.ContainsKey(obj))
            {
                throw new ArgumentException("Object not found!");
            }
            var node = _nodeDic[obj];

            Dictionary<DependencyGraphNode<T>, int> depthDic = new();

            node.FindDependencyNodes(depthDic, 0);

            depthDic.Remove(node);

            SortedList<int, T> sortedList = new(IntComparer.Instance);

            foreach (var tuple in depthDic)
            {
                sortedList.Add(tuple.Value, tuple.Key.Object);
            }
            return sortedList.Values.Reverse().ToList();
        }

        public bool IsUsed(T obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            if (!_nodeDic.ContainsKey(obj))
            {
                throw new ArgumentException("Object not found!");
            }
            return _nodeDic[obj].IsUsed;

        }


        private class IntComparer : IComparer<int>
        {
            public static IntComparer Instance { get; } = new();

            public int Compare(int x, int y)
            {
                if (x > y) return 1;
                if (x < y) return -1;
                return 1;
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternUtils.Dependency_Graph
{
    internal class DependencyGraphNode<T>
    {
        public T Object { get; }

        public DependencyGraphNode(T obj)
        {
            Object = obj;
        }

        public bool IsUsed => Dependents.Count != 0;
        public HashSet<DependencyGraphNode<T>> Dependents { get; } = new();
        public HashSet<DependencyGraphNode<T>> Dependencies { get; } = new();

        internal void FindDependentNodes(Dictionary<DependencyGraphNode<T>, int> depthDic, int depth)
        {
            if (!depthDic.ContainsKey(this))
            {
                depthDic.Add(this, depth);
            }
            else if (depthDic[this] < depth)
            {
                depthDic[this] = depth;
            }
            else
            {
                return;
            }

            foreach (var dep in Dependents)
            {
                dep.FindDependentNodes(depthDic, depth + 1);
            }
        }
        internal void FindDependencyNodes(Dictionary<DependencyGraphNode<T>, int> depthDic, int depth)
        {
            if (!depthDic.ContainsKey(this))
            {
                depthDic.Add(this, depth);
            }
            else if (depthDic[this] < depth)
            {
                depthDic[this] = depth;
            }
            else
            {
                return;
            }

            foreach (var dep in Dependencies)
            {
                dep.FindDependencyNodes(depthDic, depth + 1);
            }
        }
    }
}

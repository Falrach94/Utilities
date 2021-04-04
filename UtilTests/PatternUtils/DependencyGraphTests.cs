using Microsoft.VisualStudio.TestTools.UnitTesting;
using PatternUtils.Dependency_Graph;
using PatternUtils.Module_Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilTests.PatternUtilsTests.Mocks;

namespace UtilTests.PatternUtilsTests
{
    [TestClass]
    public class DependencyGraphTests
    {
        /// <summary>
        /// Test adding and removing objects.
        /// Assert order in SortedList is correct.
        /// Assert IsUsed works correctly.
        /// </summary>
        [TestMethod]
        public void TestAddRemove()
        {
            IDependencyGraph<MockDependencyObject> graph = new DependencyGraph<MockDependencyObject>();

            MockDependencyObject a = new("a");
            MockDependencyObject b = new("b",a);
            MockDependencyObject c = new("c");
            MockDependencyObject d = new("d");
            c.Dependencies.Add(d);

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                graph.IsUsed(null);
            });
            Assert.ThrowsException<ArgumentException>(() =>
            {
                graph.IsUsed(a);
            });

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                graph.AddObject(a, null);
            });
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                graph.AddObject(null, a.Dependencies);
            });
            graph.AddObject(a, a.Dependencies);

            Assert.IsFalse(graph.IsUsed(a));

            Assert.IsTrue(graph.DependentList.Count == 1 && graph.DependentList[0] == a);
            Assert.ThrowsException<ArgumentException>(() =>
            {
                graph.AddObject(a, a.Dependencies);
            });

            graph.AddObject(b, b.Dependencies);


            Assert.IsTrue(graph.IsUsed(a));
            Assert.IsFalse(graph.IsUsed(b));

            CollectionAssert.AreEqual(graph.DependingList.ToArray(), new[] { a, b });
            CollectionAssert.AreEqual(graph.DependentList.ToArray(), new[] { b, a });

            Assert.ThrowsException<DependencyException>(() =>
            {
                graph.AddObject(c, c.Dependencies);
            });

            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                graph.RemoveObject(null);
            });
            Assert.ThrowsException<ArgumentException>(() =>
            {
                graph.RemoveObject(d);
            });
            Assert.ThrowsException<DependencyException>(() =>
            {
                graph.RemoveObject(a);
            });
            graph.RemoveObject(b);
            Assert.ThrowsException<ArgumentException>(() =>
            {
                graph.IsUsed(b);
            });
            Assert.IsFalse(graph.IsUsed(a));


            graph.RemoveObject(a);

            Assert.IsTrue(graph.DependentList.Count == 0);

            c = new MockDependencyObject("c", a);
            d = new MockDependencyObject("d", a, c);
            var e = new MockDependencyObject("e", a, d);
            var f = new MockDependencyObject("f", e, b);
            var g = new MockDependencyObject("g", c);
            var h = new MockDependencyObject("h", b, a);
            var i = new MockDependencyObject("i", a,d,e,f);
            var j = new MockDependencyObject("j", a);
            var k = new MockDependencyObject("k");

            Dictionary<MockDependencyObject, MockDependencyObject[]> dependingDic = new();
            dependingDic.Add(a, Array.Empty<MockDependencyObject>());
            dependingDic.Add(b, new[] { a });
            dependingDic.Add(c, new[] { a });
            dependingDic.Add(d, new[] { a, c });
            dependingDic.Add(e, new[] { a, d , c });
            dependingDic.Add(f, new[] { a, b, c, e, d });
            dependingDic.Add(g, new[] { c, a });
            dependingDic.Add(h, new[] { a, b });
            dependingDic.Add(i, new[] { a, c, d, e, b, f });
            dependingDic.Add(j, new[] { a });
            dependingDic.Add(k, Array.Empty<MockDependencyObject>());

            Dictionary<MockDependencyObject, MockDependencyObject[]> dependentDic = new();
            dependentDic.Add(a, new[] { b, c, d, e, f, g, h, i, j});
            dependentDic.Add(b, new[] { h, f, i});
            dependentDic.Add(c, new[] { g, e, d, f, i});
            dependentDic.Add(d, new[] { e, f, i });
            dependentDic.Add(e, new[] { f, i });
            dependentDic.Add(f, new[] { i });
            dependentDic.Add(g, Array.Empty<MockDependencyObject>());
            dependentDic.Add(h, Array.Empty<MockDependencyObject>());
            dependentDic.Add(i, Array.Empty<MockDependencyObject>());
            dependentDic.Add(j, Array.Empty<MockDependencyObject>());
            dependentDic.Add(k, Array.Empty<MockDependencyObject>());

            graph.AddObject(a, a.Dependencies);
            graph.AddObject(b, b.Dependencies);
            graph.AddObject(c, c.Dependencies);
            Assert.ThrowsException<DependencyException>(() =>
            {
                graph.AddObject(e, e.Dependencies);
            });
            graph.AddObject(d, d.Dependencies);
            graph.AddObject(e, e.Dependencies);
            graph.AddObject(f, f.Dependencies);
            graph.AddObject(g, g.Dependencies);
            graph.AddObject(h, h.Dependencies);
            graph.AddObject(i, i.Dependencies);
            graph.AddObject(j, j.Dependencies);
            graph.AddObject(k, k.Dependencies);

            AssertDependingOrder(graph.DependingList);
            AssertDependentOrder(graph.DependentList);

            var all = new[] { a, b, c, d, e, f, g, h, i, j, k };
            CollectionAssert.AreEquivalent(all, graph.DependentList.ToArray());
            CollectionAssert.AreEquivalent(graph.DependingList.ToArray(), graph.DependentList.ToArray());

            foreach (var obj in all)
            {
                var dependencies = graph.GetDependencies(obj);
                var exp = dependingDic[obj];
                CollectionAssert.AreEquivalent(exp, dependencies, $"failed for {obj.Name}, expected: {ToString(exp)}, actual: {ToString(dependencies.ToArray())}");
                AssertDependingOrder(dependencies);

                exp = dependentDic[obj];
                var dependents= graph.GetDependents(obj);
                CollectionAssert.AreEquivalent(exp, dependents, $"failed for {obj.Name}, expected: {ToString(exp)}, actual: {ToString(dependents.ToArray())}");
                AssertDependentOrder(dependents);
            }

        }

        static string ToString(MockDependencyObject[] objs)
        {
            return String.Join(", ", objs.Select(o => o.Name));
        }


        static void AssertDependingOrder(IReadOnlyList<MockDependencyObject> list)
        {
            HashSet<MockDependencyObject> set = new();

            foreach (var obj in list)
            {
                foreach (var dep in obj.Dependencies)
                {
                    Assert.IsTrue(set.Contains(dep), $"{ToString(list.ToArray())}: error at {dep.Name}");
                }

                set.Add(obj);
            }
        }
        static void AssertDependentOrder(IReadOnlyList<MockDependencyObject> list)
        {
            HashSet<MockDependencyObject> set = new();

            foreach (var obj in list)
            {
                foreach (var dep in obj.Dependencies)
                {
                    Assert.IsFalse(set.Contains(dep), $"{ToString(list.ToArray())}: error at {dep.Name}");
                }

                set.Add(obj);
            }
        }

    }
}

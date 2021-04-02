using System;
using PatternUtils.Dependency_Graph;
using System.Collections.Generic;

namespace PatternUtils.Dependency_Graph
{
    public interface IDependencyGraph<T>
    {
        /// <summary>
        /// list of objects, sorted such that every object only depends on objects that came before it
        /// </summary>
        public IReadOnlyList<T> DependingList { get; }

        /// <summary>
        /// list of objects, sorted such that every object only depends on objects that come after it
        /// </summary>
        public IReadOnlyList<T> DependentList { get; }

        /// <summary>
        /// Inserts object into graph.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="dependentObjects"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">obj was already added</exception>
        /// <exception cref="DependencyException">dependent object was not found</exception>
        public void AddObject(T obj, IEnumerable<T> dependentObjects);

        /// <summary>
        /// Removes object from graph.
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">object not found</exception>
        /// <exception cref="DependencyException">open dependencies</exception>
        public void RemoveObject(T obj);

        /// <summary>
        /// Checks wether other objects are dependent of the given object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>true: no dependencies found</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">object not found</exception>
        public bool IsUsed(T obj);

        /// <summary>
        /// Finds all objects that are directly or indirectly dependent of a given object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>List with all dependent objects, ordered from the farthest to the nearest dependency.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">object not found</exception>
        public List<T> GetDependents(T obj);
        /// <summary>
        /// Finds all objects from which a given object is directly or indirectly dependent of.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>List with all dependency objects, ordered from the farthest to the nearest dependency.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">object not found</exception>
        public List<T> GetDependencies(T obj);


    }
}
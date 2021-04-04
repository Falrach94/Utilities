using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilTests.PatternUtilsTests.Mocks
{
    public class MockDependencyObject
    {
        public MockDependencyObject(string name, params MockDependencyObject[] deps)
        {
            Dependencies = new(deps);
            Name = name;
        }

        public List<MockDependencyObject> Dependencies { get; private set; }
        public string Name { get; private set; }
    }
}

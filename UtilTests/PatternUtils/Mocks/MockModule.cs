using PatternUtils.Module_Framework;
using PatternUtils.Module_Framework.Data;
using PatternUtils.Module_Framework.Data.builder;
using PatternUtils.Module_Framework.Interfaces;
using SyncUtils;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilTests.PatternUtilsTests.Mocks
{
    public class MockModule : ModuleControl
    {
        public const string NAME = "Name";
        public static readonly PatternUtils.Version VERSION = PatternUtils.Version.Create(1, 2, 3);

        public bool Working { get; set; } = true;
        public bool Blocking { get; set; } = false;

        public MockModule()
            : this(true, false)
        {

        }
            

        public MockModule(bool working, bool blocking)
            : base(NAME, VERSION)
        {
            Working = working;
            Blocking = blocking;
        }
        public MockModule(string name, PatternUtils.Version version, bool working, bool blocking)
            : base(name, version)
        {
            Working = working;
            Blocking = blocking;
        }

        private void DebugTest()
        {
            if(!Working)
            {
                throw new MockException();
            }
            while (Blocking) ;
        }

        protected override Task ResetAsync()
        {
            DebugTest();
            return Task.CompletedTask;
        }

        protected override Task StartAsync()
        {
            DebugTest();
            return Task.CompletedTask;
        }

        protected override Task StopAsync()
        {
            DebugTest();
            return Task.CompletedTask;
        }

        protected override Task UninitializeAsync()
        {
            DebugTest();
            return Task.CompletedTask;
        }

        protected override Task InitializeAsync(IInterfaceProvider interfaceProvider, LockToken token)
        {
            DebugTest();
            return Task.CompletedTask;
        }

        protected virtual void DefineModuleInterfaces(ModuleBuilder builder)
        {

        }
            

        protected override void DefineModule(ModuleBuilder builder)
        {
            DebugTest();
            DefineModuleInterfaces(builder);
        }
    }
}

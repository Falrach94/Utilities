using Microsoft.VisualStudio.TestTools.UnitTesting;
using PatternUtils.Module_Framework;
using PatternUtils.Module_Framework.Data;
using PatternUtils.Module_Framework.Data.builder;
using SyncUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilTests.PatternUtilsTests.Mocks;
using UtilTests.TestUtilities;

namespace UtilTests.PatternUtilsTests
{
    [TestClass]
    public class ModuleControlTests
    {
        class FailingTestModule : ModuleControl
        {

            public FailingTestModule(string name, PatternUtils.Version version)
                : base(name, version)
            {
            }

            public bool Working { get; set; } = false;
            public bool Blocking { get; set; } = false;

            private void TestFct()
            {
                if (!Working) throw new MockException();
                while (Blocking) ;
            }

            protected override void DefineModule(ModuleBuilder builder)
            {
                TestFct();

                builder.SetInfo("Test Module2", PatternUtils.Version.Create(1, 2, 1));

            }

            protected override Task ResetAsync()
            {
                throw new System.NotImplementedException();
            }

            protected override Task StartAsync()
            {
                throw new System.NotImplementedException();
            }

            protected override Task StopAsync()
            {
                throw new System.NotImplementedException();
            }

            protected override Task UninitializeAsync()
            {
                throw new System.NotImplementedException();
            }

            protected override Task InitializeAsync(IInterfaceProvider interfaceProvider, LockToken token)
            {
                throw new System.NotImplementedException();
            }
        }

        class BlockingTestModule : ModuleControl
        {

            public BlockingTestModule(string name, PatternUtils.Version version)
                : base(name, version)
            {
            }

            public bool Working { get; set; } = true;
            public bool Blocking { get; set; } = true;

            private void TestFct()
            {
                if (!Working) throw new MockException();
                while (Blocking) ;
            }

            protected override void DefineModule(ModuleBuilder builder)
            {
                TestFct();

                builder.SetInfo("Test Module2", PatternUtils.Version.Create(1, 2, 1));

            }

            protected override Task ResetAsync()
            {
                throw new System.NotImplementedException();
            }

            protected override Task StartAsync()
            {
                throw new System.NotImplementedException();
            }

            protected override Task StopAsync()
            {
                throw new System.NotImplementedException();
            }

            protected override Task UninitializeAsync()
            {
                throw new System.NotImplementedException();
            }

            protected override Task InitializeAsync(IInterfaceProvider interfaceProvider, LockToken token)
            {
                throw new System.NotImplementedException();
            }
        }

        enum MethodType
        {
            Init,
            Reset,
            Stop,
            Start,
            UnInit
        }

        class TestModule : ModuleControl
        {

            public TestModule(string name, PatternUtils.Version version, bool working, bool blocking)
                : base(name, version)
            {
                Working = working;
                Blocking = blocking;
            }

            public bool Working { get; set; } = true;
            public bool Blocking { get; set; } = false;
            public MethodType LastMethod { get; private set; }

            private void TestFct()
            {
                if (!Working) throw new MockException();
                while (Blocking) ;
            }

            protected override void DefineModule(ModuleBuilder builder)
            {
                TestFct();

                builder.SetInfo("Test Module2", PatternUtils.Version.Create(1, 2, 1));

            }

            protected override Task InitializeAsync(IInterfaceProvider interfaceProvider, LockToken token)
            {
                LastMethod = MethodType.Init;
                TestFct();
                return Task.CompletedTask;
            }

            protected override Task ResetAsync()
            {
                LastMethod = MethodType.Reset;
                TestFct();
                return Task.CompletedTask;
            }

            protected override Task StartAsync()
            {
                LastMethod = MethodType.Start;
                TestFct();
                return Task.CompletedTask;
            }

            protected override Task StopAsync()
            {
                LastMethod = MethodType.Stop;
                TestFct();
                return Task.CompletedTask;
            }

            protected override Task UninitializeAsync()
            {
                LastMethod = MethodType.UnInit;
                TestFct();
                return Task.CompletedTask;
            }
        }

        class MockInterfaceProvider : IInterfaceProvider
        {
            public Task<T> GetInterfaceAsync<T>(LockToken token)
            {
                throw new NotImplementedException();
            }
        }

        public static readonly PatternUtils.Version VERSION = PatternUtils.Version.Create(1, 2, 3);
        public static readonly string NAME = "Test Module";

        [TestMethod]
        public void ModuleCreationTests()
        {
            Assert.ThrowsException<InitializationFailedException>(() =>
            {
                _ = new FailingTestModule(NAME, VERSION);
            });
            Assert.ThrowsException<InitializationFailedException>(() =>
            {
                _ = new BlockingTestModule(NAME, VERSION);
            });

            var module = new TestModule(NAME, VERSION, true, false);

            Assert.AreEqual(ModuleState.Uninitialized, module.State);
            Assert.IsTrue(module.Header.Info.Name.Equals(NAME));
            Assert.IsTrue(module.Header.Info.Version.Equals(VERSION));

            Assert.IsTrue(module.Header.Control == module
                       && module.Header.InterfaceDependencies != null
                       && module.Header.ManagedInterfaces != null
                       && module.Header.ManagerInterfaces != null
                       && module.Header.ProvidedInterfaces != null);
        }


        [TestMethod]
        public void TransitionTests()
        {
            MockInterfaceProvider interfaceProvider = new();

            var module = new TestModule(NAME, VERSION, true, false);
            Assert.AreEqual(ModuleState.Uninitialized, module.State);

            module.Working = false; //ensure methods throw exceptions if called

            // illegal transitions from 'unitialized' 

            TestUtils.AssertException<InvalidModuleStateException>(
                module.TransitionStateAsync(ModuleState.Error, TimeSpan.FromMilliseconds(90)), 100);
            TestUtils.AssertException<InvalidModuleStateException>(
                module.TransitionStateAsync(ModuleState.Resetting, TimeSpan.FromMilliseconds(90)), 100);
            TestUtils.AssertException<InvalidModuleStateException>(
                module.TransitionStateAsync(ModuleState.Stopped, TimeSpan.FromMilliseconds(90)), 100);
            TestUtils.AssertException<InvalidModuleStateException>(
                module.TransitionStateAsync(ModuleState.Uninitialized, TimeSpan.FromMilliseconds(90)), 100);

            module.Working = true;

            //transition 'unitialized' -> 'stopped'

            TestUtils.AssertTask(module.InitializeAsync(interfaceProvider, TimeSpan.FromMilliseconds(10), null), 30);
            Assert.AreEqual(MethodType.Init, module.LastMethod);
            Assert.AreEqual(ModuleState.Stopped, module.State);

            //illegal transitions from 'stopped'
            TestUtils.AssertException<InvalidModuleStateException>(
                module.TransitionStateAsync(ModuleState.Error, TimeSpan.FromMilliseconds(10)));
            TestUtils.AssertException<InvalidModuleStateException>(
                module.TransitionStateAsync(ModuleState.Stopped, TimeSpan.FromMilliseconds(10)));
          
            Assert.AreEqual(ModuleState.Stopped, module.State);

            //transition 'stopped' -> 'unitialized'
            TestUtils.AssertTask(module.TransitionStateAsync(ModuleState.Uninitialized, TimeSpan.FromMilliseconds(10)));
            Assert.AreEqual(MethodType.UnInit, module.LastMethod);
            Assert.AreEqual(ModuleState.Uninitialized, module.State);

            //transition 'unitialized' -> 'stopped'
            TestUtils.AssertTask(module.InitializeAsync(interfaceProvider, TimeSpan.FromMilliseconds(10), null), 30);
            Assert.AreEqual(MethodType.Init, module.LastMethod);
            Assert.AreEqual(ModuleState.Stopped, module.State);

            //transition 'stopped' -> 'running'
            TestUtils.AssertTask(module.TransitionStateAsync(ModuleState.Running, TimeSpan.FromMilliseconds(10)));
            Assert.AreEqual(MethodType.Start, module.LastMethod);
            Assert.AreEqual(ModuleState.Running, module.State);

            //illegal transitions from 'stopped'
            TestUtils.AssertException<InvalidModuleStateException>(
                module.TransitionStateAsync(ModuleState.Error, TimeSpan.FromMilliseconds(10)));
            TestUtils.AssertException<InvalidModuleStateException>(
                module.TransitionStateAsync(ModuleState.Resetting, TimeSpan.FromMilliseconds(10)));
            TestUtils.AssertException<InvalidModuleStateException>(
                module.TransitionStateAsync(ModuleState.Uninitialized, TimeSpan.FromMilliseconds(10)));
            TestUtils.AssertException<InvalidModuleStateException>(
                module.TransitionStateAsync(ModuleState.Running, TimeSpan.FromMilliseconds(10)));

            Assert.AreEqual(ModuleState.Running, module.State);

            //transition 'running' -> 'stopped'
            TestUtils.AssertTask(module.TransitionStateAsync(ModuleState.Stopped, TimeSpan.FromMilliseconds(10)));
            Assert.AreEqual(MethodType.Stop, module.LastMethod);
            Assert.AreEqual(ModuleState.Stopped, module.State);

            //transition 'stopped' -> 'reset' -> 'stopped'
            TestUtils.AssertTask(module.TransitionStateAsync(ModuleState.Resetting, TimeSpan.FromMilliseconds(10)));
            Assert.AreEqual(MethodType.Reset, module.LastMethod);
            Assert.AreEqual(ModuleState.Stopped, module.State);
        }

        [TestMethod]
        public void TimeoutTests()
        {
            MockInterfaceProvider interfaceProvider = new();
            TestModule module;

            //uninitialized -> stopped
            module = new(NAME, VERSION, true, true);
            TestUtils.AssertException<TimeoutException>(
                module.InitializeAsync(interfaceProvider, TimeSpan.FromMilliseconds(10), null));
            Assert.AreEqual(ModuleState.Error, module.State);

            //stopped -> unitialized
            module = new(NAME, VERSION, true, false);
            TestUtils.AssertTask(module.InitializeAsync(interfaceProvider, TimeSpan.FromMilliseconds(10), null));
            module.Blocking = true;
            TestUtils.AssertException<TimeoutException>(
                module.TransitionStateAsync(ModuleState.Uninitialized, TimeSpan.FromMilliseconds(10)));
            Assert.AreEqual(ModuleState.Error, module.State);

            //stopped -> running
            module = new(NAME, VERSION, true, false);
            TestUtils.AssertTask(module.InitializeAsync(interfaceProvider, TimeSpan.FromMilliseconds(10), null));
            module.Blocking = true;
            TestUtils.AssertException<TimeoutException>(
                module.TransitionStateAsync(ModuleState.Running, TimeSpan.FromMilliseconds(10)));
            Assert.AreEqual(ModuleState.Error, module.State);

            //stopped -> resetting
            module = new(NAME, VERSION, true, false);
            TestUtils.AssertTask(module.InitializeAsync(interfaceProvider, TimeSpan.FromMilliseconds(10), null));
            module.Blocking = true;
            TestUtils.AssertException<TimeoutException>(
                module.TransitionStateAsync(ModuleState.Resetting, TimeSpan.FromMilliseconds(10)));
            Assert.AreEqual(ModuleState.Error, module.State);

            //running -> stopped
            module = new(NAME, VERSION, true, false);
            TestUtils.AssertTask(module.InitializeAsync(interfaceProvider, TimeSpan.FromMilliseconds(10), null));
            TestUtils.AssertTask(module.TransitionStateAsync(ModuleState.Running, TimeSpan.FromMilliseconds(10)));
            module.Blocking = true;
            TestUtils.AssertException<TimeoutException>(
                module.TransitionStateAsync(ModuleState.Stopped, TimeSpan.FromMilliseconds(10)));
            Assert.AreEqual(ModuleState.Error, module.State);

        }
        [TestMethod]
        public void FailingMethodTests()
        {
            MockInterfaceProvider interfaceProvider = new();
            TestModule module;

            //uninitialized -> stopped
            module = new(NAME, VERSION, false, false);
            TestUtils.AssertException<ModuleMethodException>(
                module.InitializeAsync(interfaceProvider, TimeSpan.FromMilliseconds(10), null));
            Assert.AreEqual(ModuleState.Error, module.State);

            //stopped -> unitialized
            module = new(NAME, VERSION, true, false);
            TestUtils.AssertTask(module.InitializeAsync(interfaceProvider, TimeSpan.FromMilliseconds(10), null));
            module.Working = false;
            TestUtils.AssertException<ModuleMethodException>(
                module.TransitionStateAsync(ModuleState.Uninitialized, TimeSpan.FromMilliseconds(10)));
            Assert.AreEqual(ModuleState.Error, module.State);

            //stopped -> running
            module = new(NAME, VERSION, true, false);
            TestUtils.AssertTask(module.InitializeAsync(interfaceProvider, TimeSpan.FromMilliseconds(10), null));
            module.Working = false;
            TestUtils.AssertException<ModuleMethodException>(
                module.TransitionStateAsync(ModuleState.Running, TimeSpan.FromMilliseconds(10)));
            Assert.AreEqual(ModuleState.Error, module.State);

            //stopped -> resetting
            module = new(NAME, VERSION, true, false);
            TestUtils.AssertTask(module.InitializeAsync(interfaceProvider, TimeSpan.FromMilliseconds(10), null));
            module.Working = false;
            TestUtils.AssertException<ModuleMethodException>(
                module.TransitionStateAsync(ModuleState.Resetting, TimeSpan.FromMilliseconds(10)));
            Assert.AreEqual(ModuleState.Error, module.State);

            //running -> stopped
            module = new(NAME, VERSION, true, false);
            TestUtils.AssertTask(module.InitializeAsync(interfaceProvider, TimeSpan.FromMilliseconds(10), null));
            TestUtils.AssertTask(module.TransitionStateAsync(ModuleState.Running, TimeSpan.FromMilliseconds(10)));
            module.Working = false;
            TestUtils.AssertException<ModuleMethodException>(
                module.TransitionStateAsync(ModuleState.Stopped, TimeSpan.FromMilliseconds(10)));
            Assert.AreEqual(ModuleState.Error, module.State);

        }
    }
}
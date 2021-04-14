using Microsoft.VisualStudio.TestTools.UnitTesting;
using PatternUtils.Dependency_Graph;
using PatternUtils.Module_Framework;
using PatternUtils.Module_Framework.Data;
using PatternUtils.Module_Framework.Data.builder;
using PatternUtils.Module_Framework.Impl;
using PatternUtils.Module_Framework.Interfaces;
using SyncUtils;
using SyncUtils.Barrier;
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
    public class ModuleManagerTests
    {
        [TestMethod]
        public void CheckCompatibilityTest()
        {
            ModuleManager manager = new();

            MockModule module = new(true, false);



            TestUtils.AssertException<ArgumentNullException>(manager.CheckModuleCompatibilityAsync(null));

            var report = TestUtils.AssertTask(manager.CheckModuleCompatibilityAsync(module.Header));
            Assert.IsTrue(report.IsCompatible);
        }

        class TestInterface
        {

        }

        class InterfaceProviderModule : MockModule
        {
            public InterfaceA A { get; } = new();
            public TestInterface Test { get; } = new();
            protected override void DefineModuleInterfaces(ModuleBuilder builder)
            {
                A.Info = new InterfaceInfo(typeof(InterfaceA), PatternUtils.Version.Create(1, 1));
                builder.SetProvidedInterfaces(A, 
                                              new ModuleInterfaceWrapper<TestInterface>(Test, PatternUtils.Version.Create(1)));
            }
        }
        class InterfaceUserModule : MockModule
        {
            public InterfaceA A { get; private set; }
            public TestInterface Test { get; private set; }
            protected override void DefineModuleInterfaces(ModuleBuilder builder)
            {
                builder.SetDependencies(new InterfaceInfo(typeof(InterfaceA), PatternUtils.Version.Create(1, 1)),
                                        new InterfaceInfo(typeof(TestInterface), PatternUtils.Version.Create(1, 0)));
            }

            protected async override Task InitializeAsync(IInterfaceProvider interfaceProvider, LockToken token)
            {
                A = await interfaceProvider.GetInterfaceAsync<InterfaceA>(token);
                Test = await interfaceProvider.GetInterfaceAsync<TestInterface>(token);
            }
        }

        [TestMethod]
        public void UseDependencyInterface()
        {
            ModuleManager manager = new();

            InterfaceUserModule userModule = new();
            InterfaceProviderModule providerModule = new();

            TestUtils.AssertTask(manager.RegisterModuleAsync(providerModule.Header));
            TestUtils.AssertTask(manager.RegisterModuleAsync(userModule.Header));
            Assert.IsTrue(userModule.A == providerModule.A);
            Assert.IsTrue(userModule.Test == providerModule.Test);
        }

        [TestMethod]
        public void SingleModuleTest()
        {

            ModuleManager manager = new();
            ModuleManager manager2 = new();

            MockModule module = new(true, false);


            //get dependent/depending modules of null
            TestUtils.AssertException<ArgumentNullException>(manager.GetDependentModules(null));
            TestUtils.AssertException<ArgumentNullException>(manager.GetDependingModules(null));
            //get dependent/depending modules of unregistered module
            TestUtils.AssertException<ArgumentException>(manager.GetDependentModules(module.Header));
            TestUtils.AssertException<ArgumentException>(manager.GetDependingModules(module.Header));

            //register null module 
            TestUtils.AssertException<ArgumentNullException>(manager.RegisterModuleAsync(null));
            Assert.AreEqual(ModuleState.Uninitialized, module.State);

            //start null module
            TestUtils.AssertException<ArgumentNullException>(manager.StartModuleAsync(null, true));
            TestUtils.AssertException<ArgumentNullException>(manager.StartModuleAsync(null, false));
            //stop null module
            TestUtils.AssertException<ArgumentNullException>(manager.StopModuleAsync(null, true));
            TestUtils.AssertException<ArgumentNullException>(manager.StopModuleAsync(null, false));
            //reset null module
            TestUtils.AssertException<ArgumentNullException>(manager.ResetModuleAsync(null));
            TestUtils.AssertException<ArgumentNullException>(manager.ResetModuleAsync(null));
            //start unregistered module
            TestUtils.AssertException<ArgumentException>(manager.StartModuleAsync(module.Header, true));
            TestUtils.AssertException<ArgumentException>(manager.StartModuleAsync(module.Header, false));
            //stop unregistered module
            TestUtils.AssertException<ArgumentException>(manager.StopModuleAsync(module.Header, true));
            TestUtils.AssertException<ArgumentException>(manager.StopModuleAsync(module.Header, false));
            //reset unregistered module
            TestUtils.AssertException<ArgumentException>(manager.ResetModuleAsync(module.Header));
            TestUtils.AssertException<ArgumentException>(manager.ResetModuleAsync(module.Header));

            //stop all modules without registered modules
            TestUtils.AssertTask(manager.StopAllModulesAsync());
            //start all modules without registered modules
            TestUtils.AssertTask(manager.StartAllModulesAsync());
            //start all modules without registered modules
            TestUtils.AssertTask(manager.ResetAllModulesAsync());




            //register valid module
            TestUtils.AssertTask(manager.RegisterModuleAsync(module.Header));
            Assert.AreEqual(ModuleState.Stopped, module.State);
            CollectionAssert.AreEqual(new[] { module.Header }, manager.Modules.ToArray());

            //register module in state stopped
            TestUtils.AssertException<InvalidModuleStateException>(manager.RegisterModuleAsync(module.Header));

            //unregister null module
            TestUtils.AssertException<ArgumentNullException>(manager.UnregisterModuleAsync(null));

            //unregister valid module
            TestUtils.AssertTask(manager.UnregisterModuleAsync(module.Header));
            Assert.IsTrue(module.State == ModuleState.Uninitialized);
            Assert.IsFalse(manager.Modules.Any());

            //unregister module in state uninitialized
            TestUtils.AssertException<ArgumentException>(manager.UnregisterModuleAsync(module.Header));

            //unregister module in state stopped from wrong manager
            TestUtils.AssertTask(manager.RegisterModuleAsync(module.Header));
            TestUtils.AssertException<ArgumentException>(manager2.UnregisterModuleAsync(module.Header));

            //stop stopped module
            TestUtils.AssertException<InvalidModuleStateException>(manager.StopModuleAsync(module.Header, true));
            TestUtils.AssertException<InvalidModuleStateException>(manager.StopModuleAsync(module.Header, false));
            Assert.AreEqual(ModuleState.Stopped, module.State);

            //start stopped module (parameter true)
            TestUtils.AssertTask(manager.StartModuleAsync(module.Header, true));
            Assert.AreEqual(ModuleState.Running, module.State);

            //start running module
            TestUtils.AssertException<InvalidModuleStateException>(manager.StartModuleAsync(module.Header, true));
            TestUtils.AssertException<InvalidModuleStateException>(manager.StartModuleAsync(module.Header, false));
            //reset running module            
            TestUtils.AssertException<InvalidModuleStateException>(manager.ResetModuleAsync(module.Header));
            Assert.AreEqual(ModuleState.Running, module.State);

            //stop running module (parameter true)
            TestUtils.AssertTask(manager.StopModuleAsync(module.Header, true));
            Assert.AreEqual(ModuleState.Stopped, module.State);

            //start and stop running module (parameter false)
            TestUtils.AssertTask(manager.StartModuleAsync(module.Header, false));
            Assert.AreEqual(ModuleState.Running, module.State);
            TestUtils.AssertTask(manager.StopModuleAsync(module.Header, false));
            Assert.AreEqual(ModuleState.Stopped, module.State);

            //reset stopped module
            TestUtils.AssertTask(manager.ResetModuleAsync(module.Header));
            Assert.AreEqual(ModuleState.Stopped, module.State);

            //reset all stopped module
            TestUtils.AssertTask(manager.ResetAllModulesAsync());
            Assert.AreEqual(ModuleState.Stopped, module.State);

            //start all stopped modules            
            TestUtils.AssertTask(manager.StartAllModulesAsync());
            Assert.AreEqual(ModuleState.Running, module.State);

            //start all stopped modules            
            TestUtils.AssertTask(manager.StopAllModulesAsync());
            Assert.AreEqual(ModuleState.Stopped, module.State);

        }

        [TestMethod]
        public void MultipleModuleTest()
        {
            ModuleManager manager = new();

            MockModule module1 = new(true, false);
            MockModule module2 = new(true, false);

            TestUtils.AssertTask(manager.RegisterModuleAsync(module1.Header));
            TestUtils.AssertTask(manager.RegisterModuleAsync(module2.Header));

            CollectionAssert.AreEquivalent(new[] { module1.Header, module2.Header }, manager.Modules.ToArray());

            TestUtils.AssertTask(manager.ResetAllModulesAsync());

            Assert.AreEqual(ModuleState.Stopped, module1.State);
            Assert.AreEqual(ModuleState.Stopped, module2.State);

           TestUtils.AssertTask(manager.StartAllModulesAsync());

            Assert.AreEqual(ModuleState.Running, module1.State);
            Assert.AreEqual(ModuleState.Running, module2.State);

            TestUtils.AssertTask(manager.StopAllModulesAsync());

            Assert.AreEqual(ModuleState.Stopped, module1.State);
            Assert.AreEqual(ModuleState.Stopped, module2.State);
        }

        public class InterfaceA : IModuleInterface
        {
            //public PatternUtils.Version Version { get; set; } = PatternUtils.Version.Create(1);

            public InterfaceInfo Info { get; set; } = new InterfaceInfo(typeof(InterfaceA), PatternUtils.Version.Create(1));
        }
        public class InterfaceB : IModuleInterface
        {
            public InterfaceInfo Info { get; set; } = new InterfaceInfo(typeof(InterfaceB), PatternUtils.Version.Create(1));
         //   public PatternUtils.Version Version { get; set; } = PatternUtils.Version.Create(1);
        }
        public class InterfaceC : IModuleInterface
        {
            public InterfaceInfo Info { get; set; } = new InterfaceInfo(typeof(InterfaceC), PatternUtils.Version.Create(2));
         //   public PatternUtils.Version Version { get; set; } = PatternUtils.Version.Create(2);
        }

        public class ModuleA : MockModule
        {
            public InterfaceA A { get; } = new();
            protected override void DefineModuleInterfaces(ModuleBuilder builder)
            {
                builder.SetProvidedInterfaces(A);
            }
        }
        public class ModuleA2 : MockModule
        {
            public InterfaceA A { get; } = new();
            protected override void DefineModuleInterfaces(ModuleBuilder builder)
            {
                A.Info = new InterfaceInfo(typeof(InterfaceA), PatternUtils.Version.Create(1, 1));
                builder.SetProvidedInterfaces(A);
            }
        }
        public class ModuleB : MockModule
        {
            public InterfaceB B { get; } = new();
            protected override void DefineModuleInterfaces(ModuleBuilder builder)
            {
                builder.SetProvidedInterfaces(B);
                builder.SetDependencies(new InterfaceInfo(typeof(InterfaceA), 1, 0));
            }
        }
        public class ModuleB2 : MockModule
        {
            public InterfaceB B { get; } = new();
            protected override void DefineModuleInterfaces(ModuleBuilder builder)
            {
                B.Info = new InterfaceInfo(typeof(InterfaceB), PatternUtils.Version.Create(1, 1));
                builder.SetProvidedInterfaces(B);
                builder.SetDependencies(new InterfaceInfo(typeof(InterfaceA), 1, 1));
            }
        }
        public class ModuleB3 : MockModule
        {
            public InterfaceB B { get; } = new();
            protected override void DefineModuleInterfaces(ModuleBuilder builder)
            {
                B.Info = new InterfaceInfo(typeof(InterfaceB), PatternUtils.Version.Create(1, 1));
                builder.SetProvidedInterfaces(B);
                builder.SetDependencies(new InterfaceInfo(typeof(InterfaceA), 1, 0));
            }
        }
        public class ModuleC : MockModule
        {
            public InterfaceC C { get; } = new();
            protected override void DefineModuleInterfaces(ModuleBuilder builder)
            {
                builder.SetProvidedInterfaces(C);
                builder.SetDependencies(new InterfaceInfo(typeof(InterfaceA), 1, 0),
                                        new InterfaceInfo(typeof(InterfaceB), 1, 0));
            }
        }
        public class ModuleD : MockModule
        {
            protected override void DefineModuleInterfaces(ModuleBuilder builder)
            {
                builder.SetDependencies(new InterfaceInfo(typeof(InterfaceC), 1, 0));
            }
        }

        public class MInterfaceWrapperA : ManagedInterfaceWrapper<MInterfaceA>
        {
            public event EventHandler<IManagerInterface> Registration;
            public event EventHandler Unregistration;
            public MInterfaceWrapperA(MInterfaceA obj, PatternUtils.Version version) : base(obj, version)
            {
            }
            protected override void OnRegistration(IManagerInterface obj)
            {
                Registration?.Invoke(this, obj);
            }
            protected override void OnUnregistration()
            {
                Unregistration?.Invoke(this, null);
            }
        }

        public class MInterfaceA
        {
        }
        public class MInterfaceB
        {

        }
        public class MInterfaceWrapperB : ManagedInterfaceWrapper<MInterfaceB>
        {
            public event EventHandler<IManagerInterface> Registration;
            public event EventHandler Unregistration;
            public MInterfaceWrapperB(MInterfaceB obj, PatternUtils.Version version) : base(obj, version)
            {
            }
            protected override void OnRegistration(IManagerInterface obj)
            {
                Registration?.Invoke(this, obj);
            }
            protected override void OnUnregistration()
            {
                Unregistration?.Invoke(this, null);
            }
        }

        public class ManagerA : ManagerInterface<MInterfaceA>
        {
            public event EventHandler<IManagedInterface> RegisteredClass;
            protected override void OnRegistration(ManagedInterfaceWrapper<MInterfaceA> obj)
            {
                RegisteredClass?.Invoke(this, obj);
            }
            public ManagerA(PatternUtils.Version version, PatternUtils.Version managedVersion)
                : base(version, managedVersion)
            {
            }
        }
        public class ManagerB : ManagerInterface<MInterfaceA>
        {
            public event EventHandler<IManagedInterface> RegisteredClass;
            protected override void OnRegistration(ManagedInterfaceWrapper<MInterfaceA> obj)
            {
                RegisteredClass?.Invoke(this, obj);
            }
            public ManagerB(PatternUtils.Version version, PatternUtils.Version managedVersion)
                : base(version, managedVersion)
            {
            }
        }
        public class ManagerC : ManagerInterface<MInterfaceB>
        {
            public event EventHandler<IManagedInterface> RegisteredClass;
            protected override void OnRegistration(ManagedInterfaceWrapper<MInterfaceB> obj)
            {
                RegisteredClass?.Invoke(this, obj);
            }
            public ManagerC(PatternUtils.Version version, PatternUtils.Version managedVersion)
                : base(version, managedVersion)
            {
            }
        }

        static PatternUtils.Version ManagerVersion;
        static PatternUtils.Version InterfaceVersion;

        public class ManagerModuleA : MockModule
        {
            public ManagerA ManagerClass { get; } = new(ManagerVersion, InterfaceVersion);

            protected override void DefineModuleInterfaces(ModuleBuilder builder)
            {
                builder.SetManagerInterfaces(ManagerClass);
            }
        }
        public class ManagerModuleB : MockModule
        {
            public ManagerB ManagerClass { get; } = new(ManagerVersion, InterfaceVersion);

            protected override void DefineModuleInterfaces(ModuleBuilder builder)
            {
                builder.SetManagerInterfaces(ManagerClass);
            }
        }
        public class ManagerModuleC : MockModule
        {
            public ManagerC ManagerClass { get; } = new(ManagerVersion, InterfaceVersion);

            protected override void DefineModuleInterfaces(ModuleBuilder builder)
            {
                builder.SetManagerInterfaces(ManagerClass);
            }
        }
        public class InterfaceModuleA : MockModule
        {
            public MInterfaceWrapperA ManagedClass { get; } = new(new MInterfaceA(), InterfaceVersion);

            protected override void DefineModuleInterfaces(ModuleBuilder builder)
            {
                builder.SetManagedInterfaces(ManagedClass);
            }
        }
        public class InterfaceModuleB : MockModule
        {
            public MInterfaceWrapperB ManagedClass { get; } = new(new MInterfaceB(), InterfaceVersion);

            protected override void DefineModuleInterfaces(ModuleBuilder builder)
            {
                builder.SetManagedInterfaces(ManagedClass);
            }
        }

        class ManagedInterface
        {
            public Action MockHandler { get; set; }
            public void MockMethod()
            {
                MockHandler?.Invoke();
            }
        }

        class ManagedModule : ModuleControl
        {
            public ManagedModule()
                :base("Managed", PatternUtils.Version.Create(1))
            {
            }

            public ManagedInterface ManagedInterface { get; } = new();

            protected override void DefineModule(ModuleBuilder builder)
            {
                builder.SetManagedInterfaces(new ManagedInterfaceWrapper<ManagedInterface>(ManagedInterface, PatternUtils.Version.Create(1)));
            }

            protected override Task InitializeAsync(IInterfaceProvider interfaceProvider, LockToken providerLockToken)
            {
                return Task.CompletedTask;
            }

            protected override Task ResetAsync()
            {
                return Task.CompletedTask;
            }

            protected override Task StartAsync()
            {
                return Task.CompletedTask;
            }

            protected override Task StopAsync()
            {
                return Task.CompletedTask;
            }

            protected override Task UninitializeAsync()
            {
                return Task.CompletedTask;
            }
        }

        class ManagerModule : ModuleControl
        {
            public ManagerModule() : base("manager", PatternUtils.Version.Create(1))
            {
            }

            public List<ManagedInterface> List { get; } = new();

            protected override void DefineModule(ModuleBuilder builder)
            {
                builder.SetManagerInterfaces(new ManagerInterface<ManagedInterface>(PatternUtils.Version.Create(1), PatternUtils.Version.Create(1),
                        Register, Unregister));
            }

            private void Register(ManagedInterface obj)
            {
                List.Add(obj);
            }

            private void Unregister(ManagedInterface obj)
            {
                List.Remove(obj);
            }

            protected override Task InitializeAsync(IInterfaceProvider interfaceProvider, LockToken providerLockToken)
            {
                return Task.CompletedTask;
            }

            protected override Task ResetAsync()
            {
                return Task.CompletedTask;
            }

            protected override Task StartAsync()
            {
                return Task.CompletedTask;
            }

            protected override Task StopAsync()
            {
                return Task.CompletedTask;
            }

            protected override Task UninitializeAsync()
            {
                return Task.CompletedTask;
            }
        }

        [TestMethod]
        public void TestManagedInterfaceWrapper()
        {
            ModuleManager manager = new();
            ManagerModule managerModule = new();
            ManagedModule managedModule1 = new();
            ManagedModule managedModule2 = new();

            TestUtils.AssertTask(manager.RegisterModuleAsync(managedModule1.Header));
            TestUtils.AssertTask(manager.RegisterModuleAsync(managerModule.Header));
            TestUtils.AssertTask(manager.RegisterModuleAsync(managedModule2.Header));

            Assert.AreEqual(2, managerModule.List.Count);

            TestUtils.AssertTask(manager.UnregisterModuleAsync(managerModule.Header));

            Assert.AreEqual(0, managerModule.List.Count);



        }



        [TestMethod]
        public void ManagedInterfaceTest()
        {
            /*
                Manager:
                    1. ManagerA:1, version 1.1.0    uses    InterfaceA 1.1
                    2. ManagerA:2, version 1.2.0    uses    InterfaceA 1.1 => conflicts with 1.
                    3. ManagerA:3, version 1.2.0    uses    InterfaceA 3.1 => conflicts with 1.
                    4. ManagerB, version 1.2.0    uses    InterfaceA 3.1 => conflicts with 1.
                    5. ManagerC, version 1.2.0    uses    InterfaceB 1.1 => no conflict, fits managed 6
                Managed:
                    1. InterfaceA:1, version 1.1
                    2. InterfaceA:2, version 1.0 => incompatible
                    3. InterfaceA:3, version 0.1 => incompatible
                    4. InterfaceA:4, version 2.1 => incompatible
                    5. InterfaceA:5, version 1.2 => compatible
                    6. InterfaceB, version 1.1 => no conflict and no fitting manager
            
             
                1. (1/1) add manager then fitting interface
                2. (1/0) remove interface
                3. (0/1) reregister interface and remove manager
                4. (1/1) reregister manager
                5. (2/1) register conflicting manager (same manager type but other version, same interface type and version)
                6. (3/1) register conflicting manager (same manager type but other version, same interface type but other version)
                7. (4/1) register conflicting manager (other manager type, same interface type but other version)
                8. (4/2) register incompatible interface (same major, smaller minor)
                9. (4/3) register incompatible interface (smaller major)
                10. (4/4) register incompatible interface (bigger major)
                11. (4/5) register interface (bigger minor)
                12. (4/6) register interface (other type)
                13. (5/6) register manager (other manager type, other interface type)
             */

            ModuleManager manager = new();

            ManagerVersion = PatternUtils.Version.Create(1, 1, 0);
            InterfaceVersion = PatternUtils.Version.Create(1, 1);
            ManagerModuleA managerA1 = new();
            ManagerVersion = PatternUtils.Version.Create(1, 2, 0);
            InterfaceVersion = PatternUtils.Version.Create(1, 1);
            ManagerModuleA managerA2 = new();
            ManagerVersion = PatternUtils.Version.Create(1, 2, 0);
            InterfaceVersion = PatternUtils.Version.Create(3, 1);
            ManagerModuleA managerA3 = new();
            ManagerVersion = PatternUtils.Version.Create(1, 2, 0);
            InterfaceVersion = PatternUtils.Version.Create(3, 1);
            ManagerModuleB managerB = new();
            ManagerVersion = PatternUtils.Version.Create(1, 2, 0);
            InterfaceVersion = PatternUtils.Version.Create(1, 1);
            ManagerModuleC managerC = new();

            InterfaceVersion = PatternUtils.Version.Create(1, 1);
            InterfaceModuleA interfaceA1 = new();
            InterfaceVersion = PatternUtils.Version.Create(1, 0);
            InterfaceModuleA interfaceA2 = new();
            InterfaceVersion = PatternUtils.Version.Create(0, 1);
            InterfaceModuleA interfaceA3 = new();
            InterfaceVersion = PatternUtils.Version.Create(2, 1);
            InterfaceModuleA interfaceA4 = new();
            InterfaceVersion = PatternUtils.Version.Create(1, 2);
            InterfaceModuleA interfaceA5 = new();
            InterfaceVersion = PatternUtils.Version.Create(1, 1);
            InterfaceModuleB interfaceB = new();

            AsyncBarrier managedRegBarrier = new(1, false);
            AsyncBarrier managerRegBarrier = new(1, false);
            AsyncBarrier managedUnRegBarrier = new(1, false);

            EventHandler<IManagedInterface> managedInterfaceRegistered = (obj, args) =>
            {
                // Assert.AreEqual(managerA.ManagerClass, obj);
                // Assert.AreEqual(managedA.ManagedClass, args);
                managerRegBarrier.SignalAsync().Wait();
            };
            EventHandler<IManagerInterface> registeredOnManager = (obj, args) =>
            {
                // Assert.AreEqual(managerA.ManagerClass, obj);
                // Assert.AreEqual(managedA.ManagedClass, args);
                managedRegBarrier.SignalAsync().Wait();
            };
            EventHandler unregisteredFromManager = (obj, args) =>
            {
                // Assert.AreEqual(managerA.ManagerClass, obj);
                // Assert.AreEqual(managedA.ManagedClass, args);
                managedUnRegBarrier.SignalAsync().Wait();
            };

            managerA1.ManagerClass.RegisteredClass += managedInterfaceRegistered;
            managerA2.ManagerClass.RegisteredClass += managedInterfaceRegistered;
            managerA3.ManagerClass.RegisteredClass += managedInterfaceRegistered;
            managerB.ManagerClass.RegisteredClass += managedInterfaceRegistered;
            managerC.ManagerClass.RegisteredClass += managedInterfaceRegistered;

            interfaceA1.ManagedClass.Registration += registeredOnManager;
            interfaceA2.ManagedClass.Registration += registeredOnManager;
            interfaceA3.ManagedClass.Registration += registeredOnManager;
            interfaceA4.ManagedClass.Registration += registeredOnManager;
            interfaceA5.ManagedClass.Registration += registeredOnManager;
            interfaceB.ManagedClass.Registration += registeredOnManager;

            interfaceA1.ManagedClass.Unregistration += unregisteredFromManager;
            interfaceA2.ManagedClass.Unregistration += unregisteredFromManager;
            interfaceA3.ManagedClass.Unregistration += unregisteredFromManager;
            interfaceA4.ManagedClass.Unregistration += unregisteredFromManager;
            interfaceA5.ManagedClass.Unregistration += unregisteredFromManager;
            interfaceB.ManagedClass.Unregistration += unregisteredFromManager;

            Action<int> reset = (v) =>
            {
                managedRegBarrier = new(v, false);
                managerRegBarrier = new(v, false);
                managedUnRegBarrier = new(v, false);
            };
            Action resetForException = () =>
            {
                managedRegBarrier = new(1, false);
                managerRegBarrier = new(1, false);
                managedUnRegBarrier = new(1, false);
                _ = managedRegBarrier.SignalAsync();
                _ = managerRegBarrier.SignalAsync();
                _ = managedUnRegBarrier.SignalAsync();
            };



            //1. (1 / 1) add manager then fitting interface
            TestUtils.AssertTask(manager.RegisterModuleAsync(managerA1.Header));
            TestUtils.AssertTask(manager.RegisterModuleAsync(interfaceA1.Header));

            TestUtils.AssertTask(managerRegBarrier.WaitAsync());
            TestUtils.AssertTask(managedRegBarrier.WaitAsync());

            Assert.IsTrue(interfaceA1.ManagedClass.IsRegistered);

            //2. (1/0) remove interface
            TestUtils.AssertTask(manager.UnregisterModuleAsync(interfaceA1.Header));
            TestUtils.AssertTask(managedUnRegBarrier.WaitAsync());
            Assert.IsFalse(interfaceA1.ManagedClass.IsRegistered);
            reset(1);

            //3. (0/1) reregister interface and remove manager
            TestUtils.AssertTask(manager.RegisterModuleAsync(interfaceA1.Header));
            TestUtils.AssertTask(managerRegBarrier.WaitAsync());
            TestUtils.AssertTask(managedRegBarrier.WaitAsync());

            Assert.IsTrue(interfaceA1.ManagedClass.IsRegistered);

            TestUtils.AssertTask(manager.UnregisterModuleAsync(managerA1.Header));

            TestUtils.AssertTask(managedUnRegBarrier.WaitAsync());
            Assert.IsFalse(interfaceA1.ManagedClass.IsRegistered);
            reset(1);

            //4. (1/1) reregister manager
            TestUtils.AssertTask(manager.RegisterModuleAsync(managerA1.Header));
            TestUtils.AssertTask(managerRegBarrier.WaitAsync());
            TestUtils.AssertTask(managedRegBarrier.WaitAsync());

            Assert.IsTrue(interfaceA1.ManagedClass.IsRegistered);

            resetForException();//0 => throw exception if one of the handlers is called

            //5. (2/1) register conflicting manager(same manager type but other version, same interface type and version)
            TestUtils.AssertException<ModuleIncompatibleException>(manager.RegisterModuleAsync(managerA2.Header));

            //6. (3/1) register conflicting manager(same manager type but other version, same interface type but other version)
            TestUtils.AssertException<ModuleIncompatibleException>(manager.RegisterModuleAsync(managerA3.Header));

            //7. (4/1) register conflicting manager(other manager type, same interface type but other version)
            TestUtils.AssertException<ModuleIncompatibleException>(manager.RegisterModuleAsync(managerB.Header));

            //8. (4/2) register incompatible interface (same major, smaller minor)
            TestUtils.AssertTask(manager.RegisterModuleAsync(interfaceA2.Header));

            //9. (4/3) register incompatible interface (smaller major)
            TestUtils.AssertTask(manager.RegisterModuleAsync(interfaceA3.Header));

            //10. (4/4) register incompatible interface (bigger major)
            TestUtils.AssertTask(manager.RegisterModuleAsync(interfaceA4.Header));

            //11. (4/5) register interface (bigger minor)
            reset(1);
            TestUtils.AssertTask(manager.RegisterModuleAsync(interfaceA5.Header));
            TestUtils.AssertTask(managerRegBarrier.WaitAsync());
            TestUtils.AssertTask(managedRegBarrier.WaitAsync());

            //12. (4/6) register interface (other type)
            resetForException();
            TestUtils.AssertTask(manager.RegisterModuleAsync(interfaceB.Header));

            //13. (5/6) register manager(other manager type, other interface type)
            reset(1);
            TestUtils.AssertTask(manager.RegisterModuleAsync(managerC.Header));
            TestUtils.AssertTask(managerRegBarrier.WaitAsync());
            TestUtils.AssertTask(managedRegBarrier.WaitAsync());

        }

        [TestMethod]
        public void DependencyModuleTest()
        {
            ModuleManager manager = new();

            ModuleA a = new();
            ModuleA aa = new();
            ModuleA2 a2 = new();
            ModuleB b = new();
            ModuleB2 b2 = new();
            ModuleC c = new();
            ModuleD d = new();

            /*
                A   -> A1.0
                AA  -> A1.0
                A2  -> A1.1
                B   -> B1.0
                    <- A1.0
                B2  -> B1.1
                    <- A1.1
                C   -> C2.0
                    <- A1.0
                    <- B1.1
                D   -> 
                    <- C1.0
             */

            //1. error: register module with dependency on one not registered interface (B)

            //2. register module without dependencies (A)

            //3. error: register module with dependency with only one present dependency (C)

            //4. error: register module with same provided interface (AA, A vs AA)
            //5. error: register module with same provided interface but other version (A2, A vs A2)
            //6. error: register module with dependency on incompatible registered interface (B2, A <- B2)

            //7. register module with dependency on present interface (B, A <- B)

            //8. error: unregister module with used interface (A, A <- B)

            //9. unregister module that uses other interface (B, A <- B)

            //10. unregister module with previously used interface (A, A <-/- B)

            //11. register module with previously conflicting interface (A2, A vs A2)

            //12. register module with dependency on older version of present interface (B, A2 <- B)

            //13. unregister module B, register module B2

            //14. register module with multiple dependencies (C, C <- A, B)            

            //15. error: register module dependent on older but incompatible version of present interface (D, C <- D)
            //16. error: start module with inactive dependencies (B2)

            //17. start modules and dependencies (B2 -> A2)

            //18. start all modules

            //19. error: stop module with active dependents (A)

            //20. stop module and active dependents (B)

            //21. error: reset all modules with not all modules stopped

            //22. stop all modules

            //23. reset all modules

            //
            //
            //

            var ex = TestUtils.AssertException<ModuleIncompatibleException>(manager.RegisterModuleAsync(c.Header));
            Assert.IsTrue(!ex.Report.HasInterfaceConflicts
                        && !ex.Report.HasManagedInterfaceConflicts
                        && ex.Report.HasUnsatisfiedDependencies);
            CollectionAssert.AreEquivalent(new[] { new DependencyError() { Type = DependencyErrorType.NotFound, ExpectedInterface = c.Header.InterfaceDependencies[0] },
                                                   new DependencyError() { Type = DependencyErrorType.NotFound, ExpectedInterface = c.Header.InterfaceDependencies[1] },},
                                           ex.Report.DependencyErrors);

            //1. error: register module with dependency on one not registered interface (B)
            ex = TestUtils.AssertException<ModuleIncompatibleException>(manager.RegisterModuleAsync(b.Header));
            Assert.IsTrue(!ex.Report.HasInterfaceConflicts
                        && !ex.Report.HasManagedInterfaceConflicts
                        && ex.Report.HasUnsatisfiedDependencies);
            CollectionAssert.AreEquivalent(new[] { new DependencyError() { Type = DependencyErrorType.NotFound, ExpectedInterface = b.Header.InterfaceDependencies[0] },},
                                           ex.Report.DependencyErrors);

            //2. register module without dependencies (A)
            TestUtils.AssertTask(manager.RegisterModuleAsync(a.Header));

            //3. error: register module with dependency with only one present dependency (C)
            ex = TestUtils.AssertException<ModuleIncompatibleException>(manager.RegisterModuleAsync(c.Header));
            Assert.IsTrue(!ex.Report.HasInterfaceConflicts
                         && !ex.Report.HasManagedInterfaceConflicts
                         && ex.Report.HasUnsatisfiedDependencies);
            CollectionAssert.AreEquivalent(new[] { new DependencyError() { Type = DependencyErrorType.NotFound, ExpectedInterface = c.Header.InterfaceDependencies[1] }, },
                                           ex.Report.DependencyErrors);

            //4. error: register module with same provided interface (AA, A vs AA)
            ex = TestUtils.AssertException<ModuleIncompatibleException>(manager.RegisterModuleAsync(aa.Header));
            Assert.IsTrue(ex.Report.HasInterfaceConflicts
                         && !ex.Report.HasManagedInterfaceConflicts
                         && !ex.Report.HasUnsatisfiedDependencies);
            CollectionAssert.AreEquivalent(new[] { a.A },
                                           ex.Report.InterfaceConflicts);

            //5. error: register module with same provided interface but other version (A2, A vs A2)
            ex = TestUtils.AssertException<ModuleIncompatibleException>(manager.RegisterModuleAsync(a2.Header));
            Assert.IsTrue(ex.Report.HasInterfaceConflicts
                         && !ex.Report.HasManagedInterfaceConflicts
                         && !ex.Report.HasUnsatisfiedDependencies);
            CollectionAssert.AreEquivalent(new[] { a.A},
                                           ex.Report.InterfaceConflicts);

            //6. error: register module with dependency on incompatible registered interface (B2, A <- B2)
            //register module B2 -> A1.1 expected, A1.0 present 
            ex = TestUtils.AssertException<ModuleIncompatibleException>(manager.RegisterModuleAsync(b2.Header));
            Assert.IsTrue(!ex.Report.HasInterfaceConflicts
                         && !ex.Report.HasManagedInterfaceConflicts
                         && ex.Report.HasUnsatisfiedDependencies);
            CollectionAssert.AreEquivalent(new[] { new DependencyError() { Type = DependencyErrorType.NotCompatible, ExpectedInterface = b2.Header.InterfaceDependencies[0], ActualInterface = a.A.Info }, },
                                           ex.Report.DependencyErrors);

            //7. register module with dependency on present interface (B, A <- B)
            TestUtils.AssertTask(manager.RegisterModuleAsync(b.Header));

            //8. error: unregister module with used interface (A, A <- B)
            TestUtils.AssertException<DependencyException>(manager.UnregisterModuleAsync(a.Header));

            //9. unregister module that uses other interface (B, A <- B)
            TestUtils.AssertTask(manager.UnregisterModuleAsync(b.Header));

            //10. unregister module with previously used interface (A, A <-/- B)
            TestUtils.AssertTask(manager.UnregisterModuleAsync(a.Header));

            //11. register module with previously conflicting interface (A2, A vs A2)
            TestUtils.AssertTask(manager.RegisterModuleAsync(a2.Header));

            //12. register module with dependency on older version of present interface (B, A2 <- B)
            TestUtils.AssertTask(manager.RegisterModuleAsync(b.Header));

            //13. unregister module B, register module B2
            TestUtils.AssertTask(manager.UnregisterModuleAsync(b.Header));
            TestUtils.AssertTask(manager.RegisterModuleAsync(b2.Header));

            //14. register module with multiple dependencies (C, C <- A, B)    
            TestUtils.AssertTask(manager.RegisterModuleAsync(c.Header));

            //15. error: register module dependent on older but incompatible version of present interface (D, C <- D)
            ex = TestUtils.AssertException<ModuleIncompatibleException>(manager.RegisterModuleAsync(d.Header));
            Assert.IsTrue(!ex.Report.HasInterfaceConflicts
                         && !ex.Report.HasManagedInterfaceConflicts
                         && ex.Report.HasUnsatisfiedDependencies);
            CollectionAssert.AreEquivalent(new[] { new DependencyError() { Type = DependencyErrorType.NotCompatible, ExpectedInterface = d.Header.InterfaceDependencies[0], ActualInterface = c.C.Info}, },
                                           ex.Report.DependencyErrors);

            //16. error: start module with inactive dependencies (B2)
            TestUtils.AssertException<DependencyException>(manager.StartModuleAsync(b2.Header, false));

            //17. start modules and dependencies (B2 -> A2)
            TestUtils.AssertTask(manager.StartModuleAsync(b2.Header, true));

            //18. start all modules
            TestUtils.AssertTask(manager.StartAllModulesAsync());

            //19. error: stop module with active dependents (A)
            TestUtils.AssertException<DependencyException>(manager.StopModuleAsync(a2.Header, false));

            //20. stop module and active dependents (B)
            TestUtils.AssertTask(manager.StopModuleAsync(b2.Header, true));

            //21. error: reset all modules with not all modules stopped
            TestUtils.AssertException<InvalidOperationException>(manager.ResetAllModulesAsync());

            //22. stop all modules
            TestUtils.AssertTask(manager.StartAllModulesAsync());
            TestUtils.AssertTask(manager.StopAllModulesAsync());

            //23. reset all modules
            TestUtils.AssertTask(manager.ResetAllModulesAsync());

            //24. start all modules
            TestUtils.AssertTask(manager.StartAllModulesAsync());


        }

    }
}

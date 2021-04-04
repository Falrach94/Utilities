using PatternUtils.Module_Framework.Data;
using PatternUtils.Module_Framework.Data.builder;
using PatternUtils.Module_Framework.Impl;
using PatternUtils.Module_Framework.Interfaces;
using SyncUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternUtils.Module_Framework
{
    /// <summary>
    /// Provides control access to a modules state.
    /// </summary>
    public abstract class ModuleControl
    {
        private SemaphoreLock _lock = new();

        public ModuleState State { get; private set; }
        public ModuleHeader Header { get; }

        protected abstract void DefineModule(ModuleBuilder builder);
        protected abstract Task ResetAsync();
        protected abstract Task StartAsync();
        protected abstract Task StopAsync();
        protected abstract Task UninitializeAsync();
        protected abstract Task InitializeAsync(IInterfaceProvider interfaceProvider, LockToken providerLockToken);

        /// <summary>
        /// Creates module data according to implementation.
        /// </summary>
        /// <exception cref="InitializationFailedException">custom initialization failed</exception>
        /// <exception cref="ArgumentNullException"></exception>
        protected ModuleControl(string name, PatternUtils.Version version)
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            ModuleBuilder builder = new();
            
            try
            {
                if(!Task.Run(() => DefineModule(builder)).Wait(100))
                {
                    throw new TimeoutException("DefineModule timed out!");
                }

                builder.SetControl(this)
                       .SetInfo(name, version);
                Header = builder.CreateModule();
            }
            catch(Exception ex)
            {
                throw new InitializationFailedException("Creating module data failed!", ex);
            }
        }

        /// <summary>
        /// Initializes module. Must only be called if all interface dependencies are met. 
        /// Sucessfully initialized module is in state stopped.
        /// </summary>
        /// <param name="interfaceProvider"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidModuleStateException"></exception>
        /// <exception cref="ModuleMethodException"></exception>
        /// <exception cref="TimeoutException"></exception>
        public async Task InitializeAsync(IInterfaceProvider interfaceProvider, TimeSpan timeout, LockToken providerLockToken)
        {
            if (interfaceProvider is null)
            {
                throw new ArgumentNullException(nameof(interfaceProvider));
            }

            using var token = await _lock.LockAsync();

            if(State != ModuleState.Uninitialized)
            {
                throw new InvalidModuleStateException("Initialization is only allowed in state 'Uninitialized'!");
            }

            try
            {
                if (!Task.Run(() => InitializeAsync(interfaceProvider, providerLockToken)).Wait(timeout))
                {
                    throw new TimeoutException("Initialization timed out!");
                }
            }
            catch (TimeoutException)
            {
                State = ModuleState.Error;
                throw; 
            }
            catch (Exception ex)
            {
                State = ModuleState.Error;
                throw new ModuleMethodException("Initialization failed!", ex);
            }

            State = ModuleState.Stopped;
        }


        /// <summary>
        /// Tansitions module state to required state.
        /// 'Reset' state will automatically transition back to 'Stopped' when complete.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        /// <exception cref="InvalidModuleStateException">transition not allowed</exception>
        /// <exception cref="ModuleMethodException">module method failed</exception>
        /// <exception cref="TimeoutException">module method failed</exception>
        public async Task TransitionStateAsync(ModuleState state, TimeSpan timeout)
        {
            using var token = await _lock.LockAsync();

            Task task;

            switch(state)
            {
                case ModuleState.Error:
                    throw new InvalidModuleStateException("Module can't be forced into error state.");
                case ModuleState.Resetting:
                    if(State != ModuleState.Stopped)
                    {
                        throw new InvalidModuleStateException("Reset is only allowed in 'Stopped' state!");
                    }
                    task = Task.Run(ResetAsync);
                    break;
                case ModuleState.Running:
                    if (State != ModuleState.Stopped)
                    {
                        throw new InvalidModuleStateException("Start is only allowed in 'Stopped' state!");
                    }
                    task = Task.Run(StartAsync);
                    break;
                case ModuleState.Stopped:
                    if (State != ModuleState.Running)
                    {
                        throw new InvalidModuleStateException("Stop is only allowed in 'Running' state!");
                    }
                    task = Task.Run(StopAsync);
                    break;
                case ModuleState.Uninitialized:
                    if (State != ModuleState.Stopped)
                    {
                        throw new InvalidModuleStateException("Uninitialize is only allowed in 'Stopped' state!");
                    }
                    task = Task.Run(UninitializeAsync);
                    break;
                default:
                    throw new InvalidModuleStateException("Unrecognized state!");
            }

            State = state;
            try
            {
                if (!task.Wait(timeout))
                {
                    State = ModuleState.Error;
                    throw new TimeoutException($"Modules '{State}' method timed out!");
                }
            }
            catch (TimeoutException)
            {
                State = ModuleState.Error;
                throw;
            }
            catch (Exception ex)
            {
                State = ModuleState.Error;
                throw new ModuleMethodException($"Modules '{State}' method failed! (Exception)", ex);
            }

            if (State == ModuleState.Resetting)
            {
                State = ModuleState.Stopped;
            }
        }
    }
}

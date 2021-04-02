using PatternUtils.Module_Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternUtils.Module_Framework.Data
{
    internal class ReportBuilder
    {
        private List<DependencyError> _dependencyErrors = new();
        private List<IModuleInterface> _interfaceConflicts = new();
        private List<IManagerInterface> _managedInterfaceConflicts = new();
        private List<ModuleHeader> _dependencyModules = new();
        public void AddConflict(IModuleInterface interf)
        {
            _interfaceConflicts.Add(interf);
        }
        public void AddManagerConflict(IManagerInterface interf)
        {
            _managedInterfaceConflicts.Add(interf);
        }
        public void AddIncompatibleDependencyError(InterfaceInfo expected, InterfaceInfo actual)
        {
            _dependencyErrors.Add(new DependencyError() { ExpectedInterface = expected, ActualInterface = actual, Type = DependencyErrorType.NotCompatible });
        }
        public void AddUnsatisfiedDependencyError(InterfaceInfo expected)
        {
            _dependencyErrors.Add(new DependencyError() { ExpectedInterface = expected, Type = DependencyErrorType.NotFound });
        }
        public void AddDependencyModule(ModuleHeader module)
        {
            if (!_dependencyModules.Contains(module))
            {
                _dependencyModules.Add(module);
            }
        }
        public CompatibilityReport CreateReport()
        {
            return new CompatibilityReport(_dependencyErrors, _interfaceConflicts, _managedInterfaceConflicts, _dependencyModules);
        }
    }
}

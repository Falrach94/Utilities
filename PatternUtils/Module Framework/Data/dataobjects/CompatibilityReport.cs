using PatternUtils.Module_Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternUtils.Module_Framework.Data
{
    public record CompatibilityReport
    {
        public CompatibilityReport(List<DependencyError> dependencyErrors,
                                    List<IModuleInterface> interfaceConflicts, 
                                    List<IManagerInterface> managedInterfaceConflicts,
                                    List<ModuleHeader> moduleDependencies)
        {
            DependencyErrors = dependencyErrors ?? throw new ArgumentNullException(nameof(dependencyErrors));
            InterfaceConflicts = interfaceConflicts ?? throw new ArgumentNullException(nameof(interfaceConflicts));
            ManagedInterfaceConflicts = managedInterfaceConflicts ?? throw new ArgumentNullException(nameof(managedInterfaceConflicts));
            ModuleDependencies = moduleDependencies ?? throw new ArgumentNullException(nameof(moduleDependencies));
        }

        public bool IsCompatible => !HasUnsatisfiedDependencies && !HasInterfaceConflicts && !HasManagedInterfaceConflicts;
        public bool HasUnsatisfiedDependencies => DependencyErrors.Count != 0;
        public bool HasInterfaceConflicts => InterfaceConflicts.Count != 0;
        public bool HasManagedInterfaceConflicts => ManagedInterfaceConflicts.Count != 0;

        public List<DependencyError> DependencyErrors { get; }
        public List<IModuleInterface> InterfaceConflicts { get; }
        public List<IManagerInterface> ManagedInterfaceConflicts { get; }

        public List<ModuleHeader> ModuleDependencies { get; }

    }
}

﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TermInjectorPlus {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.12.0.0")]
    internal sealed partial class TermInjectorPlusSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static TermInjectorPlusSettings defaultInstance = ((TermInjectorPlusSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new TermInjectorPlusSettings())));
        
        public static TermInjectorPlusSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("editrules")]
        public string EditRuleDir {
            get {
                return ((string)(this["EditRuleDir"]));
            }
            set {
                this["EditRuleDir"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("TermInjectorPlus")]
        public string TermInjectorDir {
            get {
                return ((string)(this["TermInjectorDir"]));
            }
            set {
                this["TermInjectorDir"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("configs")]
        public string ConfigDir {
            get {
                return ((string)(this["ConfigDir"]));
            }
            set {
                this["ConfigDir"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("logs")]
        public string LogDir {
            get {
                return ((string)(this["LogDir"]));
            }
            set {
                this["LogDir"] = value;
            }
        }
    }
}

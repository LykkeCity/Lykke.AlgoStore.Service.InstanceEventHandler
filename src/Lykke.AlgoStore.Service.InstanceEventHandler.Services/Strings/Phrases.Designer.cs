﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Services.Strings {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Phrases {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Phrases() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Lykke.AlgoStore.Service.InstanceEventHandler.Services.Strings.Phrases", typeof(Phrases).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to All function names must be provided.
        /// </summary>
        public static string AllFunctionNamesMustBeProvided {
            get {
                return ResourceManager.GetString("AllFunctionNamesMustBeProvided", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Authorization token does not correspond to provided Instance Ids.
        /// </summary>
        public static string AuthorizationTokenDoesNotCorrespondToProvidedInstanceIds {
            get {
                return ResourceManager.GetString("AuthorizationTokenDoesNotCorrespondToProvidedInstanceIds", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Candle values cannot be empty.
        /// </summary>
        public static string CandleValuesCannotBeEmpty {
            get {
                return ResourceManager.GetString("CandleValuesCannotBeEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Function values cannot be empty.
        /// </summary>
        public static string FunctionValuesCannotBeEmpty {
            get {
                return ResourceManager.GetString("FunctionValuesCannotBeEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to All candle values must have InstanceId provided.
        /// </summary>
        public static string InstanceIdForAllCandleValues {
            get {
                return ResourceManager.GetString("InstanceIdForAllCandleValues", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to All function values must have InstanceId provided.
        /// </summary>
        public static string InstanceIdForAllFunctionValues {
            get {
                return ResourceManager.GetString("InstanceIdForAllFunctionValues", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to All trade values must have InstanceId provided.
        /// </summary>
        public static string InstanceIdForAllTradeValues {
            get {
                return ResourceManager.GetString("InstanceIdForAllTradeValues", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot save more than 100 function chart update records per batch.
        /// </summary>
        public static string MaxRecordsPerBatchReached {
            get {
                return ResourceManager.GetString("MaxRecordsPerBatchReached", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to InstanceId must be the same for all candle values.
        /// </summary>
        public static string SameInstanceIdForAllCandleValues {
            get {
                return ResourceManager.GetString("SameInstanceIdForAllCandleValues", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to InstanceId must be the same for all function values.
        /// </summary>
        public static string SameInstanceIdForAllFunctionValues {
            get {
                return ResourceManager.GetString("SameInstanceIdForAllFunctionValues", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to InstanceId must be the same for all trade values.
        /// </summary>
        public static string SameInstanceIdForAllTradeValues {
            get {
                return ResourceManager.GetString("SameInstanceIdForAllTradeValues", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Trade values cannot be empty.
        /// </summary>
        public static string TradeValuesCannotBeEmpty {
            get {
                return ResourceManager.GetString("TradeValuesCannotBeEmpty", resourceCulture);
            }
        }
    }
}

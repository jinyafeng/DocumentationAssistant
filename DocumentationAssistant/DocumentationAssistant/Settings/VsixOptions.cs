using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Threading;

namespace DocumentationAssistant.Settings
{
    public class VsixOptions
    {
        /// <summary>
        ///   DocumentationAssistant.Vsix2022Package GUID string.
        /// </summary>
        public const string PackageGuidString = "3e24c848-8f26-42ef-8e4a-0cbd613f9481";

        public const string Version = "2.0";

    }

    public class BridgedOptions
    {
        private static BridgedOptions _options;

        public BridgedOptions(bool isEnabledForPublishMembersOnly, bool useNaturalLanguageForReturnNode)
        {
            IsEnabledForPublishMembersOnly = isEnabledForPublishMembersOnly;
            UseNaturalLanguageForReturnNode = useNaturalLanguageForReturnNode;
        }
        public bool IsEnabledForPublishMembersOnly { get; set; }

        public bool UseNaturalLanguageForReturnNode { get; set; }

        public static Func<BridgedOptions> RegisterOptionLoaderCallback {get;set;}

        public static BridgedOptions Options
        {
            get
            {
                return RegisterOptionLoaderCallback?.Invoke();
            }
            set
            {
                _options = value;
            }
        }
    }

   
}

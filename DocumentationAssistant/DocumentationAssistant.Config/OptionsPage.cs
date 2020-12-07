using Microsoft.VisualStudio.Shell;
using System.ComponentModel;

namespace DocumentationAssistant.Config
{
	public class OptionsPage : DialogPage
	{
		[Category("Options")]
		[DisplayName("OnlyEnabledToPublishMembers")]
		[Description("Only public members will be checked when the value is true.")]
		public bool EnabledToPublishMembersOnly { get; set; } = false;
	}
}

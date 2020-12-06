using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentatioAssistant.Configuration
{
	public class OptionsPage : DialogPage
	{
		[Category("Options")]
		[DisplayName("OnlyEnabledToPublishMembers")]
		[Description("Only public members will be checked when the value is true.")]
		public bool EnabledToPublishMembersOnly { get; set; } = false;
	}
}

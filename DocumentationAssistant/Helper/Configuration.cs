using DocumentationAssistant.Config;
using Microsoft.VisualStudio.Shell;

namespace DocumentationAssistant.Helper
{
	public static class Configuration
	{
		private static IOptionsService _optionsService = null;

		public static bool IsEnabledForPublishMembersOnly
		{
			get
			{
				if (_optionsService == null)
				{
					try
					{
						_optionsService = Package.GetGlobalService(typeof(SOptionsService)) as IOptionsService;
					}
					catch (System.Exception)
					{
						// Do unit test will cause the exception: "Message=Could not load type 'System.Windows.Threading.Dispatcher' from assembly 'WindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35'."
						// Don't know why this cause error.
					}
				}

				var result = false;
				if (_optionsService != null)
				{
					result = _optionsService.IsEnabledToPublicMembersOnly();
				}

				return result;
			}
		}
	}
}

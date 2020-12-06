using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using Task = System.Threading.Tasks.Task;

namespace DocumentatioAssistant.Configuration
{
	/// <summary>
	/// This is the class that implements the package exposed by this assembly.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The minimum requirement for a class to be considered a valid package for Visual Studio
	/// is to implement the IVsPackage interface and register itself with the shell.
	/// This package uses the helper classes defined inside the Managed Package Framework (MPF)
	/// to do it: it derives from the Package class that provides the implementation of the
	/// IVsPackage interface and uses the registration attributes defined in the framework to
	/// register itself and its components with the shell. These attributes tell the pkgdef creation
	/// utility what data to put into .pkgdef file.
	/// </para>
	/// <para>
	/// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
	/// </para>
	/// </remarks>
	[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
	[Guid(OptionsPackage.PackageGuidString)]
	[ProvideService(typeof(SOptionsService))]
	[ProvideOptionPage(typeof(OptionsPage), "Documentation Assistant", "General", 0, 0, true)]
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
	public sealed class OptionsPackage : AsyncPackage
	{
		/// <summary>
		/// OptionsPackage GUID string.
		/// </summary>
		public const string PackageGuidString = "8e146ab5-3ad6-48c8-b3ac-6fcc104521f6";

		/// <summary>
		/// Initializes a new instance of the <see cref="OptionsPackage"/> class.
		/// </summary>
		public OptionsPackage()
		{
			// Inside this method you can place any initialization code that does not require
			// any Visual Studio service because at this point the package object is created but
			// not sited yet inside Visual Studio environment. The place to do all the other
			// initialization is the Initialize method.
		}

		/// <summary>
		/// Initialization of the package; this method is called right after the package is sited, so this is the place
		/// where you can put all the initialization code that rely on services provided by VisualStudio.
		/// </summary>
		/// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
		/// <param name="progress">A provider for progress updates.</param>
		/// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
		protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
		{
			// When initialized asynchronously, the current thread may be a background thread at this point.
			// Do any initialization that requires the UI thread after switching to the UI thread.
			await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

			((IServiceContainer)this).AddService(typeof(SOptionsService), CreateService, true);
        }

		private object CreateService(IServiceContainer container, Type serviceType)
		{
			if (typeof(SOptionsService) == serviceType)
				return new OptionsService(this);
			return null;
		}
	}

	public class OptionsService : SOptionsService, IOptionsService
	{
		private Package serviceProvider;

		public OptionsService(Package sp)
		{
			serviceProvider = sp;
		}

		bool IOptionsService.IsEnabledToPublicMembersOnly()
		{
			OptionsPage page = (OptionsPage)this.serviceProvider.GetDialogPage(typeof(OptionsPage));
			return page.EnabledToPublishMembersOnly;
		}
	}
	public interface SOptionsService
	{
	}
	public interface IOptionsService
	{
		bool IsEnabledToPublicMembersOnly();
	}

}

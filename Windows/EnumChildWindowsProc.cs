using System;

namespace Remotion.WindowFinder.Windows
{
  /// <summary>
  /// A callback used by <see cref="IWin32WindowsNativeMethods.EnumChildWindows"/> to handle each window found.
  /// </summary>
  /// <seealso href="http://msdn.microsoft.com/en-us/library/ms633498.aspx"/>
  public delegate bool EnumChildWindowsProc (IntPtr windowHandle, WindowFinderEnumChildWindowsProcContext context);
}
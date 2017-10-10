// This file is part of the re-motion Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// re-motion is free software; you can redistribute it and/or modify it under 
// the terms of the GNU Lesser General Public License as published by the 
// Free Software Foundation; either version 2.1 of the License, 
// or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Runtime.InteropServices;
using System.Text;
using Remotion.Utilities;

namespace Remotion.WindowFinder.Windows
{
  /// <summary>
  /// Implementation of <see cref="IWin32WindowsNativeMethods"/>. Delegates calls to external methods
  /// </summary>
  public sealed class Win32WindowsNativeMethods : IWin32WindowsNativeMethods
  {
    [DllImport ("user32.dll", SetLastError = true)]
    private static extern bool EnumWindows (EnumWindowsProc lpEnumFunc, WindowFinderEnumWindowsProcContext data);

    [DllImport ("user32.dll", SetLastError = true)]
    private static extern bool EnumChildWindows (IntPtr hwndParent, EnumChildWindowsProc lpEnumFunc, WindowFinderEnumChildWindowsProcContext data);

    [DllImport ("user32.dll", SetLastError = true)]
    private static extern uint GetWindowThreadProcessId (IntPtr hWnd, out int lpdwProcessId);

    [DllImport ("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern int GetClassName (IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

    [DllImport ("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern int GetWindowText (IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    bool IWin32WindowsNativeMethods.EnumWindows (EnumWindowsProc enumWindowsCallback, WindowFinderEnumWindowsProcContext context)
    {
      ArgumentUtility.CheckNotNull ("enumWindowsCallback", enumWindowsCallback);
      ArgumentUtility.CheckNotNull ("context", context);

      return EnumWindows (enumWindowsCallback, context);
    }

    void IWin32WindowsNativeMethods.EnumChildWindows (
        IntPtr parentWindowHandle, EnumChildWindowsProc enumWindowsCallback, WindowFinderEnumChildWindowsProcContext context)
    {
      ArgumentUtility.CheckNotNull ("enumWindowsCallback", enumWindowsCallback);
      ArgumentUtility.CheckNotNull ("context", context);

      EnumChildWindows (parentWindowHandle, enumWindowsCallback, context);
    }

    int IWin32WindowsNativeMethods.GetWindowThreadProcessID (IntPtr windowHandle)
    {
      int processID;
      uint threadID = GetWindowThreadProcessId (windowHandle, out processID);
      if (threadID == 0)
        return 0;
      return processID;
    }

    int IWin32WindowsNativeMethods.GetClassName (IntPtr windowHandle, StringBuilder className, int classNameMaxLength)
    {
      ArgumentUtility.CheckNotNull ("className", className);

      return GetClassName (windowHandle, className, classNameMaxLength);
    }

    int IWin32WindowsNativeMethods.GetWindowText (IntPtr windowHandle, StringBuilder windowText, int windowTextMaxLength)
    {
      ArgumentUtility.CheckNotNull ("windowText", windowText);

      return GetWindowText (windowHandle, windowText, windowTextMaxLength);
    }

    int IWin32WindowsNativeMethods.GetLastWin32Error ()
    {
      return Marshal.GetLastWin32Error();
    }
  }
}
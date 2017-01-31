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
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using Remotion.Utilities;

namespace Remotion.WindowFinder.Windows
{
  /// <summary>
  /// Win32-implementation of the <see cref="IWindowFinder"/> interface.
  /// </summary>
  public sealed class NativeWindowFinder : IWindowFinder
  {
    private class Result
    {
      public bool IsMatch;
      public string Value;
    }

    private readonly IWin32WindowsNativeMethods _nativeMethods;
    private readonly int _currentProcessID;

    public NativeWindowFinder (IWin32WindowsNativeMethods nativeMethods)
    {
      ArgumentUtility.CheckNotNull ("nativeMethods", nativeMethods);

      _nativeMethods = nativeMethods;
      var currentProcess = Process.GetCurrentProcess();
      _currentProcessID = currentProcess.Id;
    }

    public WindowInformation[] FindWindows (WindowFilterCriteria filterCriteria)
    {
      ArgumentUtility.CheckNotNull ("filterCriteria", filterCriteria);

      var context = CreateWindowFinderEnumWindowsProcContext (filterCriteria);

      if (!_nativeMethods.EnumWindows (EnumWindowsCallback, context))
        throw CreateWin32ExceptionForError (_nativeMethods.GetLastWin32Error(), "Error executing 'EnumWindows'.");

      return context.Windows.ToArray();
    }

    private WindowFinderEnumWindowsProcContext CreateWindowFinderEnumWindowsProcContext (WindowFilterCriteria filterCriteria)
    {
      return new WindowFinderEnumWindowsProcContext (
          _nativeMethods,
          filterCriteria.ExcludeOwnProcess ? _currentProcessID : (int?) null,
          filterCriteria.ProcessID,
          filterCriteria.ClassName,
          filterCriteria.WindowTitle,
          filterCriteria.IncludeChildWindows
          );
    }

    /// <remarks>Must be static because the delegate is passed to unmanaged code.</remarks>>
    private static bool EnumWindowsCallback (IntPtr windowHandle, WindowFinderEnumWindowsProcContext context)
    {
      int processID = context.NativeMethods.GetWindowThreadProcessID (windowHandle);
      if (processID == 0)
        return true;

      if (context.OwnProcessIDConstraint.HasValue && processID == context.OwnProcessIDConstraint.Value)
        return true;

      if (context.ProcessIDConstraint.HasValue && processID != context.ProcessIDConstraint.Value)
        return true;

      var classNameResult = MatchClassName (context, windowHandle);
      if (classNameResult == null)
        return true;

      var windowTextResult = MatchWindowText (context, windowHandle);
      if (windowTextResult == null)
        return true;

      var windowInformation = new WindowInformation (
          processID,
          windowHandle,
          classNameResult.Value,
          windowTextResult.Value,
          null);

      if (classNameResult.IsMatch && windowTextResult.IsMatch)
        context.Windows.Add (windowInformation);

      if (context.IncludeChildWindows)
        FindChildWindows (windowHandle, context, windowInformation);

      return true;
    }

    private static void FindChildWindows (IntPtr windowHandle, WindowFinderEnumWindowsProcContext context, WindowInformation windowInformation)
    {
      var childWindowContext = new WindowFinderEnumChildWindowsProcContext (
          context.NativeMethods,
          context.ClassNameConstraint,
          context.WindowTextConstraint,
          windowInformation);

      context.NativeMethods.EnumChildWindows (windowHandle, EnumChildWindowsCallback, childWindowContext);
      context.Windows.AddRange (childWindowContext.Windows);
    }

    private static bool EnumChildWindowsCallback (IntPtr windowHandle, WindowFinderEnumChildWindowsProcContext context)
    {
      var classNameResult = MatchClassName (context, windowHandle);
      if (classNameResult == null)
        return true;

      var windowTextResult = MatchWindowText (context, windowHandle);
      if (windowTextResult == null)
        return true;

      var windowInformation = new WindowInformation (
          context.ParentWindow.ProcessID,
          windowHandle,
          classNameResult.Value,
          windowTextResult.Value,
          context.ParentWindow);

      if (classNameResult.IsMatch && windowTextResult.IsMatch)
        context.Windows.Add (windowInformation);

      return true;
    }

    private static Result MatchClassName (WindowFinderEnumWindowsProcContextBase context, IntPtr windowHandle)
    {
      var className = new StringBuilder (256);

      var length = context.NativeMethods.GetClassName (windowHandle, className, className.Capacity);
      if (length == 0)
      {
        // Marshal.GetLastWin32Error could be used to retrieve the detailed error information but since the error is not logged, there's no need.
        return null;
      }

      var result = new Result
                   {
                       Value = className.ToString(),
                       IsMatch = context.ClassNameConstraint == null || context.ClassNameConstraint.IsMatch (className.ToString())
                   };

      return result;
    }

    private static Result MatchWindowText (WindowFinderEnumWindowsProcContextBase context, IntPtr windowHandle)
    {
      var windowText = new StringBuilder (1024);

      context.NativeMethods.GetWindowText (windowHandle, windowText, windowText.Capacity);
      // ignores return value (length). Window text with length 0 can be either a window with an empty window text or an error.

      var result = new Result
                   {
                       Value = windowText.ToString(),
                       IsMatch = context.WindowTextConstraint == null || context.WindowTextConstraint.IsMatch (windowText.ToString())
                   };

      return result;
    }

    private static Win32Exception CreateWin32ExceptionForError (int lastError, string errorMessage)
    {
      return new Win32Exception (lastError, errorMessage + " " + new Win32Exception (lastError).Message);
    }
  }
}
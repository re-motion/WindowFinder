// This file is part of re-vision (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 

using System;
using System.Text;

namespace Remotion.WindowFinder.Windows
{
  /// <summary>
  /// Declares the native methods used by the <see cref="NativeWindowFinder"/>. See <see cref="Win32WindowsNativeMethods"/> for the implementation. 
  /// </summary>
  public interface IWin32WindowsNativeMethods
  {
    /// <summary>
    /// Wrapps EnumWindows.
    /// </summary>
    /// <returns>
    /// If the function succeeds, the return value is <see langword="true"/>.
    /// If the function fails, the return value is <see langword="false"/>. 
    /// To get extended error information, call <see cref="GetLastWin32Error"/>.
    /// </returns>
    /// <seealso href="http://msdn.microsoft.com/en-us/library/ms633497.aspx"/>.
    bool EnumWindows (EnumWindowsProc enumWindowsCallback, WindowFinderEnumWindowsProcContext context);

    /// <summary>
    /// Wrapps EnumChildWindows.
    /// </summary>
    /// <remarks>
    /// Calling <see cref="GetLastWin32Error"/> does not return extended error information for this method.
    /// </remarks>
    /// <seealso href="http://msdn.microsoft.com/en-us/library/ms633494.aspx"/>.
    void EnumChildWindows (IntPtr parentWindowHandle, EnumChildWindowsProc enumWindowsCallback, WindowFinderEnumChildWindowsProcContext context);

    /// <summary>
    /// Wrapps GetWindowThreadProcessID.
    /// </summary>
    /// <returns>
    /// Returns the process-ID. The thread-ID of the thread that created the window is not returned. 
    /// If the function fails, the return value is zero.
    /// </returns>
    /// <seealso href="http://msdn.microsoft.com/en-us/library/ms633522.aspx"/>.
    // The return value is the identifier of the thread that created the window. 
    int GetWindowThreadProcessID (IntPtr windowHandle);

    /// <summary>
    /// Wrapps GetClassName.
    /// </summary>
    /// <returns>
    /// If the function succeeds, the return value is the number of characters copied to the buffer, not including the terminating null character.
    /// If the function fails, the return value is zero. 
    /// To get extended error information, call <see cref="GetLastWin32Error"/>. 
    /// </returns>
    /// <seealso href="http://msdn.microsoft.com/en-us/library/ms633582.aspx"/>.
    int GetClassName (IntPtr windowHandle, StringBuilder className, int classNameMaxLength);

    /// <summary>
    /// Wrapps GetWindowText.
    /// </summary>
    /// <returns>
    /// If the function succeeds, the return value is the length, in characters, of the copied string, not including the terminating null character. 
    /// If the window has no title bar or text, if the title bar is empty, or if the window or control handle is invalid, the return value is zero. 
    /// To get extended error information, call <see cref="GetLastWin32Error"/>. 
    /// </returns>
    /// <seealso href="http://msdn.microsoft.com/en-us/library/ms633520.aspx"/>.
    int GetWindowText (IntPtr windowHandle, StringBuilder windowText, int windowTextMaxLength);

    int GetLastWin32Error ();
  }
}
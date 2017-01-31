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
using System.Text.RegularExpressions;

namespace Remotion.WindowFinder.Windows
{
  /// <summary>
  /// Contains all information required by <see cref="EnumChildWindowsProc"/>.
  /// </summary>
  public sealed class WindowFinderEnumChildWindowsProcContext : WindowFinderEnumWindowsProcContextBase
  {
    private readonly WindowInformation _parentWindow;

    public WindowFinderEnumChildWindowsProcContext (
        IWin32WindowsNativeMethods nativeMethods,
        Regex classNameConstraint,
        Regex windowTextConstraint,
        WindowInformation parentWindow)
        : base (nativeMethods, classNameConstraint, windowTextConstraint)
    {
      _parentWindow = parentWindow;
    }

    public WindowInformation ParentWindow
    {
      get { return _parentWindow; }
    }
  }
}
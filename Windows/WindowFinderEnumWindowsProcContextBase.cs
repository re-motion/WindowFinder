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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Remotion.Utilities;

namespace Remotion.WindowFinder.Windows
{
  /// <summary>
  /// Contains all information required by both <see cref="EnumWindowsProc"/> and <see cref="EnumChildWindowsProc"/>.
  /// </summary>
  public abstract class WindowFinderEnumWindowsProcContextBase
  {
    private readonly IWin32WindowsNativeMethods _nativeMethods;
    private readonly Regex _classNameConstraint;
    private readonly Regex _windowTextConstraint;
    private readonly List<WindowInformation> _windows = new List<WindowInformation>();

    protected WindowFinderEnumWindowsProcContextBase (
        IWin32WindowsNativeMethods nativeMethods,
        Regex classNameConstraint,
        Regex windowTextConstraint)
    {
      ArgumentUtility.CheckNotNull ("nativeMethods", nativeMethods);

      _nativeMethods = nativeMethods;
      _classNameConstraint = classNameConstraint;
      _windowTextConstraint = windowTextConstraint;
    }

    public IWin32WindowsNativeMethods NativeMethods
    {
      get { return _nativeMethods; }
    }

    public Regex ClassNameConstraint
    {
      get { return _classNameConstraint; }
    }

    public Regex WindowTextConstraint
    {
      get { return _windowTextConstraint; }
    }

    public List<WindowInformation> Windows
    {
      get { return _windows; }
    }
  }
}
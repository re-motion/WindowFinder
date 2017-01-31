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
using JetBrains.Annotations;

namespace Remotion.WindowFinder
{
  /// <summary>
  /// Contains all information about a window found via <see cref="IWindowFinder"/>.
  /// </summary>
  public sealed class WindowInformation
  {
    private readonly int _processID;

    [NotNull]
    private readonly string _className;

    [NotNull]
    private readonly string _windowText;

    [CanBeNull]
    private readonly WindowInformation _parentWindow;

    private readonly IntPtr _windowHandle;

    public WindowInformation (
        int processID,
        IntPtr windowHandle,
        [NotNull] string className,
        [NotNull] string windowText,
        [CanBeNull] WindowInformation parentWindow)
    {
      if (className == null)
        throw new ArgumentNullException ("className");

      if (windowText == null)
        throw new ArgumentNullException ("windowText");

      _processID = processID;
      _windowHandle = windowHandle;
      _className = className;
      _windowText = windowText;
      _parentWindow = parentWindow;
    }

    public int ProcessID
    {
      get { return _processID; }
    }

    [NotNull]
    public string ClassName
    {
      get { return _className; }
    }

    [NotNull]
    public string WindowText
    {
      get { return _windowText; }
    }

    public IntPtr WindowHandle
    {
      get { return _windowHandle; }
    }

    [CanBeNull]
    public WindowInformation ParentWindow
    {
      get { return _parentWindow; }
    }
  }
}
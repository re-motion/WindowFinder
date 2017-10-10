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
using JetBrains.Annotations;

namespace Remotion.WindowFinder
{
  /// <summary>
  /// Contains the filter criteria used by <see cref="IWindowFinder"/>.
  /// </summary>
  public sealed class WindowFilterCriteria
  {
    public bool ExcludeOwnProcess { get; set; }

    public int? ProcessID { get; set; }

    [CanBeNull]
    public Regex ClassName { get; set; }

    [CanBeNull]
    public Regex WindowTitle { get; set; }

    public bool IncludeChildWindows { get; set; }
  }
}
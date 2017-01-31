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
using NUnit.Framework;
using Rhino.Mocks;

namespace Remotion.WindowFinder.Windows.UnitTests
{
  [TestFixture]
  public class NativeWindowFinderTest_FilterByProcess : NativeWindowFinderTestBase
  {
    [Test]
    public void FindWindows_ExcludeOwnProcess ()
    {
      var windowHandle1 = new IntPtr (100);
      var windowHandle2 = new IntPtr (200);
      var windowHandle3 = new IntPtr (300);

      NativeMethodsStub.Stub (stub => stub.EnumWindows (Arg<EnumWindowsProc>.Is.NotNull, Arg<WindowFinderEnumWindowsProcContext>.Is.NotNull))
          .WhenCalled (
              mi =>
              {
                Assert.That (InvokeEnumWindowsProc (mi, windowHandle1), Is.True);
                Assert.That (InvokeEnumWindowsProc (mi, windowHandle2), Is.True);
                Assert.That (InvokeEnumWindowsProc (mi, windowHandle3), Is.True);
              })
          .Return (true);

      StubNativeMethodsForWindowInformation (NativeMethodsStub, windowHandle1, 101, "Class1", "Window1");
      StubNativeMethodsForWindowInformation (NativeMethodsStub, windowHandle2, CurrentProcessID, "Class2", "Window2");
      StubNativeMethodsForWindowInformation (NativeMethodsStub, windowHandle3, 301, "Class3", "Window3");

      var windows = WindowFinder.FindWindows (new WindowFilterCriteria { ExcludeOwnProcess = true });

      Assert.That (windows.Length, Is.EqualTo (2));
      AssertWindowInformation (windows[0], windowHandle1, 101, "Class1", "Window1");
      AssertWindowInformation (windows[1], windowHandle3, 301, "Class3", "Window3");
    }

    [Test]
    public void FindWindows_FilterByProcessID ()
    {
      var windowHandle1 = new IntPtr (100);
      var windowHandle2 = new IntPtr (200);
      var windowHandle3 = new IntPtr (300);
      var windowHandle4 = new IntPtr (400);

      NativeMethodsStub.Stub (stub => stub.EnumWindows (Arg<EnumWindowsProc>.Is.NotNull, Arg<WindowFinderEnumWindowsProcContext>.Is.NotNull))
          .WhenCalled (
              mi =>
              {
                Assert.That (InvokeEnumWindowsProc (mi, windowHandle1), Is.True);
                Assert.That (InvokeEnumWindowsProc (mi, windowHandle2), Is.True);
                Assert.That (InvokeEnumWindowsProc (mi, windowHandle3), Is.True);
                Assert.That (InvokeEnumWindowsProc (mi, windowHandle4), Is.True);
              })
          .Return (true);

      StubNativeMethodsForWindowInformation (NativeMethodsStub, windowHandle1, 101, "Class1", "Window1");
      StubNativeMethodsForWindowInformation (NativeMethodsStub, windowHandle2, CurrentProcessID, "Class2", "Window2");
      StubNativeMethodsForWindowInformation (NativeMethodsStub, windowHandle3, 101, "Class3", "Window3");
      StubNativeMethodsForWindowInformation (NativeMethodsStub, windowHandle4, 401, "Class4", "Window4");

      var windows = WindowFinder.FindWindows (new WindowFilterCriteria { ProcessID = 101 });

      Assert.That (windows.Length, Is.EqualTo (2));
      AssertWindowInformation (windows[0], windowHandle1, 101, "Class1", "Window1");
      AssertWindowInformation (windows[1], windowHandle3, 101, "Class3", "Window3");
    }
  }
}
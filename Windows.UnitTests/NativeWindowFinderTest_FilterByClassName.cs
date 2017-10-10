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
using System.Text.RegularExpressions;
using NUnit.Framework;
using Rhino.Mocks;

namespace Remotion.WindowFinder.Windows.UnitTests
{
  [TestFixture]
  public class NativeWindowFinderTest_FilterByClassName : NativeWindowFinderTestBase
  {
    [Test]
    public void FindWindows ()
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

      StubNativeMethodsForWindowInformation (NativeMethodsStub, windowHandle1, 101, "xClass1", "Window1");
      StubNativeMethodsForWindowInformation (NativeMethodsStub, windowHandle2, 201, "Class1x", "Window2");
      StubNativeMethodsForWindowInformation (NativeMethodsStub, windowHandle3, 301, "Class3", "Window3");

      var windows = WindowFinder.FindWindows (new WindowFilterCriteria { ClassName = new Regex ("Class1") });

      Assert.That (windows.Length, Is.EqualTo (2));
      AssertWindowInformation (windows[0], windowHandle1, 101, "xClass1", "Window1");
      AssertWindowInformation (windows[1], windowHandle2, 201, "Class1x", "Window2");
    }

    [Test]
    public void FindWindows_IncludeChildWindows_MatchesParentAndChild ()
    {
      var windowHandle1 = new IntPtr (100);
      var windowHandle2 = new IntPtr (200);
      var subWindowHandle11 = new IntPtr (1001);
      var subWindowHandle12 = new IntPtr (1002);
      var subWindowHandle21 = new IntPtr (2001);

      NativeMethodsStub.Stub (stub => stub.EnumWindows (Arg<EnumWindowsProc>.Is.NotNull, Arg<WindowFinderEnumWindowsProcContext>.Is.NotNull))
          .WhenCalled (
              mi =>
              {
                Assert.That (InvokeEnumWindowsProc (mi, windowHandle1), Is.True);
                Assert.That (InvokeEnumWindowsProc (mi, windowHandle2), Is.True);
              })
          .Return (true);

      NativeMethodsStub.Stub (
          stub =>
              stub.EnumChildWindows (
                  Arg.Is (windowHandle1), Arg<EnumChildWindowsProc>.Is.NotNull, Arg<WindowFinderEnumChildWindowsProcContext>.Is.NotNull))
          .WhenCalled (
              mi =>
              {
                Assert.That (InvokeEnumChildWindowsProc (mi, subWindowHandle11), Is.True);
                Assert.That (InvokeEnumChildWindowsProc (mi, subWindowHandle12), Is.True);
                Assert.That (InvokeEnumChildWindowsProc (mi, subWindowHandle21), Is.True);
              });

      StubNativeMethodsForWindowInformation (NativeMethodsStub, windowHandle1, 101, "Class1", "Window1");
      StubNativeMethodsForWindowInformation (NativeMethodsStub, windowHandle2, 201, "Class2", "Window2");
      StubNativeMethodsForWindowInformation (NativeMethodsStub, subWindowHandle11, 101, "Class1.1", "Window1.1");
      StubNativeMethodsForWindowInformation (NativeMethodsStub, subWindowHandle12, 101, "Class1.2", "Window1.2");
      StubNativeMethodsForWindowInformation (NativeMethodsStub, subWindowHandle21, 201, "Class2.1", "Window2.1");

      var windows = WindowFinder.FindWindows (new WindowFilterCriteria { IncludeChildWindows = true, ClassName = new Regex ("Class1") });

      Assert.That (windows.Length, Is.EqualTo (3));
      AssertWindowInformation (windows[0], windowHandle1, 101, "Class1", "Window1");
      AssertChildWindowInformation (windows[1], subWindowHandle11, 101, "Class1.1", "Window1.1", windowHandle1);
      AssertChildWindowInformation (windows[2], subWindowHandle12, 101, "Class1.2", "Window1.2", windowHandle1);
    }

    [Test]
    public void FindWindows_IncludeChildWindows_MatchesOnlyChildWindow ()
    {
      var windowHandle1 = new IntPtr (100);
      var windowHandle2 = new IntPtr (200);
      var subWindowHandle11 = new IntPtr (1001);
      var subWindowHandle12 = new IntPtr (1002);
      var subWindowHandle21 = new IntPtr (2001);

      NativeMethodsStub.Stub (stub => stub.EnumWindows (Arg<EnumWindowsProc>.Is.NotNull, Arg<WindowFinderEnumWindowsProcContext>.Is.NotNull))
          .WhenCalled (
              mi =>
              {
                Assert.That (InvokeEnumWindowsProc (mi, windowHandle1), Is.True);
                Assert.That (InvokeEnumWindowsProc (mi, windowHandle2), Is.True);
              })
          .Return (true);

      NativeMethodsStub.Stub (
          stub =>
              stub.EnumChildWindows (
                  Arg.Is (windowHandle1), Arg<EnumChildWindowsProc>.Is.NotNull, Arg<WindowFinderEnumChildWindowsProcContext>.Is.NotNull))
          .WhenCalled (
              mi =>
              {
                Assert.That (InvokeEnumChildWindowsProc (mi, subWindowHandle11), Is.True);
                Assert.That (InvokeEnumChildWindowsProc (mi, subWindowHandle12), Is.True);
                Assert.That (InvokeEnumChildWindowsProc (mi, subWindowHandle21), Is.True);
              });

      StubNativeMethodsForWindowInformation (NativeMethodsStub, windowHandle1, 101, "Class1", "Window1");
      StubNativeMethodsForWindowInformation (NativeMethodsStub, windowHandle2, 201, "Class2", "Window2");
      StubNativeMethodsForWindowInformation (NativeMethodsStub, subWindowHandle11, 101, "Class1.1", "Window1.1");
      StubNativeMethodsForWindowInformation (NativeMethodsStub, subWindowHandle12, 101, "Class1.2", "Window1.2");
      StubNativeMethodsForWindowInformation (NativeMethodsStub, subWindowHandle21, 201, "Class2.1", "Window2.1");

      var windows = WindowFinder.FindWindows (new WindowFilterCriteria { IncludeChildWindows = true, ClassName = new Regex ("Class1.") });

      Assert.That (windows.Length, Is.EqualTo (2));
      AssertChildWindowInformation (windows[0], subWindowHandle11, 101, "Class1.1", "Window1.1", windowHandle1);
      AssertChildWindowInformation (windows[1], subWindowHandle12, 101, "Class1.2", "Window1.2", windowHandle1);
    }
  }
}
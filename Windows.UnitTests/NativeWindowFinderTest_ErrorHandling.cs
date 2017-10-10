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
using System.ComponentModel;
using NUnit.Framework;
using Rhino.Mocks;

namespace Remotion.WindowFinder.Windows.UnitTests
{
  [TestFixture]
  public class NativeWindowFinderTest_ErrorHandling : NativeWindowFinderTestBase
  {
    [Test]
    [ExpectedException (typeof (Win32Exception), ExpectedMessage = "Error executing 'EnumWindows'. Unknown error (0xffffffff)")]
    public void FindWindows_HandlesEnumWindowsReturnedFalse ()
    {
      NativeMethodsStub.Stub (stub => stub.EnumWindows (null, null))
          .IgnoreArguments()
          .WhenCalled (mi => NativeMethodsStub.Stub (stub => stub.GetLastWin32Error()).Return (-1).Repeat.Once())
          .Return (false);

      WindowFinder.FindWindows (new WindowFilterCriteria());
    }

    [Test]
    public void FindWindows_SkipsWindowsWhereGetWindowThreadProcessIDReturnsZero ()
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
      StubNativeMethodsForWindowInformation (NativeMethodsStub, windowHandle2, 0, "Class2", "Window2");
      StubNativeMethodsForWindowInformation (NativeMethodsStub, windowHandle3, 301, "Class3", "Window3");

      var windows = WindowFinder.FindWindows (new WindowFilterCriteria());

      Assert.That (windows.Length, Is.EqualTo (2));
      AssertWindowInformation (windows[0], windowHandle1, 101, "Class1", "Window1");
      AssertWindowInformation (windows[1], windowHandle3, 301, "Class3", "Window3");
    }

    [Test]
    public void FindWindows_IncludesWindowsWhereGetWindowTextReturnsZero ()
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
      StubNativeMethodsForWindowInformation (NativeMethodsStub, windowHandle2, 201, "Class2", "");
      StubNativeMethodsForWindowInformation (NativeMethodsStub, windowHandle3, 301, "Class3", "Window3");

      var windows = WindowFinder.FindWindows (new WindowFilterCriteria());

      Assert.That (windows.Length, Is.EqualTo (3));
      AssertWindowInformation (windows[0], windowHandle1, 101, "Class1", "Window1");
      AssertWindowInformation (windows[1], windowHandle2, 201, "Class2", "");
      AssertWindowInformation (windows[2], windowHandle3, 301, "Class3", "Window3");
    }

    [Test]
    public void FindWindows_SkipsWindowsWhereGetClassNameReturnsZero ()
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
      StubNativeMethodsForWindowInformation (NativeMethodsStub, windowHandle2, 201, "", "Window2");
      StubNativeMethodsForWindowInformation (NativeMethodsStub, windowHandle3, 301, "Class3", "Window3");

      var windows = WindowFinder.FindWindows (new WindowFilterCriteria());

      Assert.That (windows.Length, Is.EqualTo (2));
      AssertWindowInformation (windows[0], windowHandle1, 101, "Class1", "Window1");
      AssertWindowInformation (windows[1], windowHandle3, 301, "Class3", "Window3");
    }

    [Test]
    public void FindWindows_IncludesChildWindowsWhereGetWindowTextReturnsZero ()
    {
      var windowHandle1 = new IntPtr (100);
      var subWindowHandle11 = new IntPtr (1001);
      var subWindowHandle12 = new IntPtr (1002);

      NativeMethodsStub.Stub (stub => stub.EnumWindows (Arg<EnumWindowsProc>.Is.NotNull, Arg<WindowFinderEnumWindowsProcContext>.Is.NotNull))
          .WhenCalled (mi => Assert.That (InvokeEnumWindowsProc (mi, windowHandle1), Is.True))
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
              });

      StubNativeMethodsForWindowInformation (NativeMethodsStub, windowHandle1, 101, "Class1", "Window1");
      StubNativeMethodsForWindowInformation (NativeMethodsStub, subWindowHandle11, 101, "Class1.1", "");
      StubNativeMethodsForWindowInformation (NativeMethodsStub, subWindowHandle12, 101, "Class1.2", "Window1.2");

      var windows = WindowFinder.FindWindows (new WindowFilterCriteria { IncludeChildWindows = true });

      Assert.That (windows.Length, Is.EqualTo (3));
      AssertWindowInformation (windows[0], windowHandle1, 101, "Class1", "Window1");
      AssertChildWindowInformation (windows[1], subWindowHandle11, 101, "Class1.1", "", windowHandle1);
      AssertChildWindowInformation (windows[2], subWindowHandle12, 101, "Class1.2", "Window1.2", windowHandle1);
    }

    [Test]
    public void FindWindows_SkipsChildWindowsWhereGetClassNameReturnsZero ()
    {
      var windowHandle1 = new IntPtr (100);
      var windowHandle2 = new IntPtr (200);
      var windowHandle3 = new IntPtr (300);
      var subWindowHandle11 = new IntPtr (1001);
      var subWindowHandle12 = new IntPtr (1002);
      var subWindowHandle31 = new IntPtr (3001);

      NativeMethodsStub.Stub (stub => stub.EnumWindows (Arg<EnumWindowsProc>.Is.NotNull, Arg<WindowFinderEnumWindowsProcContext>.Is.NotNull))
          .WhenCalled (
              mi =>
              {
                Assert.That (InvokeEnumWindowsProc (mi, windowHandle1), Is.True);
                Assert.That (InvokeEnumWindowsProc (mi, windowHandle2), Is.True);
                Assert.That (InvokeEnumWindowsProc (mi, windowHandle3), Is.True);
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
              });

      NativeMethodsStub.Stub (
          stub =>
              stub.EnumChildWindows (
                  Arg.Is (windowHandle3), Arg<EnumChildWindowsProc>.Is.NotNull, Arg<WindowFinderEnumChildWindowsProcContext>.Is.NotNull))
          .WhenCalled (mi => Assert.That (InvokeEnumChildWindowsProc (mi, subWindowHandle31), Is.True));


      StubNativeMethodsForWindowInformation (NativeMethodsStub, windowHandle1, 101, "Class1", "Window1");
      StubNativeMethodsForWindowInformation (NativeMethodsStub, windowHandle2, 201, "", "Window2");
      StubNativeMethodsForWindowInformation (NativeMethodsStub, windowHandle3, 301, "Class3", "Window3");
      StubNativeMethodsForWindowInformation (NativeMethodsStub, subWindowHandle11, 101, "", "Window1.1");
      StubNativeMethodsForWindowInformation (NativeMethodsStub, subWindowHandle12, 101, "Class1.2", "Window1.2");
      StubNativeMethodsForWindowInformation (NativeMethodsStub, subWindowHandle31, 301, "Class3.1", "Window3.1");

      var windows = WindowFinder.FindWindows (new WindowFilterCriteria { IncludeChildWindows = true });

      Assert.That (windows.Length, Is.EqualTo (4));
      AssertWindowInformation (windows[0], windowHandle1, 101, "Class1", "Window1");
      AssertChildWindowInformation (windows[1], subWindowHandle12, 101, "Class1.2", "Window1.2", windowHandle1);
      AssertWindowInformation (windows[2], windowHandle3, 301, "Class3", "Window3");
      AssertChildWindowInformation (windows[3], subWindowHandle31, 301, "Class3.1", "Window3.1", windowHandle3);
    }
  }
}
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
using System.Diagnostics;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;

namespace Remotion.WindowFinder.Windows.UnitTests
{
  public class NativeWindowFinderTestBase
  {
    private IWindowFinder _windowFinder;
    private IWin32WindowsNativeMethods _nativeMethodsStub;
    private int _currentProcessID;

    [SetUp]
    public virtual void SetUp ()
    {
      var currentProcess = Process.GetCurrentProcess();
      _currentProcessID = currentProcess.Id;
      _nativeMethodsStub = MockRepository.GenerateStub<IWin32WindowsNativeMethods>();

      _windowFinder = new NativeWindowFinder (_nativeMethodsStub);
    }

    protected int CurrentProcessID
    {
      get { return _currentProcessID; }
    }

    protected IWin32WindowsNativeMethods NativeMethodsStub
    {
      get { return _nativeMethodsStub; }
    }

    protected IWindowFinder WindowFinder
    {
      get { return _windowFinder; }
    }

    protected bool InvokeEnumWindowsProc (MethodInvocation methodInvocation, IntPtr windowHandle)
    {
      return ((EnumWindowsProc) methodInvocation.Arguments[0]).Invoke (
          windowHandle, (WindowFinderEnumWindowsProcContext) methodInvocation.Arguments[1]);
    }

    protected bool InvokeEnumChildWindowsProc (MethodInvocation methodInvocation, IntPtr windowHandle)
    {
      return ((EnumChildWindowsProc) methodInvocation.Arguments[1]).Invoke (
          windowHandle, (WindowFinderEnumChildWindowsProcContext) methodInvocation.Arguments[2]);
    }

    protected void StubNativeMethodsForWindowInformation (
        IWin32WindowsNativeMethods nativeMethodsStub, IntPtr windowHandle, int processID, string className, string windowText)
    {
      StubGetWindowThreadProcessID (nativeMethodsStub, windowHandle, processID);
      StubGetClassName (nativeMethodsStub, windowHandle, className);
      StubGetWindowText (nativeMethodsStub, windowHandle, windowText);
    }

    private void StubGetWindowThreadProcessID (IWin32WindowsNativeMethods nativeMethodsStub, IntPtr windowHandle, int processID)
    {
      nativeMethodsStub.Stub (stub => stub.GetWindowThreadProcessID (windowHandle)).Return (processID);
    }

    private void StubGetClassName (IWin32WindowsNativeMethods nativeMethodsStub, IntPtr windowHandle, string className)
    {
      nativeMethodsStub.Stub (stub => stub.GetClassName (Arg.Is (windowHandle), Arg<StringBuilder>.Is.NotNull, Arg.Is (256)))
          .WhenCalled (mi => ((StringBuilder) mi.Arguments[1]).Append (className))
          .Return (className.Length);
    }

    private void StubGetWindowText (IWin32WindowsNativeMethods nativeMethodsStub, IntPtr windowHandle, string windowText)
    {
      nativeMethodsStub.Stub (stub => stub.GetWindowText (Arg.Is (windowHandle), Arg<StringBuilder>.Is.NotNull, Arg.Is (1024)))
          .WhenCalled (mi => ((StringBuilder) mi.Arguments[1]).Append (windowText))
          .Return (windowText.Length);
    }

    protected void AssertWindowInformation (
        WindowInformation actualWindowInformation,
        IntPtr expectedWindowHandle,
        int expectedProcessID,
        string expectedClassName,
        string expectedWindowText)
    {
      Assert.That (actualWindowInformation.WindowHandle, Is.EqualTo (expectedWindowHandle));
      Assert.That (actualWindowInformation.ProcessID, Is.EqualTo (expectedProcessID));
      Assert.That (actualWindowInformation.ClassName, Is.EqualTo (expectedClassName));
      Assert.That (actualWindowInformation.WindowText, Is.EqualTo (expectedWindowText));
      Assert.That (actualWindowInformation.ParentWindow, Is.Null);
    }

    protected void AssertChildWindowInformation (
        WindowInformation actualWindowInformation,
        IntPtr expectedWindowHandle,
        int expectedProcessID,
        string expectedClassName,
        string expectedWindowText,
        IntPtr expectedParentWindowHandle)
    {
      Assert.That (actualWindowInformation.WindowHandle, Is.EqualTo (expectedWindowHandle));
      Assert.That (actualWindowInformation.ProcessID, Is.EqualTo (expectedProcessID));
      Assert.That (actualWindowInformation.ClassName, Is.EqualTo (expectedClassName));
      Assert.That (actualWindowInformation.WindowText, Is.EqualTo (expectedWindowText));
      Assert.That (actualWindowInformation.ParentWindow.WindowHandle, Is.EqualTo (expectedParentWindowHandle));
    }
  }
}
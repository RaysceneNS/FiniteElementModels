using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Tao.OpenGl;
using Tao.Platform.Windows;

namespace UI.Controls.Viewport
{
    internal sealed class GLContext : IDisposable
    {
        private readonly IntPtr _handle;
        private bool _disposed;

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        internal GLContext(IntPtr handle)
        {
            _handle = handle;

            var descriptor = new Gdi.PIXELFORMATDESCRIPTOR();
            descriptor.nSize = (short) Marshal.SizeOf(descriptor);
            descriptor.nVersion = 1;
            descriptor.dwFlags = Gdi.PFD_DRAW_TO_WINDOW | Gdi.PFD_SUPPORT_OPENGL | Gdi.PFD_DOUBLEBUFFER | Gdi.PFD_SWAP_EXCHANGE;
            descriptor.iPixelType = Gdi.PFD_TYPE_RGBA;
            descriptor.cColorBits = 32;
            descriptor.cRedBits = 0;
            descriptor.cRedShift = 0;
            descriptor.cGreenBits = 0;
            descriptor.cGreenShift = 0;
            descriptor.cBlueBits = 0;
            descriptor.cBlueShift = 0;
            descriptor.cAlphaBits = 0;
            descriptor.cAlphaShift = 0;
            descriptor.cAccumBits = 0;
            descriptor.cAccumRedBits = 0;
            descriptor.cAccumGreenBits = 0;
            descriptor.cAccumBlueBits = 0;
            descriptor.cAccumAlphaBits = 0;
            descriptor.cDepthBits = 24;
            descriptor.cStencilBits = 8;
            descriptor.cAuxBuffers = 0;
            descriptor.iLayerType = Gdi.PFD_MAIN_PLANE;
            descriptor.bReserved = 0;
            descriptor.dwLayerMask = 0;
            descriptor.dwVisibleMask = 0;
            descriptor.dwDamageMask = 0;

            // Attempt to get the device context
            DeviceContext = User.GetDC(_handle);

            // Did we not get a device context?
            if (DeviceContext == IntPtr.Zero)
                throw new GLContextException("Can not create a GL device context.");

            // Attempt to find an appropriate pixel format
            var pixelFormat = Gdi.ChoosePixelFormat(DeviceContext, ref descriptor);
            // Did windows not find a matching pixel format?
            if (pixelFormat == 0)
                throw new GLContextException("Can not find a suitable PixelFormat.");

            // Are we not able to set the pixel format?
            if (!Gdi.SetPixelFormat(DeviceContext, pixelFormat, ref descriptor))
                throw new GLContextException($"Can not set the chosen PixelFormat. Chosen PixelFormat was {pixelFormat}.");

            // Attempt to get the rendering context
            RenderingContext = Wgl.wglCreateContext(DeviceContext);
            if (RenderingContext == IntPtr.Zero)
                throw new GLContextException("Can not create a GL rendering context.");

            // Attempt to activate the rendering context
            MakeCurrent();
        }

        internal IntPtr DeviceContext { get; private set; }
        internal IntPtr RenderingContext { get; private set; }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~GLContext()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                // Dispose of resources held by this instance.
                DestroyContexts();
                _disposed = true;
            }
        }

        private void DestroyContexts()
        {
            if (RenderingContext != IntPtr.Zero)
            {
                Wgl.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
                Wgl.wglDeleteContext(RenderingContext);
                RenderingContext = IntPtr.Zero;
            }

            if (DeviceContext != IntPtr.Zero)
            {
                if (_handle != IntPtr.Zero)
                    User.ReleaseDC(_handle, DeviceContext);

                DeviceContext = IntPtr.Zero;
            }
        }

        internal void MakeCurrent()
        {
            if (Wgl.wglGetCurrentContext() != RenderingContext)
                Wgl.wglMakeCurrent(DeviceContext, RenderingContext);
        }

        internal void SwapBuffers()
        {
            if (Gdi.SwapBuffersFast(DeviceContext) == Gl.GL_FALSE)
                throw new GLContextException("Call to SwapBuffersFast() failed.");
        }
    }
}
using System;
using Core.Geometry;
using Tao.OpenGl;

namespace UI.Controls.Viewport
{
    internal sealed class RasterFont : IDisposable
    {
        // define each letter as one byte for each row in the character
        // this is lucida console 10 point
        private static readonly byte[,] Letters =
        {
            {0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0}, // 
            {0x0, 0x0, 0x10, 0x0, 0x0, 0x10, 0x10, 0x10, 0x10, 0x10, 0x10, 0x0, 0x0}, //!
            {0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x24, 0x24, 0x24, 0x0}, //"
            {0x0, 0x0, 0x50, 0x50, 0x50, 0xfe, 0x28, 0x7f, 0x14, 0x14, 0x14, 0x0, 0x0}, //#
            {0x0, 0x10, 0x78, 0x14, 0x14, 0x1c, 0x38, 0x70, 0x50, 0x50, 0x3c, 0x10, 0x0}, //$
            {0x0, 0x0, 0x86, 0x49, 0x29, 0x16, 0x18, 0x68, 0x94, 0x92, 0x61, 0x0, 0x0}, //%
            {0x0, 0x0, 0x7e, 0xc6, 0x8e, 0x89, 0x71, 0x38, 0x24, 0x24, 0x18, 0x0, 0x0}, //&
            {0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x10, 0x10, 0x10, 0x0}, //'
            {0x6, 0x8, 0x10, 0x30, 0x20, 0x20, 0x20, 0x20, 0x30, 0x10, 0x8, 0x6, 0x0}, //(
            {0x60, 0x10, 0x8, 0x4, 0x4, 0x4, 0x4, 0x4, 0x4, 0x8, 0x10, 0x60, 0x0}, //)
            {0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x24, 0x3c, 0x24, 0x52, 0x10, 0x0, 0x0}, //*
            {0x0, 0x0, 0x10, 0x10, 0x10, 0xfe, 0x10, 0x10, 0x10, 0x0, 0x0, 0x0, 0x0}, //+
            {0x20, 0x10, 0x30, 0x30, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0}, //,
            {0x0, 0x0, 0x0, 0x0, 0x0, 0x7e, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0}, //-
            {0x0, 0x0, 0x30, 0x30, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0}, //.
            {0x80, 0x40, 0x40, 0x20, 0x10, 0x10, 0x8, 0x8, 0x4, 0x2, 0x2, 0x1, 0x0}, ////
            {0x0, 0x0, 0x3c, 0x24, 0x42, 0x42, 0x42, 0x42, 0x42, 0x24, 0x18, 0x0, 0x0}, //0
            {0x0, 0x0, 0xfe, 0x10, 0x10, 0x10, 0x10, 0x10, 0x10, 0xd0, 0x30, 0x0, 0x0}, //1
            {0x0, 0x0, 0x7c, 0x40, 0x20, 0x10, 0x8, 0x4, 0x4, 0x4, 0x78, 0x0, 0x0}, //2
            {0x0, 0x0, 0x78, 0x4, 0x4, 0xc, 0x30, 0x8, 0x4, 0x4, 0x78, 0x0, 0x0}, //3
            {0x0, 0x0, 0x8, 0x8, 0xfc, 0x88, 0x48, 0x28, 0x28, 0x18, 0x8, 0x0, 0x0}, //4
            {0x0, 0x0, 0x38, 0x4, 0x4, 0x4, 0x4, 0x38, 0x20, 0x20, 0x3c, 0x0, 0x0}, //5
            {0x0, 0x0, 0x1c, 0x22, 0x42, 0x42, 0x62, 0x5c, 0x40, 0x20, 0x1c, 0x0, 0x0}, //6
            {0x0, 0x0, 0x20, 0x20, 0x10, 0x10, 0x8, 0x8, 0x4, 0x2, 0x7e, 0x0, 0x0}, //7
            {0x0, 0x0, 0x3c, 0x42, 0x42, 0x46, 0x3c, 0x24, 0x42, 0x42, 0x3c, 0x0, 0x0}, //8
            {0x0, 0x0, 0x38, 0x4, 0x2, 0x3a, 0x46, 0x42, 0x42, 0x44, 0x38, 0x0, 0x0}, //9
            {0x0, 0x0, 0x18, 0x18, 0x0, 0x0, 0x0, 0x18, 0x18, 0x0, 0x0, 0x0, 0x0}, //:
            {0x20, 0x10, 0x30, 0x30, 0x0, 0x0, 0x0, 0x30, 0x30, 0x0, 0x0, 0x0, 0x0}, //;
            {0x0, 0x0, 0x2, 0xc, 0x10, 0x60, 0x10, 0xc, 0x2, 0x0, 0x0, 0x0, 0x0}, //<
            {0x0, 0x0, 0x0, 0x0, 0x7e, 0x0, 0x7e, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0}, //=
            {0x0, 0x0, 0x40, 0x30, 0x8, 0x6, 0x8, 0x30, 0x40, 0x0, 0x0, 0x0, 0x0}, //>
            {0x0, 0x0, 0x10, 0x0, 0x0, 0x10, 0x8, 0x4, 0x2, 0x42, 0x7c, 0x0, 0x0}, //?
            {0x0, 0x0, 0x3c, 0x44, 0x9b, 0xa6, 0xa2, 0xb2, 0xde, 0x62, 0x3c, 0x0, 0x0}, //@
            {0x0, 0x0, 0x81, 0x42, 0x7e, 0x24, 0x24, 0x24, 0x18, 0x18, 0x0, 0x0, 0x0}, //A
            {0x0, 0x0, 0x7c, 0x42, 0x42, 0x42, 0x7c, 0x42, 0x42, 0x7c, 0x0, 0x0, 0x0}, //B
            {0x0, 0x0, 0x3e, 0x40, 0x80, 0x80, 0x80, 0x80, 0x40, 0x3e, 0x0, 0x0, 0x0}, //C
            {0x0, 0x0, 0x78, 0x44, 0x42, 0x42, 0x42, 0x42, 0x44, 0x78, 0x0, 0x0, 0x0}, //D
            {0x0, 0x0, 0x7e, 0x40, 0x40, 0x7c, 0x40, 0x40, 0x40, 0x7e, 0x0, 0x0, 0x0}, //E
            {0x0, 0x0, 0x40, 0x40, 0x40, 0x7c, 0x40, 0x40, 0x40, 0x7e, 0x0, 0x0, 0x0}, //F
            {0x0, 0x0, 0x3e, 0x42, 0x82, 0x8e, 0x80, 0x80, 0x40, 0x3e, 0x0, 0x0, 0x0}, //G
            {0x0, 0x0, 0x42, 0x42, 0x42, 0x42, 0x7e, 0x42, 0x42, 0x42, 0x0, 0x0, 0x0}, //H
            {0x0, 0x0, 0x7c, 0x10, 0x10, 0x10, 0x10, 0x10, 0x10, 0x7c, 0x0, 0x0, 0x0}, //I
            {0x0, 0x0, 0x78, 0x4, 0x4, 0x4, 0x4, 0x4, 0x4, 0x3c, 0x0, 0x0, 0x0}, //J
            {0x0, 0x0, 0x42, 0x44, 0x48, 0x50, 0x70, 0x48, 0x44, 0x42, 0x0, 0x0, 0x0}, //K
            {0x0, 0x0, 0x7e, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x0, 0x0, 0x0}, //L
            {0x0, 0x0, 0x82, 0x82, 0x92, 0xaa, 0xaa, 0xaa, 0xc6, 0xc6, 0x0, 0x0, 0x0}, //M
            {0x0, 0x0, 0x42, 0x46, 0x4a, 0x4a, 0x52, 0x52, 0x62, 0x42, 0x0, 0x0, 0x0}, //N
            {0x0, 0x0, 0x38, 0x44, 0x82, 0x82, 0x82, 0x82, 0x44, 0x38, 0x0, 0x0, 0x0}, //O
            {0x0, 0x0, 0x40, 0x40, 0x40, 0x7c, 0x42, 0x42, 0x42, 0x7c, 0x0, 0x0, 0x0}, //P
            {0x3, 0x6, 0x38, 0x44, 0x82, 0x82, 0x82, 0x82, 0x44, 0x38, 0x0, 0x0, 0x0}, //Q
            {0x0, 0x0, 0x42, 0x44, 0x48, 0x78, 0x44, 0x44, 0x44, 0x78, 0x0, 0x0, 0x0}, //R
            {0x0, 0x0, 0x7c, 0x2, 0x2, 0x4, 0x38, 0x40, 0x40, 0x3e, 0x0, 0x0, 0x0}, //S
            {0x0, 0x0, 0x10, 0x10, 0x10, 0x10, 0x10, 0x10, 0x10, 0xfe, 0x0, 0x0, 0x0}, //T
            {0x0, 0x0, 0x3c, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x42, 0x0, 0x0, 0x0}, //U
            {0x0, 0x0, 0x10, 0x38, 0x28, 0x24, 0x44, 0x42, 0x42, 0x81, 0x0, 0x0, 0x0}, //V
            {0x0, 0x0, 0x24, 0x66, 0x6a, 0x5a, 0x5a, 0x92, 0x81, 0x81, 0x0, 0x0, 0x0}, //W
            {0x0, 0x0, 0x81, 0x42, 0x24, 0x18, 0x18, 0x24, 0x42, 0x81, 0x0, 0x0, 0x0}, //X
            {0x0, 0x0, 0x10, 0x10, 0x10, 0x10, 0x28, 0x28, 0x44, 0x82, 0x0, 0x0, 0x0}, //Y
            {0x0, 0x0, 0xfe, 0x40, 0x20, 0x10, 0x8, 0x4, 0x2, 0xfe, 0x0, 0x0, 0x0}, //Z
            {0x1e, 0x10, 0x10, 0x10, 0x10, 0x10, 0x10, 0x10, 0x10, 0x10, 0x10, 0x1e, 0x0}, //[
            {0x1, 0x2, 0x2, 0x4, 0x8, 0x8, 0x10, 0x10, 0x20, 0x40, 0x40, 0x80, 0x0}, //\
            {0x78, 0x8, 0x8, 0x8, 0x8, 0x8, 0x8, 0x8, 0x8, 0x8, 0x8, 0x78, 0x0}, //]
            {0x0, 0x0, 0x0, 0x0, 0x42, 0x24, 0x24, 0x14, 0x18, 0x8, 0x8, 0x0, 0x0}, //^
            {0x0, 0xff, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0}, //_
            {0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x8, 0x10, 0x0}, //`
            {0x0, 0x0, 0x3e, 0x44, 0x44, 0x3c, 0x4, 0x4, 0x38, 0x0, 0x0, 0x0, 0x0}, //a
            {0x0, 0x0, 0x5c, 0x62, 0x42, 0x42, 0x42, 0x62, 0x5c, 0x40, 0x40, 0x40, 0x0}, //b
            {0x0, 0x0, 0x1e, 0x20, 0x40, 0x40, 0x40, 0x20, 0x1e, 0x0, 0x0, 0x0, 0x0}, //c
            {0x0, 0x0, 0x3a, 0x46, 0x42, 0x42, 0x42, 0x46, 0x3a, 0x2, 0x2, 0x2, 0x0}, //d
            {0x0, 0x0, 0x3e, 0x40, 0x40, 0x7e, 0x42, 0x22, 0x3c, 0x0, 0x0, 0x0, 0x0}, //e
            {0x0, 0x0, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0xfe, 0x20, 0x20, 0x1e, 0x0}, //f
            {0x2, 0x2, 0x3a, 0x46, 0x42, 0x42, 0x42, 0x46, 0x3a, 0x0, 0x0, 0x0, 0x0}, //g
            {0x0, 0x0, 0x42, 0x42, 0x42, 0x42, 0x42, 0x62, 0x5c, 0x40, 0x40, 0x40, 0x0}, //h
            {0x0, 0x0, 0x8, 0x8, 0x8, 0x8, 0x8, 0x8, 0x78, 0x0, 0x18, 0x18, 0x0}, //i
            {0x8, 0x8, 0x8, 0x8, 0x8, 0x8, 0x8, 0x8, 0x78, 0x0, 0x18, 0x18, 0x0}, //j
            {0x0, 0x0, 0x44, 0x48, 0x50, 0x60, 0x50, 0x48, 0x44, 0x40, 0x40, 0x40, 0x0}, //k
            {0x0, 0x0, 0x8, 0x8, 0x8, 0x8, 0x8, 0x8, 0x8, 0x8, 0x8, 0x78, 0x0}, //l
            {0x0, 0x0, 0x92, 0x92, 0x92, 0x92, 0x92, 0xda, 0xb6, 0x0, 0x0, 0x0, 0x0}, //m
            {0x0, 0x0, 0x42, 0x42, 0x42, 0x42, 0x42, 0x62, 0x5c, 0x0, 0x0, 0x0, 0x0}, //n
            {0x0, 0x0, 0x3c, 0x42, 0x42, 0x42, 0x42, 0x42, 0x3c, 0x0, 0x0, 0x0, 0x0}, //o
            {0x40, 0x40, 0x5c, 0x62, 0x42, 0x42, 0x42, 0x62, 0x5c, 0x0, 0x0, 0x0, 0x0}, //p
            {0x2, 0x2, 0x3a, 0x46, 0x42, 0x42, 0x42, 0x46, 0x3a, 0x0, 0x0, 0x0, 0x0}, //q
            {0x0, 0x0, 0x40, 0x40, 0x40, 0x40, 0x40, 0x64, 0x5c, 0x0, 0x0, 0x0, 0x0}, //r
            {0x0, 0x0, 0x78, 0x4, 0x4, 0x18, 0x60, 0x40, 0x3c, 0x0, 0x0, 0x0, 0x0}, //s
            {0x0, 0x0, 0x1c, 0x20, 0x20, 0x20, 0x20, 0x20, 0xfc, 0x20, 0x0, 0x0, 0x0}, //t
            {0x0, 0x0, 0x3a, 0x46, 0x42, 0x42, 0x42, 0x42, 0x42, 0x0, 0x0, 0x0, 0x0}, //u
            {0x0, 0x0, 0x10, 0x28, 0x28, 0x44, 0x44, 0x44, 0x82, 0x0, 0x0, 0x0, 0x0}, //v
            {0x0, 0x0, 0x24, 0x24, 0x6a, 0x5a, 0x5a, 0x91, 0x81, 0x0, 0x0, 0x0, 0x0}, //w
            {0x0, 0x0, 0x42, 0x24, 0x18, 0x18, 0x18, 0x24, 0x42, 0x0, 0x0, 0x0, 0x0}, //x
            {0x30, 0x10, 0x18, 0x18, 0x24, 0x24, 0x42, 0x42, 0x81, 0x0, 0x0, 0x0, 0x0}, //y
            {0x0, 0x0, 0x7e, 0x20, 0x10, 0x8, 0x4, 0x2, 0x7e, 0x0, 0x0, 0x0, 0x0}, //z
            {0xc, 0x10, 0x10, 0x10, 0x10, 0x10, 0x60, 0x10, 0x10, 0x10, 0x10, 0x1c, 0x0}, //{
            {0x10, 0x10, 0x10, 0x10, 0x10, 0x10, 0x10, 0x10, 0x10, 0x10, 0x10, 0x10, 0x0}, //|
            {0x30, 0x8, 0x8, 0x8, 0x8, 0x8, 0x6, 0x8, 0x8, 0x8, 0x8, 0x30, 0x0}, //}
            {0x0, 0x0, 0x0, 0x0, 0x8e, 0x71, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0} //
        };

        private int _fontOffset;

        internal RasterFont()
        {
            Initialize();
        }

        private void Initialize()
        {
            const int glyphHeight = 13;
            const int glyphWidth = 8;
            var numChars = Letters.Length / glyphHeight;

            int i, j;
            var letter = new byte[glyphHeight];
            Gl.glPixelStorei(Gl.GL_UNPACK_ALIGNMENT, 1);

            _fontOffset = Gl.glGenLists(numChars);
            for (i = 0, j = 32; i < numChars; i++, j++)
            {
                for (var k = 0; k < glyphHeight; k++)
                {
                    letter[k] = Letters[i, k];
                }

                Gl.glNewList(_fontOffset + j, Gl.GL_COMPILE);
                Gl.glBitmap(glyphWidth, glyphHeight, 0.0f, 0.0f, glyphWidth, 0.0f, letter);
                Gl.glEndList();
            }
        }

        private void Print(string text, float x, float y)
        {
            Gl.glPushAttrib(Gl.GL_ENABLE_BIT | Gl.GL_LIST_BIT);
            {
                Gl.glDisable(Gl.GL_DEPTH_TEST);
                Gl.glRasterPos2f(x, y);

                Gl.glListBase(_fontOffset);

                var bytes = new byte[text.Length];
                for (var i = 0; i < text.Length; i++)
                {
                    bytes[i] = (byte) text[i];
                }

                Gl.glCallLists(text.Length, Gl.GL_UNSIGNED_BYTE, bytes);
            }
            Gl.glPopAttrib();
        }

        public void Print(string text, Point2 position)
        {
            Print(text, position.X, position.Y);
        }

        public void Dispose()
        {
            Dispose(true);
        }
        
        private void Dispose(bool disposing)
        {
            if (disposing)
                Gl.glDeleteLists(_fontOffset, 26);
        }
    }
}
namespace WindowsJedi.Algorithms
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    public static class ImageQuant
    {
        public static void RescaleImage(Bitmap image, uint nColors)
        {
            var Width = image.Width;
            var Height = image.Height;

            // Lock a rectangular portion of the bitmap for writing.
            var rect = new Rectangle(0, 0, Width, Height);

            var bitmapData = image.LockBits(
                rect,
                ImageLockMode.ReadWrite,
                image.PixelFormat);

            // Write to the temporary buffer that is provided by LockBits.
            // Copy the pixels from the source image in this loop.
            // Because you want an index, convert RGB to the appropriate
            // palette index here.
            var pixels = bitmapData.Scan0;

            unsafe
            {
                // Get the pointer to the image bits.
                // This is the unsafe operation.
                byte* pBits;
                if (bitmapData.Stride > 0)
                    pBits = (byte*)pixels.ToPointer();
                else
                    // If the Stide is negative, Scan0 points to the last 
                    // scanline in the buffer. To normalize the loop, obtain
                    // a pointer to the front of the buffer that is located 
                    // (Height-1) scanlines previous.
                    pBits = (byte*)pixels.ToPointer() + bitmapData.Stride * (Height - 1);
                var stride = (uint)Math.Abs(bitmapData.Stride);

                //var flip = 85 / nColors;

                for (uint row = 0; row < Height; ++row)
                {
                    for (uint col = 0; col < bitmapData.Stride; ++col)
                    {
                        byte* p8bppPixel = pBits + row * stride + col;

                        var old = (*p8bppPixel >> 5);
                        *p8bppPixel = (byte)(old << 5);

                    } /* end loop for col */
                } /* end loop for row */
            } /* end unsafe */

            // To commit the changes, unlock the portion of the bitmap.  
            image.UnlockBits(bitmapData);
        }
    }
}

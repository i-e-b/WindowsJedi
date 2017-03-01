using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace WindowsJedi.Algorithms
{
    using System;
    using System.Text;
    using System.Windows.Forms;

    public class GifWriter : IDisposable
    {
        FileStream _fileStream;
        readonly BinaryWriter _fileWriter;
        bool _firstFrame;
        readonly Size _size;
        readonly bool _drawCursor;
        readonly Bitmap _screenBuffer; // IDEA: copy using a small (32x32?) tile at a higher rate?
        readonly MemoryStream _buffer;

        readonly byte[] applicationExtension = {
            33,  //extension introducer
            255, //application extension
            11,  //size of block
            78,  //N
            69,  //E
            84,  //T
            83,  //S
            67,  //C
            65,  //A
            80,  //P
            69,  //E
            50,  //2
            46,  //.
            48,  //0
            3,   //Size of block
            1,   //
            0,   //
            0,   //
            0    //Block terminator
        };
        readonly byte[] graphicControlExtension = {
                 33,   //Extension introducer
                 249,  //Graphic control extension
                 4,    //Size of block
                 9,    //Flags: reserved, disposal method, user input, transparent color
                 5,    //Delay time low byte (hundredths of a second -- this relates to 20 fps)
                 0,    //Delay time high byte
                 255,  //Transparent color index
                 0     //Block terminator
            };

        /// <summary>
        /// Start an animated GIF with the given size and file name.
        /// Will immediately open the file, but headers are written on first frame
        /// </summary>
        public GifWriter(string fileName, Size size, bool drawCursor)
        {
            _buffer = new MemoryStream();
            _size = size;
            _drawCursor = drawCursor;
            _firstFrame = true;
            _fileStream = File.Open(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            _fileWriter = new BinaryWriter(_fileStream, Encoding.ASCII);

            // Get a render target for reading from the screen.
            // we do it this way to get the same pixel format as the screen.
            using (var g = Graphics.FromHwnd(IntPtr.Zero))
            {
                _screenBuffer = new Bitmap(size.Width, size.Height, g);
            }
        }

        public void Close()
        {
            lock (_fileStream)
            {
                if (_fileStream == null) { return; }

                _fileWriter.Write(";"); //Image terminator
                _fileWriter.Flush();
                _fileWriter.Close();
                _fileWriter.Dispose();

                _fileStream = null;
            }
        }

        public void Dispose()
        {
            Close();
        }

        /// <summary>
        /// Copy a section from the screen into the GIF file.
        /// Supply the top-left co-ord, the size is fixed to the overall image size.
        /// </summary>
        public void WriteScreenFrame(Point topLeft) {
            using (var g = Graphics.FromImage(_screenBuffer))
            {
                g.CopyFromScreen(topLeft, Point.Empty, _size);

                if (_drawCursor && Cursor.Current != null) { // this doesn't draw the correct cursor, but at least it shows *something*
                    var pt = new Point(Cursor.Position.X - topLeft.X - Cursor.Current.HotSpot.X, Cursor.Position.Y - topLeft.Y - Cursor.Current.HotSpot.Y);
                    var rect = new Rectangle(pt, Cursor.Current.Size);
                    Cursor.Current.Draw(g, rect);
                }
            }
            WriteImageFrame(_screenBuffer);
        }

        public void WriteImageFrame(Bitmap frame)
        {
            _buffer.SetLength(0);

            frame.Save(_buffer, ImageFormat.Gif); // This makes a mess of the dithering. To be improved... ( http://codebetter.com/brendantompkins/2007/06/14/gif-image-color-quantizer-now-with-safe-goodness/ )
            var gifFrame = _buffer.ToArray();

            if (_firstFrame)
            {
                _firstFrame = false;
                //only write these the first time....
                _fileWriter.Write(gifFrame, 0, 781); //Header & global color table
                _fileWriter.Write(applicationExtension, 0, 19); //Application extension
            }

            _fileWriter.Write(graphicControlExtension, 0, 8); //Graphic extension. TODO: frame duration
            _fileWriter.Write(gifFrame, 789, gifFrame.Length - 790); //Image data (with duplicated headers chopped out)
        }
    }
}

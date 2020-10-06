using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Media;
using System.Windows.Forms;

namespace LSSDReportHelper
{
    public class ScreenShooter
    {
        private readonly SoundPlayer _player = new SoundPlayer("camera.wav");

        public string SaveImage(string path)
        {
            using var captureBitmap = new Bitmap(Screen.AllScreens[0].Bounds.Width, Screen.AllScreens[0].Bounds.Height, PixelFormat.Format32bppArgb);
            var captureRectangle = Screen.AllScreens[0].Bounds;
            using var captureGraphics = Graphics.FromImage(captureBitmap);
            captureGraphics.CopyFromScreen(captureRectangle.Left, captureRectangle.Top, 0, 0, captureRectangle.Size);
            var fileCount = Directory.GetFiles(path).Length;
            var fileName = $@"{path}\[{fileCount + 1}] {DateTime.Now:dd.MM.yyyy-hh.mm.ss}.jpg";
            captureBitmap.Save(fileName, ImageFormat.Jpeg);
            _player.Play();
            return fileName;
        }
    }
}
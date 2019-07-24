namespace Isolines3D.UI.Extensions
{
    using System;
    using System.Drawing;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Класс расширений для <see cref="Bitmap"/>.
    /// </summary>
    public static class BitmapExtensions
    {

        /// <summary>
        /// Конвертирует в ресурс изображения.
        /// </summary>
        /// <param name="bitmapSource">Изображение полученное через <see cref="Bitmap"/>.</param>
        public static ImageSource ConvertToImageSource(
            this Bitmap bitmapSource) => System.Windows.Interop
            .Imaging.CreateBitmapSourceFromHBitmap(
                bitmapSource.GetHbitmap(), 
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
    }
}

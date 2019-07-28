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

        /// <summary>
        /// Попытка установить цвет пикселя на данное расположение в Bitmap. 
        /// При неудаче не устанавливает пиксель.
        /// </summary>
        /// <param name="bitmapSource">Изображение полученное через <see cref="Bitmap"/>.</param>
        /// <param name="xPoint">Позиция по ширине.</param>
        /// <param name="yPoint">Позиция по высоте.</param>
        /// <param name="color">Цвет устанавливаемого пикселя.</param>
        public static void TrySetPixel(this Bitmap bitmapSource, int xPoint, int yPoint, System.Drawing.Color color)
        {
            try
            {
                bitmapSource.SetPixel(xPoint, yPoint, color);
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// Проверяет существует ли пиксель по указанному индексу.
        /// </summary>
        /// <param name="bitmapSource">Изображение полученное через <see cref="Bitmap"/>.</param>
        /// <param name="xPoint">Позиция по ширине.</param>
        /// <param name="yPoint">Позиция по высоте.</param>
        /// <returns>Возвращает флаг существования пикселя.</returns>
        public static bool IsPixelExist(this Bitmap bitmapSource, int xPoint, int yPoint)
        {
            try
            {
                var pixel = bitmapSource.GetPixel(xPoint, yPoint);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}

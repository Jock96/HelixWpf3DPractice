namespace Isolines3D.UI.Commands
{
    using Isolines3D.UI.Extensions;
    using Isolines3D.UI.Helpers;
    using Isolines3D.UI.ViewModels;
    using System;
    using System.Drawing;
    using System.IO;

    /// <summary>
    /// Команда формирования модели по изолиниям.
    /// </summary>
    public class CreateByIsolinesCommand : BaseTCommand<MainWindowVM>
    {
        /// <summary>
        /// Выполнение команды.
        /// </summary>
        /// <param name="mainWindowVM">Вью-модель главного окна.</param>
        protected override void Execute(MainWindowVM mainWindowVM)
        {
            var resourcesPath = PathHelper.GetResourcesPath() + "\\Example.bmp";
            var random = new Random();

            var width = (int)mainWindowVM.BasePlane.Width;
            var length = (int)mainWindowVM.BasePlane.Length;

            //var width = 20;
            //var length = 20;

            var startPointX = random.Next(width + 1);
            var startPointY = random.Next(length + 1);

            using (var bitmap = new Bitmap(width + 1, length + 1))
            {
                var color = Color.FromArgb(byte.MaxValue, byte.MinValue, byte.MinValue);

                // Центральная точка.
                bitmap.TrySetPixel(startPointX, startPointY, color);

                int maxPixelIndex = byte.MaxValue + 1;
                var colorOffset = 5;

                for (var index = 0; index < maxPixelIndex / colorOffset; ++index)
                {
                    //FillColorMap(width, length, bitmap);
                    FillColorMap(width, length, bitmap, colorOffset);
                }

                // Для посмотра созданной карты виртуальных изолиний.
                bitmap.Save(resourcesPath);

                mainWindowVM.ImageSource = bitmap.ConvertToImageSource();
                mainWindowVM.Model = mainWindowVM.ModelCreatorUtil.CreateModelByImage(bitmap);
            }
        }

        private static void FillColorMap(int width, int length, Bitmap bitmap, int colorOffset = 0)
        {
            var random = new Random();

            for (var indexX = 0; indexX < width + 1; ++indexX)
            {
                for (var indexY = 0; indexY < length + 1; ++indexY)
                {
                    var currentPixel = bitmap.GetPixel(indexX, indexY);

                    if (currentPixel.R == 0)
                        continue;

                    var newColor = Color.White;

                    try
                    {
                        newColor = Color.FromArgb(currentPixel.R - colorOffset, currentPixel.G, currentPixel.B);
                    }
                    catch
                    {
                        continue;
                    }

                    var nextPixel = Color.White;

                    // Нижний пиксель.
                    if (bitmap.IsPixelExist(indexX, indexY - 1))
                        nextPixel = bitmap.GetPixel(indexX, indexY - 1);

                    if (nextPixel.R == 0)
                    {
                        var gridCollapse = random.Next(colorOffset);
                        var deltaFactor = random.Next(-1, 2);
                        
                        try
                        {
                            newColor = Color.FromArgb(currentPixel.R - colorOffset + deltaFactor * gridCollapse, currentPixel.G, currentPixel.B);
                        }
                        catch
                        {
                            // Ignore.
                        }

                        bitmap.TrySetPixel(indexX, indexY - 1, newColor);
                    }

                    // Верхний пиксель.
                    if (bitmap.IsPixelExist(indexX, indexY + 1))
                        nextPixel = bitmap.GetPixel(indexX, indexY + 1);

                    if (nextPixel.R == 0)
                    {
                        var gridCollapse = random.Next(colorOffset);
                        var deltaFactor = random.Next(-1, 2);

                        try
                        {
                            newColor = Color.FromArgb(currentPixel.R - colorOffset + deltaFactor * gridCollapse, currentPixel.G, currentPixel.B);
                        }
                        catch
                        {
                            // Ignore.
                        }

                        bitmap.TrySetPixel(indexX, indexY + 1, newColor);
                    }

                    // Левый пиксель.
                    if (bitmap.IsPixelExist(indexX - 1, indexY))
                        nextPixel = bitmap.GetPixel(indexX - 1, indexY);

                    if (nextPixel.R == 0)
                    {
                        var gridCollapse = random.Next(colorOffset);
                        var deltaFactor = random.Next(-1, 2);

                        try
                        {
                            newColor = Color.FromArgb(currentPixel.R - colorOffset + deltaFactor * gridCollapse, currentPixel.G, currentPixel.B);
                        }
                        catch
                        {
                            // Ignore.
                        }

                        bitmap.TrySetPixel(indexX - 1, indexY, newColor);
                    }

                    // Правый пиксель.
                    if (bitmap.IsPixelExist(indexX + 1, indexY))
                        nextPixel = bitmap.GetPixel(indexX + 1, indexY);

                    if (nextPixel.R == 0)
                    {
                        var gridCollapse = random.Next(colorOffset);
                        var deltaFactor = random.Next(-1, 2);

                        try
                        {
                            newColor = Color.FromArgb(currentPixel.R - colorOffset + deltaFactor * gridCollapse, currentPixel.G, currentPixel.B);
                        }
                        catch
                        {
                            // Ignore.
                        }

                        bitmap.TrySetPixel(indexX + 1, indexY, newColor);
                    }
                }
            }
        }
    }
}

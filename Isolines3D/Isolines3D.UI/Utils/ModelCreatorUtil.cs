namespace Isolines3D.UI.Utils
{
    using System;
    using System.Drawing;
    using System.Windows.Media.Media3D;

    using Color = System.Windows.Media.Color;
    using Isolines3D.UI.Helpers;

    /// <summary>
    /// Инструмент создание 3D модели.
    /// </summary>
    public class ModelCreatorUtil
    {
        /// <summary>
        /// Длинна модели.
        /// </summary>
        private static int _modelLenght;

        /// <summary>
        /// Ширина модели.
        /// </summary>
        private static int _modelWidth;

        /// <summary>
        /// Инструмент создание 3D модели.
        /// </summary>
        /// <param name="modelWidth">Ширина модели.</param>
        /// <param name="modelLenght">Длинна модели.</param>
        public ModelCreatorUtil(int modelWidth, int modelLenght)
        {
            _modelWidth = modelWidth;
            _modelLenght = modelLenght;
        }

        /// <summary>
        /// Создаёт 3D модель по изображению.
        /// </summary>
        /// <param name="bitmap">Изображение.</param>
        /// <param name="smoothingFactor">Сглаживание модели.</param>
        /// <returns>Возвращает 3D модель.</returns>
        public Model3DGroup CreateModelByImage(Bitmap bitmap, double smoothingFactor = 0)
        {
            var colorMap = ImageToColorMap(bitmap);
            var gridMatrix = new int[_modelWidth + 1, _modelLenght + 1];

            for (var indexX = 0; indexX < _modelWidth; ++indexX)
            {
                for (var indexY = 0; indexY < _modelLenght; ++indexY)
                    gridMatrix[indexX, indexY] = colorMap[indexX, indexY].R;
            }

            return ModelCreatorHelper.BuildModel(smoothingFactor, gridMatrix, _modelWidth, _modelLenght);
        }

        /// <summary>
        /// Создание модели по изолиниям.
        /// </summary>
        /// <param name="bitmap">Изображение.</param>
        /// <param name="smoothingFactor">Сглаживание модели.</param>
        /// <returns>Возвращает 3D модель.</returns>
        public Model3DGroup CreateModelByIsolines(Bitmap bitmap, double smoothingFactor = 0)
        {
            // Переделать.

            var pointMap = PointMapFromIsolines(bitmap);
            var gridMatrix = new int[_modelWidth + 1, _modelLenght + 1];

            for (var indexX = 0; indexX < _modelWidth; ++indexX)
            {
                for (var indexY = 0; indexY < _modelLenght; ++indexY)
                    gridMatrix[indexX, indexY] = pointMap[indexX, indexY];
            }

            return ModelCreatorHelper.BuildIsolines(gridMatrix, _modelWidth, _modelLenght);
        }

        /// <summary>
        /// Создание случайной трёхмерной поверхности.
        /// </summary>
        public Model3DGroup CreateRandomModel()
        {
            var gridMatrix = new int[_modelWidth + 1, _modelLenght + 1];
            var random = new Random();

            for (var indexX = 0; indexX < _modelWidth; ++indexX)
            {
                for (var indexY = 0; indexY < _modelLenght; ++indexY) gridMatrix[indexX, indexY] = random.Next(2, 10);
            }

            return ModelCreatorHelper.BuildModel(random.NextDouble(), gridMatrix, _modelWidth, _modelLenght);
        }

        /// <summary>
        /// Формирование карты по изолиниям.
        /// </summary>
        /// <remarks>
        /// Работает только на тестовом варианте.
        /// </remarks>
        /// <param name="bitmap">Изображение.</param>
        /// <returns>Возвращает карту цветов для преобразования в модель.</returns>
        private int[,] PointMapFromIsolines(Bitmap bitmap)
        {
            var pointMap = new int[_modelWidth + 1, _modelLenght + 1];
            var image = bitmap;

            var newSize = new Size(0, 0);

            if (bitmap.Width != _modelWidth ||
                bitmap.Height != _modelLenght)
            {
                newSize = new Size(_modelWidth + 1, _modelLenght + 1);
            }
            else
            {
                newSize = bitmap.Size;
            }

            // Поворот для корректного преобразования в битовую карту и модель.
            image.RotateFlip(RotateFlipType.Rotate180FlipX);

            using (var resizedBitmap = new Bitmap(image, newSize))
            {
                for (var indexX = resizedBitmap.Width - 1; indexX >= 0; --indexX)
                {
                    for (var indexY = 0; indexY < resizedBitmap.Height; ++indexY)
                    {
                        var currentPixel = resizedBitmap.GetPixel(indexX, indexY);

                        if (currentPixel.R == 0 &&
                            currentPixel.G == 0 &&
                            currentPixel.B == 0)
                        {
                            pointMap[indexX, indexY] = 1;
                        }
                    }
                }
            }

            return pointMap;
        }

        /// <summary>
        /// Конвертирует изображение в карту (двумерный массив) цветов.
        /// </summary>
        /// <returns>Карту цветов обработанного изображения.</returns>
        private Color[,] ImageToColorMap(Bitmap bitmap)
        {
            var colorMap = new Color[_modelWidth + 1, _modelLenght + 1];

            if (bitmap.Width != _modelWidth ||
                bitmap.Height != _modelLenght)
            {
                var image = bitmap;
                var newSize = new Size(_modelWidth + 1, _modelLenght + 1);

                // Поворот для корректного преобразования в битовую карту и модель.
                image.RotateFlip(RotateFlipType.Rotate180FlipX);

                using (var resizedBitmap = new Bitmap(image, newSize))
                {
                    for (var indexX = 0; indexX < resizedBitmap.Width; ++indexX)
                    {
                        for (var indexY = 0; indexY < resizedBitmap.Height; ++indexY)
                        {
                            var currentPixel = resizedBitmap.GetPixel(indexX, indexY);

                            var color = Color.FromArgb(currentPixel.A, currentPixel.R,
                                currentPixel.G, currentPixel.B);

                            colorMap[indexX, indexY] = color;
                        }
                    }
                }
            }

            return colorMap;
        }
    }
}
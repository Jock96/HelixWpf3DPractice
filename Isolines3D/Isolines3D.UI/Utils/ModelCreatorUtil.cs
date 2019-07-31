namespace Isolines3D.UI.Utils
{
    using HelixToolkit.Wpf;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Инструмент создание 3D модели.
    /// </summary>
    public class ModelCreatorUtil
    {
        /// <summary>
        /// Ширина модели.
        /// </summary>
        private static int _modelWidth;

        /// <summary>
        /// Длинна модели.
        /// </summary>
        private static int _modelLenght;

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
        /// Создание поверхности.
        /// </summary>
        /// <param name="gridMatrix">Сетка точек для расположения высот.</param>
        /// <param name="meshBuilder">Инструмент для работы с геометрией.</param>
        /// <param name="smoothingFactor">Сглаживание для конвертацииипостроения геометрии.</param>
        private static void CreateTerrain(int[,] gridMatrix, MeshBuilder meshBuilder, double smoothingFactor = 1)
        {
            var halfOfWidth = _modelWidth / 2;
            var halfOfLength = _modelLenght / 2;

            //var pointsList = new List<List<Point3D>>();
            var pointsArray = new List<Point3D>();

            for (var indexX = -halfOfWidth; indexX <= halfOfWidth; ++indexX)
            {
                for (var indexY = -halfOfLength; indexY <= halfOfLength; ++indexY)
                {
                    try
                    {
                        var pointOne = new Point3D(indexX, indexY, gridMatrix[indexX + halfOfWidth, indexY + halfOfLength] * smoothingFactor);
                        var pointTwo = new Point3D(indexX + 1, indexY, gridMatrix[indexX + halfOfWidth + 1, indexY + halfOfLength] * smoothingFactor);
                        var pointThree = new Point3D(indexX, indexY + 1, gridMatrix[indexX + halfOfWidth, indexY + halfOfLength + 1] * smoothingFactor);

                        meshBuilder.AddTriangle(pointOne, pointTwo, pointThree);

                        pointsArray.Add(pointOne);
                        pointsArray.Add(pointTwo);
                        pointsArray.Add(pointThree);
                    }
                    catch
                    {
                        // Ignore.
                    }

                    try
                    {
                        var pointFour = new Point3D(indexX + 1, indexY, gridMatrix[indexX + halfOfWidth + 1, indexY + halfOfLength] * smoothingFactor);
                        var pointFive = new Point3D(indexX, indexY + 1, gridMatrix[indexX + halfOfWidth, indexY + halfOfLength + 1] * smoothingFactor);
                        var pointSix = new Point3D(indexX + 1, indexY + 1, gridMatrix[indexX + halfOfWidth + 1, indexY + halfOfLength + 1] * smoothingFactor);

                        meshBuilder.AddTriangle(pointFour, pointFive, pointSix);

                        pointsArray.Add(pointFour);
                        pointsArray.Add(pointFive);
                        pointsArray.Add(pointSix);
                    }
                    catch
                    {
                        // Ignore.
                    }
                }
            }

            // Убрать, если будет виснуть.
            meshBuilder.CreateNormals = true;

            foreach (var point in pointsArray)
                meshBuilder.AddBox(point, 1, 1, 1);
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
                for (var indexY = 0; indexY < _modelLenght; ++indexY)
                {
                    gridMatrix[indexX, indexY] = random.Next(2, 10);
                }
            }

            var modelGroup = new Model3DGroup();
            var meshBuilder = new MeshBuilder(false, false);

            CreateTerrain(gridMatrix, meshBuilder);

            var mesh = meshBuilder.ToMesh(true);
            var redMaterial = MaterialHelper.CreateMaterial(Colors.Red);

            modelGroup.Children.Add(new GeometryModel3D
            {
                Geometry = mesh,
                Material = redMaterial,
                BackMaterial = redMaterial
            });

            return modelGroup;
        }

        /// <summary>
        /// Создание модели по изолиниям.
        /// </summary>
        /// <param name="bitmap">Изображение.</param>
        /// <param name="smoothingFactor">Сглаживание модели.</param>
        /// <returns>Возвращает 3D модель.</returns>
        public Model3DGroup CreateModelByIsolines(Bitmap bitmap, double smoothingFactor = 0)
        {
            var colorMap = ColorMapFromIsolines(bitmap);

            var gridMatrix = new int[_modelWidth + 1, _modelLenght + 1];

            for (var indexX = 0; indexX < _modelWidth; ++indexX)
            {
                for (var indexY = 0; indexY < _modelLenght; ++indexY)
                {
                    gridMatrix[indexX, indexY] = colorMap[indexX, indexY].R;
                }
            }

            return BuildModel(smoothingFactor, gridMatrix);
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
                {
                    gridMatrix[indexX, indexY] = colorMap[indexX, indexY].R;
                }
            }

            return BuildModel(smoothingFactor, gridMatrix);
        }

        /// <summary>
        /// Создать модель по сетке высот и заданым сглаживанием.
        /// </summary>
        /// <param name="smoothingFactor">Фактор сглаживание.</param>
        /// <param name="gridMatrix">Сетка высот.</param>
        /// <returns>Возвращает построенную модель.</returns>
        private static Model3DGroup BuildModel(double smoothingFactor, int[,] gridMatrix)
        {
            var modelGroup = new Model3DGroup();
            var meshBuilder = new MeshBuilder(false, false);

            meshBuilder.CreateNormals = true;

            if (smoothingFactor == 0)
                CreateTerrain(gridMatrix, meshBuilder, 0.1);
            else
                CreateTerrain(gridMatrix, meshBuilder, smoothingFactor);

            var mesh = meshBuilder.ToMesh(true);
            var material = MaterialHelper.CreateMaterial(Colors.Green);

            modelGroup.Children.Add(new GeometryModel3D
            {
                Geometry = mesh,
                Material = material,
                BackMaterial = material
            });

            return modelGroup;
        }

        /// <summary>
        /// Формирование карты по изолиниям.
        /// </summary>
        /// <remarks>
        /// Работает только на тестовом варианте.
        /// </remarks>
        /// <param name="bitmap">Изображение.</param>
        /// <returns>Возвращает карту цветов для преобразования в модель.</returns>
        private System.Windows.Media.Color[,] ColorMapFromIsolines(Bitmap bitmap)
        {
            var colorMap = new System.Windows.Media.Color[_modelWidth + 1, _modelLenght + 1];

            if (bitmap.Width != _modelWidth ||
                bitmap.Height != _modelLenght)
            {
                var image = bitmap;
                var newSize = new System.Drawing.Size(_modelWidth + 1, _modelLenght + 1);

                // Поворот для корректного преобразования в битовую карту и модель.
                image.RotateFlip(RotateFlipType.Rotate180FlipX);

                using (var resizedBitmap = new Bitmap(image, newSize))
                {
                    for (var indexX = 0; indexX < resizedBitmap.Width; ++indexX)
                    {
                        for (var indexY = 0; indexY < resizedBitmap.Height; ++indexY)
                        {
                            var currentPixel = resizedBitmap.GetPixel(indexX, indexY);

                            var color = System.Windows.Media.Color.FromArgb(0,0,0,0);

                            // Изображение залито в белом фоне цветом с содержанием R
                            // равным: 255, 254, 253 и т.д.

                            switch (currentPixel.R)
                            {
                                case 255:
                                    color.R = 1;
                                    break;

                                case 254:
                                    color.R = 45;
                                    break;

                                case 253:
                                    color.R = 95;
                                    break;

                                case 252:
                                    color.R = 150;
                                    break;

                                case 251:
                                    color.R = 200;
                                    break;

                                case 250:
                                    color.R = 250;
                                    break;

                                default:
                                    break;
                            }

                            colorMap[indexX, indexY] = color;
                        }
                    }
                }
            }

            return colorMap;
        }

        /// <summary>
        /// Конвертирует изображение в карту (двумерный массив) цветов.
        /// </summary>
        /// <returns>Карту цветов обработанного изображения.</returns>
        private System.Windows.Media.Color[,] ImageToColorMap(Bitmap bitmap)
        {
            var colorMap = new System.Windows.Media.Color[_modelWidth + 1, _modelLenght + 1];

            if (bitmap.Width != _modelWidth ||
                bitmap.Height != _modelLenght)
            {
                var image = bitmap;
                var newSize = new System.Drawing.Size(_modelWidth + 1, _modelLenght + 1);

                // Поворот для корректного преобразования в битовую карту и модель.
                image.RotateFlip(RotateFlipType.Rotate180FlipX);

                using (var resizedBitmap = new Bitmap(image, newSize))
                {
                    for (var indexX = 0; indexX < resizedBitmap.Width; ++indexX)
                    {
                        for (var indexY = 0; indexY < resizedBitmap.Height; ++indexY)
                        {
                            var currentPixel = resizedBitmap.GetPixel(indexX, indexY);

                            var color = System.Windows.Media.Color.
                                FromArgb(currentPixel.A, currentPixel.R,
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

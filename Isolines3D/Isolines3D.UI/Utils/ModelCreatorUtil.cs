namespace Isolines3D.UI.Utils
{
    using HelixToolkit.Wpf;
    using System;
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

            for (var indexX = -halfOfWidth; indexX <= halfOfWidth; ++indexX)
            {
                for (var indexY = -halfOfLength; indexY <= halfOfLength; ++indexY)
                {
                    try
                    {
                        meshBuilder.AddTriangle(
                        new Point3D(indexX, indexY, gridMatrix[indexX + halfOfWidth, indexY + halfOfLength] * smoothingFactor),
                        new Point3D(indexX + 1, indexY, gridMatrix[indexX + halfOfWidth + 1, indexY + halfOfLength] * smoothingFactor),
                        new Point3D(indexX, indexY + 1, gridMatrix[indexX + halfOfWidth, indexY + halfOfLength + 1] * smoothingFactor));
                    }
                    catch
                    {
                        // Ignore.
                    }

                    try
                    {
                        meshBuilder.AddTriangle(
                            new Point3D(indexX + 1, indexY, gridMatrix[indexX + halfOfWidth + 1, indexY + halfOfLength] * smoothingFactor),
                            new Point3D(indexX, indexY + 1, gridMatrix[indexX + halfOfWidth, indexY + halfOfLength + 1] * smoothingFactor),
                            new Point3D(indexX + 1, indexY + 1, gridMatrix[indexX + halfOfWidth + 1, indexY + halfOfLength + 1] * smoothingFactor));
                    }
                    catch
                    {
                        // Ignore.
                    }
                }
            }
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
        /// Создаёт 3D модель по изображению.
        /// </summary>
        /// <param name="bitmap">Изображение.</param>
        /// <returns>Возвращает 3D модель.</returns>
        public Model3DGroup CreateModelByImage(Bitmap bitmap)
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

            var modelGroup = new Model3DGroup();
            var meshBuilder = new MeshBuilder(false, false);

            CreateTerrain(gridMatrix, meshBuilder, 0.1);

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

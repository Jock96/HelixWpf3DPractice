namespace Isolines3D.UI.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    using HelixToolkit.Wpf;

    using Color = System.Windows.Media.Color;

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

            return BuildModel(smoothingFactor, gridMatrix);
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

            var modelGroup = BuildIsolines(gridMatrix);

            //var transform = new TranslateTransform3D
            //{
            //    OffsetZ = 5
            //};

            //modelGroup.Children[0].Transform = transform;

            //transform.OffsetZ = 7;
            //modelGroup.Children[1].Transform = transform;

            //transform.OffsetZ = 15;
            //modelGroup.Children[2].Transform = transform;

            //transform.OffsetZ = 12;
            //modelGroup.Children[3].Transform = transform;

            return modelGroup;
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
        /// Находит ближайшие точки со значениями.
        /// </summary>
        /// <param name="gridMatrix">Матрица точек.</param>
        /// <param name="firstIndex">Индекс первый.</param>
        /// <param name="secondIndex">Индекс второй.</param>
        /// <returns>Возвращает список ближайших точек со значениями.</returns>
        private static List<Point3D> FindNearestPoint(int[,] gridMatrix, int firstIndex, int secondIndex, int halfOfWidth, int halfOfLength)
        {
            var points = new List<Point3D>();

            // Обход по часовой стрелке начаная с 00 : 00.

            try
            {
                if (gridMatrix[firstIndex + 1, secondIndex] == 1)
                    points.Add(new Point3D(firstIndex + 1, secondIndex, 1));
            }
            catch { }

            try
            {
                if (gridMatrix[firstIndex + 1, secondIndex + 1] == 1)
                    points.Add(new Point3D(firstIndex + 1, secondIndex + 1, 1));
            }
            catch { }

            try
            {
                if (gridMatrix[firstIndex, secondIndex + 1] == 1)
                    points.Add(new Point3D(firstIndex, secondIndex + 1, 1));
            }
            catch { }

            try
            {
                if (gridMatrix[firstIndex - 1, secondIndex + 1] == 1)
                    points.Add(new Point3D(firstIndex - 1, secondIndex + 1, 1));
            }
            catch { }

            try
            {
                if (gridMatrix[firstIndex - 1, secondIndex] == 1)
                    points.Add(new Point3D(firstIndex - 1, secondIndex, 1));
            }
            catch { }

            try
            {
                if (gridMatrix[firstIndex - 1, secondIndex - 1] == 1)
                    points.Add(new Point3D(firstIndex - 1, secondIndex - 1, 1));
            }
            catch { }

            try
            {
                if (gridMatrix[firstIndex, secondIndex - 1] == 1)
                    points.Add(new Point3D(firstIndex, secondIndex - 1, 1));
            }
            catch { }

            try
            {
                if (gridMatrix[firstIndex + 1, secondIndex - 1] == 1)
                    points.Add(new Point3D(firstIndex + 1, secondIndex - 1, 1));
            }
            catch { }

            return points;
        }

        /// <summary>
        /// Создать линию по сетке точек.
        /// </summary>
        /// <param name="gridMatrix">Сетка точек.</param>
        /// <returns>Возвращает построенную модель.</returns>
        private static Model3DGroup BuildIsolines(int[,] gridMatrix)
        {
            var modelGroup = new Model3DGroup();
            var meshBuilder = new MeshBuilder(false, false) { CreateNormals = true };

            var halfOfWidth = _modelWidth / 2;
            var halfOfLength = _modelLenght / 2;

            var pointsByCoords = new List<Point3D>();

            for (var indexX = -halfOfWidth; indexX <= halfOfWidth; ++indexX)
            {
                for (var indexY = -halfOfLength; indexY <= halfOfLength; ++indexY)
                {
                    var currentPoint = gridMatrix[indexX + halfOfWidth, indexY + halfOfLength];

                    if (currentPoint == 1)
                        pointsByCoords.Add(new Point3D(indexX, indexY, 1));

                    continue;
                }
            }

            var zCoordinate = 2;

            do
            {
                var firstPoint = new Point3D(pointsByCoords.First().X,
                    pointsByCoords.First().Y, 1);

                var startPoint = new Point3D(pointsByCoords.First().X,
                    pointsByCoords.First().Y, 1);

                var pointsOfLine = new List<Point3D>
                {
                    firstPoint
                };

                while (true)
                {
                    var gridPoints = FindNearestPoint(gridMatrix, (int)firstPoint.X + halfOfWidth,
                        (int)firstPoint.Y + halfOfLength, halfOfWidth, halfOfLength);

                    var realPoints = new List<Point3D>();

                    foreach (var point in gridPoints)
                        realPoints.Add(new Point3D(point.X - halfOfWidth,
                            point.Y - halfOfLength, 1));

                    if (realPoints.All(element => pointsOfLine.Contains(element)))
                    {
                        // Попытка выйти из loop.

                        var listOfNearestPoints = new List<Point3D>();

                        foreach (var realPoint in realPoints)
                        {
                            var nearestPoints = FindNearestPoint(gridMatrix, (int)realPoint.X + halfOfWidth,
                                (int)realPoint.Y + halfOfLength, halfOfWidth, halfOfLength);

                            foreach (var point in nearestPoints)
                                listOfNearestPoints.Add(new Point3D(point.X - halfOfWidth,
                                    point.Y - halfOfLength, 1));
                        }

                        listOfNearestPoints = listOfNearestPoints.Distinct().ToList();

                        if (!listOfNearestPoints.Contains(startPoint))
                        {
                            firstPoint = listOfNearestPoints.Find(point => !pointsOfLine.Contains(point));
                            continue;
                        }

                        CreateIsolinesByPoints(modelGroup, meshBuilder, pointsByCoords, pointsOfLine, zCoordinate);
                        zCoordinate += 5;

                        break;
                    }
                    else
                    {
                        foreach (var nextPoint in realPoints)
                        {
                            if (!pointsOfLine.Contains(nextPoint))
                                pointsOfLine.Add(nextPoint);
                        }

                        firstPoint = pointsOfLine.Last();
                    }
                }
            }
            while (pointsByCoords.Any());

            return modelGroup;
        }

        /// <summary>
        /// Создание изолиний по точкам.
        /// </summary>
        /// <param name="modelGroup">Группа модели.</param>
        /// <param name="meshBuilder">Инструмент создания геометрии.</param>
        /// <param name="pointsByCoords">Точки по матрице.</param>
        /// <param name="pointsOfLine">Точки полилинии.</param>
        /// <param name="zCoordinate">Координата высоты.</param>
        private static void CreateIsolinesByPoints(Model3DGroup modelGroup, MeshBuilder meshBuilder,
            List<Point3D> pointsByCoords, List<Point3D> pointsOfLine, double zCoordinate)
        {
            // создаём линию и очищаем словарь.

            foreach (var point in pointsOfLine)
            {
                var indexOfNext = pointsOfLine.IndexOf(point) + 1;

                var firstPoint = new Point3D(point.X, point.Y, zCoordinate);
                var secondPoint = new Point3D();

                try
                {
                    secondPoint = new Point3D(pointsOfLine[indexOfNext].X, pointsOfLine[indexOfNext].Y, zCoordinate);
                }
                catch
                {
                    secondPoint = new Point3D(pointsOfLine.First().X, pointsOfLine.First().Y, zCoordinate);
                }
                finally
                {
                    meshBuilder.AddPipe(firstPoint, secondPoint, 0, 0.5, 10);
                    //meshBuilder.AddSphere(firstPoint, 1, 10, 10);
                    //meshBuilder.AddSphere(firstPoint, 1, 5, 5);
                }
            }

            var y = new List<Point3D>();

            foreach (var x in pointsOfLine)
            {
                y.Add(new Point3D(x.X - 1, x.Y - 1, 1));
            }

            var intersect = pointsByCoords.Intersect(pointsOfLine).ToList();

            if (intersect.Any())
            {
                pointsByCoords.RemoveAll(point => intersect.Contains(point));
                var mesh = meshBuilder.ToMesh(true);
                var material = MaterialHelper.CreateMaterial(Colors.Green);

                modelGroup.Children.Add(new GeometryModel3D
                {
                    Geometry = mesh,
                    Material = material,
                    BackMaterial = material
                });
            }
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
        /// Создание модели реверсивного треугольника.
        /// </summary>
        /// <param name="gridMatrix">Матрица точек.</param>
        /// <param name="meshBuilder">Инструмент создания поверхности.</param>
        /// <param name="smoothingFactor">Фактор сглаживания.</param>
        /// <param name="halfOfWidth">Половина ширины модели.</param>
        /// <param name="halfOfLength">Половина длинны модели.</param>
        /// <param name="indexX">Точка вставки по X.</param>
        /// <param name="indexY">Точка вставки по Y.</param>
        private static void CreateReversedTriangle(int[,] gridMatrix, MeshBuilder meshBuilder, double smoothingFactor,
            int halfOfWidth, int halfOfLength, int indexX, int indexY)
        {
            var pointFour = new Point3D(indexX + 1, indexY,
                gridMatrix[indexX + halfOfWidth + 1, indexY + halfOfLength] * smoothingFactor);
            var pointFive = new Point3D(indexX, indexY + 1,
                gridMatrix[indexX + halfOfWidth, indexY + halfOfLength + 1] * smoothingFactor);
            var pointSix = new Point3D(indexX + 1, indexY + 1,
                gridMatrix[indexX + halfOfWidth + 1, indexY + halfOfLength + 1] * smoothingFactor);

            meshBuilder.AddTriangle(pointFour, pointFive, pointSix);
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
                    meshBuilder.AddSphere(new Point3D(indexX, indexY, gridMatrix[indexX + halfOfWidth, indexY + halfOfLength] * smoothingFactor), 1, 4, 4);

                    try
                    {
                        CreateTriangle(gridMatrix, meshBuilder, smoothingFactor, halfOfWidth, halfOfLength, indexX,
                            indexY);
                    }
                    catch
                    {
                        // Ignore.
                    }

                    try
                    {
                        CreateReversedTriangle(gridMatrix, meshBuilder, smoothingFactor, halfOfWidth, halfOfLength,
                            indexX, indexY);
                    }
                    catch
                    {
                        // Ignore.
                    }
                }
            }


            //meshBuilder.CreateNormals = true;

            //foreach (var point in pointsArray)
            //    meshBuilder.AddBox(point, 1, 1, 1);
        }

        /// <summary>
        /// Создание модели треугольника.
        /// </summary>
        /// <param name="gridMatrix">Матрица точек.</param>
        /// <param name="meshBuilder">Инструмент создания поверхности.</param>
        /// <param name="smoothingFactor">Фактор сглаживания.</param>
        /// <param name="halfOfWidth">Половина ширины модели.</param>
        /// <param name="halfOfLength">Половина длинны модели.</param>
        /// <param name="indexX">Точка вставки по X.</param>
        /// <param name="indexY">Точка вставки по Y.</param>
        private static void CreateTriangle(int[,] gridMatrix, MeshBuilder meshBuilder, double smoothingFactor,
            int halfOfWidth, int halfOfLength, int indexX, int indexY)
        {
            var pointOne = new Point3D(indexX, indexY,
                gridMatrix[indexX + halfOfWidth, indexY + halfOfLength] * smoothingFactor);
            var pointTwo = new Point3D(indexX + 1, indexY,
                gridMatrix[indexX + halfOfWidth + 1, indexY + halfOfLength] * smoothingFactor);
            var pointThree = new Point3D(indexX, indexY + 1,
                gridMatrix[indexX + halfOfWidth, indexY + halfOfLength + 1] * smoothingFactor);

            meshBuilder.AddTriangle(pointOne, pointTwo, pointThree);
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
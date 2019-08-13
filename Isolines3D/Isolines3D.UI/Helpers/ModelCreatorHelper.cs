namespace Isolines3D.UI.Helpers
{
    using HelixToolkit.Wpf;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using TriangulationDelone;

    public static class ModelCreatorHelper
    {
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
        /// <param name="modelWidth">Ширина модели.</param>
        /// <param name="modelLength">Длина модели.</param>
        /// <returns>Возвращает построенную модель.</returns>
        public static Model3DGroup BuildIsolines(int[,] gridMatrix, int modelWidth, int modelLength)
        {
            var modelGroup = new Model3DGroup();
            var meshBuilder = new MeshBuilder(false, false) { CreateNormals = true };

            var halfOfWidth = modelWidth / 2;
            var halfOfLength = modelLength / 2;

            var pointsByCoords = new List<Point3D>();

            for (var indexX = -halfOfWidth; indexX <= halfOfWidth; ++indexX)
            {
                for (var indexY = -halfOfLength; indexY <= halfOfLength; ++indexY)
                {
                    var currentPoint = gridMatrix[indexX + halfOfWidth, indexY + halfOfLength];

                    if (currentPoint == 1)
                        pointsByCoords.Add(new Point3D(indexX, indexY, 1));
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

                    foreach (var nextPoint in realPoints)
                    {
                        if (!pointsOfLine.Contains(nextPoint))
                            pointsOfLine.Add(nextPoint);
                    }

                    firstPoint = pointsOfLine.Last();
                }
            }
            while (pointsByCoords.Any());

            BuildGrid(meshBuilder, halfOfWidth, halfOfLength);

            var mesh = meshBuilder.ToMesh(true);
            var material = MaterialHelper.CreateMaterial(Colors.Gray);

            modelGroup.Children.Add(new GeometryModel3D
            {
                Geometry = mesh,
                Material = material,
                BackMaterial = material
            });

            indexer = 0;
            lineModels.Clear();
            return modelGroup;
        }

        /// <summary>
        /// Построить поверхность.
        /// </summary>
        /// <param name="meshBuilder">Инструмент создания поверхности.</param>
        /// <param name="halfOfWidth">Половина ширины модели.</param>
        /// <param name="halfOfLength">Половина длинные модели.</param>
        private static void BuildGrid(MeshBuilder meshBuilder, int halfOfWidth, int halfOfLength)
        {
            var points = new List<Point3D>();

            var upBorder = new List<Point3D>();
            var rightBorder = new List<Point3D>();
            var bottomBorder = new List<Point3D>();
            var leftBorder = new List<Point3D>();

            for (var indexX = -halfOfWidth; indexX <= halfOfWidth; ++indexX)
                upBorder.Add(new Point3D(indexX, halfOfLength, 0));

            for (var indexY = -halfOfLength; indexY <= halfOfLength; ++indexY)
                rightBorder.Add(new Point3D(halfOfWidth, indexY, 0));

            for (var indexX = -halfOfWidth; indexX <= halfOfWidth; ++indexX)
                bottomBorder.Add(new Point3D(indexX, -halfOfLength, 0));

            for (var indexY = -halfOfLength; indexY <= halfOfLength; ++indexY)
                leftBorder.Add(new Point3D(-halfOfWidth, indexY, 0));

            var borderPoints = new List<Point3D>();

            borderPoints.AddRange(upBorder);
            borderPoints.AddRange(bottomBorder);
            borderPoints.AddRange(leftBorder);
            borderPoints.AddRange(rightBorder);

            foreach (var lineModel in lineModels)
                points.AddRange(lineModel.Value);

            var listOfVertex = new List<Vertex>();

            points.ForEach(point => listOfVertex.Add(new Vertex((float)point.X, (float)point.Y, (float)point.Z)));

            var mesh = new Mesh();

            mesh.Compute(listOfVertex, upBorder.First(), halfOfWidth * 2, halfOfLength * 2);
            mesh.Draw(meshBuilder);
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
                    // Для визуального отображения полилинии.
                    //meshBuilder.AddPipe(firstPoint, secondPoint, 0, 0.5, 10);
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
                var material = MaterialHelper.CreateMaterial(Colors.Gray);

                modelGroup.Children.Add(new GeometryModel3D
                {
                    Geometry = mesh,
                    Material = material,
                    BackMaterial = material
                });

                lineModels.Add(indexer, intersect);

                var copyOfIntersect = new List<Point3D>();

                intersect.ForEach(element => copyOfIntersect.Add(element));

                foreach (var point in copyOfIntersect)
                {
                    var indexOfPoint = intersect.IndexOf(point);

                    intersect[indexOfPoint] = new Point3D(point.X, point.Y, zCoordinate);
                }

                indexer += 1;
            }
        }

        /// <summary>
        /// Список линий на сцене.
        /// </summary>
        private static Dictionary<int, List<Point3D>> lineModels = new Dictionary<int, List<Point3D>>();

        /// <summary>
        /// Индекс входных линий.
        /// </summary>
        private static int indexer = 0;

        /// <summary>
        /// Создать модель по сетке высот и заданым сглаживанием.
        /// </summary>
        /// <param name="smoothingFactor">Фактор сглаживание.</param>
        /// <param name="gridMatrix">Сетка высот.</param>
        /// <param name="modelWidth">Ширина модели.</param>
        /// <param name="modelLength">Длина модели.</param>
        /// <returns>Возвращает построенную модель.</returns>
        public static Model3DGroup BuildModel(double smoothingFactor, int[,] gridMatrix, int modelWidth, int modelLength)
        {
            var modelGroup = new Model3DGroup();
            var meshBuilder = new MeshBuilder(false, false)
            {
                CreateNormals = true
            };

            if (smoothingFactor == 0)
                CreateTerrain(gridMatrix, meshBuilder, modelWidth, modelLength, 0.1);
            else
                CreateTerrain(gridMatrix, meshBuilder, modelWidth, modelLength, smoothingFactor);

            var mesh = meshBuilder.ToMesh(true);
            var material = MaterialHelper.CreateMaterial(Colors.Gray);

            modelGroup.Children.Add(new GeometryModel3D
            {
                Geometry = mesh,
                Material = material,
                BackMaterial = material
            });

            return modelGroup;
        }

        /// <summary>
        /// Создание поверхности.
        /// </summary>
        /// <param name="gridMatrix">Сетка точек для расположения высот.</param>
        /// <param name="meshBuilder">Инструмент для работы с геометрией.</param>
        /// <param name="modelWidth">Ширина модели.</param>
        /// <param name="modelLength">Длина модели.</param>
        /// <param name="smoothingFactor">Сглаживание для конвертацииипостроения геометрии.</param>
        private static void CreateTerrain(int[,] gridMatrix, MeshBuilder meshBuilder, int modelWidth, int modelLength, double smoothingFactor = 1)
        {
            var halfOfWidth = modelWidth / 2;
            var halfOfLength = modelLength / 2;

            for (var indexX = -halfOfWidth; indexX <= halfOfWidth; ++indexX)
            {
                for (var indexY = -halfOfLength; indexY <= halfOfLength; ++indexY)
                {
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
    }
}

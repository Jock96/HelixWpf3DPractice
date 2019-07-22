namespace Isolines3D.UI.ViewModels
{
    using HelixToolkit.Wpf;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Linq;
    using Isolines3D.UI.Commands;

    /// <summary>
    /// Вью-модель главного окна.
    /// </summary>
    public class MainWindowVM : BaseVM
    {
        /// <summary>
        /// Адаптация среды 3D инструменртами HelixToolkit.
        /// </summary>
        private HelixViewport3D _helixViewport3D;

        /// <summary>
        /// Адаптация среды 3D инструменртами HelixToolkit.
        /// </summary>
        public HelixViewport3D HelixViewport3D
        {
            get => _helixViewport3D;
            set
            {
                _helixViewport3D = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Команда "скрыть/показать направление света на сцене".
        /// </summary>
        public ShowLightsDirectionCommand ShowLightsDirectionCommand { get; set; }

        /// <summary>
        /// Вью-модель главного окна.
        /// </summary>
        /// <param name="helixViewport3D">Адаптация среды 3D инструменртами HelixToolkit.</param>
        public MainWindowVM(HelixViewport3D helixViewport3D)
        {
            _helixViewport3D = helixViewport3D;
            InitializeViewPort3D();

            helixViewport3D.Loaded += ViewportInitializeHandle;
            CreateRandomGridByPlane();

            ShowLightsDirectionCommand = new ShowLightsDirectionCommand();
        }

        /// <summary>
        /// Создание виртуальной сетки с рандомными значениями для изолиний.
        /// </summary>
        private void CreateRandomGridByPlane()
        {
            var gridMatrix = new int[_basePlaneWidth + 1, _basePlaneLength + 1];

            var random = new Random();

            for (var indexX = 0; indexX < _basePlaneWidth; ++indexX)
            {
                for (var indexY = 0; indexY < _basePlaneLength; ++indexY)
                {
                    gridMatrix[indexX, indexY] = random.Next(2, 10);
                }
            }

            var modelGroup = new Model3DGroup();
            var meshBuilder = new MeshBuilder(false, false);

            CreteTerrain(gridMatrix, meshBuilder);

            var mesh = meshBuilder.ToMesh(true);
            var redMaterial = MaterialHelper.CreateMaterial(Colors.Red);

            modelGroup.Children.Add(new GeometryModel3D
            {
                Geometry = mesh,
                Material = redMaterial,
                BackMaterial = redMaterial
            });

            Model = modelGroup;

            ////(Model as Model3DGroup).Children;

            ////var randomMaterial = MaterialHelper.CreateMaterial(
            ////    Color.FromRgb(
            ////    (byte)random.Next(byte.MinValue, byte.MaxValue),
            ////    (byte)random.Next(byte.MinValue, byte.MaxValue),
            ////    (byte)random.Next(byte.MinValue, byte.MaxValue)));
        }

        /// <summary>
        /// Создание поверхности с перепадами.
        /// </summary>
        /// <param name="gridMatrix">Сетка точек для расположения высот.</param>
        /// <param name="meshBuilder">Инструмент для работы с геометрией.</param>
        private static void CreteTerrain(int[,] gridMatrix, MeshBuilder meshBuilder)
        {
            var halfOfWidth = _basePlaneWidth / 2;
            var halfOfLength = _basePlaneLength / 2;

            for (var indexX = -halfOfWidth; indexX <= halfOfWidth; ++indexX)
            {
                for (var indexY = -halfOfLength; indexY <= halfOfLength; ++indexY)
                {
                    try
                    {
                        meshBuilder.AddTriangle(
                        new Point3D(indexX, indexY, gridMatrix[indexX + halfOfWidth, indexY + halfOfLength]),
                        new Point3D(indexX + 1, indexY, gridMatrix[indexX + halfOfWidth + 1, indexY + halfOfLength]),
                        new Point3D(indexX, indexY + 1, gridMatrix[indexX + halfOfWidth, indexY + halfOfLength + 1]));
                    }
                    catch
                    {
                        // Ignore.
                    }

                    try
                    {
                        meshBuilder.AddTriangle(
                            new Point3D(indexX + 1, indexY, gridMatrix[indexX + halfOfWidth + 1, indexY + halfOfLength]),
                            new Point3D(indexX, indexY + 1, gridMatrix[indexX + halfOfWidth, indexY + halfOfLength + 1]),
                            new Point3D(indexX + 1, indexY + 1, gridMatrix[indexX + halfOfWidth + 1, indexY + halfOfLength + 1]));
                    }
                    catch
                    {
                        // Ignore.
                    }
                }
            }
        }

        /// <summary>
        /// События при инициализации 3D сцены.
        /// </summary>
        /// <param name="sender">Объект отправитель.</param>
        /// <param name="e">Аргументы события.</param>
        private void ViewportInitializeHandle(object sender, EventArgs e)
        {
            var newUpDirection = _helixViewport3D.Camera.UpDirection;
            var animationTime = 2000d;
            var newDirection = new Vector3D();

            newDirection = new Vector3D(0, 1, -0.5);
            _helixViewport3D.Camera.ChangeDirection(newDirection, newUpDirection, animationTime);

            newDirection = new Vector3D(-1, 0, -0.5); 
            _helixViewport3D.Camera.ChangeDirection(newDirection, newUpDirection, animationTime);
        }

        /// <summary>
        /// Обработка элементов <see cref="HelixViewport3D"/> перед загрузкой.
        /// </summary>
        private void InitializeViewPort3D()
        {
            _helixViewport3D.ShowFrameRate = true;
            _helixViewport3D.ShowCameraInfo = true;
            _helixViewport3D.CalculateCursorPosition = true;

            foreach (var child in _helixViewport3D.Children)
            {
                if (child is RectangleVisual3D rectanglevisual3D)
                {
                    rectanglevisual3D.Content = BasePlane.Content;
                    continue;
                }

                if (child is LightSetup light)
                {
                    light.ShowLights = false;
                    continue;
                }
            }
        }

        /// <summary>
        /// Длина базового плана.
        /// </summary>
        /// <remarks>Размеры имеют тип <see cref="int"/>
        /// для удобства составления сетки с геометрией.</remarks>
        private static int _basePlaneLength = 20;

        /// <summary>
        /// Ширина базового плана.
        /// </summary>
        /// <remarks>Размеры имеют тип <see cref="int"/>
        /// для удобства составления сетки с геометрией.</remarks>
        private static int _basePlaneWidth = 20;

        /// <summary>
        /// Модель базовго плана.
        /// </summary>
        private RectangleVisual3D _basePlane = new RectangleVisual3D
        {
            Material = MaterialHelper.CreateMaterial(Colors.Blue),
            Fill = BrushHelper.CreateGradientBrush(Colors.Blue, Colors.White),
            Length = _basePlaneLength,
            Width = _basePlaneWidth
        };

        /// <summary>
        /// 3D-модель.
        /// </summary>
        public Model3D Model { get; set; }

        /// <summary>
        /// Модель базовго плана.
        /// </summary>
        public RectangleVisual3D BasePlane
        {
            get => _basePlane;
            set
            {
                _basePlane = value;
                OnPropertyChanged();
            }
        }
    }
}

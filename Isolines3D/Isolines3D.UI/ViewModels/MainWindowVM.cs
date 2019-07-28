namespace Isolines3D.UI.ViewModels
{
    using HelixToolkit.Wpf;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using System;
    using Isolines3D.UI.Commands;
    using System.Drawing;
    using Isolines3D.UI.Properties;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using Isolines3D.UI.Extensions;
    using Isolines3D.UI.Utils;

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
        /// Команда задаёт случайный материал поверхности на сцене.
        /// </summary>
        public RandomMaterialCommand RandomMaterialCommand { get; set; }

        /// <summary>
        /// Команда выбора карты для генерации поверхности.
        /// </summary>
        public ChooseMapCommand ChooseMapCommand { get; set; }

        /// <summary>
        /// Поле для обращения к нструменту создания 3D модели.
        /// </summary>
        public readonly ModelCreatorUtil ModelCreatorUtil;

        /// <summary>
        /// Команда формирования модели по изолиниям.
        /// </summary>
        public CreateByIsolinesCommand CreateByIsolinesCommand { get; set; }

        /// <summary>
        /// Вью-модель главного окна.
        /// </summary>
        /// <param name="helixViewport3D">Адаптация среды 3D инструменртами HelixToolkit.</param>
        public MainWindowVM(HelixViewport3D helixViewport3D)
        {
            _helixViewport3D = helixViewport3D;
            InitializeViewPort3D();

            var bitmapSource = new Bitmap(Resources.VerteciesMap);
            _imageSource = bitmapSource.ConvertToImageSource();

            ModelCreatorUtil = new ModelCreatorUtil(_basePlaneWidth, _basePlaneLength);
            Model = ModelCreatorUtil.CreateModelByImage(bitmapSource);

            ShowLightsDirectionCommand = new ShowLightsDirectionCommand();
            RandomMaterialCommand = new RandomMaterialCommand();

            ChooseMapCommand = new ChooseMapCommand();
            CreateByIsolinesCommand = new CreateByIsolinesCommand();

            helixViewport3D.Loaded += ViewportInitializeHandle;
        }

        /// <summary>
        /// Изображение карты.
        /// </summary>
        private ImageSource _imageSource;

        /// <summary>
        /// Изображение карты.
        /// </summary>
        public ImageSource ImageSource
        {
            get => _imageSource;
            set
            {
                _imageSource = value;
                OnPropertyChanged();
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
            var animationTime = 3500d;
            var newDirection = new Vector3D();

            newDirection = new Vector3D(1, 0, -0.5);
            _helixViewport3D.Camera.ChangeDirection(newDirection, newUpDirection, animationTime);

            newDirection = new Vector3D(0, 1, -0.5); 
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
        private static int _basePlaneLength = 199;

        /// <summary>
        /// Ширина базового плана.
        /// </summary>
        /// <remarks>Размеры имеют тип <see cref="int"/>
        /// для удобства составления сетки с геометрией.</remarks>
        private static int _basePlaneWidth = 199;

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
        private Model3D _model;

        /// <summary>
        /// 3D-модель.
        /// </summary>
        public Model3D Model
        {
            get => _model;
            set
            {
                _model = value;
                OnPropertyChanged();
            }
        }

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

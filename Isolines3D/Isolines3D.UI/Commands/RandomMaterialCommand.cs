namespace Isolines3D.UI.Commands
{
    using HelixToolkit.Wpf;
    using Isolines3D.UI.ViewModels;
    using System;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Задаёт случайный материал поверхности на сцене.
    /// </summary>
    public class RandomMaterialCommand : BaseTCommand<MainWindowVM>
    {
        /// <summary>
        /// Выполнение команды.
        /// </summary>
        /// <param name="mainWindowVM">Вью-модель главного окна.</param>
        protected override void Execute(MainWindowVM mainWindowVM)
        {
            var random = new Random();
            var mainModel = mainWindowVM.Model;

            var randomMaterial = MaterialHelper.CreateMaterial(
                Color.FromRgb(
                    (byte)random.Next(byte.MinValue, byte.MaxValue),
                    (byte)random.Next(byte.MinValue, byte.MaxValue),
                    (byte)random.Next(byte.MinValue, byte.MaxValue)));

            var planeModel = mainWindowVM.BasePlane;
            planeModel.Material = randomMaterial;
            planeModel.BackMaterial = randomMaterial;

            //foreach (var child in (planeModel as Model3DGroup).Children)
            //{
            //    if (child is GeometryModel3D model)
            //    {
            //        model.Material = randomMaterial;
            //        model.BackMaterial = randomMaterial;
            //    }
            //}

            foreach (var child in (mainModel as Model3DGroup).Children)
            {
                if (child is GeometryModel3D model)
                {
                    model.Material = randomMaterial;
                    model.BackMaterial = randomMaterial;
                }
            }
        }
    }
}

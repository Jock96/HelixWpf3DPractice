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

            foreach (var child in (mainModel as Model3DGroup).Children)
            {
                if (child is GeometryModel3D model)
                {
                    var randomMaterial = MaterialHelper.CreateMaterial(
                        Color.FromRgb(
                        (byte)random.Next(byte.MinValue, byte.MaxValue),
                        (byte)random.Next(byte.MinValue, byte.MaxValue),
                        (byte)random.Next(byte.MinValue, byte.MaxValue)));

                    model.Material = randomMaterial;

                    //randomMaterial = MaterialHelper.CreateMaterial(
                    //    Color.FromRgb(
                    //    (byte)random.Next(byte.MinValue, byte.MaxValue),
                    //    (byte)random.Next(byte.MinValue, byte.MaxValue),
                    //    (byte)random.Next(byte.MinValue, byte.MaxValue)));

                    model.BackMaterial = randomMaterial;
                }
            }
        }
    }
}

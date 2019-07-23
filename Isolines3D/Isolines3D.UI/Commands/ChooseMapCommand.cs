namespace Isolines3D.UI.Commands
{
    using Isolines3D.UI.Properties;
    using Isolines3D.UI.ViewModels;
    using Microsoft.Win32;
    using System.Drawing;
    using System.IO;

    /// <summary>
    /// Команда выбора карты для генерации поверхности.
    /// </summary>
    public class ChooseMapCommand : BaseTCommand<MainWindowVM>
    {
        /// <summary>
        /// Выполнение команды.
        /// </summary>
        /// <param name="mainWindowVM">Вью-модель главного окна.</param>
        protected override void Execute(MainWindowVM mainWindowVM)
        {
            var fileDialog = new OpenFileDialog();

            fileDialog.Filter = @"(*.bmp)|*.bmp";
            fileDialog.InitialDirectory = Directory.GetCurrentDirectory();

            var file = fileDialog.ShowDialog();

            if (!file.HasValue)
                return;

            using (var fileStream = fileDialog.OpenFile())
            using (var bitmap = new Bitmap(fileStream))
            {
                mainWindowVM.SetImageSource(bitmap);

                mainWindowVM.CreateGridByPlaneByImage(bitmap);
            }
        }
    }
}

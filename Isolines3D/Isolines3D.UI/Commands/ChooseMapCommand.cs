namespace Isolines3D.UI.Commands
{
    using Isolines3D.UI.Extensions;
    using Isolines3D.UI.Helpers;
    using Isolines3D.UI.Properties;
    using Isolines3D.UI.Utils;
    using Isolines3D.UI.ViewModels;
    using Microsoft.Win32;
    using System;
    using System.Drawing;
    using System.IO;
    using System.Windows;
    using System.Windows.Media.Media3D;

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
            var fileDialog = new OpenFileDialog
            {
                Filter = @"(*.bmp)|*.bmp",
                InitialDirectory = PathHelper.GetResourcesPath()
            };

            var file = fileDialog.ShowDialog();

            if (!file.HasValue || string.Equals(string.Empty, fileDialog.FileName))
                return;

            try
            {
                using (var fileStream = fileDialog.OpenFile())
                using (var bitmap = new Bitmap(fileStream))
                {
                    mainWindowVM.ImageSource = bitmap.ConvertToImageSource();
                    mainWindowVM.Model = mainWindowVM.ModelCreatorUtil.CreateModelByImage(bitmap);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Не удалось импортировать файл!" +
                    $"\nОшибка: {exception.ToString()}");
            }
        }
    }
}

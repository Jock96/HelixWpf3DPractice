namespace Isolines3D.UI.Commands
{
    using HelixToolkit.Wpf;
    using Isolines3D.UI.ViewModels;

    /// <summary>
    /// Скрыть/показать направление света на сцене.
    /// </summary>
    public class ShowLightsDirectionCommand : BaseTCommand<MainWindowVM>
    {
        /// <summary>
        /// Выполнение команды.
        /// </summary>
        /// <param name="mainWindowVM">Вью-модель главного окна.</param>
        protected override void Execute(MainWindowVM mainWindowVM)
        {
            foreach (var child in mainWindowVM.HelixViewport3D.Children)
            {
                if (child is LightSetup light)
                {
                    if (light.ShowLights == false)
                    {
                        light.ShowLights = true;
                        continue;
                    }

                    if (light.ShowLights == true)
                    {
                        light.ShowLights = false;
                        continue;
                    }
                }
            }
        }
    }
}

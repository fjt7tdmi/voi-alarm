using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using VoiAlarm.Models;
using VoiAlarm.ViewModels;

namespace VoiAlarm
{
    /// <summary>
    /// CevioUnavailableWaningDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class ExternalToolsDialog : Window
    {
        private readonly Core core;

        internal ExternalToolsDialog(Core core)
        {
            this.core = core;

            InitializeComponent();

            externalToolsView.DataContext = new ExternalToolsViewModel(core);
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            var doNotShow = checkBoxDoNotShow.IsChecked.GetValueOrDefault(false);

            core.SettingsManager.Value.ShowExternalToolsCheckDialog = !doNotShow;
            core.SettingsManager.Save();

            Close();
        }
    }
}

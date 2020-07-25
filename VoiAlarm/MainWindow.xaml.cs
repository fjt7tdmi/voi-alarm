using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Timers;

using Microsoft.Win32;

using VoiAlarm.Models;
using VoiAlarm.ViewModels;
using VoiAlarm.Views;
using System.Threading.Tasks;

namespace VoiAlarm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Timer timer = new Timer(1000);

        private readonly Core core;

        // ViewModel
        private readonly MainWindowViewModel mainWindowViewModel;

        public MainWindow()
        {
            Trace.WriteLine("Application started.");

            InitializeComponent();

            core = new Core();

            // Show ExternalsToolsDialog
            if (core.SettingsManager.Value.ShowExternalToolsCheckDialog)
            {
                var dialog = new ExternalToolsDialog(core);
                dialog.ShowDialog();
            }

            // Initialize ViewModel
            mainWindowViewModel = new MainWindowViewModel();
            DataContext = mainWindowViewModel;

            ShowAlarmListView();

            // Start timer
            core.StartTimer();

            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    menuTime.Text = DateTime.Now.ToString("HH:mm:ss");
                });
            }
            catch (TaskCanceledException)
            {
                // アプリケーション終了時に例外が起きることがあるが無視
            }
        }

        private void OnListViewItemSelected(object sender, MouseButtonEventArgs e)
        {
            var index = listView.SelectedIndex;

            if (index == 0)
            {
                ShowAlarmListView();
                menuButton.IsChecked = false;
            }
            if (index == 1)
            {
                ShowSettingsView();
                menuButton.IsChecked = false;
            }
            if (index == 2)
            {
                ShowExternalToolsView();
                menuButton.IsChecked = false;
            }
        }

        private void ShowAlarmListView()
        {
            var view = new AlarmListView();
            view.DataContext = new AlarmListViewModel(core);

            mainWindowViewModel.Content = view;
        }

        private void ShowSettingsView()
        {
            var settingsView = new SettingsView();
            settingsView.DataContext = new SettingsViewModel(core);

            mainWindowViewModel.Content = settingsView;
        }

        private void ShowExternalToolsView()
        {
            var view = new ExternalToolsView();
            view.DataContext = new ExternalToolsViewModel(core);

            mainWindowViewModel.Content = view;
        }
    }
}

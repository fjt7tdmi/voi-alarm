/*
 * Copyright (c) Akifumi Fujita
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System.ComponentModel;
using System.Windows.Input;

using VoiAlarm.Commands;
using VoiAlarm.Models;

namespace VoiAlarm.ViewModels
{
    internal class SettingsViewModel : INotifyPropertyChanged
    {
        private readonly Core core;

        private bool autoStartBouyomiChan;
        private bool autoStartCevio;
        private string bouyomiChanPath;
        private int bouyomiChanPort;
        private bool showExternalToolsCheckDialog;

        public bool AutoStartBouyomiChan
        {
            get => autoStartBouyomiChan;
            set
            {
                autoStartBouyomiChan = value;
                OnPropertyChanged(nameof(AutoStartBouyomiChan));
            }
        }

        public bool AutoStartCevio
        {
            get => autoStartCevio;
            set
            {
                autoStartCevio = value;
                OnPropertyChanged(nameof(AutoStartCevio));
            }
        }

        public string BouyomiChanPath
        {
            get => bouyomiChanPath;
            set
            {
                bouyomiChanPath = value;
                OnPropertyChanged(nameof(BouyomiChanPath));
            }
        }

        public bool ShowExternalToolsCheckDialog
        {
            get => showExternalToolsCheckDialog;
            set
            {
                showExternalToolsCheckDialog = value;
                OnPropertyChanged(nameof(ShowExternalToolsCheckDialog));
            }
        }

        public int BouyomiChanPort
        {
            get => bouyomiChanPort;
            set
            {
                bouyomiChanPort = value;
                OnPropertyChanged(nameof(BouyomiChanPort));
            }
        }

        public ICommand ChooseFileCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsViewModel(Core core)
        {
            this.core = core;

            var settings = core.SettingsManager.Value;

            AutoStartBouyomiChan = settings.AutoStartBouyomiChan;
            AutoStartCevio = settings.AutoStartCevio;
            BouyomiChanPath = settings.BouyomiChanPath;
            BouyomiChanPort = settings.BouyomiChanPort;
            ShowExternalToolsCheckDialog = settings.ShowExternalToolsCheckDialog;

            ChooseFileCommand = new RelayCommand(() =>
            {
                var dialog = new System.Windows.Forms.OpenFileDialog
                {
                    Multiselect = false,
                };
                dialog.ShowDialog();

                BouyomiChanPath = dialog.FileName;
            });

            // TODO: unsubscribe the event with proper timing
            PropertyChanged += UpdateSettings;
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UpdateSettings(object sender, PropertyChangedEventArgs e)
        {
            var settings = core.SettingsManager.Value;

            settings.AutoStartBouyomiChan = autoStartBouyomiChan;
            settings.AutoStartCevio = autoStartCevio;
            settings.BouyomiChanPath = bouyomiChanPath;
            settings.BouyomiChanPort = bouyomiChanPort;
            settings.ShowExternalToolsCheckDialog = showExternalToolsCheckDialog;

            core.SettingsManager.Save();
        }
    }
}

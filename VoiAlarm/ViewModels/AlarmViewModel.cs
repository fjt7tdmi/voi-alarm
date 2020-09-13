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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

using VoiAlarm.Commands;
using VoiAlarm.Models;
using VoiAlarm.Views;

namespace VoiAlarm.ViewModels
{
    internal class AlarmViewModel : INotifyPropertyChanged
    {
        private readonly Core core;
        private readonly Alarm alarm;
        private readonly AlarmHeaderViewModel alarmHeaderViewModel;

        private readonly IList<(string, string)> playerVoices;

        public ObservableCollection<string> FilePlayModes { get; } = new ObservableCollection<string>();

        public ObservableCollection<string> Voices { get; } = new ObservableCollection<string>();

        public AlarmHeaderView Header { get; } = new AlarmHeaderView();

        public DateTime Time
        {
            get => DateTime.Parse(alarm.Time);
            set
            {
                alarm.Time = value.ToString("HH:mm");
                OnPropertyChanged(nameof(Time));
                alarmHeaderViewModel.OnPropertyChanged();
            }
        }

        public bool IsEnabledMonday
        {
            get => alarm.IsEnabledMonday;
            set
            {
                alarm.IsEnabledMonday = value;
                OnPropertyChanged(nameof(IsEnabledMonday));
                alarmHeaderViewModel.OnPropertyChanged();
            }
        }

        public bool IsEnabledTuesday
        {
            get => alarm.IsEnabledTuesday;
            set
            {
                alarm.IsEnabledTuesday = value;
                OnPropertyChanged(nameof(IsEnabledTuesday));
                alarmHeaderViewModel.OnPropertyChanged();
            }
        }

        public bool IsEnabledWednesday
        {
            get => alarm.IsEnabledWednesday;
            set
            {
                alarm.IsEnabledWednesday = value;
                OnPropertyChanged(nameof(IsEnabledWednesday));
                alarmHeaderViewModel.OnPropertyChanged();
            }
        }

        public bool IsEnabledThursday
        {
            get => alarm.IsEnabledThursday;
            set
            {
                alarm.IsEnabledThursday = value;
                OnPropertyChanged(nameof(IsEnabledThursday));
                alarmHeaderViewModel.OnPropertyChanged();
            }
        }

        public bool IsEnabledFriday
        {
            get => alarm.IsEnabledFriday;
            set
            {
                alarm.IsEnabledFriday = value;
                OnPropertyChanged(nameof(IsEnabledFriday));
                alarmHeaderViewModel.OnPropertyChanged();
            }
        }

        public bool IsEnabledSaturday
        {
            get => alarm.IsEnabledSaturday;
            set
            {
                alarm.IsEnabledSaturday = value;
                OnPropertyChanged(nameof(IsEnabledSaturday));
                alarmHeaderViewModel.OnPropertyChanged();
            }
        }

        public bool IsEnabledSunday
        {
            get => alarm.IsEnabledSunday;
            set
            {
                alarm.IsEnabledSunday = value;
                OnPropertyChanged(nameof(IsEnabledSunday));
                alarmHeaderViewModel.OnPropertyChanged();
            }
        }

        public string Voice
        {
            get => Utils.GetDisplayNameForVoice(alarm.PlayerType, alarm.VoiceType);
            set
            {
                var playerVoice = GetPlayerVoiceFromDisplayName(value);
                alarm.PlayerType = playerVoice.Item1;
                alarm.VoiceType = playerVoice.Item2;
                OnPropertyChanged(nameof(Voice));
            }
        }

        public string Message
        {
            get => alarm.Message;
            set
            {
                alarm.Message = value;
                OnPropertyChanged(nameof(Message));
                alarmHeaderViewModel.OnPropertyChanged();
            }
        }

        public string FilePlayMode
        {
            get => Utils.GetDisplayName(alarm.FilePlayMode);
            set
            {
                alarm.FilePlayMode = Utils.GetFilePlayMode(value);
                OnPropertyChanged(nameof(FilePlayMode));
                OnPropertyChanged(nameof(IsFileSelectEnabled));
                alarmHeaderViewModel.OnPropertyChanged();
            }
        }

        public string FilePath
        {
            get => alarm.FilePath;
            set
            {
                alarm.FilePath = value;
                OnPropertyChanged(nameof(FilePath));
            }
        }

        public bool IsFileSelectEnabled
        {
            get => alarm.FilePlayMode != Models.FilePlayMode.None;
        }

        public ICommand ChooseFileCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public AlarmViewModel(AlarmListViewModel alarmListViewModel, Core core, Alarm alarm)
        {
            this.core = core;
            this.alarm = alarm;

            alarmHeaderViewModel = new AlarmHeaderViewModel(core, alarmListViewModel, this, alarm);
            Header.DataContext = alarmHeaderViewModel;

            // Initialize playerVoices
            playerVoices = core.PlayerManager.PlayerVoices;

            var alarmPlayerVoice = (alarm.PlayerType, alarm.VoiceType);
            if (!playerVoices.Contains(alarmPlayerVoice))
            {
                playerVoices.Add(alarmPlayerVoice);
            }

            // Initialize FilePlayModes
            FilePlayModes.Add(Utils.GetDisplayName(Models.FilePlayMode.None));
            FilePlayModes.Add(Utils.GetDisplayName(Models.FilePlayMode.BeforeVoice));
            
            // Initialize Voices
            foreach (var playerVoice in playerVoices)
            {
                Voices.Add(Utils.GetDisplayNameForVoice(playerVoice.Item1, playerVoice.Item2));
            }

            ChooseFileCommand = new RelayCommand(() =>
            {
                var dialog = new System.Windows.Forms.OpenFileDialog
                {
                    Multiselect = false,
                };
                dialog.ShowDialog();

                FilePath = dialog.FileName;
            });
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            core.SettingsManager.Save();
        }

        private (string, string) GetPlayerVoiceFromDisplayName(string displayName)
        {
            if (displayName != null)
            {
                foreach (var playerVoice in playerVoices)
                {
                    if (displayName.Equals(Utils.GetDisplayNameForVoice(playerVoice.Item1, playerVoice.Item2)))
                    {
                        return playerVoice;
                    }
                }
            }

            return (string.Empty, string.Empty);
        }
    }
}

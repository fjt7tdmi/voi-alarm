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

using VoiAlarm.Models;
using VoiAlarm.Commands;

namespace VoiAlarm.ViewModels
{
    internal class AlarmListViewModel : INotifyPropertyChanged
    {
        public Core Core { get; }

        public ObservableCollection<AlarmViewModel> Alarms { get; }

        public ICommand AddCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public AlarmListViewModel(Core core)
        {
            Core = core;

            Alarms = new ObservableCollection<AlarmViewModel>();

            AddCommand = new RelayCommand(() =>
            {
                AddElement();
            });

            // Load initial data
            var alarms = Core.SettingsManager.Value.Alarms;

            foreach (var alarm in alarms)
            {
                Alarms.Add(new AlarmViewModel(this, Core, alarm));
            }

            OnPropertyChanged(nameof(Alarms));
        }

        public void AddElement()
        {
            var alarms = Core.SettingsManager.Value.Alarms;
            var alarm = new Alarm()
            {
                Time = "08:45",
                IsEnabledMonday = true,
                IsEnabledTuesday = true,
                IsEnabledWednesday = true,
                IsEnabledThursday = true,
                IsEnabledFriday = true,
                Message = "こんにちは",
                PlayerType = Core.PlayerManager.PlayerVoices.First().Item1,
                VoiceType = Core.PlayerManager.PlayerVoices.First().Item2,
            };

            alarms.Add(alarm);
            Alarms.Add(new AlarmViewModel(this, Core, alarm));

            OnPropertyChanged(nameof(Alarms));
        }

        public void CopyElement(AlarmViewModel alarmViewModel)
        {
            var index = GetIndex(alarmViewModel);

            if (index >= 0)
            {
                var alarms = Core.SettingsManager.Value.Alarms;
                var alarm = alarms[index].Clone();
                
                alarms.Insert(index, alarm);
                Alarms.Insert(index, new AlarmViewModel(this, Core, alarm));

                OnPropertyChanged(nameof(Alarms));
            }
        }

        public void RemoveElement(AlarmViewModel alarmViewModel)
        {
            var index = GetIndex(alarmViewModel);

            if (index >= 0)
            {
                var alarms = Core.SettingsManager.Value.Alarms;

                alarms.RemoveAt(index);
                Alarms.RemoveAt(index);

                OnPropertyChanged(nameof(Alarms));
            }
        }

        private int GetIndex(AlarmViewModel alarmViewModel)
        {
            for (int i = 0; i < Alarms.Count; i++)
            {
                if (Alarms[i].Equals(alarmViewModel))
                {
                    return i;
                }
            }

            return -1;
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

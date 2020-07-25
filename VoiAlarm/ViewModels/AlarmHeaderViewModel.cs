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

namespace VoiAlarm.ViewModels
{
    internal class AlarmHeaderViewModel : INotifyPropertyChanged
    {
        private readonly Alarm alarm;

        public string Time
        {
            get => alarm.Time;
        }

        public string DaysOfWeek
        {
            get => GetEnabledDaysOfWeek();
        }

        public string Message
        {
            get => alarm.Message;
        }

        public ICommand PlayCommand { get; }

        public ICommand CopyCommand { get; }

        public ICommand RemoveCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public AlarmHeaderViewModel(Core core, AlarmListViewModel alarmListViewModel, AlarmViewModel alarmViewModel, Alarm alarm)
        {
            this.alarm = alarm;

            PlayCommand = new RelayCommand(() =>
            {
                core.PlayerManager.Play(alarm);
            });

            CopyCommand = new RelayCommand(() =>
            {
                alarmListViewModel.CopyElement(alarmViewModel);
            });

            RemoveCommand = new RelayCommand(() =>
            {
                alarmListViewModel.RemoveElement(alarmViewModel);
            });
        }

        public void OnPropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Time)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DaysOfWeek)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Message)));
        }

        private string GetEnabledDaysOfWeek()
        {
            var list = new List<string>();

            if (alarm.IsEnabledMonday)
            {
                list.Add("月");
            }
            if (alarm.IsEnabledTuesday)
            {
                list.Add("火");
            }
            if (alarm.IsEnabledWednesday)
            {
                list.Add("水");
            }
            if (alarm.IsEnabledThursday)
            {
                list.Add("木");
            }
            if (alarm.IsEnabledFriday)
            {
                list.Add("金");
            }
            if (alarm.IsEnabledSaturday)
            {
                list.Add("土");
            }
            if (alarm.IsEnabledSunday)
            {
                list.Add("日");
            }

            if (list.Count > 0)
            {
                return string.Join("、", list);
            }
            else
            {
                return "無効";
            }
        }
    }
}

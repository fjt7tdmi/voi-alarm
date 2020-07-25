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
using System.Diagnostics;
using System.Timers;

namespace VoiAlarm.Models
{
    internal class Core
    {
        private readonly Timer timer = new Timer(1000);

        public SettingsManager SettingsManager { get; }

        public PlayerManager PlayerManager { get; }

        public Core()
        {
            SettingsManager = new SettingsManager();
            PlayerManager = new PlayerManager(SettingsManager);
        }

        public void StartTimer()
        {
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            var now = DateTime.Now;
            if (now.Second != 0)
            {
                return;
            }
            
            // Process every minute and zero seconds.
            lock (SettingsManager.LockGuard)
            {
                foreach (var alarm in SettingsManager.Value.Alarms)
                {
                    if (IsEnabled(alarm, now) && IsTimeMatched(alarm, now))
                    {
                        Trace.WriteLine("Trigger alarm");

                        PlayerManager.Play(alarm);
                    }
                }
            }
        }

        private bool IsEnabled(Alarm alarm, DateTime now)
        {
            switch (now.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return alarm.IsEnabledMonday;
                case DayOfWeek.Tuesday:
                    return alarm.IsEnabledTuesday;
                case DayOfWeek.Wednesday:
                    return alarm.IsEnabledWednesday;
                case DayOfWeek.Thursday:
                    return alarm.IsEnabledThursday;
                case DayOfWeek.Friday:
                    return alarm.IsEnabledFriday;
                case DayOfWeek.Saturday:
                    return alarm.IsEnabledSaturday;
                case DayOfWeek.Sunday:
                    return alarm.IsEnabledSunday;
                default:
                    throw new NotImplementedException("Unreachable");
            };
        }

        private bool IsTimeMatched(Alarm alarm, DateTime now)
        {
            var alarmTime = DateTime.Parse(alarm.Time);
            return alarmTime.Hour == now.Hour && alarmTime.Minute == now.Minute;
        }
    }
}

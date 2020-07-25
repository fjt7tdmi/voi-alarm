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
using System.IO;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

namespace VoiAlarm.Models
{
    internal class SettingsManager
    {
        private readonly string SettingsPath = "Settings.xml";
        private readonly string DefaultSettingsPath = "Settings.default.xml";

        public object LockGuard { get; } = new object();

        public Settings Value { get; private set; }

        public SettingsManager()
        {
            bool loaded = false;

            if (!loaded && File.Exists(SettingsPath))
            {
                // use settings file in the current directory
                try
                {
                    Load(SettingsPath);
                    loaded = true;
                }
                catch
                {
                    // デフォルトを使うため、ここでは例外を握りつぶす
                }
            }

            if (!loaded && File.Exists(DefaultSettingsPath))
            {
                // Copy default settings
                try
                {
                    Load(DefaultSettingsPath);
                    Save();
                    loaded = true;
                }
                catch
                {
                    // ダイアログを表示するため、ここでは例外を握りつぶす
                }
            }

            if (!loaded)
            {
                MessageBox.Show($"設定ファイル ({SettingsPath}) が見つかりません。設定ファイルを修復するか、{nameof(VoiAlarm)} を再インストールしてください。");
                Environment.Exit(1);
            }
        }

        private void Load(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open))
            {
                var serializer = new XmlSerializer(typeof(Settings));
                Value = (Settings)serializer.Deserialize(stream);
            }
        }

        public void Save()
        {
            var xmlWriterSettings = new XmlWriterSettings
            {
                Indent = true,
                NewLineOnAttributes = true,
            };

            using (var xmlWriter = XmlWriter.Create(SettingsPath, xmlWriterSettings))
            {
                var serializer = new XmlSerializer(typeof(Settings));
                serializer.Serialize(xmlWriter, Value);

                xmlWriter.Flush();
            }
        }
    }
}

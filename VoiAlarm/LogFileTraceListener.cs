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
using System.IO;

namespace VoiAlarm
{
    public class LogFileTraceListener : TraceListener
    {
        private const string directoryName = "Logs";
        private const int maxLogFileCount = 5;

        public override void Write(string message)
        {
            CreateLogDirectoryIfNotExists();

            using (var fileStream = new FileStream(GetLogFilePath(), FileMode.Append))
            using (var writer = new StreamWriter(fileStream))
            {
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                
                writer.Write($"[{timestamp}] {message}");
            }

            RemoveOldFiles();
        }

        public override void WriteLine(string message)
        {
            CreateLogDirectoryIfNotExists();

            using (var fileStream = new FileStream(GetLogFilePath(), FileMode.Append))
            using (var writer = new StreamWriter(fileStream))
            {
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

                writer.WriteLine($"[{timestamp}] {message}");
            }

            RemoveOldFiles();
        }

        private void CreateLogDirectoryIfNotExists()
        {
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
        }

        private string GetLogFilePath()
        {
            return $"{directoryName}/{DateTime.Now:yyyyMMdd}.log";
        }

        private void RemoveOldFiles()
        {
            var files = Directory.GetFiles("Logs", "*.log");

            // Sort by file name (date)
            Array.Sort(files);

            var removeCount = files.Length - maxLogFileCount;

            for (int i = 0; i < removeCount; i++)
            {
                File.Delete(files[i]);
            }
        }
    }
}

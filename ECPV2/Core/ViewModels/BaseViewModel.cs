using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace ECPV2.Core.ViewModels
{
    internal abstract class BaseViewModel : INotifyPropertyChanged, IDisposable
    {
        private const string LOG_PATH = @"C:\Logs\log.txt";
        private bool _isDisposed;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void EnsureLogDirectory()
        {
            try
            {
                var dir = Path.GetDirectoryName(LOG_PATH);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir!);
            }
            catch { }
        }

        protected void LogException(string context, Exception ex)
        {
            try
            {
                string entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {context}\n" +
                               $"Message: {ex.Message}\n" +
                               $"StackTrace: {ex.StackTrace}\n" +
                               new string('-', 50) + "\n";
                File.AppendAllText(LOG_PATH, entry);
            }
            catch { }
        }

        protected void LogInfo(string message)
        {
            try
            {
                string entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] INFO: {message}\n";
                File.AppendAllText(LOG_PATH, entry);
            }
            catch { }
        }

        public virtual void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
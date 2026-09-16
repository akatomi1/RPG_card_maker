using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RPGCardMaker.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        private readonly Dictionary<string, List<string>> _errors = new();

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool HasErrors => _errors.Count > 0;

        // Повертає всі повідомлення про помилки одразу (зручно показати в одному MessageBox)
        public IEnumerable<string> GetAllErrorMessages()
        {
            var messages = new List<string>();
            foreach (var kvp in _errors)
                messages.AddRange(kvp.Value);
            return messages;
        }

        // Викликається WPF-біндингом, коли треба підсвітити конкретне поле
        public IEnumerable GetErrors(string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || !_errors.ContainsKey(propertyName))
                return Array.Empty<string>();

            return _errors[propertyName];
        }

        // Додає помилку валідації для властивості (якщо така ще не додана) і повідомляє UI
        protected void AddError(string propertyName, string errorMessage)
        {
            if (!_errors.ContainsKey(propertyName))
                _errors[propertyName] = new List<string>();

            if (!_errors[propertyName].Contains(errorMessage))
            {
                _errors[propertyName].Add(errorMessage);
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
                OnPropertyChanged(nameof(HasErrors));
            }
        }

        // Знімає всі помилки для властивості
        protected void ClearErrors(string propertyName)
        {
            if (_errors.Remove(propertyName))
            {
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
                OnPropertyChanged(nameof(HasErrors));
            }
        }
    }
}
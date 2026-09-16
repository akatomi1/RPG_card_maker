using Microsoft.Win32;
using RPGCardMaker.Commands;
using RPGCardMaker.Models;
using RPGCardMaker.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;

namespace RPGCardMaker.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private Card _currentCard;
        public Card CurrentCard
        {
            get => _currentCard;
            set { _currentCard = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Card> _savedCards;
        public ObservableCollection<Card> SavedCards
        {
            get => _savedCards;
            set { _savedCards = value; OnPropertyChanged(); }
        }

        private Card _selectedSavedCard;
        public Card SelectedSavedCard
        {
            get => _selectedSavedCard;
            set
            {
                _selectedSavedCard = value;
                OnPropertyChanged();
                if (_selectedSavedCard != null)
                {
                    CurrentCard = _selectedSavedCard;
                }
            }
        }

        private ObservableCollection<string> _cardTypes;
        public ObservableCollection<string> CardTypes
        {
            get => _cardTypes;
            set { _cardTypes = value; OnPropertyChanged(); }
        }

        // Колекція для рідкості
        private ObservableCollection<string> _rarities;
        public ObservableCollection<string> Rarities
        {
            get => _rarities;
            set { _rarities = value; OnPropertyChanged(); }
        }

        private string _newTypeInput;
        public string NewTypeInput
        {
            get => _newTypeInput;
            set { _newTypeInput = value; OnPropertyChanged(); }
        }

        private Dictionary<string, string> _uiStrings;
        public Dictionary<string, string> UIStrings
        {
            get => _uiStrings;
            set { _uiStrings = value; OnPropertyChanged(); }
        }

        private Dictionary<string, Dictionary<string, string>> _localizedUiDictionary;
        private Dictionary<string, ObservableCollection<string>> _localizedTypes;
        private Dictionary<string, ObservableCollection<string>> _localizedRarities; // Словник рідкостей
        private string _currentLanguage = "uk";

        public ICommand SaveCommand { get; }
        public ICommand SelectImageCommand { get; }
        public ICommand AddTypeCommand { get; }
        public ICommand EditTypeCommand { get; }
        public ICommand DeleteTypeCommand { get; }
        public ICommand OpenSettingsCommand { get; }
        public ICommand NewCardCommand { get; }
        public ICommand DeleteSavedCardCommand { get; }

        private const string TypesFilePath = "cardTypes.json";
        private const string CardsFilePath = "savedCards.json";
        private const string DefaultBackgroundColor = "#FAFAFA";

        public MainViewModel()
        {
            SavedCards = new ObservableCollection<Card>();
            CurrentCard = new Card();
            _localizedTypes = new Dictionary<string, ObservableCollection<string>>();
            _localizedUiDictionary = new Dictionary<string, Dictionary<string, string>>();
            _localizedRarities = new Dictionary<string, ObservableCollection<string>>();
            _uiStrings = new Dictionary<string, string>();
            CardTypes = new ObservableCollection<string>();
            Rarities = new ObservableCollection<string>();

            SetupTranslations();
            LoadTypes();
            LoadSavedCards();

            CurrentCard.Name = _currentLanguage == "en" ? "New Card" : "Нова картка";
            CurrentCard.Description = _currentLanguage == "en" ? "Enter description here..." : "Введіть опис тут...";
            CurrentCard.Rarity = Rarities.FirstOrDefault() ?? "";
            CurrentCard.BackgroundColor = DefaultBackgroundColor;

            SaveCommand = new RelayCommand(ExecuteSave);
            SelectImageCommand = new RelayCommand(ExecuteSelectImage);
            AddTypeCommand = new RelayCommand(ExecuteAddType);
            EditTypeCommand = new RelayCommand(ExecuteEditType);
            DeleteTypeCommand = new RelayCommand(ExecuteDeleteType);
            OpenSettingsCommand = new RelayCommand(ExecuteOpenSettings);
            NewCardCommand = new RelayCommand(ExecuteNewCard);
            DeleteSavedCardCommand = new RelayCommand(ExecuteDeleteSavedCard, CanDeleteSavedCard);

            SettingsWindow.LanguageChanged += OnLanguageChanged;
        }

        private void SetupTranslations()
        {
            _localizedUiDictionary["uk"] = new Dictionary<string, string>
            {
                { "History", "Мої проєкти (Історія):" },
                { "NewCardBtn", " 📝 Нова " },
                { "CardName", "Назва картки:" },
                { "CardType", "Тип картки:" },
                { "TypePlaceholder", "Додайте свій перший тип картки" },
                { "Rarity", "Рідкість:" },
                { "Health", "Здоров'я (HP):" },
                { "Attack", "Атака:" },
                { "Defense", "Захист:" },
                { "ManaCost", "Вартість мани:" },
                { "Description", "Опис:" },
                { "Ability", "Здібність / Ефект:" },
                { "BackgroundColor", "Колір картки (фракція):" },
                { "BgNeutral", "Нейтральний" },
                { "BgFire", "Вогонь" },
                { "BgWater", "Вода" },
                { "BgNature", "Природа" },
                { "BgArcane", "Магія" },
                { "SelectImage", "Вибрати зображення..." },
                { "SaveProject", "💾 Зберегти проєкт картки" },
                { "Settings", "⚙ Налаштування" },
                { "SavePng", "💾 Зберегти зображення (PNG)" },
                { "DefaultName", "Нова картка" },
                { "DefaultDesc", "Введіть опис тут..." },
                { "ImagePlaceholder", "[Зображення]" },
                { "DeleteCardTooltip", "Видалити обрану картку з історії" }
            };

            _localizedUiDictionary["en"] = new Dictionary<string, string>
            {
                { "History", "My Projects (History):" },
                { "NewCardBtn", " 📝 New " },
                { "CardName", "Card Name:" },
                { "CardType", "Card Type:" },
                { "TypePlaceholder", "Add your first card type" },
                { "Rarity", "Rarity:" },
                { "Health", "Health (HP):" },
                { "Attack", "Attack:" },
                { "Defense", "Defense:" },
                { "ManaCost", "Mana Cost:" },
                { "Description", "Description:" },
                { "Ability", "Ability / Effect:" },
                { "BackgroundColor", "Card Color (faction):" },
                { "BgNeutral", "Neutral" },
                { "BgFire", "Fire" },
                { "BgWater", "Water" },
                { "BgNature", "Nature" },
                { "BgArcane", "Arcane" },
                { "SelectImage", "Select Image..." },
                { "SaveProject", "💾 Save Card Project" },
                { "Settings", "⚙ Settings" },
                { "SavePng", "💾 Save Image (PNG)" },
                { "DefaultName", "New Card" },
                { "DefaultDesc", "Enter description here..." },
                { "ImagePlaceholder", "[Image]" },
                { "DeleteCardTooltip", "Delete the selected card from history" }
            };

            _localizedRarities["uk"] = new ObservableCollection<string> { "Звичайна", "Рідкісна", "Епічна", "Легендарна" };
            _localizedRarities["en"] = new ObservableCollection<string> { "Common", "Rare", "Epic", "Legendary" };

            UIStrings = _localizedUiDictionary[_currentLanguage];
            Rarities = _localizedRarities[_currentLanguage];
        }

        private void OnLanguageChanged(string langCode)
        {
            // Запам'ятовуємо позицію поточної рідкості/типу у старомовному списку,
            // щоб після зміни мови підставити відповідник на тій самій позиції,
            // а не втратити значення, якщо воно не було дефолтним.
            int rarityIndex = Rarities?.IndexOf(CurrentCard.Rarity) ?? -1;
            int cardTypeIndex = CardTypes?.IndexOf(CurrentCard.CardType) ?? -1;

            _currentLanguage = langCode;
            UIStrings = _localizedUiDictionary[_currentLanguage];

            // Оновлюємо списки
            Rarities = _localizedRarities[_currentLanguage];

            if (CurrentCard.Name == "Нова картка" || CurrentCard.Name == "New Card")
                CurrentCard.Name = UIStrings["DefaultName"];

            if (CurrentCard.Description == "Введіть опис тут..." || CurrentCard.Description == "Enter description here...")
                CurrentCard.Description = UIStrings["DefaultDesc"];

            // Переклад рідкості за позицією у списку (а не лише для дефолтного значення),
            // щоб обрана рідкість не губилась при зміні мови
            if (rarityIndex >= 0 && rarityIndex < Rarities.Count)
                CurrentCard.Rarity = Rarities[rarityIndex];
            else
                CurrentCard.Rarity = Rarities.FirstOrDefault() ?? "";

            if (_localizedTypes.ContainsKey(_currentLanguage))
            {
                CardTypes = _localizedTypes[_currentLanguage];

                if (cardTypeIndex >= 0 && cardTypeIndex < CardTypes.Count)
                    CurrentCard.CardType = CardTypes[cardTypeIndex];
                else if (string.IsNullOrEmpty(CurrentCard.CardType))
                    CurrentCard.CardType = CardTypes.FirstOrDefault() ?? "";
            }
        }

        private void LoadTypes()
        {
            if (File.Exists(TypesFilePath))
            {
                try
                {
                    string json = File.ReadAllText(TypesFilePath);
                    _localizedTypes = JsonSerializer.Deserialize<Dictionary<string, ObservableCollection<string>>>(json) ?? new();
                }
                catch (JsonException)
                {
                    _localizedTypes = new Dictionary<string, ObservableCollection<string>>();
                }
            }

            // Типи за замовчуванням: істота (Персонаж), зброя, заклинання та звичайний предмет
            if (!_localizedTypes.ContainsKey("uk"))
                _localizedTypes["uk"] = new ObservableCollection<string> { "Зброя", "Персонаж", "Заклинання", "Предмет" };

            if (!_localizedTypes.ContainsKey("en"))
                _localizedTypes["en"] = new ObservableCollection<string> { "Weapon", "Character", "Spell", "Item" };

            SaveTypes();
            CardTypes = _localizedTypes[_currentLanguage];
            if (string.IsNullOrEmpty(CurrentCard.CardType))
                CurrentCard.CardType = CardTypes.FirstOrDefault() ?? "";
        }

        private void SaveTypes()
        {
            string json = JsonSerializer.Serialize(_localizedTypes, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(TypesFilePath, json);
        }

        private void LoadSavedCards()
        {
            if (File.Exists(CardsFilePath))
            {
                try
                {
                    string json = File.ReadAllText(CardsFilePath);
                    var cards = JsonSerializer.Deserialize<ObservableCollection<Card>>(json);
                    if (cards != null) SavedCards = cards;
                }
                catch (JsonException) { }
            }
        }

        private void SaveSavedCardsToDisk()
        {
            string json = JsonSerializer.Serialize(SavedCards, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(CardsFilePath, json);
        }

        private void ExecuteAddType(object? parameter)
        {
            if (string.IsNullOrWhiteSpace(NewTypeInput)) return;
            string newType = NewTypeInput.Trim();

            if (!CardTypes.Contains(newType))
            {
                CardTypes.Add(newType);
                SaveTypes();
            }
            CurrentCard.CardType = newType;
            NewTypeInput = string.Empty;
        }

        private void ExecuteEditType(object? parameter)
        {
            string oldType = CurrentCard.CardType;
            string newType = NewTypeInput?.Trim();

            if (!string.IsNullOrEmpty(oldType) && !string.IsNullOrWhiteSpace(newType) && oldType != newType)
            {
                int index = CardTypes.IndexOf(oldType);
                if (index >= 0)
                {
                    CardTypes[index] = newType;
                    CurrentCard.CardType = newType;
                    SaveTypes();
                    NewTypeInput = string.Empty;
                }
            }
        }

        private void ExecuteDeleteType(object? parameter)
        {
            string selectedType = CurrentCard.CardType;

            if (CardTypes.Contains(selectedType))
            {
                CardTypes.Remove(selectedType);
                SaveTypes();
                CurrentCard.CardType = CardTypes.FirstOrDefault() ?? "";
            }
        }

        private void ExecuteSelectImage(object? parameter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Оберіть зображення",
                Filter = "Зображення (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string baseDir = System.AppDomain.CurrentDomain.BaseDirectory;
                    string imagesFolder = Path.Combine(baseDir, "Images");

                    if (!Directory.Exists(imagesFolder)) Directory.CreateDirectory(imagesFolder);

                    string sourcePath = openFileDialog.FileName;
                    string extension = Path.GetExtension(sourcePath);
                    string uniqueFileName = System.Guid.NewGuid().ToString() + extension;
                    string destPath = Path.Combine(imagesFolder, uniqueFileName);

                    File.Copy(sourcePath, destPath, true);
                    CurrentCard.ImagePath = Path.Combine("Images", uniqueFileName);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void ExecuteSave(object? parameter)
        {
            if (CurrentCard.HasErrors)
            {
                string details = string.Join("\n", CurrentCard.GetAllErrorMessages());
                MessageBox.Show($"Виправте помилки у полях перед збереженням:\n\n{details}", "Помилка валідації", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!SavedCards.Contains(CurrentCard))
            {
                SavedCards.Add(CurrentCard);
            }

            SaveSavedCardsToDisk();

            MessageBox.Show("Проєкт успішно збережено в історію!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool CanDeleteSavedCard(object? parameter) => SelectedSavedCard != null;

        private void ExecuteDeleteSavedCard(object? parameter)
        {
            if (SelectedSavedCard == null) return;

            var result = MessageBox.Show(
                "Видалити обрану картку з історії? Це не можна скасувати.",
                "Підтвердження видалення",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            SavedCards.Remove(SelectedSavedCard);
            SaveSavedCardsToDisk();
            SelectedSavedCard = null;
            ExecuteNewCard(null);
        }

        private void ExecuteNewCard(object? parameter)
        {
            CurrentCard = new Card();
            CurrentCard.Name = UIStrings["DefaultName"];
            CurrentCard.Description = UIStrings["DefaultDesc"];
            CurrentCard.CardType = CardTypes.FirstOrDefault() ?? "";
            CurrentCard.Rarity = Rarities.FirstOrDefault() ?? "";
            CurrentCard.Health = 0;
            CurrentCard.Attack = 0;
            CurrentCard.Defense = 0;
            CurrentCard.ManaCost = 0;
            CurrentCard.IncludeHealth = true;
            CurrentCard.IncludeAttack = true;
            CurrentCard.IncludeDefense = true;
            CurrentCard.IncludeManaCost = true;
            CurrentCard.Ability = "";
            CurrentCard.BackgroundColor = DefaultBackgroundColor;
            SelectedSavedCard = null;
        }

        private string _currentTheme = "LightTheme";

        private void ExecuteOpenSettings(object? parameter)
        {
            var settingsWindow = new SettingsWindow(_currentLanguage, _currentTheme);
            settingsWindow.ShowDialog();
        }
    }
}
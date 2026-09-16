using RPGCardMaker.ViewModels;

namespace RPGCardMaker.Models
{
    public class Card : BaseViewModel
    {
        private string _name = "Нова картка";
        private string _cardType = "Зброя";
        private string _description = "Введіть опис тут...";
        private string _ability = "";
        private string _imagePath = "";

        // RPG характеристики
        private string _rarity = "Звичайна";
        private int _health = 0;
        private int _attack = 0;
        private int _defense = 0;
        private int _manaCost = 0;

        private bool _includeHealth = true;
        private bool _includeAttack = true;
        private bool _includeDefense = true;
        private bool _includeManaCost = true;

        private string _backgroundColor = "#FAFAFA";

        public Card()
        {
        }

        public Card(bool isEnglish)
        {
            _name = isEnglish ? "New Card" : "Нова картка";
            _description = isEnglish ? "Enter description here..." : "Введіть опис тут...";
            _rarity = isEnglish ? "Common" : "Звичайна";
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
                ValidateName();
            }
        }

        public string CardType
        {
            get => _cardType;
            set { _cardType = value; OnPropertyChanged(); }
        }

        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        // Унікальна здібність/ефект картки — окремо від сюжетного опису
        public string Ability
        {
            get => _ability;
            set { _ability = value; OnPropertyChanged(); }
        }

        public string ImagePath
        {
            get => _imagePath;
            set { _imagePath = value; OnPropertyChanged(); }
        }

        // --- RPG характеристики ---

        public string Rarity
        {
            get => _rarity;
            set { _rarity = value; OnPropertyChanged(); }
        }

        public int Health
        {
            get => _health;
            set { _health = value; OnPropertyChanged(); ValidateStat(nameof(Health)); }
        }

        public int Attack
        {
            get => _attack;
            set { _attack = value; OnPropertyChanged(); ValidateStat(nameof(Attack)); }
        }

        public int Defense
        {
            get => _defense;
            set { _defense = value; OnPropertyChanged(); ValidateStat(nameof(Defense)); }
        }

        public int ManaCost
        {
            get => _manaCost;
            set { _manaCost = value; OnPropertyChanged(); ValidateStat(nameof(ManaCost)); }
        }

        // --- Перемикачі "чи включати стат" ---

        public bool IncludeHealth
        {
            get => _includeHealth;
            set { _includeHealth = value; OnPropertyChanged(); ValidateStat(nameof(Health)); }
        }

        public bool IncludeAttack
        {
            get => _includeAttack;
            set { _includeAttack = value; OnPropertyChanged(); ValidateStat(nameof(Attack)); }
        }

        public bool IncludeDefense
        {
            get => _includeDefense;
            set { _includeDefense = value; OnPropertyChanged(); ValidateStat(nameof(Defense)); }
        }

        public bool IncludeManaCost
        {
            get => _includeManaCost;
            set { _includeManaCost = value; OnPropertyChanged(); ValidateStat(nameof(ManaCost)); }
        }

        public string BackgroundColor
        {
            get => _backgroundColor;
            set { _backgroundColor = value; OnPropertyChanged(); }
        }

        public override string ToString()
        {
            return Name;
        }

        // --- Валідація ---

        private void ValidateName()
        {
            ClearErrors(nameof(Name));
            if (string.IsNullOrWhiteSpace(Name))
                AddError(nameof(Name), "Назва картки не може бути порожньою.");
        }

        // Якщо стат вимкнено чекбоксом — помилка для нього знімається і не перевіряється
        private void ValidateStat(string propertyName)
        {
            ClearErrors(propertyName);

            bool isIncluded = propertyName switch
            {
                nameof(Health) => IncludeHealth,
                nameof(Attack) => IncludeAttack,
                nameof(Defense) => IncludeDefense,
                nameof(ManaCost) => IncludeManaCost,
                _ => true
            };

            if (!isIncluded) return;

            int value = propertyName switch
            {
                nameof(Health) => Health,
                nameof(Attack) => Attack,
                nameof(Defense) => Defense,
                nameof(ManaCost) => ManaCost,
                _ => 0
            };

            if (value < 0 || value > 999)
                AddError(propertyName, "Значення має бути в межах від 0 до 999.");
        }
    }
}
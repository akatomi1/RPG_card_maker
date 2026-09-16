using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RPGCardMaker.Models
{
    public class LocalizationData
    {
        public Dictionary<string, string> UI { get; set; } = new Dictionary<string, string>();
        public ObservableCollection<string> Rarities { get; set; } = new ObservableCollection<string>();
    }
}
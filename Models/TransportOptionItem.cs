using CommunityToolkit.Mvvm.ComponentModel;

namespace PackMeUp.Models
{
    public partial class TransportOptionItem : ObservableObject
    {
        public string Name { get; }

        [ObservableProperty]
        private bool isSelected;

        public TransportOptionItem(string name)
        {
            Name = name;
        }
    }
}

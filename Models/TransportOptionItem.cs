using CommunityToolkit.Mvvm.ComponentModel;

namespace Packo.Models
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

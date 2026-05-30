using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PackMeUp.Models
{
    public partial class ChildAgeItem : ObservableObject
    {
        private int _age;

        public int Age
        {
            get => _age;
            set => SetProperty(ref _age, value);
        }

        public ChildAgeItem(int age)
        {
            Age = age;
        }

        [RelayCommand]
        private void IncreaseAge()
        {
            if (Age < 17)
                Age++;
        }

        [RelayCommand]
        private void DecreaseAge()
        {
            if (Age > 0)
                Age--;
        }
    }
}

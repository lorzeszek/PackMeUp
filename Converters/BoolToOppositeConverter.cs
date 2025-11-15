using System.Globalization;

namespace PackMeUp.Converters
{
    public class BoolToOppositeConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isTrueOrFalse)
                return !isTrueOrFalse;

            return Binding.DoNothing;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
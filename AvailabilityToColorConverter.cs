using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Theater
{
    public class AvailabilityToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SeatStatus status)
            {
                return status switch
                {
                    SeatStatus.Available => new SolidColorBrush(Color.FromRgb(36, 39, 64)),
                    SeatStatus.Occupied => new SolidColorBrush(Color.FromRgb(168, 2, 2)),
                    SeatStatus.Selected => new SolidColorBrush(Color.FromRgb(212, 141, 0)),
                    _ => Brushes.Gray
                };
            }
            return Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public enum SeatStatus { Available, Occupied, Selected }
}

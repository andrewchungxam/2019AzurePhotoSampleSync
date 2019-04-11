using System;
using System.Globalization;
using Xamarin.Forms;

namespace AzureBlobStorageSampleApp
{
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => !((bool)value);

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => !((bool)value);
    }

    public class AddBarcodeWordConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { 
            if (value == null)
                return string.Empty;
            var barcode = (string)value;
            return $"Barcode: {barcode}";

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { 
            throw new NotImplementedException();
        }
    }

    public class AddBarcodeAndNoBarcodeWordConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { 
            if (value == null)
                return $"No Barcode";
            var barcode = (string)value;
            return $"Barcode: {barcode}";

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { 
            throw new NotImplementedException();
        }
    }

    public class IsBarcodeAndNoBarcodeWordConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { 
            if (value == null)
                return $"No Barcode";
            var barcode = (string)value;
            return $"Barcode available";

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { 
            throw new NotImplementedException();
        }
    }

    public class DateTimeOffSetToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { 
            if (value == null)
                return string.Empty;
            var dateTimeOffsetValue = (DateTimeOffset)value;
            if (dateTimeOffsetValue == DateTimeOffset.MinValue)
                return string.Empty;

            var dtString = dateTimeOffsetValue.ToLocalTime().ToString("MMM d, h:mm tt", new CultureInfo("en-US"));

            return dtString;


        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { 
            throw new NotImplementedException();
        }
    }



    public class DateTimeOffSetMDYToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { 
            if (value == null)
                return string.Empty;
            var dateTimeOffsetValue = (DateTimeOffset)value;
            if (dateTimeOffsetValue == DateTimeOffset.MinValue)
                return string.Empty;

            var dtString = dateTimeOffsetValue.ToLocalTime().ToString("MMMM d, yyyy", new CultureInfo("en-US"));
            return dtString;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { 
            throw new NotImplementedException();
        }
    }



    public class DateTimeOffSetMDYHTToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { 
            if (value == null)
                return string.Empty;
            var dateTimeOffsetValue = (DateTimeOffset)value;
            if (dateTimeOffsetValue == DateTimeOffset.MinValue)
                return string.Empty;

            var dtString = dateTimeOffsetValue.ToLocalTime().ToString("MMMM d, yyyy h:mm tt", new CultureInfo("en-US"));

            return dtString;


        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { 
            throw new NotImplementedException();
        }
    }

    public class CheckAndModifyLocalFiles : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { 
            var valueWord = (string)value;

            if (value == null)
            { 
                return string.Empty;
            } else if ( valueWord.Contains("http"))            
                { 
                return valueWord; 
            } 
                else if (valueWord.Contains(App.LocalPhotoFolderName))
            {
                var fullString = $"{App.LocalAppDirectoryPath}{valueWord}";
                return fullString;
            }
            else 
            { 
                return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { 
            throw new NotImplementedException();
        }
    }


    public class AddCaptionWordConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { 
            if (value == null)
                return string.Empty;
            var valueWord = (string)value;
            return $"Caption: {valueWord}";

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { 
            throw new NotImplementedException();
        }
    }

    public class AddColorWordConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { 
            if (value == null)
                return string.Empty;
            var valueWord = (string)value;
            return $"Colors: {valueWord}";

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { 
            throw new NotImplementedException();
        }
    }

    public class AddObjectDescriptionWordConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { 
            if (value == null)
                return string.Empty;
            var valueWord = (string)value;
            return $"Object description: {valueWord}";

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { 
            throw new NotImplementedException();
        }
    }

    public class AddTagsWordConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { 
            if (value == null)
                return string.Empty;
            var valueWord = (string)value;
            return $"Tags: {valueWord}";

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { 
            throw new NotImplementedException();
        }
    }

    public class AddCustomVisionWordConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { 
            if (value == null)
                return string.Empty;
            var valueWord = (string)value;
            return $"Custom vision tags: {valueWord}";

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { 
            throw new NotImplementedException();
        }
    }
}



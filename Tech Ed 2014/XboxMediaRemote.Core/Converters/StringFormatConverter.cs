﻿using System;
using System.Globalization;
using Windows.UI.Xaml.Data;

namespace XboxMediaRemote.Core.Converters
{
    public class StringFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            parameter = parameter ?? "{0}";

            return String.Format(CultureInfo.CurrentUICulture, parameter.ToString(), value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}

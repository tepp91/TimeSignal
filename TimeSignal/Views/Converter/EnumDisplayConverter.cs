using System;
using System.Globalization;
using System.Windows.Data;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace TimeSignal.Views.Converter
{
	/// <summary>
	/// Enumに設定されているDisplay属性のNameに変換します
	/// </summary>
	class EnumDisplayConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			FieldInfo field = value.GetType().GetField( value.ToString() );
			DisplayAttribute attr = field.GetCustomAttribute<DisplayAttribute>();
			if ( attr != null ) {
				return attr.Name;
			}
			return value.ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

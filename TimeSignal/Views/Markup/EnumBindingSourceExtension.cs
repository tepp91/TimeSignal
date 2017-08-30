using System;
using System.Windows.Markup;

namespace TimeSignal.Views.Markup
{
	public class EnumBindingSourceExtension : MarkupExtension
	{
		public Type EnumType { get; private set; }

		public EnumBindingSourceExtension()
		{
		}

		public EnumBindingSourceExtension( Type enumType )
		{
			EnumType = enumType;
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return Enum.GetValues( EnumType );
		}
	}
}

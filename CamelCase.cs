using System.Linq;

namespace RaiImage
{
    public static class StringHelper
	{
		public static string ToTitle(this string anyCase)
		{
			return char.ToUpper(anyCase[0]) + anyCase.Substring(1).ToLower();
		}
		public static string[] CamelSplit(this string anyCase)
		{
			return new CamelCase(anyCase).Array;
		}
	}
	/// <summary>
	/// some call it PascalCase, the first letter is also uppercase 
	/// </summary>
	public class CamelCase
	{
		public string[] Array
		{
			get
			{
				// taken from http://stackoverflow.com/a/21327150
				if (array == null)
				{
					var a = System.Text.RegularExpressions.Regex.Split(s, @"(?=\p{Lu}\p{Ll})|(?<=\p{Ll})(?=\p{Lu})", System.Text.RegularExpressions.RegexOptions.Compiled);
					array = (from _ in a where !string.IsNullOrWhiteSpace(_) select _).ToArray();
				}
					
				return array;
			}
			set
			{
				array = value;
				s = null;
			}
		}	private string[] array;
		public string String
		{
			get
			{
				return s ?? string.Join("", array);
			}
			set
			{
				s = value;
				array = null;
			}
		}	private string s;
		public CamelCase(string[] listToConvertToCamelCase)
		{
			Array = listToConvertToCamelCase;
		}
		public CamelCase(string alreadyInCamelCase)
		{
			String = alreadyInCamelCase;
		}
	}
}

namespace Bash
{
	using System;
	public class SingleQuotedString : ICommandLineParameter
	{
		string content;
		public SingleQuotedString ()
		{
		}

		public string Text {
			get {
				return '\'' + content + '\'';
			}

			set {
				if ( value.Length > 1
				    && content.StartsWith("'", StringComparison.InvariantCulture) 
				    && content.EndsWith("'", StringComparison.InvariantCulture) )
				{
					content = value.Substring(1, value.Length - 2);
				}
				else
				{
					content = value;
				}
			}
		}
	}
}


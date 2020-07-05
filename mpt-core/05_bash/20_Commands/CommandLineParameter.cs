namespace Bash
{
	public class CommandLineParameter : ICommandLineParameter
	{
		public CommandLineParameter(string text)
		{
			this.Text = text;
		}
		public string Text { get; set; }
	}
}

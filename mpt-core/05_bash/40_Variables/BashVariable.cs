namespace Bash
{
	public class BashVariable
	{
		public BashVariable ()
		{
		}
		public BashVariable (string name)
		{
			this.Name = name;
		}
		public string Name {
			get; set;
		}
	}
}

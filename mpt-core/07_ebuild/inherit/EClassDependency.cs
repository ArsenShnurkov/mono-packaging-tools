namespace Ebuild
{
	public class EClassDependency
	{
		public EClassDependency ()
		{
		}
		public EClassDependency (string name)
		{
			this.Text = name;
		}
		public string Text { get; set; }
	}
}

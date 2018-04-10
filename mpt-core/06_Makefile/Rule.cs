namespace Makefile
{
	using System.Collections.Generic;
	public class Rule
	{
		List<FileOrTargetName> destinations = new List<FileOrTargetName>();
		List<FileOrTargetName> Destinations {
			get
			{
				return destinations;
			}
		}
		List<FileOrTargetName> sources = new List<FileOrTargetName>();
		List<FileOrTargetName> Sources {
			get
			{
				return sources;
			}
		}
		Recipe recipe = new Recipe();
		Recipe Recipe
		{
			get
			{
				return recipe;
			}
		}
	}
}

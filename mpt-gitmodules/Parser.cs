// https://www.kernel.org/pub/software/scm/git/docs/gitmodules.html
// The .gitmodules file, located in the top-level directory of a Git working tree, 
// is a text file with a syntax matching the requirements of git-config(1).
// https://www.kernel.org/pub/software/scm/git/docs/git-config.html
// git config [<file-option>] --remove-section name
// git config --global --remove-section test user
// You can edit the ~/.gitconfig file in your home folder. This is where all --global settings are saved.

// The syntax is fairly flexible and permissive; whitespaces are mostly ignored.
// The # and ; characters begin comments to the end of line, blank lines are ignored.
// The file consists of sections and variables.
// A section begins with the name of the section in square brackets and continues until the next section begins.
// Section names are case-insensitive.
// Only alphanumeric characters, - and . are allowed in section names. 
// Each variable must belong to some section, which means that there must be a section header before the first setting of a variable.
// Sections can be further divided into subsections.
// The following escape sequences (beside \" and \\) are recognized:
// \n for newline character (NL), \t for horizontal tabulation (HT, TAB) and \b for backspace (BS)


namespace mptgitmodules
{
	using System;

	public class Parser
	{
		public Parser ()
		{
		}
	}
}


namespace Bash
{
	public interface ICommandStatementOrVariable
	{
	}
	public interface IBashCommand : ICommandStatementOrVariable
	{
	}
	public interface IBashStatement : ICommandStatementOrVariable
	{
	}
	public interface IBashVariable : ICommandStatementOrVariable
	{
	}

	public interface IBashFunction
	{
	}
	public interface IPartOfScript : ICommandStatementOrVariable, IBashFunction
	{
	}
}

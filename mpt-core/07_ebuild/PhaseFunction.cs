namespace Ebuild
{
	using Bash;
	public enum Phase
	{
		src_unpack,
		src_prepare,
		src_compile,
		src_test,
		src_install,
	}
	public class PhaseFunction
	{
		protected BashFunction underlayingObject;
		public PhaseFunction (BashFunction function, Phase phase)
		{
			this.underlayingObject = function;
			this.Phase = phase;
		}
		public Phase Phase {get; set; }
		public BashFunction UnderlayingObject
		{
			get
			{
				return underlayingObject;
			}
		}
	}
}

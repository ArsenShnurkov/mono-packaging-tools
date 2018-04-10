namespace mptcore
{
	using System;
	using Microsoft.VisualStudio.TextTemplating; // for ITextTemplatingSessionHost interface
	using Mono.TextTemplating; // for TemplateGenerator class

	[Serializable()]
	public class SessionTemplateGenerator : TemplateGenerator, ITextTemplatingSessionHost
	{
		ITextTemplatingSession session;

		public SessionTemplateGenerator ()
		{
			session  = new TextTemplatingSession(); 
		}

		public ITextTemplatingSession Session {
			get {
				return session; 
			}

			set {
				session = value; 
			}
		}

		public ITextTemplatingSession CreateSession ()
		{
			return session;
		}
	}
}

namespace BuildAutomation
{
	using System;
	using System.ComponentModel;
	using System.Windows;
	using System.Xml;

	public class ProjectAssemblyReference
	{
		ProjectAssemblyCSharp parent;
		XmlElement underlying_object = null;
		ObservableReference aggregated_object = null;

		public ProjectAssemblyReference(ProjectAssemblyCSharp parent, XmlElement underlying_object)
		{
			this.parent = parent;
			this.underlying_object = underlying_object;
			Load();
#if true
			this.aggregated_object.PropertyChanged += PropertyChangedEventHandler;
#else
			WeakEventManager<INotifyPropertyChanged,PropertyChangedEventArgs>.AddHandler(
				this.aggregated_object, 
				nameof(INotifyPropertyChanged.PropertyChanged), 
				PropertyChangedEventHandler);
#endif
		}

		public XmlElement UnderlyingObject
		{
			get
			{
				return underlying_object;
			}
			set
			{
				underlying_object = value;
			}
		}
		public ProjectAssemblyCSharp Parent
		{
			get
			{
				return parent;
			}
			set
			{
				parent = value;
			}
		}
		public IAssemblyVersion AssemblyVersion
		{
			get
			{
				return aggregated_object.AssemblyVersion;
			}
			set
			{
				aggregated_object.AssemblyVersion = value;
			}
		}
		public string HintPath
		{
			get
			{
				return aggregated_object.HintPath;
			}
			set
			{
				aggregated_object.HintPath = value;
			}
		}
		public string PackageName
		{
			get
			{
				return aggregated_object.PackageName;
			}
			set
			{
				aggregated_object.PackageName = value;
			}
		}
		public static string GetKeyForItem(ProjectAssemblyReference item)
		{
			return ObservableReference.GetKeyForItem(item.aggregated_object);
		}

		void Load ()
		{
			var xmlManager = new XmlNamespaceManager (this.UnderlyingObject.OwnerDocument.NameTable);
			xmlManager.AddNamespace ("prefix", "http://schemas.microsoft.com/developer/msbuild/2003");

			string referenceInclude = this.UnderlyingObject.Attributes.GetNamedItem ("Include").InnerText;

			string refInnerText = this.UnderlyingObject.SelectSingleNode (@"prefix:HintPath", xmlManager)?.InnerText;
			string referenceHintPath = string.IsNullOrWhiteSpace(refInnerText)?null:refInnerText.Trim();

			string pkgInnerText = this.UnderlyingObject.SelectSingleNode (@"prefix:Package", xmlManager)?.InnerText?.Trim ();
			string referencePackage = string.IsNullOrWhiteSpace(pkgInnerText)?null:pkgInnerText.Trim();

			this.aggregated_object = new ObservableReference(referenceInclude, referenceHintPath, referencePackage);
		}

		void PropertyChangedEventHandler (object sender, PropertyChangedEventArgs e)
		{
			Save();
		}

		void Save ()
		{
			throw new NotImplementedException();
		}
	}
}

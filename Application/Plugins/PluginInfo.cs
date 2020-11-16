using System;
using System.Collections.Generic;

namespace GumpStudio.Plugins
{
	[Serializable]
	public sealed class PluginInfo : IEquatable<PluginInfo>
	{
		private readonly int _Hash;

		public string Name { get; }
		public string Version { get; }
		public string AuthorName { get; }
		public string AuthorContact { get; }
		public string Description { get; }

		public PluginInfo(string name, string version, string authorName, string authorContact, string description)
		{
			Name = name;
			Version = version;
			AuthorName = authorName;
			AuthorContact = authorContact;
			Description = description;

			unchecked
			{
				var hash = 1;

				var comparer = EqualityComparer<string>.Default;

				hash = (hash * 397) ^ comparer.GetHashCode(Name);
				hash = (hash * 397) ^ comparer.GetHashCode(Version);
				hash = (hash * 397) ^ comparer.GetHashCode(AuthorName);
				hash = (hash * 397) ^ comparer.GetHashCode(AuthorContact);
				hash = (hash * 397) ^ comparer.GetHashCode(Description);

				_Hash = hash;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is PluginInfo info && Equals(info);
		}

		public bool Equals(PluginInfo info)
		{
			return _Hash == info?._Hash;
		}

		public override int GetHashCode()
		{
			return _Hash;
		}

		public override string ToString()
		{
			return Name;
		}

		public static bool operator ==(PluginInfo left, PluginInfo right)
		{
			return left?._Hash == right?._Hash;
		}

		public static bool operator !=(PluginInfo left, PluginInfo right)
		{
			return left?._Hash != right?._Hash;
		}
	}
}
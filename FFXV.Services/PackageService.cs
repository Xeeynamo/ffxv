using SQEX.Ebony;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace FFXV.Services
{
	public class PackageService
	{
		public static Package Open(XElement element)
		{
			element = element.Name == "package" ? element : null;
			return Deserialize<Package>(element);
		}

		public static Dictionary<string, bool> GetSupportedTypes(Dictionary<string, bool> typesFound, XElement element)
		{
			var typeName = (string)element.Attribute("type");
			if (typeName != null && !typesFound.ContainsKey(typeName))
			{
				typesFound.Add(typeName, GetSafeType(typeName, false) != null);
			}

			foreach (var childElement in element.Elements())
			{
				typesFound = GetSupportedTypes(typesFound, childElement);
			}

			return typesFound;
		}

		private static T Deserialize<T>(XElement element)
		{
			return (T)Deserialize(element, typeof(T));
		}

		private static object Deserialize(XElement element, Type type)
		{
			if (DynamicAttribute.IsObjectDynamic(type))
			{
				return DeserializeObject(element.Elements(), type);
			}

			var item = Activator.CreateInstance(type);
			var properties = type.GetProperties()
				.ToDictionary(x => x.Name.ToLower(), x => x);

			foreach (var attribute in element.Attributes())
			{
				if (properties.TryGetValue(attribute.Name.ToString().ToLower(), out var property))
				{
					var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
					var value = Convert.ChangeType(attribute.Value, propertyType);
					property.SetValue(item, value);
				}
			}
			
			if (item is SQEX.Ebony.Object obj)
			{
				var childType = GetSafeType($"{obj.Type}");
				if (childType != null)
				{
					obj.Item = DeserializeObject(element.Elements(), childType);
				}
			}
			else
			{
				foreach (var e in element.Elements())
				{
					if (properties.TryGetValue(e.Name.ToString().ToLower(), out var property))
					{
						bool isCollection = e.HasAttributes == false && e.HasElements;
						if (isCollection)
						{
							var collectionType = property.PropertyType.GenericTypeArguments[0];
							var collection = DeserializeCollection(e, collectionType);
							property.SetValue(item, collection);
						}
						else
						{
							throw new NotImplementedException();
						}
					}
				}
			}

			return item;
		}

		public static ICollection<T> DeserializeCollection<T>(XElement element)
		{
			return (ICollection<T>)DeserializeCollection(element, typeof(T));
		}

		public static object DeserializeCollection(XElement element, Type type)
		{
			var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(type));

			foreach (var e in element.Elements())
			{
				var item = Deserialize(e, type);
				list.Add(item);
			}

			return list;
		}

		public static T DeserializeObject<T>(IEnumerable<XElement> elements)
		{
			return (T)DeserializeObject(elements, typeof(Type));
		}

		public static object DeserializeObject(IEnumerable<XElement> elements, Type type)
		{
			var item = Activator.CreateInstance(type);
			var typeProperties = type.GetProperties();
			var properties = typeProperties
				.Select(x => new KeyValuePair<string, PropertyInfo>(x.Name.ToLower(), x))
				.Concat(typeProperties.Select(x => new KeyValuePair<string, PropertyInfo>($"{x.Name.ToLower()}_", x)))
				.ToDictionary(x => x.Key, x => x.Value);

			foreach (var e in elements)
			{
				if (properties.TryGetValue(e.Name.ToString().ToLower(), out var property))
				{
					var isNullable = property.PropertyType.Name.StartsWith("Nullable");
					var isCollection = !isNullable && property.PropertyType.GenericTypeArguments.Length > 0;

					if (isCollection)
					{
						var collectionType = property.PropertyType.GenericTypeArguments[0];
						var collection = DeserializeCollection(e, collectionType);
						property.SetValue(item, collection);
					}
					else if (e.HasElements)
					{
						var value = DeserializeObject(e.Elements(), property.PropertyType);
						property.SetValue(item, value);
					}
					else
					{
						object value = DeserializePrimitive(e, property.PropertyType);
						property.SetValue(item, value);
					}
				}
			}

			return item;
		}

		private static object DeserializePrimitive(XElement element, Type type)
		{
			if (type == typeof(Float4))
			{
				return Float4.FromString(element.Value);
			}
			else if (type == typeof(Color))
			{
				return Color.FromString(element.Value);
			}
			else if (type.IsEnum)
			{
				return DeserializeEnum(element, type);
			}
			else if (type.Name.StartsWith("Nullable"))
			{
				if (string.IsNullOrEmpty(element.Value))
				{
					return null;
				}
				
				return Convert.ChangeType(element.Value, type.GenericTypeArguments[0]);
			}
			else
			{
				return Convert.ChangeType(element.Value, type);
			}
		}

		private static T DeserializeEnum<T>(XElement element)
		{
			return (T)DeserializeEnum(element, typeof(T));
		}

		private static object DeserializeEnum(XElement element, Type type)
		{
			if (!type.IsEnum)
				throw new ArgumentException($"Type {type.Name} is not an enum");

			// Get enum by name, not by value (there is an attribute called 'value')
			return Enum.Parse(type, element.Value.ToString());
		}

		static List<string> _assemblies = new List<string> { "SQEX.Ebony", "Black" };
		static Dictionary<string, Type> _types = new Dictionary<string, Type>();

		public static Type GetSafeType(string typeName, bool throwsException = false)
		{
			if (_types.TryGetValue(typeName, out var foundType))
				return foundType;

			foreach (var assembly in _assemblies)
			{
				if (typeName.IndexOf(assembly) == 0)
				{
					var type = Type.GetType($"{typeName},{assembly}");
					if (type == null)
						continue;

					_types[typeName] = type;
					return type;
				}
			}

			if (throwsException)
			{
				throw new Exception($"Type {typeName} not found");
			}

			return null;
		}
	}
}

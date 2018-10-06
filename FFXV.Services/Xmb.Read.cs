using System.IO;
using System.Xml.Linq;

namespace FFXV.Services
{
	public partial class Xmb
	{
		private XElement ReadNode(Element xmbElement)
		{
			var element = new XElement(xmbElement.Name);

			var variantValue = variants[xmbElement.VariantOffset];
			element.Value = variantValue.Name;

			for (int i = 0; i < xmbElement.AttributeCount; i++)
			{
				var attributeIndex = attributeIndexTable[xmbElement.AttributeTableIndex + i];
				var attribute = attributes[attributeIndex];
				var value = variants[attribute.VariantOffset];
				element.Add(new XAttribute(attribute.Name, value.Name));
			}

			for (int i = 0; i < xmbElement.ElementCount; i++)
			{
				var elementIndex = elementIndexTable[xmbElement.ElementTableIndex + i];
				var node = ReadNode(elements[elementIndex]);
				element.Add(node);
			}

			return element;
		}

		private XElement ReadRootNode()
		{
			return ReadNode(elements[rootElement]);
		}

		public static XElement Open(Stream stream)
		{
			return new Xmb(new BinaryReader(stream))
				.ReadRootNode();
		}
	}
}

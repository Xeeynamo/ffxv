using System;
using System.IO;
using System.Linq;

namespace FFXV.Services
{
	internal class Xmb2Writer
	{
		private Xmb _xmb;
		private BinaryWriter _writer;

		private int[] elementsOffset;
		private int[] attributesOffset;
		private int[] variantsOffset;

		public Xmb2Writer(Xmb xmb, BinaryWriter writer)
		{
			_xmb = xmb;
			_writer = writer;

			elementsOffset = new int[xmb.Elements.Count()];
			attributesOffset = new int[xmb.Elements.Count()];
			variantsOffset = new int[xmb.Elements.Count()];
		}

		public void Convert()
		{
			_writer.Write(0x32424D58); // XMB2
			_writer.Write(0x9C0);
			_writer.Write(0);
			_writer.WriteOffset(() => WriteElement(_xmb.RootElementIndex));
			
			//_writer.Write(_xmb.RootElementIndex * 12 + 4);
			//foreach (var element in _xmb.Elements)
			//{
			//	Write(element);
			//}
		}

		private int WriteElement(int elementIndex)
		{
			if (elementsOffset[elementIndex] != 0)
				return elementsOffset[elementIndex];

			var element = _xmb.Elements.ElementAt(elementIndex);

			int elementOffset;
			if (element.ElementCount > 0)
			{
				for (int i = 0; i < element.ElementCount; i++)
				{
					WriteElement(_xmb.ElementIndexTable.ElementAt(element.ElementTableIndex + i));
				}

				elementOffset = 0x7EADBEEF;
			}
			else
			{
				var variant = _xmb.Variants.ElementAt(element.VariantOffset);
				if (!string.IsNullOrEmpty(variant.Name))
				{
					elementOffset = 0x7EADC0DE;
				}
				else
				{
					elementOffset = 0;
				}
			}

			elementsOffset[elementIndex] = (int)_writer.BaseStream.Position;
			_writer.Write(elementOffset);
			_writer.Write(0x4b8);
			_writer.Write((short)element.ElementCount);
			_writer.Write((byte)0);
			_writer.Write((byte)element.AttributeCount);

			return elementsOffset[elementIndex];
		}

		public void Write(Xmb.Attribute attribute)
		{

		}

		public void Write(Xmb.Variant variant)
		{

		}
	}

	public partial class Xmb2
	{
		public static void Create(Xmb xmb, BinaryWriter writer)
		{
			new Xmb2Writer(xmb, writer).Convert();
		}
	}

	public static class BinaryWriterExtension
	{
		public static void WriteOffset(this BinaryWriter writer, Func<int> func)
		{
			var curPos = (int)writer.BaseStream.Position;
			writer.BaseStream.Position += 4; // move away at least by 4 bytes

			var offset = func();
			writer.BaseStream.Position = curPos;
			writer.Write(offset - curPos);
		}
	}
}

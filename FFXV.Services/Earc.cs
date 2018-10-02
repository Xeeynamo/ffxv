using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Xe;

namespace FFXV.Services
{
	public class Earc
	{
		private class EntryStream : Stream
		{
			private class ChunkDesc
			{
				public int CmpOff;
				public int CmpLen;
				public int DecOff;
				public int DecLen;
			}

			private BinaryReader _reader;
			private Entry _entry;
			private List<ChunkDesc> _chunks = new List<ChunkDesc>();
			private long _offset;
			private long _length;
			private long _position;
			private bool _isCompressed;

			private ChunkDesc _currentChunk;
			private byte[] _currentChunkData;

			public override bool CanRead => true;

			public override bool CanSeek => true;

			public override bool CanWrite => false;

			public override long Length => _length;

			public override long Position
			{
				get => _position;
				set
				{
					if (value < 0 || value > Length)
						throw new IOException("Specified position is out of range.");
					_position = value;
				}
			}

			public EntryStream(BinaryReader reader, Entry entry)
			{
				_reader = reader;
				_entry = entry;
				_offset = entry.DataOffset;
				_position = 0;

				_isCompressed = entry.Type.HasFlag(ArchiveType.Compressed);
				if (_isCompressed)
				{
					lock (reader)
					{
						_reader.BaseStream.Position = _offset;
						int cmpOffset = 8;
						int decOffset = 0;

						while (decOffset < entry.UncompressedLength)
						{
							int cmpLen = _reader.ReadInt32();
							int decLen = _reader.ReadInt32();
							int align = (cmpLen & 3);
							if (align > 0)
								cmpLen += 4 - align;
							_reader.BaseStream.Position += cmpLen;
							_chunks.Add(new ChunkDesc()
							{
								CmpOff = cmpOffset,
								CmpLen = cmpLen,
								DecOff = decOffset,
								DecLen = decLen
							});
							cmpOffset += cmpLen + 8;
							decOffset += decLen;
						}
						_length = decOffset;
					}
				}
				else
				{
					_length = entry.Length;
				}
			}

			public override void Flush()
			{
			}

			public override int Read(byte[] buffer, int offset, int count)
			{
				if (_isCompressed)
				{
					if (_currentChunk == null || Position < _currentChunk.DecOff ||
						Position >= (_currentChunk.DecOff + _currentChunk.DecLen))
					{
						_currentChunkData = null;
						foreach (var chunk in _chunks)
						{
							if (Position >= chunk.DecOff &&
								Position < (chunk.DecOff + chunk.DecLen))
							{
								_currentChunk = chunk;
								_currentChunkData = new byte[_currentChunk.DecLen];
								lock (_reader.BaseStream)
								{
									_reader.BaseStream.Position = _entry.DataOffset + _currentChunk.CmpOff;
									short header = _reader.ReadInt16(); // Skip header
									new DeflateStream(_reader.BaseStream, CompressionMode.Decompress)
										.Read(_currentChunkData, 0, _currentChunkData.Length);
								}
								break;
							}
						}
					}
					if (_currentChunkData != null)
					{
						int realOffset = (int)(Position - _currentChunk.DecOff);
						int read = Math.Min(count, _currentChunk.DecLen - realOffset);
						if (read < count)
						{
							Buffer.BlockCopy(_currentChunkData, realOffset, buffer, offset, read);
							_position += read;
							if (Position < Length)
							{
								read += Read(buffer, offset + read, count - read);
							}
						}
						else
						{
							Buffer.BlockCopy(_currentChunkData, realOffset, buffer, offset, read);
							_position += read;
						}
						return read;
					}
					return 0;
				}
				else
				{
					lock (_reader.BaseStream)
					{
						_reader.BaseStream.Position = _offset + Position;
						var toRead = (int)Math.Min(count, Length - Position);
						var read = _reader.BaseStream.Read(buffer, 0, toRead);
						_position += read;
						return read;
					}
				}
			}

			public override long Seek(long offset, SeekOrigin origin)
			{
				switch (origin)
				{
					case SeekOrigin.Begin:
						return Position = offset;
					case SeekOrigin.Current:
						return Position += offset;
					case SeekOrigin.End:
						return Position = Length + offset;
					default:
						throw new ArgumentException();
				}
			}

			public override void SetLength(long value)
			{
				throw new NotSupportedException();
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				throw new NotSupportedException();
			}
		}

		[Flags]
		public enum ArchiveType
		{
			Unknown = 1,
			Compressed = 2,
			Extern = 4,
		}

		public class Entry
		{
			public ulong Hash;
			public uint UncompressedLength;
			public int Length;

			public ArchiveType Type { get; internal set; }
			public uint FileNameOffset;
			public uint DataOffset;
			public uint Unk1C;
			public uint OriginalFileNameOffset;
			public uint Unk24;

			public string FileName { get; set; }
			public string OriginalFileName { get; set; }
		}

		private long _basePos;
		private BinaryReader _reader;

		public Entry[] Entries { get; }

		public static bool TryOpen(BinaryReader reader)
		{
			return reader.ReadUInt32() == 0x46415243;
		}

		public Earc(BinaryReader reader)
		{
			_reader = reader;
			_basePos = reader.BaseStream.Position;

			var magicCode = reader.ReadUInt32();
			var fileStart = reader.ReadUInt16();
			var unk = reader.ReadUInt16();  // Must be 3 for FARC files
			var dependenciesCount = reader.ReadInt32();
			var unk0c = reader.ReadInt32();
			var offFileTable = reader.ReadInt32();
			reader.BaseStream.Position = _basePos + fileStart;

			var chunkFilenames = reader.ReadInt32();
			var chunkUnk2 = reader.ReadInt32();
			var chunkData = reader.ReadInt32();

			reader.BaseStream.Position = _basePos + offFileTable;
			Entries = new Entry[dependenciesCount];
			for (int i = 0; i < dependenciesCount; i++)
			{
				Entries[i] = new Entry()
				{
					Hash = reader.ReadUInt64(),
					UncompressedLength = reader.ReadUInt32(),
					Length = reader.ReadInt32(),
					Type = (ArchiveType)reader.ReadUInt32(),
					FileNameOffset = reader.ReadUInt32(),
					DataOffset = reader.ReadUInt32(),
					Unk1C = reader.ReadUInt32(),
					OriginalFileNameOffset = reader.ReadUInt32(),
					Unk24 = reader.ReadUInt32(),
				};
			}

			var buffer = new char[0x80];
			for (int i = 0; i < Entries.Length; i++)
			{
				var entry = Entries[i];
				reader.BaseStream.Position = _basePos + entry.FileNameOffset;
				entry.FileName = reader.ReadCString();
				reader.BaseStream.Position = _basePos + entry.OriginalFileNameOffset;
				entry.OriginalFileName = reader.ReadCString();
			}
		}

		public Stream GetStream(Entry entry)
		{
			return new EntryStream(_reader, entry);
		}

		public byte[] GetEntryData(int index)
		{
			var entry = Entries[index];
			lock (_reader)
			{
				var memStream = new MemoryStream(0x20000);
				_reader.BaseStream.Position = _basePos + entry.DataOffset;

				for (int pos = 0, uncLength = 0; pos < entry.Length;)
				{
					var srcBlockLength = _reader.ReadInt32();
					var dstBlockLength = _reader.ReadInt32();
					if (srcBlockLength == 0 ||
						dstBlockLength == 0)
						break;

					pos += srcBlockLength + 8;
					uncLength += dstBlockLength;
					if (memStream.Capacity < uncLength)
						memStream.Capacity = uncLength;

					var nextBlock = _reader.BaseStream.Position + srcBlockLength;
					var deflateHeader = _reader.ReadInt16(); // Skip header
					new DeflateStream(_reader.BaseStream, CompressionMode.Decompress).CopyTo(memStream);

					var discard = nextBlock & 3;
					if ((discard & 3) != 0)
						_reader.BaseStream.Position = nextBlock + 4 - discard;
					else
						_reader.BaseStream.Position = nextBlock;
				}
				return memStream.GetBuffer();
			}
		}
	}
}

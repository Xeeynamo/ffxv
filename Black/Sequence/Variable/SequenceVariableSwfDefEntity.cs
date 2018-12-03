using Black.Entity.Data.Menu;
using SQEX.Ebony.Framework.Sequence;
using System;
using System.Collections.Generic;

namespace Black.Sequence.Variable
{
	public class SequenceVariableSwfDefEntity : SequenceVariableSwfEntity
	{
		public bool IsSetTextFieldConfig { get; set; }

		public bool IsSetLabelSound { get; set; }

		public ICollection<SwfTextFieldConfigArrayItem> TextFiledConfigList { get => null; set => throw new NotImplementedException(); }

		public ICollection<object> LabelAccessoryList { get => null; set => throw new NotImplementedException(); }

		public ICollection<object> NameList { get => null; set => throw new NotImplementedException(); }

		public ICollection<object> FixidList { get => null; set => throw new NotImplementedException(); }
	}
}

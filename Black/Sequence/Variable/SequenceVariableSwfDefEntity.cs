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

		// TODO
		public ICollection<SwfTextFieldConfigArrayItem> TextFiledConfigList { get; set; }

		public ICollection<SwfLabelAccessoryArrayItem> LabelAccessoryList { get; set; }

		public ICollection<SwfStringArrayItem> NameList { get; set; }

		public ICollection<SwfFixidArrayItem> FixidList { get; set; }
	}
}

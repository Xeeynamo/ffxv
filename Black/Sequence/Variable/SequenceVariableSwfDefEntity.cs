using Black.Entity.Data.Menu;
using SQEX.Ebony.Framework.Sequence;
using System;
using System.Collections.Generic;

namespace Black.Sequence.Variable
{
	public class SequenceVariableSwfDefEntity
	{
		// TODO
		public string RefInPorts { get; set; }

		// TODO
		public string RefOutPorts { get; set; }

		public string Comment { get; set; }

		public int PinNum { get; set; }

		public SequenceContainerPin DynamicVarInputPin8 { get; set; }
		public SequenceContainerPin DynamicVarInputPin7 { get; set; }
		public SequenceContainerPin DynamicVarInputPin6 { get; set; }
		public SequenceContainerPin DynamicVarInputPin5 { get; set; }
		public SequenceContainerPin DynamicVarInputPin4 { get; set; }
		public SequenceContainerPin DynamicVarInputPin3 { get; set; }
		public SequenceContainerPin DynamicVarInputPin2 { get; set; }
		public SequenceContainerPin DynamicVarInputPin1 { get; set; }
		public SequenceContainerPin DynamicVarInputPin32 { get; set; }
		public SequenceContainerPin DynamicVarInputPin31 { get; set; }
		public SequenceContainerPin DynamicVarInputPin30 { get; set; }
		public SequenceContainerPin DynamicVarInputPin29 { get; set; }
		public SequenceContainerPin DynamicVarInputPin28 { get; set; }
		public SequenceContainerPin DynamicVarInputPin27 { get; set; }
		public SequenceContainerPin DynamicVarInputPin26 { get; set; }
		public SequenceContainerPin DynamicVarInputPin25 { get; set; }
		public SequenceContainerPin DynamicVarInputPin24 { get; set; }
		public SequenceContainerPin DynamicVarInputPin23 { get; set; }
		public SequenceContainerPin DynamicVarInputPin22 { get; set; }
		public SequenceContainerPin DynamicVarInputPin21 { get; set; }
		public SequenceContainerPin DynamicVarInputPin20 { get; set; }
		public SequenceContainerPin DynamicVarInputPin19 { get; set; }
		public SequenceContainerPin DynamicVarInputPin18 { get; set; }
		public SequenceContainerPin DynamicVarInputPin17 { get; set; }
		public SequenceContainerPin DynamicVarInputPin16 { get; set; }
		public SequenceContainerPin DynamicVarInputPin15 { get; set; }
		public SequenceContainerPin DynamicVarInputPin14 { get; set; }
		public SequenceContainerPin DynamicVarInputPin13 { get; set; }
		public SequenceContainerPin DynamicVarInputPin12 { get; set; }
		public SequenceContainerPin DynamicVarInputPin11 { get; set; }
		public SequenceContainerPin DynamicVarInputPin10 { get; set; }
		public SequenceContainerPin DynamicVarInputPin9 { get; set; }

		public string Prefix1 { get; set; }
		public string Prefix2 { get; set; }
		public string Prefix3 { get; set; }
		public string Prefix4 { get; set; }
		public string Prefix5 { get; set; }
		public string Prefix6 { get; set; }
		public string Prefix7 { get; set; }
		public string Prefix8 { get; set; }
		public string Prefix9 { get; set; }
		public string Prefix10 { get; set; }
		public string Prefix11 { get; set; }
		public string Prefix12 { get; set; }
		public string Prefix13 { get; set; }
		public string Prefix14 { get; set; }
		public string Prefix15 { get; set; }
		public string Prefix16 { get; set; }
		public string Prefix17 { get; set; }
		public string Prefix18 { get; set; }
		public string Prefix19 { get; set; }
		public string Prefix20 { get; set; }
		public string Prefix21 { get; set; }
		public string Prefix22 { get; set; }
		public string Prefix23 { get; set; }
		public string Prefix24 { get; set; }
		public string Prefix25 { get; set; }
		public string Prefix26 { get; set; }
		public string Prefix27 { get; set; }
		public string Prefix28 { get; set; }
		public string Prefix29 { get; set; }
		public string Prefix30 { get; set; }
		public string Prefix31 { get; set; }
		public string Prefix32 { get; set; }

		public SequenceContainerPin OutValue { get; set; }

		public SequenceContainerPin EntityPointer { get; set; }

		public bool IsSetTextFieldConfig { get; set; }

		public bool IsSetLabelSound { get; set; }

		public ICollection<SwfTextFieldConfigArrayItem> TextFiledConfigList { get => null; set => throw new NotImplementedException(); }

		public ICollection<object> LabelAccessoryList { get => null; set => throw new NotImplementedException(); }

		public ICollection<object> NameList { get => null; set => throw new NotImplementedException(); }

		public ICollection<object> FixidList { get => null; set => throw new NotImplementedException(); }

		public double PositionX { get; set; }

		public double PositionY { get; set; }
	}


}

using OrcaMDF.Core.Engine.Records;
using OrcaMDF.Framework;

namespace OrcaMDF.Core.Engine.Pages
{
	internal class TextMixPage : RecordPage
	{
		public TextRecord[] Records { get; protected set; }

		public TextMixPage(byte[] bytes, Database database)
			: base(bytes, database)
		{
			parseRecords();
		}

		private void parseRecords()
		{
			Records = new TextRecord[Header.SlotCnt];

			int cnt = 0;
			foreach (short recordOffset in SlotArray)
				Records[cnt++] = new TextRecord(ArrayHelper.SliceArray(RawBytes, recordOffset, RawBytes.Length - recordOffset), this);
		}
	}
}
namespace FTN.ESI.SIMES.CIM.CIMAdapter
{
	using System.Text;

	/// <summary>
	/// TransformAndLoadReport
	/// </summary>
	public class TransformAndLoadReport
	{
		private StringBuilder report = new StringBuilder();
		private bool success = true;


		public StringBuilder Report
		{
			get 
			{
				return report;
			}
			set
			{
				report = value;
			}
		}

		public bool Success
		{
			get
			{
				return success;
			}
			set
			{
				success = value;
			}
		}
	}
}

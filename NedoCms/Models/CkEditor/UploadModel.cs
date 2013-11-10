using System.Web;

namespace NedoCms.Models.CkEditor
{
	/// <summary>
	/// Describes file uploaded from CkEditor
	/// </summary>
	public sealed class UploadModel : EditorModel
	{
		/// <summary>
		/// Gets or sets uploaded file
		/// </summary>
		public HttpPostedFileBase Upload { get; set; }
	}

	/// <summary>
	/// Represents result of upload action
	/// </summary>
	public sealed class UploadResultModel : EditorModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UploadResultModel"/> class.
		/// </summary>
		/// <param name="model">Model to copy information from</param>
		public UploadResultModel(EditorModel model)
		{
			CKEditor = model.CKEditor;
			CKEditorFuncNum = model.CKEditorFuncNum;
			LangCode = model.LangCode;
		}

		/// <summary>
		/// Gets or sets the url of the uploaded file
		/// </summary>
		public string Url { get; set; }

		/// <summary>
		/// Gets or sets upload error. Value is null if no error occured
		/// </summary>
		public string Error { get; set; }
	}
}
﻿using CCPApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCPApp
{
	/// <summary>
	/// Generates PDFs and contains methods for generating the reports we need.
	/// Important note: this uses a modified version of itextsharp.  I rebuilt it from source,
	/// and made some custom changes required to get it to work on Xamarin.
	/// </summary>
	public interface IGeneratePdf
	{
		void Initialize(Inspection inspection, bool ScoredOnly = false);

		void NewPage();
		void CreateCommentPage(Comment comment);
		void CreateQuestionSection(ReportSection section);

		void Finish();
	}
}

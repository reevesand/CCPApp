using CCPApp.iOS;
using CCPApp.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Font = iTextSharp.text.Font;
using Rectangle = iTextSharp.text.Rectangle;
using it = iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Drawing;
using Xamarin.Forms;
using CCPApp.Utilities;

[assembly: Dependency(typeof(GeneratePdf))]
namespace CCPApp.iOS
{
	public class GeneratePdf : IGeneratePdf
	{
		PdfWriter writer;
		Document doc;
		PdfContentByte cb;

		Dictionary<string, Font> fonts = new Dictionary<string, Font>();

		Inspection inspection;
		SectionModel currentSection;
		SectionPart currentPart;

		public static float questionsTableWidth = 500;
		public static float totalQuestionsTableSegments = 89;
		bool isEmpty = true;

		static it.Color redColor = new it.Color(255, 0, 0);
		static it.Color greenColor = new it.Color(0, 255, 0);
		static it.Color blueColor = new it.Color(0, 0, 255);
		static it.Color headerBackgroundColor = new it.Color(211, 211, 211);

		public void Initialize(Inspection inspection, bool scoredOnly = false)
		{
			this.inspection = inspection;
			string fileName = "Report.pdf";
			doc = new Document(PageSize.LETTER);

			string privatePath = new FileManage().GetTempFolder();
			string fullPath = Path.Combine(privatePath, fileName);
			FileStream stream = new FileStream(fullPath, System.IO.FileMode.Create);

			writer = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, stream);
			doc.SetMargins(63, 63, 10, doc.BottomMargin);
			doc.Open();
			cb = writer.DirectContent;

			fonts["TNR12Bold"] = new Font(Font.TIMES_ROMAN, 12, Font.BOLD);
			fonts["TNR12BoldUnderline"] = new Font(Font.TIMES_ROMAN,12,Font.UNDERLINE | Font.BOLD);
			fonts["TNR12"] = new Font(Font.TIMES_ROMAN, 12);
			fonts["TNR10"] = new Font(Font.TIMES_ROMAN, 10);
			fonts["TNR10Bold"] = new Font(Font.TIMES_ROMAN, 10, Font.BOLD);
			fonts["TNR9"] = new Font(Font.TIMES_ROMAN, 9);
			fonts["TNR9Blue"] = new Font(Font.TIMES_ROMAN, 9, Font.NORMAL,iTextSharp.text.Color.BLUE);
			fonts["TNR1"] = new Font(Font.TIMES_ROMAN, 1);
			fonts["TNR11"] = new Font(Font.TIMES_ROMAN, 11);
			fonts["TNR11Bold"] = new Font(Font.TIMES_ROMAN, 11, Font.BOLD);
			fonts["TNR11BoldItalic"] = new Font(Font.TIMES_ROMAN, 11, Font.BOLDITALIC);
			fonts["TNR16Bold"] = new Font(Font.TIMES_ROMAN, 16, Font.BOLD);

		}
		public void NewPage()
		{
			doc.NewPage();
		}
		public void NewPageIfNotEmpty()
		{
			if (!DocIsEmpty())
			{
				NewPage();
			}
		}
		public void Finish()
		{
			if (DocIsEmpty())
			{	//Everything explodes if there is no page content whatsoever.
				DrawPlaceholder();
			}
			doc.Close();
		}

		internal bool DocIsEmpty()
		{
			return isEmpty;
		}
		internal int PageNumber()
		{
			return doc.PageNumber + 1;
		}

		internal void DrawPlaceholder()
		{
			cb.SetCMYKColorStroke(1, 1, 1, 1);
			cb.Rectangle(0, 0, 1, 1);
			cb.Stroke();
			cb.SetCMYKColorStroke(0, 0, 0, 0);
			isEmpty = false;
		}
		internal void SpacingPlaceholder()
		{
			Paragraph placeholder = new Paragraph(1, "\u00a0");	//Non breaking space character.  
			//This allows elements to use "Spacingbefore" to decide where they should start.
			doc.Add(placeholder);
		}

		internal void AddPageHeader()
		{
			Paragraph header = new Paragraph();
			Font boldTimes = fonts["TNR12Bold"];

			Chunk inspectionName = new Chunk(inspection.Name + "\n",boldTimes);
			Chunk orgName = new Chunk(inspection.Organization+"\n\n",boldTimes);
			Chunk dateLabel = new Chunk("Inspection Date:     ",fonts["TNR10Bold"]);
			Chunk date = new Chunk(DateTime.Now.ToString("MMMM d, yyyy"),fonts["TNR10"]);

			header.Add(inspectionName);
			header.Add(orgName);
			header.Add(dateLabel);
			header.Add(date);

			cb.MoveTo(27, 712);
			cb.LineTo(585, 712);
			cb.Stroke();

			//Paragraph placeholder = new Paragraph(1, "\u00a0");	//Non breaking space character.  
				//This allows elements to use "Spacingbefore" to decide where they should start.
			//doc.Add(placeholder);

			doc.Add(header);
		}
		internal void AddPageFooter()
		{

		}

		public void CreateCommentPage(Comment comment)
		{
			SpacingPlaceholder();
			PdfPTable table = new PdfPTable(10);
			table.TotalWidth = 550;
			table.LockedWidth = true;
			table.SplitLate = true;
			table.SplitRows = false;
			table.SpacingBefore = 15;
			Phrase phrase1;
			Phrase phrase2;
			Paragraph para;

			PdfPCell topCell = new PdfPCell();
			topCell.BackgroundColor = headerBackgroundColor;
			topCell.Colspan = 10;
			topCell.MinimumHeight = 18;

			phrase1 = new Phrase("TYPE OF VISIT:\n", fonts["TNR10Bold"]);
			phrase2 = new Phrase(inspection.Name, fonts["TNR10"]);
			para = new Paragraph();
			para.Add(phrase1);
			para.Add(phrase2);
			PdfPCell typeCell = new PdfPCell(para);
			typeCell.Colspan = 4;
			typeCell.MinimumHeight = 36;

			phrase1 = new Phrase("AREA INSPECTED:\n", fonts["TNR10Bold"]);
			phrase2 = new Phrase(comment.question.section.Title, fonts["TNR10"]);
			para = new Paragraph();
			para.Add(phrase1);
			para.Add(phrase2);
			PdfPCell areaCell = new PdfPCell(para);
			areaCell.Colspan = 4;
			areaCell.MinimumHeight = 36;

			phrase1 = new Phrase("QUESTION:\n", fonts["TNR10Bold"]);
			phrase2 = new Phrase(comment.question.ToString(), fonts["TNR10"]);
			para = new Paragraph();
			para.Add(phrase1);
			para.Add(phrase2);
			PdfPCell questionLabelCell = new PdfPCell(para);
			questionLabelCell.Colspan = 2;
			questionLabelCell.MinimumHeight = 36;

			phrase1 = new Phrase("UNIT:\n", fonts["TNR10Bold"]);
			phrase2 = new Phrase("Organization Name.  TODO add this on inspections.", fonts["TNR10"]);
			para = new Paragraph();
			para.Add(phrase1);
			para.Add(phrase2);
			PdfPCell unitCell = new PdfPCell(para);
			unitCell.Colspan = 7;
			unitCell.MinimumHeight = 36;

			phrase1 = new Phrase("DATE:\n", fonts["TNR10Bold"]);
			phrase2 = new Phrase(DateTime.Now.ToString("MMMM d, yyyy"), fonts["TNR10"]);
			para = new Paragraph();
			para.Add(phrase1);
			para.Add(phrase2);
			PdfPCell dateCell = new PdfPCell(para);
			dateCell.Colspan = 3;
			dateCell.MinimumHeight = 36;

			phrase1 = new Phrase("SUBJECT:\n", fonts["TNR10Bold"]);
			phrase2 = new Phrase(comment.Subject, fonts["TNR10"]);
			para = new Paragraph();
			para.Add(phrase1);
			para.Add(phrase2);
			PdfPCell subjectCell = new PdfPCell(para);
			subjectCell.Colspan = 10;
			subjectCell.MinimumHeight = 54;

			//PdfPCell pocCell = new PdfPCell();
			//pocCell.Colspan = 3;
			//pocCell.MinimumHeight = 30;

			para = new Paragraph();
			phrase1 = new Phrase("REFERENCES:\n", fonts["TNR10Bold"]);
			para.Add(phrase1);
			foreach (Reference reference in comment.question.References)
			{
				phrase2 = new Phrase(reference.Description+"\n", fonts["TNR10"]);
				para.Add(phrase2);
			}
			PdfPCell referenceCell = new PdfPCell(para);
			referenceCell.Colspan = 10;
			referenceCell.MinimumHeight = 54;

			//PdfPCell phoneCell = new PdfPCell();
			//questionLabelCell.Colspan = 3;
			//topCell.MinimumHeight = 30;

			phrase1 = new Phrase(comment.type.ToString().ToUpper()+"\n", fonts["TNR10Bold"]);
			phrase2 = new Phrase(comment.CommentText, fonts["TNR10"]);
			para = new Paragraph();
			para.Add(phrase1);
			para.Add(phrase2);
			PdfPCell commentCell = new PdfPCell(para);
			commentCell.Colspan = 10;
			commentCell.MinimumHeight = 130;

			phrase1 = new Phrase("DISCUSSION:\n", fonts["TNR10Bold"]);
			phrase2 = new Phrase(comment.Discussion, fonts["TNR10"]);
			para = new Paragraph();
			para.Add(phrase1);
			para.Add(phrase2);
			PdfPCell discussionCell = new PdfPCell(para);
			discussionCell.Colspan = 10;
			discussionCell.MinimumHeight = 130;

			phrase1 = new Phrase("RECOMMENDATION/ACTION TAKEN OR REQUIRED:\n", fonts["TNR10Bold"]);
			phrase2 = new Phrase(comment.Recommendation, fonts["TNR10"]);
			para = new Paragraph();
			para.Add(phrase1);
			para.Add(phrase2);
			PdfPCell recommendationCell = new PdfPCell(para);
			recommendationCell.Colspan = 10;
			recommendationCell.MinimumHeight = 130;

			phrase1 = new Phrase("INSPECTOR:\n", fonts["TNR10Bold"]);
			para = new Paragraph();
			para.Add(phrase1);
			if (inspection.inspectors.Any())
			{
				phrase2 = new Phrase(inspection.inspectors.First().Name, fonts["TNR10"]);	//TODO store on the comment.
				para.Add(phrase2);
			}
			PdfPCell inspectorCell = new PdfPCell(para);
			inspectorCell.Colspan = 10;
			inspectorCell.MinimumHeight = 36;

			table.AddCell(topCell);
			table.AddCell(typeCell);
			table.AddCell(areaCell);
			table.AddCell(questionLabelCell);
			table.AddCell(unitCell);
			table.AddCell(dateCell);
			table.AddCell(subjectCell);
			//table.AddCell(pocCell);
			table.AddCell(referenceCell);
			//table.AddCell(phoneCell);
			table.AddCell(commentCell);
			table.AddCell(discussionCell);
			table.AddCell(recommendationCell);
			table.AddCell(inspectorCell);

			doc.Add(table);

			/*cb.Rectangle(85.5F, 96.75F, 459, 639);
			cb.Stroke();
			cb.BeginText();
			BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
			cb.SetFontAndSize(bf, 12);
			cb.MoveText(86,723);
			cb.ShowText(comment.CommentText);
			cb.EndText();*/
			isEmpty = false;
		}
		public void CreateQuestionSection(ReportSection section)
		{
			currentSection = section.section;
			if (!section.PartsToRender.Any())
			{
				currentPart = null;
				QuestionPages(section.section.Questions);
			}
			else
			{
				foreach (SectionPart part in section.PartsToRender)
				{
					currentPart = part;
					CreateQuestionPart(part);
					if (part != section.PartsToRender.Last())
					{
						NewPage();
					}
				}
			}
			isEmpty = false;
			//Section score thingy.
		}
		internal void CreateQuestionPart(SectionPart part)
		{
			QuestionPages(part.Questions);
			//Part score thingy.
		}
		internal void QuestionPages(List<Question> questions){
			int nextIndex = 0;
			while (nextIndex < questions.Count)
			{
				nextIndex = QuestionTablePage(questions, nextIndex);
				if (nextIndex < questions.Count)
				{
					NewPage();
				}
			}
		}
		internal int QuestionTablePage(List<Question> questions, int nextQuestionIndex)
		{
			AddPageHeader();
			//Section and Part header.
			//Table.  Including:
				//Header

			//table setup.
			Paragraph HeaderParagraph = new Paragraph();
			HeaderParagraph.SpacingBefore = 10;
			Phrase SectionText = new Phrase("Section " + currentSection.Label + " - " + currentSection.Title + "\n",fonts["TNR12Bold"]);
			HeaderParagraph.Add(SectionText);
			if (currentPart != null)
			{
				Phrase partText = new Phrase("Part " + currentPart.Label + " - " + currentPart.Description,fonts["TNR12Bold"]);
				HeaderParagraph.Add(partText);
			}
			else
			{
				HeaderParagraph.Add(new Phrase("\n",fonts["TNR12Bold"]));
			}
			doc.Add(HeaderParagraph);

			PdfPTable table = new PdfPTable(4);
			table.TotalWidth = questionsTableWidth;
			table.LockedWidth = true;
			float[] widths = new float[] { 3, 19, 5, 14 };
			table.SetWidths(widths);
			table.HeaderRows = 1;
			table.SplitLate = true;
			table.SplitRows = false;
			table.SpacingBefore = 10;

			string[] headerStrings = new string[] { "No.", "Item of Interest", "Score", "Remarks" };
			for (int i = 0; i < headerStrings.Length; i++)
			{
				Phrase phrase = new Phrase(headerStrings[i],fonts["TNR12Bold"]);
				PdfPCell cell = new PdfPCell(phrase);
				if (i != 0)
				{
					cell.DisableBorderSide(iTextSharp.text.Rectangle.LEFT_BORDER);
				}
				if (i != headerStrings.Length - 1)
				{
					cell.DisableBorderSide(iTextSharp.text.Rectangle.RIGHT_BORDER);
				}
				cell.BackgroundColor = headerBackgroundColor;
				cell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
				table.AddCell(cell);
			}

			//loop through questions
			int numberOfPlacedQuestions = 0;
			float maximumHeight = 580;
			while(true)
			{
				int questionIndex = nextQuestionIndex + numberOfPlacedQuestions;
				Question question = questions.ElementAt(questionIndex);

				Paragraph numberCellText = new Paragraph();
				if (question.part != null)
				{
					numberCellText.Add(new Phrase("Part " + question.part.Label + "\n", fonts["TNR10Bold"]));
				}
				numberCellText.Add(new Phrase(question.numberString, fonts["TNR10Bold"]));
				PdfPCell numberCell = new PdfPCell(numberCellText );
				numberCell.BackgroundColor = headerBackgroundColor;
				numberCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
				numberCell.Rowspan = 2;

				PdfPCell questionCell = new PdfPCell(new Phrase(question.Text.Trim(), fonts["TNR10"]));
				questionCell.DisableBorderSide(Rectangle.BOTTOM_BORDER);

				ScoredQuestion score = inspection.GetScoreForQuestion(question);
				PdfPCell scoreCell;
				if (score == null)
				{
					scoreCell = new PdfPCell();
				}
				else
				{
					scoreCell = new PdfPCell(new Phrase(score.answer.ToString(), fonts["TNR10"]));
					scoreCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
				}

				PdfPCell referenceCell = new PdfPCell();
				referenceCell.PaddingBottom = 4;
				AddReferences(referenceCell, question.References);
				referenceCell.MinimumHeight = 10;
				referenceCell.DisableBorderSide(Rectangle.TOP_BORDER);

				questionCell.MinimumHeight = question.HasSubItems ? 20 : 40;
				if (table.TotalHeight + questionCell.Height + referenceCell.Height > maximumHeight)
				{	//There is not enough room for this question.  Return!
					break;
				}

				table.AddCell(numberCell);
				table.AddCell(questionCell);

				if (question.HasSubItems)
				{
					PdfPCell leftPlaceholder = new PdfPCell();
					PdfPCell rightPlaceholder = new PdfPCell();
					rightPlaceholder.BackgroundColor = headerBackgroundColor;
					leftPlaceholder.BackgroundColor = headerBackgroundColor;
					leftPlaceholder.Rowspan = 2;
					rightPlaceholder.Rowspan = 2;
					leftPlaceholder.DisableBorderSide(Rectangle.RIGHT_BORDER);
					rightPlaceholder.DisableBorderSide(Rectangle.LEFT_BORDER);
					table.AddCell(leftPlaceholder);
					table.AddCell(rightPlaceholder);
				}
				else
				{
					PdfPCell remarksCell = new PdfPCell(new Phrase(question.Remarks, fonts["TNR10"]));
					scoreCell.Rowspan = 2;
					remarksCell.Rowspan = 2;
					table.AddCell(scoreCell);
					table.AddCell(remarksCell);
				}
				table.AddCell(referenceCell);

				numberOfPlacedQuestions++;
				if (questionIndex == questions.Count - 1)
				{
					break;
				}
			}
			doc.Add(table);
			
			AddPageFooter();
			return nextQuestionIndex + numberOfPlacedQuestions;
		}
		internal void AddReferences(PdfPCell cell, List<Reference> references)
		{
			foreach (Reference reference in references)
			{
				Paragraph paragraph = new Paragraph(9);
				paragraph.Add(new Chunk("(", fonts["TNR9"]));
				paragraph.Add(new Chunk(reference.Description.Trim(), fonts["TNR9Blue"]));
				paragraph.Add(new Chunk(")", fonts["TNR9"]));
				cell.AddElement(paragraph);
			}
		}
		public void CreateSectionTotals()
		{
			isEmpty = false;
			AddPageHeader();
			PdfPTable table = new PdfPTable(8);
			table.TotalWidth = questionsTableWidth;
			table.LockedWidth = true;
			float[] widths = new float[] { 6, 36, 12, 10, 10, 12, 9, 12 };
			table.SetWidths(widths);
			table.SpacingBefore = 20;
			List<PdfPCell> headerCells = new List<PdfPCell>();

			PdfPCell topCell = new PdfPCell(new Phrase("Section Scores", fonts["TNR16Bold"]));
			topCell.Colspan = 8;
			PdfPCell emptyCell1 = new PdfPCell();
			emptyCell1.Colspan = 8;
			emptyCell1.MinimumHeight = 5;

			PdfPCell headerTitleCell = new PdfPCell(new Phrase("Section Title",fonts["TNR12Bold"]));
			headerTitleCell.Colspan = 2;
			headerTitleCell.DisableBorderSide(Rectangle.RIGHT_BORDER);
			headerCells.Add(headerTitleCell);

			PdfPCell headerAvailableCell = new PdfPCell(new Phrase("Available Points", fonts["TNR12Bold"]));
			headerAvailableCell.DisableBorderSide(Rectangle.RIGHT_BORDER);
			headerAvailableCell.DisableBorderSide(Rectangle.LEFT_BORDER);
			headerCells.Add(headerAvailableCell);

			PdfPCell headerEarnedCell = new PdfPCell(new Phrase("Earned Points", fonts["TNR12Bold"]));
			headerEarnedCell.DisableBorderSide(Rectangle.RIGHT_BORDER);
			headerEarnedCell.DisableBorderSide(Rectangle.LEFT_BORDER);
			headerCells.Add(headerEarnedCell);

			PdfPCell headerScoreCell = new PdfPCell(new Phrase("Score (%)", fonts["TNR12Bold"]));
			headerScoreCell.DisableBorderSide(Rectangle.RIGHT_BORDER);
			headerScoreCell.DisableBorderSide(Rectangle.LEFT_BORDER);
			headerCells.Add(headerScoreCell);

			PdfPCell headerRatingsCell = new PdfPCell(new Phrase("Section Ratings", fonts["TNR12Bold"]));
			headerRatingsCell.Colspan = 3;
			headerRatingsCell.DisableBorderSide(Rectangle.LEFT_BORDER);
			headerCells.Add(headerRatingsCell);

			table.AddCell(topCell);
			table.AddCell(emptyCell1);
			foreach (PdfPCell cell in headerCells)
			{
				cell.MinimumHeight = 20;
				cell.BackgroundColor = headerBackgroundColor;
				cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
				table.AddCell(cell);
			}

			foreach (SectionModel section in inspection.Checklist.Sections)
			{
				AddSectionScoreRow(section, table);				
			}
			PdfPCell emptyCell2 = new PdfPCell();
			emptyCell2.Colspan = 8;
			emptyCell2.MinimumHeight = 20;
			table.AddCell(emptyCell2);

			headerCells = new List<PdfPCell>();
			PdfPCell emptyCell3 = new PdfPCell();
			emptyCell3.DisableBorderSide(Rectangle.RIGHT_BORDER);
			emptyCell3.Colspan = 2;
			headerCells.Add(emptyCell3);
			headerCells.Add(headerAvailableCell);
			headerCells.Add(headerEarnedCell);

			PdfPCell headerTotalCell = new PdfPCell(new Phrase("Total Score", fonts["TNR12Bold"]));
			headerTotalCell.DisableBorderSide(Rectangle.LEFT_BORDER);
			headerTotalCell.DisableBorderSide(Rectangle.RIGHT_BORDER);
			headerCells.Add(headerTotalCell);

			PdfPCell headerOverallCell = new PdfPCell(new Phrase("Overall Rating", fonts["TNR12Bold"]));
			headerOverallCell.DisableBorderSide(Rectangle.LEFT_BORDER);
			headerOverallCell.Colspan = 3;
			headerCells.Add(headerOverallCell);

			foreach (PdfPCell cell in headerCells)
			{
				cell.BackgroundColor = headerBackgroundColor;
				cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
				table.AddCell(cell);
			}

			PdfPCell cumulativeCell = new PdfPCell(new Phrase("Cumulative Score", fonts["TNR11Bold"]));
			cumulativeCell.Colspan = 2;
			cumulativeCell.DisableBorderSide(Rectangle.RIGHT_BORDER);

			PdfPCell availableCell = new PdfPCell(new Phrase(inspection.availablePoints.ToString(), fonts["TNR11"]));
			availableCell.DisableBorderSide(Rectangle.LEFT_BORDER);
			availableCell.DisableBorderSide(Rectangle.RIGHT_BORDER);
			availableCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;

			PdfPCell earnedCell = new PdfPCell(new Phrase(inspection.earnedPoints.ToString(), fonts["TNR11"]));
			earnedCell.DisableBorderSide(Rectangle.LEFT_BORDER);
			earnedCell.DisableBorderSide(Rectangle.RIGHT_BORDER);
			earnedCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;

			PdfPCell scoreCell;
			if (inspection.availablePoints == 0)
			{
				scoreCell = new PdfPCell(new Phrase("N/A",fonts["TNR11"]));
			}
			else
			{
				scoreCell = new PdfPCell(new Phrase(PercentString(inspection.percentage)));
			}			
			scoreCell.DisableBorderSide(Rectangle.LEFT_BORDER);
			scoreCell.DisableBorderSide(Rectangle.RIGHT_BORDER);
			scoreCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;

			PdfPCell uCell = new ColorCodedCell(redColor, "U", ColorCodedCell.ScoreCellSide.Left, inspection.rating == Rating.Unacceptable);
			PdfPCell sCell = new ColorCodedCell(greenColor, "S", ColorCodedCell.ScoreCellSide.Center, inspection.rating == Rating.Satisfactory);
			PdfPCell cCell = new ColorCodedCell(blueColor, "C", ColorCodedCell.ScoreCellSide.Right, inspection.rating == Rating.Commendable);

			table.AddCell(cumulativeCell);
			table.AddCell(availableCell);
			table.AddCell(earnedCell);
			table.AddCell(scoreCell);
			table.AddCell(uCell);
			table.AddCell(sCell);
			table.AddCell(cCell);

			doc.Add(table);

			AddPageFooter();
		}
		internal void AddSectionScoreRow(SectionModel section, PdfPTable table)
		{
			List<PdfPCell> sectionCells = new List<PdfPCell>();
			PdfPCell labelCell = new PdfPCell(new Phrase(section.Label,fonts["TNR11Bold"]));
			labelCell.DisableBorderSide(Rectangle.RIGHT_BORDER);
			labelCell.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
			sectionCells.Add(labelCell);

			PdfPCell titleCell = new PdfPCell(new Phrase(section.Title,fonts["TNR11Bold"]));
			titleCell.DisableBorderSide(Rectangle.RIGHT_BORDER);
			titleCell.DisableBorderSide(Rectangle.LEFT_BORDER);
			sectionCells.Add(titleCell);

			PdfPCell availableCell = new PdfPCell(new Phrase(section.availablePoints.ToString(),fonts["TNR11"]));
			availableCell.DisableBorderSide(Rectangle.RIGHT_BORDER);
			availableCell.DisableBorderSide(Rectangle.LEFT_BORDER);
			availableCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
			sectionCells.Add(availableCell);

			PdfPCell earnedCell = new PdfPCell(new Phrase(section.earnedPoints.ToString(), fonts["TNR11"]));
			earnedCell.DisableBorderSide(Rectangle.RIGHT_BORDER);
			earnedCell.DisableBorderSide(Rectangle.LEFT_BORDER);
			earnedCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
			sectionCells.Add(earnedCell);

			PdfPCell scoreCell;
			if (section.availablePoints == 0)
			{
				scoreCell = new PdfPCell(new Phrase("N/A", fonts["TNR11"]));
			}
			else
			{
				scoreCell = new PdfPCell(new Phrase(PercentString(section.percentage), fonts["TNR11"]));
			}	
			scoreCell.DisableBorderSide(Rectangle.RIGHT_BORDER);
			scoreCell.DisableBorderSide(Rectangle.LEFT_BORDER);
			scoreCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
			sectionCells.Add(scoreCell);

			PdfPCell uCell = new ColorCodedCell(redColor, "U", ColorCodedCell.ScoreCellSide.Left, section.rating == Rating.Unacceptable);
			PdfPCell sCell = new ColorCodedCell(greenColor, "S", ColorCodedCell.ScoreCellSide.Center, section.rating == Rating.Satisfactory);
			PdfPCell cCell = new ColorCodedCell(blueColor, "C", ColorCodedCell.ScoreCellSide.Right, section.rating == Rating.Commendable);

			sectionCells.Add(uCell);
			sectionCells.Add(sCell);
			sectionCells.Add(cCell);

			foreach (PdfPCell cell in sectionCells)
			{
				cell.MinimumHeight = 20;
				cell.DisableBorderSide(Rectangle.TOP_BORDER);
				cell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
				table.AddCell(cell);
			}
		}
		public void CreateStructure()
		{
			isEmpty = false;
			AddPageHeader();
			Paragraph title = new Paragraph("Checklist Structure",fonts["TNR11Bold"]);
			title.IndentationLeft = 30;
			it.List list = new it.List();
			list.SetListSymbol(string.Empty);
			list.IndentationLeft = 30;
			foreach (SectionModel section in inspection.Checklist.Sections)
			{
				it.ListItem sectionName = new ListItem("Section " + section.Label + ": " + section.Title,fonts["TNR10"]);
				list.Add(sectionName);
				ListItem LastItem;
				//list.Add("Section " + section.Label + ": " + section.Title);
				if (section.SectionParts.Any())
				{
					it.List partList = new it.List();
					partList.SetListSymbol(string.Empty);
					partList.IndentationLeft = 15;
					foreach (SectionPart part in section.SectionParts)
					{
						partList.Add(new ListItem("Part "+part.Label + ": " + part.Description, fonts["TNR10"]));
					}
					LastItem = (ListItem)partList.Items[partList.Items.Count - 1];
					list.Add(partList);
				}
				else
				{
					LastItem = sectionName;
				}
				LastItem.SpacingAfter = 5;
			}
			doc.Add(title);
			doc.Add(list);
			AddPageFooter();
		}
		public void CreateScoreSheet()
		{
			isEmpty = false;
			AddPageHeader();
			float maxHeight = 612;
			float currentHeight = 0;
			foreach (SectionModel section in inspection.Checklist.Sections)
			{
				PdfPTable table = SectionScoreTable(section);
				if (currentHeight + table.TotalHeight > maxHeight)
				{
					AddPageFooter();
					currentHeight = 0;
					NewPage();
					AddPageHeader();
				}
				doc.Add(table);
				currentHeight += table.TotalHeight;
			}
			AddPageFooter();
		}
		internal PdfPTable SectionScoreTable(SectionModel section)
		{
			PdfPTable table = new PdfPTable(6);

			table.TotalWidth = 559;
			table.LockedWidth = true;
			float[] widths = new float[] { 14, 32, 17, 19, 19, 16 };
			table.SetWidths(widths);
			table.SpacingBefore = 5;

			PdfPCell LabelCell = new PdfPCell(new Phrase("Section: " + section.Label,fonts["TNR12Bold"]));
			PdfPCell TitleCell = new PdfPCell(new Phrase("    "+section.Title, fonts["TNR12Bold"]));
			TitleCell.Colspan = 5;
			LabelCell.Border = Rectangle.NO_BORDER;
			TitleCell.Border = Rectangle.NO_BORDER;

			PdfPCell HeaderPartCell = new PdfPCell(new Phrase("Part",fonts["TNR10Bold"]));
			HeaderPartCell.MinimumHeight = 22;
			PdfPCell HeaderDescriptionCell = new PdfPCell(new Phrase("Description", fonts["TNR10Bold"]));
			HeaderDescriptionCell.Colspan = 2;
			PdfPCell HeaderAvailableCell = new PdfPCell(new Phrase("Available Points", fonts["TNR10Bold"]));
			PdfPCell HeaderEarnedCell = new PdfPCell(new Phrase("Earned Points", fonts["TNR10Bold"]));
			PdfPCell HeaderScoreCell = new PdfPCell(new Phrase("Score", fonts["TNR10Bold"]));
			HeaderAvailableCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
			HeaderEarnedCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
			HeaderScoreCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
			HeaderPartCell.BackgroundColor = headerBackgroundColor;
			HeaderDescriptionCell.BackgroundColor = headerBackgroundColor;
			HeaderAvailableCell.BackgroundColor = headerBackgroundColor;
			HeaderEarnedCell.BackgroundColor = headerBackgroundColor;
			HeaderScoreCell.BackgroundColor = headerBackgroundColor;
			HeaderPartCell.DisableBorderSide(Rectangle.RIGHT_BORDER);
			HeaderDescriptionCell.DisableBorderSide(Rectangle.LEFT_BORDER);
			HeaderDescriptionCell.DisableBorderSide(Rectangle.RIGHT_BORDER);
			HeaderAvailableCell.DisableBorderSide(Rectangle.LEFT_BORDER);
			HeaderAvailableCell.DisableBorderSide(Rectangle.RIGHT_BORDER);
			HeaderEarnedCell.DisableBorderSide(Rectangle.LEFT_BORDER);
			HeaderEarnedCell.DisableBorderSide(Rectangle.RIGHT_BORDER);
			HeaderScoreCell.DisableBorderSide(Rectangle.LEFT_BORDER);

			table.AddCell(LabelCell);
			table.AddCell(TitleCell);
			table.AddCell(HeaderPartCell);
			table.AddCell(HeaderDescriptionCell);
			table.AddCell(HeaderAvailableCell);
			table.AddCell(HeaderEarnedCell);
			table.AddCell(HeaderScoreCell);

			if (section.SectionParts.Any())
			{
				foreach (SectionPart part in section.SectionParts)
				{
					if (part.availablePoints != 0)
					{
						ScoreSheetLine("Part: " + part.Label, part.Description, part.availablePoints, part.earnedPoints, part.percentage, table, part.rating);
					}
					else
					{
						ScoreSheetLine("Part: " + part.Label, part.Description, 0, 0, 0, table, Rating.None);
					}
				}
			}
			else
			{
				if (section.availablePoints != 0)
				{
					ScoreSheetLine("Section: " + section.Label, "", section.availablePoints, section.earnedPoints, section.percentage,table, section.rating);
				}
				else
				{
					ScoreSheetLine("Section: " + section.Label, "", 0, 0, 0, table, Rating.None);
				}
			}
			PdfPCell placeholderCell = new PdfPCell();
			placeholderCell.Colspan = 2;
			placeholderCell.DisableBorderSide(Rectangle.LEFT_BORDER);
			placeholderCell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
			PdfPCell totalCell = new PdfPCell(new Phrase("Section Totals", fonts["TNR11BoldItalic"]));
			totalCell.DisableBorderSide(Rectangle.RIGHT_BORDER);
			PdfPCell availableCell = new PdfPCell(new Phrase(section.availablePoints.ToString(), fonts["TNR11BoldItalic"]));
			availableCell.DisableBorderSide(Rectangle.LEFT_BORDER);
			availableCell.DisableBorderSide(Rectangle.RIGHT_BORDER);
			availableCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
			PdfPCell earnedCell = new PdfPCell(new Phrase(section.earnedPoints.ToString(), fonts["TNR11BoldItalic"]));
			earnedCell.DisableBorderSide(Rectangle.LEFT_BORDER);
			earnedCell.DisableBorderSide(Rectangle.RIGHT_BORDER);
			earnedCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
			PdfPCell ScoreCell;
			if (section.availablePoints > 0)
			{
				it.Color color;
				switch (section.rating)
				{
				case Rating.Unacceptable:
					color = redColor;
					break;
				case Rating.Satisfactory:
					color = greenColor;
					break;
				case Rating.Commendable:
					color = blueColor;
					break;
				default:
					color = it.Color.WHITE;
					break;
				}
				Phrase percentPhrase = new Phrase(PercentString(section.percentage), new Font(Font.TIMES_ROMAN, 11, Font.BOLDITALIC,color));
				ScoreCell = new PdfPCell(percentPhrase);
			}
			else
			{
				ScoreCell = new PdfPCell(new Phrase("Not Scored", fonts["TNR11BoldItalic"]));
			}
			ScoreCell.DisableBorderSide(Rectangle.LEFT_BORDER);
			ScoreCell.EnableBorderSide(Rectangle.RIGHT_BORDER);
			ScoreCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;

			table.AddCell(placeholderCell);
			table.AddCell(totalCell);
			table.AddCell(availableCell);
			table.AddCell(earnedCell);
			table.AddCell(ScoreCell);

			return table;
		}
		internal void ScoreSheetLine(string label, string description, double available, double earned, double score, PdfPTable table, Rating rating)
		{
			PdfPCell LabelCell = new PdfPCell(new Phrase(label, fonts["TNR10"]));
			LabelCell.Border = Rectangle.NO_BORDER;
			LabelCell.MinimumHeight = 22;
			LabelCell.EnableBorderSide(Rectangle.LEFT_BORDER);
			PdfPCell DescriptionCell = new PdfPCell(new Phrase(description, fonts["TNR10"]));
			DescriptionCell.Border = Rectangle.NO_BORDER;
			DescriptionCell.Colspan = 2;
			PdfPCell AvailableCell = new PdfPCell(new Phrase(available.ToString(), fonts["TNR10"]));
			AvailableCell.Border = Rectangle.NO_BORDER;
			AvailableCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
			PdfPCell EarnedCell = new PdfPCell(new Phrase(earned.ToString(), fonts["TNR10"]));
			EarnedCell.Border = Rectangle.NO_BORDER;
			EarnedCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
			PdfPCell ScoreCell;
			if (available == 0)
			{
				ScoreCell = new ColorCodedCell(it.Color.WHITE, "", ColorCodedCell.ScoreCellSide.Wide, true);
				ScoreCell = new PdfPCell(new Phrase("", fonts["TNR10"]));
			}
			else
			{
				it.Color color;
				switch (rating)
				{
				case Rating.Unacceptable:
					color = redColor;
					break;
				case Rating.Satisfactory:
					color = greenColor;
					break;
				case Rating.Commendable:
					color = blueColor;
					break;
				default:
					color = it.Color.WHITE;
					break;
				}
				ScoreCell = new ColorCodedCell(color, PercentString(score), ColorCodedCell.ScoreCellSide.Wide, true);
				//ScoreCell = new PdfPCell(new Phrase(PercentString((double)score), fonts["TNR10"]));
			}
			ScoreCell.Border = Rectangle.NO_BORDER;
			ScoreCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
			ScoreCell.EnableBorderSide(Rectangle.RIGHT_BORDER);

			table.AddCell(LabelCell);
			table.AddCell(DescriptionCell);
			table.AddCell(AvailableCell);
			table.AddCell(EarnedCell);
			table.AddCell(ScoreCell);
		}

		public void CreateScoreGraph()
		{
			isEmpty = false;
			AddPageHeader();
			float graphBottom = 504;
			float graphTop = 670.5F;
			float graphLeft = 94.5F;
			float graphRight = 549;

			float graphHeight = graphTop - graphBottom;
			float oneFifth = graphHeight / 5F;
			float sectionWidth = (graphRight - graphLeft) / (float)inspection.Checklist.Sections.Count;
			float sectionQuarter = sectionWidth / 4;

			cb.Rectangle(36, 369, 531, 333);
			cb.Stroke();

			cb.MoveTo(graphLeft, graphBottom);
			cb.LineTo(graphRight, graphBottom);
			cb.Stroke();

			cb.MoveTo(graphLeft, graphBottom);
			cb.LineTo(graphLeft, graphTop);
			cb.Stroke();

			cb.SetLineWidth(.2F);
			cb.MoveTo(graphRight, graphBottom);
			cb.LineTo(graphRight, graphTop);
			cb.Stroke();

			BaseFont baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
			BaseFont boldFont = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false);
			cb.SetFontAndSize(baseFont, 7);
			cb.BeginText();
			cb.ShowTextAligned(it.Element.ALIGN_RIGHT, "0", graphLeft - 2, graphBottom - 2, 0);
			cb.EndText();

			for (int i = 1; i <= 5; i++)
			{
				float lineY = graphBottom + i * oneFifth;
				cb.MoveTo(graphLeft, lineY);
				cb.LineTo(graphRight, lineY);
				cb.Stroke();

				cb.BeginText();
				cb.ShowTextAligned(it.Element.ALIGN_RIGHT, (20 * (i)).ToString(), graphLeft - 2, lineY - 2, 0);
				cb.EndText();
			}

			it.Color[] colorOptions = new it.Color[8];
			colorOptions[0] = it.Color.BLUE;
			colorOptions[1] = it.Color.RED;
			colorOptions[2] = it.Color.GREEN;
			colorOptions[3] = it.Color.ORANGE;
			colorOptions[4] = it.Color.YELLOW;
			colorOptions[5] = it.Color.MAGENTA;
			colorOptions[6] = it.Color.CYAN;
			colorOptions[7] = it.Color.PINK;
			for (int i = 0, colorIndex = 0; i < inspection.Checklist.Sections.Count; i++, colorIndex %= colorOptions.Length)
			{
				SectionModel section = inspection.Checklist.Sections.ElementAt(i);
				float score = (float)section.percentage;
				float left = graphLeft + sectionWidth * i + sectionQuarter;

				StringBuilder XLabel = new StringBuilder(section.Label + " : " + section.Title);
				cb.BeginText();
				cb.ShowTextAligned(it.Element.ALIGN_RIGHT, XLabel.ToString(), left + sectionQuarter, graphBottom - 5, 37);
				cb.EndText();

				if (score <= 0)
				{
					continue;
				}
				float height = graphHeight * score;

				cb.SetColorFill(colorOptions[colorIndex]);
				cb.Rectangle(left, graphBottom + .2F, sectionQuarter * 2, height);
				cb.Fill();
				colorIndex++;

				cb.ResetRGBColorFill();
			}
			cb.SetFontAndSize(boldFont, 8);
			cb.BeginText();
			cb.ShowTextAligned(it.Element.ALIGN_CENTER, "Score Range (%)", graphLeft - 20, graphBottom + (graphHeight / 2), 90);
			cb.ShowTextAligned(it.Element.ALIGN_CENTER, "Sections", (graphLeft + graphRight) / 2F, graphBottom - 80, 0);
			cb.EndText();
			AddPageFooter();
		}

		internal string PercentString(double percent)
		{
			return (percent * 100).ToString("###.##") + "%";
		}
	}

	internal class ColorCodedCell : PdfPCell
	{
		Font selectedFont = new Font(Font.TIMES_ROMAN, 11, Font.BOLD, it.Color.WHITE);
		Font nonSelectedFont = new Font(Font.TIMES_ROMAN, 11);
		it.Color nonSelectedBackgroundColor = new it.Color(218, 227, 232);
		public ColorCodedCell(it.Color SelectedBackgroundColor, string text, ScoreCellSide side, bool selected)
		{
			PdfPTable table = new PdfPTable(3);
			table.LockedWidth = true;
			float[] widths = new float[3];
			switch (side)
			{
			case ScoreCellSide.Left:
				table.TotalWidth = GeneratePdf.questionsTableWidth * 12.0F / GeneratePdf.totalQuestionsTableSegments;
				widths[0] = 7;
				widths[1] = 14;
				widths[2] = 3;
				DisableBorderSide(Rectangle.RIGHT_BORDER);
				DisableBorderSide(Rectangle.LEFT_BORDER);
				HorizontalAlignment = ALIGN_RIGHT;
				break;
			case ScoreCellSide.Center:
				table.TotalWidth = GeneratePdf.questionsTableWidth * 9.0F / GeneratePdf.totalQuestionsTableSegments;
				widths[0] = 2F;
				widths[1] = 14;
				widths[2] = 2F;
				DisableBorderSide(Rectangle.RIGHT_BORDER);
				DisableBorderSide(Rectangle.LEFT_BORDER);
				HorizontalAlignment = ALIGN_CENTER;
				break;
			case ScoreCellSide.Right:
				table.TotalWidth = GeneratePdf.questionsTableWidth * 12.0F / GeneratePdf.totalQuestionsTableSegments;
				widths[0] = 3;
				widths[1] = 15;
				widths[2] = 7;
				DisableBorderSide(Rectangle.LEFT_BORDER);
				HorizontalAlignment = ALIGN_LEFT;
				break;
			case ScoreCellSide.Wide:
				table.TotalWidth = 612 * 16F / 117F;
				widths[0] = 2;
				widths[1] = 14;
				widths[2] = 2;
				DisableBorderSide(Rectangle.LEFT_BORDER);
				HorizontalAlignment = ALIGN_CENTER;
				break;
			}
			table.SetWidths(widths);

			PdfPCell topEmptyCell = new PdfPCell();
			//topEmptyCell.MinimumHeight = 1;
			topEmptyCell.Border = Rectangle.NO_BORDER;
			PdfPCell middleEmptyCell = new PdfPCell();
			//middleEmptyCell.MinimumHeight = 10;
			middleEmptyCell.Border = Rectangle.NO_BORDER;
			PdfPCell bottomEmptyCell = new PdfPCell();
			//bottomEmptyCell.MinimumHeight = 1;
			bottomEmptyCell.Border = Rectangle.NO_BORDER;

			table.AddCell(topEmptyCell);
			table.AddCell(topEmptyCell);
			table.AddCell(topEmptyCell);
			table.AddCell(middleEmptyCell);

			PdfPCell mainCell;
			if (selected)
			{
				Phrase textPhrase = new Phrase(new Chunk(text, selectedFont));
				mainCell = new PdfPCell(textPhrase);
				mainCell.BackgroundColor = SelectedBackgroundColor;
			}
			else
			{
				Phrase textPhrase = new Phrase(new Chunk(text, nonSelectedFont));
				mainCell = new PdfPCell(textPhrase);
				mainCell.BackgroundColor = nonSelectedBackgroundColor;
			}
			mainCell.HorizontalAlignment = ALIGN_CENTER;
			mainCell.Border = Rectangle.NO_BORDER;
			table.AddCell(mainCell);

			table.AddCell(middleEmptyCell);
			table.AddCell(bottomEmptyCell);
			table.AddCell(bottomEmptyCell);
			table.AddCell(bottomEmptyCell);

			AddElement(table);
		}
		public enum ScoreCellSide
		{
			Left,
			Center,
			Right,
			Wide
		}
	}
}

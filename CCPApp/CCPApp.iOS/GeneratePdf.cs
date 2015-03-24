using CCPApp.iOS;
using CCPApp.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Font = iTextSharp.text.Font;
using Rectangle = iTextSharp.text.Rectangle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Drawing;
using Xamarin.Forms;

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
		Section currentSection;
		SectionPart currentPart;

		public void Initialize(Inspection inspection, bool scoredOnly = false)
		{
			this.inspection = inspection;
			string fileName = "Report.pdf";
			doc = new Document(PageSize.LETTER);

			string privatePath = new FileManage().GetLibraryFolder();
			string fullPath = Path.Combine(privatePath, fileName);
			FileStream stream = new FileStream(fullPath, System.IO.FileMode.Create);

			writer = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, stream);
			doc.Open();
			cb = writer.DirectContent;

			fonts["TNR12Bold"] = new Font(Font.TIMES_ROMAN, 12, Font.BOLD);
			fonts["TNR12"] = new Font(Font.TIMES_ROMAN, 12);
			fonts["TNR10"] = new Font(Font.TIMES_ROMAN, 10);
			fonts["TNR10Bold"] = new Font(Font.TIMES_ROMAN, 10, Font.BOLD);
			fonts["TNR9"] = new Font(Font.TIMES_ROMAN, 9);
			fonts["TNR9Blue"] = new Font(Font.TIMES_ROMAN, 9, Font.NORMAL,iTextSharp.text.Color.BLUE);

			doc.SetMargins(63, 63, 10, doc.BottomMargin);
		}
		public void NewPage()
		{
			doc.NewPage();
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
			return doc.PageNumber == 0;
		}

		internal void DrawPlaceholder()
		{
			cb.SetCMYKColorStroke(1, 1, 1, 1);
			cb.Rectangle(0, 0, 1, 1);
			cb.Stroke();
			cb.SetCMYKColorStroke(0, 0, 0, 0);
		}

		internal void AddPageHeader()
		{
			Paragraph header = new Paragraph();
			Font boldTimes = fonts["TNR12Bold"];

			Chunk inspectionName = new Chunk(inspection.Name + "\n",boldTimes);
			Chunk orgName = new Chunk("Organization\n\n",boldTimes);
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
			cb.Rectangle(85.5F, 96.75F, 459, 639);
			cb.Stroke();
			cb.BeginText();
			BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
			cb.SetFontAndSize(bf, 12);
			cb.MoveText(86,723);
			cb.ShowText(comment.CommentText);
			cb.EndText();
		}
		public void CreateQuestionSection(ReportSection section)
		{
			if (!section.PartsToRender.Any())
			{
				QuestionPages(section.section.Questions);
			}
			else
			{
				foreach (SectionPart part in section.PartsToRender)
				{
					CreateQuestionPart(part);
					if (part != section.PartsToRender.Last())
					{
						NewPage();
					}
				}
			}
			//Section score thingy.
			NewPage();
		}
		internal void CreateQuestionPart(SectionPart part)
		{
			QuestionPages(part.Questions);
			//Part score thingy.
		}
		internal void QuestionPages(List<Question> questions){
			bool keepGoing = true;
			int nextIndex = 0;
			while (keepGoing)
			{
				nextIndex = QuestionTablePage(questions, nextIndex);
				if (nextIndex >= questions.Count)
				{
					keepGoing = false;
				}
				else
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
				//Loop through questions until it would take up too much space.
				//Need to figure out what the "bottom" is that I can't go past.

			//table setup.
			PdfPTable table = new PdfPTable(4);
			table.TotalWidth = 500;
			table.LockedWidth = true;
			float[] widths = new float[] { 3, 19, 5, 14 };
			table.SetWidths(widths);
			iTextSharp.text.Color headerBackgroundColor = iTextSharp.text.Color.LIGHT_GRAY;
			headerBackgroundColor = new iTextSharp.text.Color(211, 211, 211);
			table.HeaderRows = 1;
			table.SplitLate = true;
			table.SplitRows = false;
			table.SpacingBefore = 50;

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
				int index = nextQuestionIndex + numberOfPlacedQuestions;
				Question question = questions[index];

				PdfPCell numberCell = new PdfPCell(new Phrase(question.ToString(), fonts["TNR10Bold"]));
				numberCell.BackgroundColor = headerBackgroundColor;
				numberCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
				numberCell.DisableBorderSide(Rectangle.BOTTOM_BORDER);

				PdfPCell questionCell = new PdfPCell(new Phrase(question.Text.Trim(), fonts["TNR10"]));
				questionCell.DisableBorderSide(Rectangle.BOTTOM_BORDER);

				PdfPCell scoreCell = new PdfPCell();
				PdfPCell remarksCell = new PdfPCell();
				if (question.HasSubItems)
				{
					questionCell.MinimumHeight = 0;
					scoreCell.BackgroundColor = headerBackgroundColor;
					scoreCell.DisableBorderSide(Rectangle.RIGHT_BORDER);
					remarksCell.BackgroundColor = headerBackgroundColor;
					remarksCell.DisableBorderSide(Rectangle.LEFT_BORDER);
				}
				else
				{
					questionCell.MinimumHeight = 50;
					//put in the score and remarks.
				}
				scoreCell.DisableBorderSide(Rectangle.BOTTOM_BORDER);
				remarksCell.DisableBorderSide(Rectangle.BOTTOM_BORDER);

				PdfPCell referenceCell = new PdfPCell();
				AddReferences(referenceCell, question.References);
				referenceCell.MinimumHeight = 10;
				referenceCell.DisableBorderSide(Rectangle.TOP_BORDER);

				if (table.TotalHeight + questionCell.Height + referenceCell.Height > maximumHeight)
				{	//There is not enough room for this question.  Return!
					break;
				}

				PdfPCell greyPlaceholder = new PdfPCell();
				greyPlaceholder.BackgroundColor = headerBackgroundColor;
				greyPlaceholder.DisableBorderSide(Rectangle.TOP_BORDER);
				PdfPCell placeholderCell = new PdfPCell();
				placeholderCell.DisableBorderSide(Rectangle.TOP_BORDER);

				table.AddCell(numberCell);
				table.AddCell(questionCell);
				table.AddCell(scoreCell);
				table.AddCell(remarksCell);

				table.AddCell(greyPlaceholder);
				table.AddCell(referenceCell);
				if (question.HasSubItems)
				{
					PdfPCell leftPlaceholder = new PdfPCell();
					PdfPCell rightPlaceholder = new PdfPCell();
					leftPlaceholder.DisableBorderSide(Rectangle.TOP_BORDER);
					rightPlaceholder.DisableBorderSide(Rectangle.TOP_BORDER);
					leftPlaceholder.DisableBorderSide(Rectangle.RIGHT_BORDER);
					rightPlaceholder.DisableBorderSide(Rectangle.LEFT_BORDER);
					leftPlaceholder.BackgroundColor = headerBackgroundColor;
					rightPlaceholder.BackgroundColor = headerBackgroundColor;

					table.AddCell(leftPlaceholder);
					table.AddCell(rightPlaceholder);
				}
				else
				{
					table.AddCell(placeholderCell);
					table.AddCell(placeholderCell);
				}

				numberOfPlacedQuestions++;
				if (index == questions.Count - 1)
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
	}
}

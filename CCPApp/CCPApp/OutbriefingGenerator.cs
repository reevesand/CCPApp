using CCPApp.Models;
using CCPApp.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CCPApp
{
	public class OutbriefingList
    {
        public string FontFamily = "Arial";
        public float FontSize = 18;
        public float RowSpacing = 12;

        public class Column
        {
            public float Width;
            public string FontFamily = string.Empty;
            public float FontSize = 0;
            public bool Italic = false;

            public bool LineSpacer = false;

            public string Title;

            public Column(float width_arg, string title_arg)
            {
                Width = width_arg;
                Title = title_arg;
            }
        }

        public class Row
        {
            public class Cell
            {
                public string Value;
                public Color Color;

                public Cell(string value_arg)
                {
                    Value = value_arg;
                    Color = Color.Transparent;
                }

                public Cell(string value_arg, Color color_arg)
                {
                    Value = value_arg;
                    Color = color_arg;
                }
            }

            public List<Cell> Cells = new List<Cell>();

            public Row(Cell[] cells_arg)
            {
                foreach (Cell cell in cells_arg)
                    Cells.Add(cell);
            }

            public Row(string[] values_arg)
            {
                foreach (string value in values_arg)
                    Cells.Add(new Cell(value));
            }
        }

        public List<Column> Columns = new List<Column>();
        public List<Row> Rows = new List<Row>();

        public int AddToSlide(APptSlide slide, bool show_headers, PointF position, int start_row_index)
        {
            float max_y = slide.Height - 80;
            float y = position.Y;
            int rows_drawn = 0;

            // create column headers
            if (show_headers)
            {
                float x = position.X;
                float max_height = 0.0f;

                foreach (Column column in Columns)
                {
                    // create header label
                    APptLabel label = slide.CreateLabel();
                    label.Left = x;
                    label.Top = y;
					label.WordWrap = true;
                    label.FontFamily = FontFamily;
                    label.FontSize = FontSize;
                    label.Bold = true;
					label.Text = column.Title;
					label.Top = y;
					label.Width = column.Width;
					label.Height = label.ActualHeight + 5;

                    x += column.Width;

                    if (label.ActualHeight > max_height)
                        max_height = label.ActualHeight;
                }

                y += max_height + RowSpacing;
            }

            // create row value labels
            for (int row_index = start_row_index; row_index < Rows.Count; row_index++)
            {
                Row row = Rows[row_index];

                float x = position.X;
                float max_height = 0.0f;

                for (int i = 0; i < Columns.Count; i++)
                {
                    Column column = Columns[i];

                    // create value label
                    APptLabel label = slide.CreateLabel();
                    label.Left = x;
                    label.Top = y;
					label.WordWrap = true;
                    label.FontFamily = (column.FontFamily.Length != 0) ? column.FontFamily : FontFamily;
                    label.FontSize = (column.FontSize != 0.0f) ? column.FontSize : FontSize;
                    label.Bold = true;
                    label.Italic = column.Italic;
                    if (row.Cells[i].Color != Color.Transparent)
                        label.color = row.Cells[i].Color;
					label.Text = row.Cells[i].Value;
					label.Top = y;
					label.Width = column.Width;
					label.Height = label.ActualHeight + 5;

                    // create line to fill space between the end of the text and the end of the row column
                    if (column.LineSpacer)
                    {
                        PointF p1 = new PointF(x + label.ActualWidth + 15, y + (label.ActualHeight * 3 / 4));
                        PointF p2 = new PointF(x + column.Width, y + (label.ActualHeight * 3 / 4));
                        APptLine line = slide.CreateLine(p1, p2);
                        line.Weight = 1.0f;
                        line.Dashed = true;
                    }

                    x += column.Width;

                    if (label.ActualHeight > max_height)
                        max_height = label.ActualHeight;
                }

                rows_drawn++;

                y += max_height + RowSpacing;
                if (y >= max_y)
                    break;
            }

            return rows_drawn;
        }

        public int AddToSlide(APptSlide slide, bool show_headers, PointF position)
        {
            return AddToSlide(slide, show_headers, position, 0);
        }
    }

    public class OutbriefingGenerator
    {
        protected int SlideCount = 0;
        protected DateTime m_Date = DateTime.Today;

		protected Inspection inspection = null;
        protected string Background = string.Empty;
        protected Color ForeColor = Color.Black;

        public OutbriefingGenerator(Inspection inspection, string Background, Color ForeColor)
        {
            this.inspection = inspection;
            this.Background = Background;
            this.ForeColor = ForeColor;
        }

        public void Generate(bool ScoredOnly)
        {
			IPptGenerator ppt = DependencyService.Get<IPptGenerator>();

            AddFrontSlide(ppt);
            AddTitleSlide(ppt);
            AddSectionListSlide(ppt, ScoredOnly);
            //AddSectionScoresSlide(ppt, ScoredOnly);
			//AddInspectionGraphSlide(ppt, ScoredOnly);
			//AddCriticalQuestionSlides(ppt, ScoredOnly);//TODO make sure this looks okay
            //AddCommentSheetSlides(ppt, 0, 0);
			//AddSectionSlides(ppt, ScoredOnly);
            //AddEndSlide(ppt);
        }

        protected APptSlide CreateTestSlide(IPptGenerator ppt)
        {
            APptSlide slide = CreateSlide(ppt);
            APptLabel label;

            float top = 100;
            for (int i = 0; i < 5; i++)
            {
                label = slide.CreateLabel();
                label.Top = top;
                label.Width = 50;
                label.Text = "test " + i.ToString() + " test test test test test test";

                top += label.Height;
            }

            return slide;
        }

        protected APptSlide CreateSlide(IPptGenerator ppt)
        {
            SlideCount++;

            APptSlide slide = ppt.CreateSlide();
            slide.Background = "background"; //TODO backgrounds? //TODO fix this
            slide.ForeColor = ForeColor;
            slide.FontFamily = "Arial";
            slide.FontSize = 16;

            return slide;
        }

        protected void AddTitleToSlide(APptSlide slide, string title)
        {
			//don't do anything, that title looks stupid.
			return;
            /*if (title.Length != 0)
            {
                // create title shadow label
                PptLabel label = slide.CreateLabel();
                label.Left = slide.InchesToPixels(-0.03f) + 1;
                label.Top = slide.InchesToPixels(0.1f) + 1;
                label.FontFamily = "Times New Roman";
                label.FontSize = 22;
                label.Bold = true;
                label.Color = Color.FromArgb(95, 95, 95);
				label.WordWrap = true;
				label.Text = title;
				label.Top = slide.InchesToPixels(0.1f) + 1;
				label.Width = slide.Width - label.Left;
				label.Height = label.ActualHeight + 5;

                // create title label
                label = slide.CreateLabel();
                label.Left = slide.InchesToPixels(-0.03f);
                label.Top = slide.InchesToPixels(0.1f);
                label.Width = slide.Width - label.Left;
                label.FontFamily = "Times New Roman";
                label.FontSize = 22;
                label.Bold = true;
                label.Color = Color.FromArgb(221, 221, 221);
                label.Text = title;
            }*/
        }

        public void AddSubtitleToSlide(APptSlide slide, string subtitle)
        {
            // create subtitle label
            APptLabel label = slide.CreateLabel();
			label.Left = slide.Width / 2;
            label.Top = slide.InchesToPixels(1.2f);
            label.Width = slide.Width;
            label.Height = slide.InchesToPixels(.44f);
            label.FontFamily = "Times New Roman";
            label.FontSize = 32;
            label.Bold = true;
            label.TextAlign = APptLabel.Alignment.Center;
            label.TextVAlign = APptLabel.VAlignment.Bottom;
            label.Text = subtitle;

            // create line below subtitle
            PointF p1 = new PointF(slide.InchesToPixels(1.17f), slide.InchesToPixels(1.79f));
            PointF p2 = new PointF(slide.InchesToPixels(8.92f), slide.InchesToPixels(1.79f));
            slide.CreateLine(p1, p2);
        }

        public void AddLogoToSlide(APptSlide slide)
        {
            // add logo
            /*(if (Properties.Settings.Default.LogoFilename.Length > 0)
            {
                try
                {
                    Image img = Image.FromFile(Program.MapPath(Properties.Settings.Default.LogoFilename));
                    SizeF size = ScaleWithAspectRatio(new SizeF(slide.InchesToPixels(0.9f), slide.InchesToPixels(1.25f)), new SizeF((float)img.Width, (float)img.Height));

                    APptSlide.Image logo = slide.CreateImage(Program.MapPath(Properties.Settings.Default.LogoFilename), slide.InchesToPixels(8.97f), slide.InchesToPixels(0.17f), size.Width, size.Height);
                }
                catch (Exception) { }
            }*/
        }

        public void AddFooterToSlide(APptSlide slide, bool show_inspection_title, bool show_slide_number)
        {
            // create line
            PointF p1 = new PointF(slide.InchesToPixels(0.08f), slide.InchesToPixels(7.17f));
            PointF p2 = new PointF(slide.Width - (p1.X * 2.0f), slide.InchesToPixels(7.17f));
            slide.CreateLine(p1, p2);

            if (show_inspection_title)
            {
                // create inspection title label
                APptLabel label = slide.CreateLabel();
                label.Left = slide.InchesToPixels(0.5f);
                label.Top = slide.InchesToPixels(7.17f);
                label.FontFamily = "Times New Roman";
                label.FontSize = 12;
                label.Bold = true;
                label.Text = inspection.Name;
            }

            if (show_slide_number)
            {
                // create slide number label
                APptLabel label = slide.CreateLabel();
                label.Left = slide.InchesToPixels(7.17f);
                label.Top = slide.InchesToPixels(7.17f);
                label.Width = slide.InchesToPixels(2.33f);
                label.TextAlign = APptLabel.Alignment.Right;
                label.FontFamily = "Times New Roman";
                label.FontSize = 12;
                label.Bold = true;
                label.Text = "Slide " + SlideCount.ToString();
            }
        }

        protected void AddCenteredTextToSlide(APptSlide slide, string text)
        {
            // create top line
            APptLine line = slide.CreateLine(new PointF(slide.InchesToPixels(2.0f), slide.InchesToPixels(1.91f)), new PointF(slide.InchesToPixels(8.0f), slide.InchesToPixels(1.91f)));
            line.style = APptLine.StyleType.ThickThin;
            line.Weight = 4.5f;

            // create bottom line
            line = slide.CreateLine(new PointF(slide.InchesToPixels(2.0f), slide.InchesToPixels(3.58f)), new PointF(slide.InchesToPixels(8.0f), slide.InchesToPixels(3.58f)));
            line.style = APptLine.StyleType.ThinThick;
            line.Weight = 4.5f;

            // create text label
            APptLabel label = slide.CreateLabel();
            label.Top = slide.InchesToPixels(2.08f);
            label.Height = slide.InchesToPixels(1.7f);
			label.Left = 0;
            label.FontFamily = "Times New Roman";
            label.FontSize = 32;
            label.Bold = true;
            label.TextAlign = APptLabel.Alignment.Center;
            label.TextVAlign = APptLabel.VAlignment.Bottom;
			label.WordWrap = true;
			label.Text = text;
			label.Top = slide.InchesToPixels(2.08f);
			label.Width = slide.Width;
			label.Height = label.ActualHeight + 5;
        }

        // similar to AddCenteredTextToSlide, just with slightly different lines
        protected void AddCenteredTextToSlide2(APptSlide slide, string text)
        {
            // create top line
            APptLine line = slide.CreateLine(new PointF(slide.InchesToPixels(1.0f), slide.InchesToPixels(2.17f)), new PointF(slide.InchesToPixels(8.92f), slide.InchesToPixels(2.17f)));
            line.style = APptLine.StyleType.ThickThin;
            line.Weight = 4.5f;

            // create bottom line
            line = slide.CreateLine(new PointF(slide.InchesToPixels(1.0f), slide.InchesToPixels(3.83f)), new PointF(slide.InchesToPixels(8.92f), slide.InchesToPixels(3.83f)));
            line.style = APptLine.StyleType.ThinThick;
            line.Weight = 4.5f;

            // create text label
            APptLabel label = slide.CreateLabel();
            label.Top = slide.InchesToPixels(2.34f);
            label.Height = slide.InchesToPixels(1.66f);
			label.Left = 0;
            label.FontFamily = "Times New Roman";
            label.FontSize = 32;
            label.Bold = true;
            label.TextAlign = APptLabel.Alignment.Center;
            label.TextVAlign = APptLabel.VAlignment.Middle;
			label.WordWrap = true;
			label.Text = text;
			label.Top = slide.InchesToPixels(2.34f);
			label.Width = slide.Width;
			label.Height = label.ActualHeight + 5;
        }

        protected void AddFrontSlide(IPptGenerator ppt)
        {
            // get unit name
            string unit = inspection.Organization;

            // create slide
            APptSlide slide = CreateSlide(ppt);
            AddCenteredTextToSlide(slide, unit);

            // create date label
            APptLabel date = slide.CreateLabel();
            date.Left = slide.InchesToPixels(5.75f);
            date.Top = slide.InchesToPixels(3.58f);
            date.Width = slide.InchesToPixels(2.33f);
            date.TextAlign = APptLabel.Alignment.Right;
            date.FontFamily = "Times New Roman";
            date.FontSize = 18;
            date.Text = FormatDate(m_Date);

            // add logo TODO
            /*if (Properties.Settings.Default.LogoFilename.Length > 0)
            {
                try
                {
                    Image img = Image.FromFile(Program.MapPath(Properties.Settings.Default.LogoFilename));
                    SizeF size = ScaleWithAspectRatio(new SizeF(slide.InchesToPixels(1.58f), slide.InchesToPixels(1.75f)), new SizeF((float)img.Width, (float)img.Height));

                    APptSlide.Image logo = slide.CreateImage(Program.MapPath(Properties.Settings.Default.LogoFilename), (slide.Width - size.Width) / 2, slide.InchesToPixels(4.0f), size.Width, size.Height);
                }
                catch (Exception) { }
            }*/
        }

        protected string FormatDate(DateTime d)
        {
            return d.ToString("MMMM d, yyyy");
        }

        protected void AddTitleSlide(IPptGenerator ppt)
        {
            // create slide
            APptSlide slide = CreateSlide(ppt);
            AddCenteredTextToSlide2(slide, inspection.Name);

			// create inspector intro label
			APptLabel label = slide.CreateLabel();
			label.Left = slide.InchesToPixels(1.5f);
			label.Top = slide.InchesToPixels(4.0f);
			//label.Width = slide.Width - label.Left - (label.Left / 2);
			label.FontSize = 22;
			label.Bold = true;
			label.Text = "By:";
			//top += label.ActualHeight + 5;

			// create inspector names label
			label = slide.CreateLabel();
			label.FontSize = 22;
			label.Bold = true;
			label.WordWrap = true;
			//label.Text = GetInspector();
			label.Left = slide.InchesToPixels(2.1f);
			label.Top = slide.InchesToPixels(4.0f);
			label.Width = slide.Width - label.Left;
			label.Height = label.ActualHeight;

            AddFooterToSlide(slide, false, true);
        }

        protected void AddSectionListSlide(IPptGenerator ppt, bool ScoredOnly)
        {
            // create list
            OutbriefingList list = new OutbriefingList();

            // create list columns
            OutbriefingList.Column column = new OutbriefingList.Column(595, string.Empty);
            column.LineSpacer = false;
            list.Columns.Add(column);
            column = new OutbriefingList.Column(5, string.Empty);
            column.FontFamily = "Times New Roman";
            column.Italic = true;
            list.Columns.Add(column);

            // create list rows
			foreach (SectionModel section in inspection.Checklist.Sections)
			{
				bool displayThisSection = true;
				if (ScoredOnly)
				{
					Tuple<double,double,double> scores = ScoringHelper.ScoreSection(section, inspection);

					displayThisSection = (scores.Item1 > 0);
				}

				if (displayThisSection)
				{
					string sectionName = "Section " + section.Label + ": " + section.Title;
					string inspector = String.Empty;// GetSectionInspector((int)sections["ID"]);
					list.Rows.Add(new OutbriefingList.Row(new string[] { sectionName, inspector }));
				}
			}

            int row_count = 0;
            while (row_count < list.Rows.Count)
            {
                // create slide
                APptSlide slide = CreateSlide(ppt);
                AddTitleToSlide(slide, "Checklist Overview");
                AddLogoToSlide(slide);
                AddSubtitleToSlide(slide, "Section Listing" + (row_count == 0 ? string.Empty : " (continued)"));
                AddFooterToSlide(slide, true, true);

                // add list to slide
                PointF position = new PointF(slide.InchesToPixels(0.83f), slide.InchesToPixels(2.12f));
                row_count += list.AddToSlide(slide, false, position, row_count);
            }
        }

        protected void AddSectionScoresSlide(IPptGenerator ppt, bool ScoredOnly)
        {
            // create list
            OutbriefingList list = new OutbriefingList();

            // create list columns
            OutbriefingList.Column column = new OutbriefingList.Column(475, string.Empty);
            column.LineSpacer = true;
            list.Columns.Add(column);
            column = new OutbriefingList.Column(75, string.Empty);
            column.FontFamily = "Times New Roman";
            column.Italic = true;
            list.Columns.Add(column);

            // get overall score
			Tuple<double, double, double> inspectionScores = ScoringHelper.ScoreInspection(inspection);
			double score = inspectionScores.Item3 * 100;
			//int score = Database.GetScoreNA(m_BranchID, m_InspectionID);

            bool allow_commendable = true;
            if (ScoringHelper.AnyUnsatisfactorySections(inspection))
                allow_commendable = false;

            // add overall score to the list
            list.Rows.Add(new OutbriefingList.Row(new OutbriefingList.Row.Cell[] { new OutbriefingList.Row.Cell("Overall Score:"), new OutbriefingList.Row.Cell((score == -1) ? "N/A" : (score.ToString() + "%"), ScoringHelper.GetScoreColor(score, inspection.Checklist,allow_commendable)) }));

            // get sections
            //OleDbCommand cmd = new OleDbCommand("SELECT SectionId AS [ID], SectionLabel AS [Label], Desc FROM [Section] ORDER BY SectionSort", Database.Connection);
            //OleDbDataReader sections = cmd.ExecuteReader();

            // add section scores to list
			foreach (SectionModel section in inspection.Checklist.Sections)
			{
				string sectionName = "Section " + section.Label + ": " + section.Title;

				// get section score
				Tuple<double, double, double> sectionScores = ScoringHelper.ScoreSection(section,inspection);
				score = sectionScores.Item3;
				double availablePoints = sectionScores.Item1;
				if (availablePoints == 0 && ScoredOnly)
				{
					continue;
				}
				list.Rows.Add(new OutbriefingList.Row(new OutbriefingList.Row.Cell[] { new OutbriefingList.Row.Cell(sectionName), new OutbriefingList.Row.Cell((score == -1) ? "N/A" : (score.ToString() + "%"), ScoringHelper.GetScoreColor(score,inspection.Checklist)) }));
			}

            int row_count = 0;
            while (row_count < list.Rows.Count)
            {
                // create slide
                APptSlide slide = CreateSlide(ppt);
                AddTitleToSlide(slide, "Checklist Overview");
                AddLogoToSlide(slide);
                AddSubtitleToSlide(slide, "Checklist and Section Scores" + (row_count == 0 ? string.Empty : " (continued)"));
                AddFooterToSlide(slide, true, true);

                // add list to slide
                PointF position = new PointF(slide.InchesToPixels(0.83f), slide.InchesToPixels(2.12f));
                row_count += list.AddToSlide(slide, false, position, row_count);
            }
        }/*

        protected void AddInspectionGraphSlide(IPptGenerator ppt, bool ScoredOnly)
        {
            // create slide
            APptSlide slide = CreateSlide(ppt);
            AddTitleToSlide(slide, "Checklist Overview");
            AddLogoToSlide(slide);
            AddSubtitleToSlide(slide, "Scoring");
            AddFooterToSlide(slide, true, true);

            // create graph
            string filename = Program.MapTempPath("InspectionGraph.png");
            ScoreGraph graph = new ScoreGraph();
            graph.BranchID = m_BranchID;
            graph.InspectionID = m_InspectionID;
            graph.Width = (int)slide.InchesToPixels(9.5f);
            graph.Height = (int)slide.InchesToPixels(4.67f);
            graph.Generate(ScoredOnly).Save(filename);

            // add graph to slide
            PointF position = new PointF(slide.InchesToPixels(0.25f), slide.InchesToPixels(2.0f));
            slide.CreateImage(filename, position.X, position.Y);
        }

        protected void AddCriticalQuestionSlides(IPptGenerator ppt, bool ScoredOnly)
        {
            // get critical questions
            string sql = "SELECT [qryCriticalQuestionsFailed].*, [Section].[Desc] AS [SectionDesc] FROM [qryCriticalQuestionsFailed]";
            sql += " INNER JOIN [Section] ON [Section].[SectionId] = [qryCriticalQuestionsFailed].[SectionId]";
            sql += " WHERE [qryCriticalQuestionsFailed].[InspectionID] = @InspectionID";

            OleDbCommand cmd = new OleDbCommand(sql, Database.Connection);
            cmd.Parameters.AddWithValue("@InspectionID", m_InspectionID);

            OleDbDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                // create slide
                APptSlide slide = CreateSlide(ppt);
                AddTitleToSlide(slide, "Checklist Overview");
                AddLogoToSlide(slide);
                AddSubtitleToSlide(slide, "Critical Questions Affecting Section\r\nScores");
                AddFooterToSlide(slide, true, true);

                float top = slide.InchesToPixels(2.12f);

                // create section label
                APptLabel label = slide.CreateLabel();
                label.Left = slide.InchesToPixels(0.83f);
                label.Top = top;
                label.Width = slide.Width - label.Left - (label.Left / 2);
                label.FontSize = 22;
                label.Bold = true;
                label.Underline = true;
                label.Text = "Section " + reader["SectionLabel"].ToString() + ": " + reader["SectionDesc"].ToString();
                top += label.ActualHeight + 5;

                // create question number label
                label = slide.CreateLabel();
                label.Left = slide.InchesToPixels(0.83f);
                label.Top = top;
                label.Width = slide.Width - label.Left - (label.Left / 2);
                label.FontSize = 18;
                label.Bold = true;
				string partLabel = reader["PartLabel"].ToString();
				if (partLabel == String.Empty)
				{
					label.Text = "Question " + reader["SectionLabel"].ToString() + "-" + reader["QuestionId"].ToString() + reader["QuesSub1Id"].ToString();
				}
				else
				{
					label.Text = "Question " + reader["PartLabel"].ToString() + "-" + reader["QuestionId"].ToString() + reader["QuesSub1Id"].ToString();
				}                
                top += label.ActualHeight + 5;

                // create question text label
                label = slide.CreateLabel();
                label.Left = slide.InchesToPixels(0.83f) + 15;
                label.Top = top;
                label.Height = slide.InchesToPixels(3.75f);
                label.FontFamily = "Times New Roman";
                label.FontSize = 18;
                label.Bold = true;
                label.Italic = true;
				label.WordWrap = true;
				label.Text = reader["Question"].ToString();
				label.Top = top;
				label.Width = slide.Width - label.Left - (label.Left / 2);
				label.Height = label.ActualHeight + 5;
            }
            reader.Close();
        }

        protected void AddSectionSlides(IPptGenerator ppt, bool ScoredOnly)
        {
            // get sections
            OleDbCommand cmd = new OleDbCommand("SELECT [SectionId], [SectionLabel], [Desc] FROM [Section] ORDER BY [SectionSort]", Database.Connection);

            OleDbDataReader sections = cmd.ExecuteReader();
            while (sections.Read())
            {
				//uncomment to only print section slides if there are comment sheets or parts
                //if (NumCommentSheets((int)sections["SectionId"], 0) == 0 && NumPartsInSection((int)sections["SectionId"]) == 0)
                //    continue;

				if (ScoredOnly)
				{
					cmd = new OleDbCommand("SELECT count(1) FROM [Score] INNER JOIN [Question] ON [Question].[QuestionSequence] = [Score].[QuestionSequence] WHERE [Score].[Scored] <> 0 AND [Score].[Disputed] = 0 AND [Score].[InspectionID] = @InspectionID AND [Question].[SectionID] = @SectionID AND [Question].[ScorableInd] <> 0", Database.Connection);
					cmd.Parameters.AddWithValue("@InspectionID", m_InspectionID);
					cmd.Parameters.AddWithValue("@SectionID", sections["SectionId"].ToString());
					int NumQuestionsAnswered = (int)cmd.ExecuteScalar();
					if (NumQuestionsAnswered == 0)
					{
						continue;
					}
				}

                string title = "Section " + sections["SectionLabel"].ToString() + ":\r\n" + sections["Desc"].ToString();

                // create section beginning slide
                APptSlide slide = CreateSlide(ppt);
                AddLogoToSlide(slide);
                AddCenteredTextToSlide2(slide, title);
                AddFooterToSlide(slide, true, true);

                AddSectionPartScoresSlide(ppt, (int)sections["SectionId"]);
                AddCommentSheetSlides(ppt, (int)sections["SectionId"], 0);

                // get parts in this section
                cmd = new OleDbCommand("SELECT [PartID], [PartLabel], [PartDescription] FROM [SectionParts] WHERE [SectionId] = @SectionID ORDER BY [PartSort]", Database.Connection);
                cmd.Parameters.AddWithValue("@SectionID", (int)sections["SectionId"]);

                OleDbDataReader parts = cmd.ExecuteReader();
                while (parts.Read())
                {
                    if (NumCommentSheets((int)sections["SectionId"], (int)parts["PartID"]) == 0)
                        continue;

                    title = "Part " + parts["PartLabel"].ToString() + ":\r\n" + parts["PartDescription"].ToString();

                    // create section part beginning slide
                    slide = CreateSlide(ppt);
                    AddLogoToSlide(slide);
                    AddCenteredTextToSlide2(slide, title);
                    AddFooterToSlide(slide, true, true);

                    AddCommentSheetSlides(ppt, (int)sections["SectionId"], (int)parts["PartID"]);
                }
                parts.Close();
            }
            sections.Close();
        }

        protected void AddSectionPartScoresSlide(IPptGenerator ppt, int SectionID)
        {
            OleDbCommand cmd = new OleDbCommand("SELECT [SectionLabel], [Desc] FROM [Section] WHERE [SectionId] = @SectionID", Database.Connection);
            cmd.Parameters.AddWithValue("@SectionID", SectionID);

            OleDbDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                // get section info
                string title = "Section " + reader["SectionLabel"].ToString() + ": " + reader["Desc"].ToString();

                // get section score
                int section_score = Database.GetScoreNA(m_BranchID, m_InspectionID, SectionID);

                // set the score to 50 if it's currently greater than 50 and any critical questions have been failed,
                if (section_score > 50 && Database.CriticalQuestionFailure(m_BranchID, m_InspectionID, SectionID, 0))
                    section_score = 50;

                // create list
                OutbriefingList list = new OutbriefingList();

                // create list columns
                OutbriefingList.Column column = new OutbriefingList.Column(475, string.Empty);
                column.LineSpacer = true;
                list.Columns.Add(column);
                column = new OutbriefingList.Column(75, string.Empty);
                column.FontFamily = "Times New Roman";
                column.Italic = true;
                list.Columns.Add(column);

                // get section parts
                cmd = new OleDbCommand("SELECT PartID AS [ID], PartLabel AS [Label], PartDescription AS [Desc] FROM [SectionParts] WHERE [SectionId] = @SectionID ORDER BY PartSort", Database.Connection);
                cmd.Parameters.AddWithValue("@SectionID", SectionID);
                OleDbDataReader parts = cmd.ExecuteReader();

                // add section part scores to list
                while (parts.Read())
                {
                    string part = "Part " + parts["Label"].ToString() + ": " + parts["Desc"].ToString();
                    int score = Database.GetScoreNA(m_BranchID, m_InspectionID, SectionID, (int)parts["ID"]);

                    list.Rows.Add(new OutbriefingList.Row(new OutbriefingList.Row.Cell[] { new OutbriefingList.Row.Cell(part), new OutbriefingList.Row.Cell((score == -1) ? "N/A" : (score.ToString() + "%"), Database.GetScoreColor(score)) }));
                }
                parts.Close();

                APptSlide slide = null;
                int row_count = 0;
				if (list.Rows.Count == 0)
				{	//no parts, so create an empty score screen.
					slide = CreateSlide(ppt);
                    AddLogoToSlide(slide);
                    AddTitleToSlide(slide, title);
                    AddFooterToSlide(slide, true, true);

                    // create section description label
                    APptLabel label = slide.CreateLabel();
                    label.Left = slide.InchesToPixels(0.83f);
                    label.Top = slide.InchesToPixels(0.7f);
                    label.Height = slide.InchesToPixels(1.77f);
                    label.FontFamily = "Times New Roman";
                    label.FontSize = 32.0f;
                    label.Bold = true;
                    label.TextVAlign = APptLabel.VAlignment.Bottom;
					label.WordWrap = true;
					label.Text = reader["Desc"].ToString();
					label.Top = slide.InchesToPixels(0.7f);
					label.Width = slide.InchesToPixels(7.0f);
					label.Height = label.ActualHeight + 5;

                    // create section score label
                    label = slide.CreateLabel();
                    label.Left = slide.InchesToPixels(7.67f);
					label.Top = slide.InchesToPixels(0.7f);
                    label.Width = slide.InchesToPixels(1.35f);
                    label.Height = slide.InchesToPixels(1.77f);
                    label.FontFamily = "Times New Roman";
                    label.FontSize = 32.0f;
                    label.Bold = true;
                    label.TextAlign = APptLabel.Alignment.Right;
                    label.TextVAlign = APptLabel.VAlignment.Bottom;
                    label.color = Database.GetScoreColor(section_score);
					label.Text = (section_score == -1) ? "N/A" : (section_score.ToString() + "%");
					label.Left = slide.InchesToPixels(7.67f);
					label.Top = slide.InchesToPixels(0.7f);

                    // create line below description/score
                    PointF p1 = new PointF(slide.InchesToPixels(1.17f), slide.InchesToPixels(1.83f));
                    PointF p2 = new PointF(slide.InchesToPixels(8.92f), slide.InchesToPixels(1.83f));
                    slide.CreateLine(p1, p2);
				}
                while (row_count < list.Rows.Count)
                {
                    // create slide
                    slide = CreateSlide(ppt);
                    AddLogoToSlide(slide);
                    AddTitleToSlide(slide, title);
                    AddFooterToSlide(slide, true, true);

                    // create section description label
                    APptLabel label = slide.CreateLabel();
					label.Left = slide.InchesToPixels(0.83f);
					label.Top = slide.InchesToPixels(0.7f);
                    label.Height = slide.InchesToPixels(1.77f);
                    label.FontFamily = "Times New Roman";
                    label.FontSize = 32.0f;
                    label.Bold = true;
                    label.TextVAlign = APptLabel.VAlignment.Bottom;
					label.WordWrap = true;
					label.Text = (row_count == 0) ? reader["Desc"].ToString() : (reader["Desc"].ToString() + " (continued)");
					label.Top = slide.InchesToPixels(0.7f);
					label.Width = slide.InchesToPixels(7.0f);
					label.Height = label.ActualHeight + 5;

                    // create section score label
                    label = slide.CreateLabel();
					label.Left = slide.InchesToPixels(7.67f);
					label.Top = slide.InchesToPixels(0.7f);
                    label.Width = slide.InchesToPixels(1.35f);
                    label.Height = slide.InchesToPixels(1.77f);
                    label.FontFamily = "Times New Roman";
                    label.FontSize = 32.0f;
                    label.Bold = true;
                    label.TextAlign = APptLabel.Alignment.Right;
                    label.TextVAlign = APptLabel.VAlignment.Bottom;
                    label.color = Database.GetScoreColor(section_score);
					label.Text = (section_score == -1) ? "N/A" : (section_score.ToString() + "%");
					label.Top = slide.InchesToPixels(0.7f);
					label.Left = slide.InchesToPixels(7.67f);

                    // create line below description/score
                    PointF p1 = new PointF(slide.InchesToPixels(1.17f), slide.InchesToPixels(1.83f));
                    PointF p2 = new PointF(slide.InchesToPixels(8.92f), slide.InchesToPixels(1.83f));
                    slide.CreateLine(p1, p2);

                    // add list to slide
                    PointF position = new PointF(slide.InchesToPixels(0.83f), slide.InchesToPixels(2.12f));
                    row_count += list.AddToSlide(slide, false, position, row_count);
                }
            }
            reader.Close();
        }

        protected int NumPartsInSection(int SectionID)
        {
            string sql = "SELECT count(1) FROM [SectionParts] WHERE [SectionId] = @SectionID";

            OleDbCommand cmd = new OleDbCommand(sql, Database.Connection);
            cmd.Parameters.AddWithValue("@SectionID", SectionID);

            return (int)cmd.ExecuteScalar();
        }

        protected string FormatCommentSheetText(string text)
        {
            const int max_text_length = 150;

            text = text.Replace("\r", " ").Replace("\n", " ");
            if (text.Length > max_text_length)
                text = text.Substring(0, max_text_length) + "…";

            return text;
        }

        protected int NumCommentSheets(int SectionID, int SectionPartID)
        {
            string sql = "SELECT count(1) FROM [Coversheet] WHERE [Coversheet].[InspectionID] = @InspectionID";

            if (SectionID != 0)
            {
                sql += " AND (([Coversheet].[QuestionSequence] <= 0 AND [Coversheet].[Section] = " + SectionID.ToString() + ")";
                sql += " OR [Coversheet].[QuestionSequence] IN (SELECT [Question].[QuestionSequence] FROM [Question] WHERE [Question].[SectionId] = " + SectionID.ToString();
                sql += " AND [Question].[SectionPartID] = " + SectionPartID.ToString() + "))";
            }
            else
            {
                sql += " AND ([Coversheet].[QuestionSequence] <= 0 AND [Coversheet].[Section] <= 0)";
            }

            // get comment sheet count
            OleDbCommand cmd = new OleDbCommand(sql, Database.Connection);
            cmd.Parameters.AddWithValue("@InspectionID", m_InspectionID);

            return (int)cmd.ExecuteScalar();
        }

        protected void AddCommentSheetSlides(IPptGenerator ppt, int SectionID, int SectionPartID)
        {
            string sql = "SELECT [Coversheet].* FROM [Coversheet] WHERE [Coversheet].[InspectionID] = @InspectionID";

            if (SectionID != 0)
            {
                sql += " AND (([Coversheet].[QuestionSequence] <= 0 AND [Coversheet].[Section] = " + SectionID.ToString() + ")";
                sql += " OR [Coversheet].[QuestionSequence] IN (SELECT [Question].[QuestionSequence] FROM [Question] WHERE [Question].[SectionId] = " + SectionID.ToString();
                sql += " AND [Question].[SectionPartID] = " + SectionPartID.ToString() + "))";
            }
            else
            {
                sql += " AND ([Coversheet].[QuestionSequence] <= 0 AND [Coversheet].[Section] <= 0)";
            }
            sql += " ORDER BY [SheetType]";

            // get comment sheets
            OleDbCommand cmd = new OleDbCommand(sql, Database.Connection);
            cmd.Parameters.AddWithValue("@InspectionID", m_InspectionID);

            OleDbDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                // create slide
                APptSlide slide = CreateSlide(ppt);
                AddLogoToSlide(slide);
                AddSubtitleToSlide(slide, reader["SheetType"].ToString() + "s");
                AddFooterToSlide(slide, true, true);

                APptLabel label;
                float top = slide.InchesToPixels(1.85f);

                // create question labels, if necessary
                if ((int)reader["QuestionSequence"] > 0)
                {
                    sql = "SELECT [Question].[SectionId], [Question].[SectionPartID], [Section].[SectionLabel], [Question].[QuestionId], [Question].[QuesSub1Id], [Question].[Question]";
                    sql += " FROM [Question] INNER JOIN [Section] ON [Section].[SectionId] = [Question].[SectionId]";
                    sql += " WHERE [QuestionSequence] = @QuestionSequence";

                    // get question
                    cmd = new OleDbCommand(sql, Database.Connection);
                    cmd.Parameters.AddWithValue("@QuestionSequence", (int)reader["QuestionSequence"]);

                    OleDbDataReader question = cmd.ExecuteReader();
                    if (question.Read())
                    {
                        AddTitleToSlide(slide, GetSectionTitle((int)question["SectionId"], (int)question["SectionPartID"]));

                        string tmp = "Question: " + question["QuestionId"].ToString() + question["QuesSub1Id"].ToString();

                        // create question number label
                        label = slide.CreateLabel();
                        label.Left = slide.InchesToPixels(0.83f);
                        label.Top = top;
                        label.Width = slide.Width - (label.Left * 2.0f);
                        label.FontFamily = "Times New Roman";
                        label.FontSize = 18.0f;
                        label.Bold = true;
                        label.Italic = true;
                        label.Underline = true;
                        label.Text = tmp;
                        top += label.ActualHeight + 10;

                        // create question text label
                        label = slide.CreateLabel();
                        label.Left = slide.InchesToPixels(1.23f);
                        label.Top = top;
                        label.FontFamily = "Times New Roman";
                        label.FontSize = 18.0f;
                        label.Bold = true;
                        label.Italic = true;
						label.WordWrap = true;
						label.Text = FormatCommentSheetText(question["Question"].ToString());
						label.Top = top;
						label.Width = slide.Width - (label.Left * 2.0f);
						label.Height = label.ActualHeight + 5;
                        top += label.ActualHeight + 10;
                    }
                    question.Close();
                }
                else
                {
                    if ((int)reader["Section"] > 0)
                        AddTitleToSlide(slide, GetSectionTitle((int)reader["Section"], 0));
                    else
                        AddTitleToSlide(slide, "Checklist Overview");
                }

                // create finding/commendable/observation caption label
                label = slide.CreateLabel();
                label.Left = slide.InchesToPixels(0.83f);
                label.Top = top;
                label.Width = slide.Width - (label.Left * 2.0f);
                label.FontSize = 16.0f;
                label.Bold = true;
                label.Color = Color.FromArgb(128, 0, 0);
                label.Text = "•    " + reader["SheetType"].ToString() + "s:";
                top += label.ActualHeight + 10;

                // create finding/commendable/observation text label
                label = slide.CreateLabel();
                label.Left = slide.InchesToPixels(1.23f);
                label.Top = top;
                label.FontSize = 14.0f;
                label.Bold = true;
				label.WordWrap = true;
				label.Text = FormatCommentSheetText(reader["Observation"].ToString());
				label.Top = top;
				label.Width = slide.Width - (label.Left * 2.0f);
				label.Height = label.ActualHeight + 5;
                top += label.ActualHeight + 30;

                // create discussion caption label
                label = slide.CreateLabel();
                label.Left = slide.InchesToPixels(0.83f);
                label.Top = top;
                label.Width = slide.Width - (label.Left * 2.0f);
                label.FontSize = 16.0f;
                label.Bold = true;
                label.Color = Color.FromArgb(128, 0, 0);
                label.Text = "•    Discussion:";
                top += label.ActualHeight + 10;

                // create discussion text label
                label = slide.CreateLabel();
                label.Left = slide.InchesToPixels(1.23f);
                label.Top = top;
                label.FontSize = 14.0f;
                label.Bold = true;
				label.WordWrap = true;
				label.Text = FormatCommentSheetText(reader["Discussion"].ToString());
				label.Top = top;
				label.Width = slide.Width - (label.Left * 2.0f);
				label.Height = label.ActualHeight + 5;
                top += label.ActualHeight + 30;

                // create recommendation caption label
                label = slide.CreateLabel();
                label.Left = slide.InchesToPixels(0.83f);
                label.Top = top;
                label.Width = slide.Width - (label.Left * 2.0f);
                label.FontSize = 16.0f;
                label.Bold = true;
                label.Color = Color.FromArgb(128, 0, 0);
                label.Text = "•    Recommendation:";
                top += label.ActualHeight + 10;

                // create recommendation text label
                label = slide.CreateLabel();
                label.Left = slide.InchesToPixels(1.23f);
                label.Top = top;
                label.FontSize = 14.0f;
                label.Bold = true;
				label.WordWrap = true;
				label.Text = FormatCommentSheetText(reader["Recommendation"].ToString());
				label.Top = top;
				label.Width = slide.Width - (label.Left * 2.0f);
				label.Height = label.ActualHeight + 5;
                top += label.ActualHeight + 30;
            }
            reader.Close();
        }

        protected void AddEndSlide(IPptGenerator ppt)
        {
            // create slide
            APptSlide slide = CreateSlide(ppt);
            AddCenteredTextToSlide(slide, "QUESTIONS?");

            // create date label
            APptLabel date = slide.CreateLabel();
            date.Left = slide.InchesToPixels(5.75f);
            date.Top = slide.InchesToPixels(3.58f);
            date.Width = slide.InchesToPixels(2.33f);
            date.TextAlign = APptLabel.Alignment.Right;
            date.FontFamily = "Times New Roman";
            date.FontSize = 18;
            date.Text = FormatDate(m_Date);

            // add logo
            if (Properties.Settings.Default.LogoFilename.Length > 0)
            {
                try
                {
                    Image img = Image.FromFile(Program.MapPath(Properties.Settings.Default.LogoFilename));
                    SizeF size = ScaleWithAspectRatio(new SizeF(slide.InchesToPixels(1.58f), slide.InchesToPixels(1.75f)), new SizeF((float)img.Width, (float)img.Height));

                    APptSlide.Image logo = slide.CreateImage(Program.MapPath(Properties.Settings.Default.LogoFilename), (slide.Width - size.Width) / 2, slide.InchesToPixels(4.0f), size.Width, size.Height);
                }
                catch (Exception) { }
            }
        }

        protected string GetSectionTitle(int SectionID, int SectionPartID)
        {
            string sql;

            if (SectionPartID != 0)
            {
                sql = "SELECT [Section].[SectionLabel], [Section].[Desc] AS [SectionDesc], [SectionParts].[PartLabel],";
                sql += " [SectionParts].[PartDescription] AS [PartDesc] FROM [Section]";
                sql += " INNER JOIN [SectionParts] ON [SectionParts].[SectionId] = [Section].[SectionId]";
                sql += " WHERE [Section].[SectionId] = @SectionID AND [SectionParts].[PartID] = @SectionPartID";
            }
            else
            {
                sql = "SELECT [SectionLabel], [Desc] AS [SectionDesc] FROM [Section]";
                sql += " WHERE [Section].[SectionId] = @SectionID";
            }

            OleDbCommand cmd = new OleDbCommand(sql, Database.Connection);
            cmd.Parameters.AddWithValue("@SectionID", SectionID);
            if (SectionPartID != 0)
                cmd.Parameters.AddWithValue("@SectionPartID", SectionPartID);

            OleDbDataReader reader = cmd.ExecuteReader();
            string title = string.Empty;
            if (reader.Read())
            {
                if (SectionPartID != 0)
                {
                    title = "Section " + reader["SectionLabel"].ToString() + " - Part " + reader["PartLabel"].ToString() + ": " + reader["PartDesc"].ToString();
                }
                else
                {
                    title = "Section " + reader["SectionLabel"].ToString() + ": " + reader["SectionDesc"].ToString();
                }
            }
            reader.Close();

            return title;
        }

        protected string GetSectionTitle(int SectionID)
        {
            return GetSectionTitle(SectionID, 0);
        }

        protected string GetSectionInspector(int SectionID)
        {
            OleDbCommand cmd = new OleDbCommand("SELECT DISTINCT [FirstName] + ' ' + [LastName] AS [Inspector] FROM [Inspector] WHERE [InspectorID] IN (SELECT [InspectorID] FROM [tblInspectorAssignment] WHERE [InspectionID] = @InspectionID AND [SectionID] = @SectionID)", Database.Connection);
            cmd.Parameters.AddWithValue("@InspectionID", m_InspectionID);
            cmd.Parameters.AddWithValue("@SectionID", SectionID);

            OleDbDataReader reader = cmd.ExecuteReader();
            string inspector = string.Empty;
            while (reader.Read())
            {
                if (inspector.Length != 0)
                    inspector += ", ";
                inspector += reader["Inspector"].ToString();
            }
            reader.Close();

            return inspector;
        }

		protected string GetInspector()
		{
			//atbChecklistInspectors
			OleDbCommand cmd = new OleDbCommand("SELECT DISTINCT [FirstName] + ' ' + [LastName] AS [Inspector] FROM [Inspector] WHERE [InspectionID] = @InspectionID", Database.Connection);
			cmd.Parameters.AddWithValue("@InspectionID", m_InspectionID);
			OleDbDataReader reader = cmd.ExecuteReader();
			string inspector = string.Empty;
			while (reader.Read())
			{
				if (inspector.Length != 0)
					inspector += ", ";
				inspector += reader["Inspector"].ToString();
			}
			reader.Close();

			return inspector;
		}

        protected SizeF ScaleWithAspectRatio(SizeF max, SizeF size)
        {
            SizeF tmp = size;

            if (tmp.Width > max.Width)
            {
                float f = max.Width / tmp.Width;
                tmp.Width *= f;
                tmp.Height *= f;
            }

            if (tmp.Height > max.Height)
            {
                float f = max.Height / tmp.Height;
                tmp.Width *= f;
                tmp.Height *= f;
            }

            return tmp;
        }*/
    }
}

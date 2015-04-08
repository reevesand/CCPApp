using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using CCPApp.Utilities;
using Xamarin.Forms;
using CCPApp.iOS.Utilities;
using System.Drawing;
using Microsoft.Office.Core;

[assembly: Dependency(typeof(PptGenerator))]
namespace CCPApp.iOS.Utilities
{
	public class PptGenerator : IPptGenerator
	{
		Application application;
		Presentation presentation;
		string FileName = null;

		public PptGenerator()
		{
			application = new Application();
			//presentation = application.Presentations.Add(Microsoft.Office.Core.MsoTriState.msoTrue);
			presentation.PageSetup.SlideWidth = 720.0f;
			presentation.PageSetup.SlideHeight = 540.0f;
		}

		public APptSlide CreateSlide()
		{
			return new PptSlide();
		}
		public void CreateBlankFile(string fileName)
		{
			FileName = fileName;
		}
		public void Save(string fileName)
		{
			FileName = fileName;
			presentation.SaveAs(FileName, Microsoft.Office.Interop.PowerPoint.PpSaveAsFileType.ppSaveAsPowerPoint7, Microsoft.Office.Core.MsoTriState.msoTrue);
		}
	}

	public class PptSlide : APptSlide
	{
		public override float Height { get; set; }
		public override string Background { get; set; }
		public override Color ForeColor { get; set; }
		public override string FontFamily { get; set; }
		public override float FontSize { get; set; }
		public override float Width { get; set; }

		public override APptLabel CreateLabel()
		{
			return new PptLabel();
		}
		public override APptLine CreateLine(PointF p1, PointF p2)
		{
			return new PptLine();
		}
		public override float InchesToPixels(float inches)
		{
			return inches;
		}
	}

	public class PptLabel : APptLabel
	{
		public override float Left { get; set; }
		public override float Top { get; set; }
		public override float Width { get; set; }
		public override bool WordWrap { get; set; }
		public override string FontFamily { get; set; }
		public override float FontSize { get; set; }
		public override bool Bold { get; set; }
		public override string Text { get; set; }
		public override float Height { get; set; }
		public override float ActualHeight
		{get{return 0;}}
		public override float ActualWidth
		{get{return 0;}}
		public override bool Italic { get; set; }
		public override Color color { get; set; }
		public override Alignment TextAlign { get; set; }
		public override VAlignment TextVAlign { get; set; }
		public override bool Underline { get; set; }
	}

	public class PptLine : APptLine
	{
		public override float Weight { get; set; }
		public override bool Dashed { get; set; }
		public override StyleType style { get; set; }
	}
}
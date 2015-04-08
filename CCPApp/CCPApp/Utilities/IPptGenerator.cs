using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CCPApp.Utilities
{
	public interface IPptGenerator
	{
		APptSlide CreateSlide();
		void CreateBlankFile(string fileName);
		void Save(string fileName);
	}
	public abstract class APptSlide
	{
		public abstract float Height { get; set; }
		public abstract APptLabel CreateLabel();
		public abstract APptLine CreateLine(PointF p1, PointF p2);
		public abstract string Background { get; set; }
		public abstract Color ForeColor{ get; set; }
		public abstract string FontFamily { get; set; }
		public abstract float FontSize { get; set; }
		public abstract float Width { get; set; }

		public abstract float InchesToPixels(float inches);
	}
	public abstract class APptLabel
	{
		public abstract float Left { get; set; }
		public abstract float Top { get; set; }
		public abstract float Width { get; set; }
		public abstract bool WordWrap { get; set; }
		public abstract string FontFamily { get; set; }
		public abstract float FontSize { get; set; }
		public abstract bool Bold { get; set; }
		public abstract string Text { get; set; }
		public abstract float Height { get; set; }
		public abstract float ActualHeight { get; }
		public abstract float ActualWidth { get; }
		public abstract bool Italic { get; set; }
		public abstract Color color { get; set; }
		public abstract Alignment TextAlign { get; set; }
		public abstract VAlignment TextVAlign { get; set; }
		public abstract bool Underline { get; set; }

		public enum Alignment
		{
			Left = 0,
			Center,
			Right
		}

		public enum VAlignment
		{
			Top = 0,
			Middle,
			Bottom
		}
	}
	public abstract class APptLine
	{
		public abstract float Weight { get; set; }
		public abstract bool Dashed { get; set; }
		public abstract StyleType style { get; set; }

		public enum StyleType
		{
			Single = 0,
			ThickBetweenThin,
			ThickThin,
			ThinThick,
			ThinThin
		}
	}
}

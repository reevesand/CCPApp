using CCPApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CCPApp.Utilities
{
	public class ScoringHelper
	{
		/// <summary>
		/// Scores a section.  Returns 1)Available 2)Earned, and 3)percentage
		/// </summary>
		public static Tuple<double, double, double> ScoreSection(SectionModel section, Inspection inspection)
		{
			List<ScoredQuestion> scores = inspection.scores;
			List<ScoredQuestion> RelevantScores = scores.Where(score => score.question.SectionId == section.Id && score.question.IsScorable()).ToList();

			return ScoreQuestions(RelevantScores);
		}
		/// <summary>
		/// Scores a part.  Returns 1)Available 2)Earned, and 3)percentage
		/// </summary>
		public static Tuple<double, double, double> ScorePart(SectionPart part, Inspection inspection)
		{
			List<ScoredQuestion> scores = inspection.scores;
			List<ScoredQuestion> RelevantScores = scores.Where(score => score.question.SectionPartId == part.Id && score.question.IsScorable()).ToList();

			return ScoreQuestions(RelevantScores);
		}
		/// <summary>
		/// Scores an inspection.  Returns 1)Available 2)Earned, and 3)percentage
		/// </summary>
		public static Tuple<double, double, double> ScoreInspection(Inspection inspection)
		{
			List<ScoredQuestion> scores = inspection.scores;
			List<ScoredQuestion> RelevantScores = scores.Where(score => score.question.IsScorable()).ToList();

			return ScoreQuestions(RelevantScores);
		}

		/// <summary>
		/// Scores a list of questions.  Returns 1)Available 2)Earned, and 3)percentage
		/// </summary>
		public static Tuple<double, double, double> ScoreQuestions(IEnumerable<ScoredQuestion> scores)
		{
			double availablePoints = scores.Count(score => score.answer == Answer.Yes || score.answer == Answer.No);
			if (availablePoints == 0)
			{
				return new Tuple<double, double, double>(0, 0, 0);
			}
			double scoredPoints = scores.Count(score => (score.answer == Answer.Yes && score.question.InvertScore == false) || (score.answer == Answer.No && score.question.InvertScore == true));
			double percentage = scoredPoints / availablePoints;
			if (scores.Any(s => s.question.Critical && (((s.answer == Answer.No) && (s.question.InvertScore == false)) || ((s.answer == Answer.Yes) && (s.question.InvertScore == true)))))
			{
				percentage = Math.Max(percentage, .5);
			}
			return new Tuple<double, double, double>(availablePoints, scoredPoints, scoredPoints / availablePoints);
		}

		public static bool AnyUnsatisfactorySections(Inspection inspection)
		{
			int threshold = inspection.Checklist.ScoreThresholdSatisfactory;
			foreach (SectionModel section in inspection.Checklist.Sections)
			{
				Tuple<double,double,double> sectionScore = ScoreSection(section, inspection);
				if (sectionScore.Item3 * 100 < threshold)
				{
					return true;
				}
			}
			return false;
		}

		public static Color GetScoreColor(double score, ChecklistModel checklist, bool allowCommendable = true)
		{
			double percentScore = score * 100;
			if (percentScore < checklist.ScoreThresholdSatisfactory)
			{
				return Color.Red;
			}
			else if (percentScore < checklist.ScoreThresholdCommendable || !allowCommendable)
			{
				return Color.Green;
			}
			else
			{
				return Color.Blue;
			}
		}
	}
	public enum Rating
	{
		None,
		Unacceptable,
		Satisfactory,
		Commendable,
	}
}

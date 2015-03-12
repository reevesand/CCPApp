using CCPApp;
using CCPApp.iOS;
using CCPApp.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Xamarin.Forms;

[assembly: Dependency(typeof(ParseChecklist))]
namespace CCPApp.iOS
{
	class ParseChecklist : IParseChecklist
	{
		ChecklistModel Model;

		public string GetChecklistId(string filename)
		{
			XmlReader reader = new FileManage().LoadXml(filename);
			XmlDocument document = new XmlDocument();
			document.Load(reader);

			for (int i = 0; i < document.ChildNodes.Count; i++)
			{
				XmlNode node = document.ChildNodes[i];
				if (node.NodeType != XmlNodeType.Element)
					continue;
				// pass node to the appropriate parsing function depending on its name
				if (node.Name == "Checklist")
					return GetId(node);
				else
				{
					//uh, this is bad and I don't know what ought to be done. TODO
				}
			}
			return null;
		}
		protected string GetId(XmlNode node)
		{
			return AttributeString(node.Attributes["Id"], System.Guid.NewGuid().ToString());
		}


		public void Parse(ChecklistModel model, string filename)
		{
			Model = model;
			XmlReader reader = new FileManage().LoadXml(filename);
			XmlDocument document = new XmlDocument();
			document.Load(reader);

			for (int i = 0; i < document.ChildNodes.Count; i++)
			{
				XmlNode node = document.ChildNodes[i];

				if (node.NodeType != XmlNodeType.Element)
					continue;

				// pass node to the appropriate parsing function depending on its name
				if (node.Name == "Checklist")
					ParseWholeChecklist(node);
				else
				{
					//uh, this is bad and I don't know what ought to be done. TODO
				}
			}
		}
		protected void ParseWholeChecklist(XmlNode node)
		{
			// get checklist information
			Model.Id = AttributeString(node.Attributes["Id"], System.Guid.NewGuid().ToString());
			Model.Title = AttributeString(node.Attributes["Title"]);
			Model.Description = AttributeString(node.Attributes["Description"]);

			Model.ContactName = AttributeString(node.Attributes["ContactName"]);
			Model.ContactPosition = AttributeString(node.Attributes["ContactPosition"]);
			Model.ContactAddress = AttributeString(node.Attributes["ContactAddress"]);
			Model.ContactCityState = AttributeString(node.Attributes["ContactCityState"]);
			Model.ContactZip = AttributeString(node.Attributes["ContactZip"]);

			Model.DocumentDirectory = AttributeString(node.Attributes["DocumentDirectory"]);
			Model.SplashBackground = AttributeString(node.Attributes["SplashBackground"]);
			Model.SplashForegroundColor = AttributeString(node.Attributes["SplashForegroundColor"], "255,255,255");
			Model.Logo = AttributeString(node.Attributes["Logo"]);
			Model.Logo2 = AttributeString(node.Attributes["Logo2"]);
			Model.LogoLocation = AttributeString(node.Attributes["LogoLocation"]);
			Model.LogoLocation2 = AttributeString(node.Attributes["LogoLocation2"]);
			Model.FontFamily = AttributeString(node.Attributes["FontFamily"], "Arial");

			Model.ResumeFormName = AttributeString(node.Attributes["ResumeFormName"]);

			Model.ScoreThresholdCommendable = AttributeInt(node.Attributes["ScoreThresholdCommendable"], 95);
			Model.ScoreThresholdSatisfactory = AttributeInt(node.Attributes["ScoreThresholdSatisfactory"], 80);
			Model.UserInvert = (AttributeString(node.Attributes["UserInvert"], "false").ToLower() == "true");

			// loop through child nodes
			for (int i = 0; i < node.ChildNodes.Count; i++)
			{
				XmlNode child = node.ChildNodes[i];

				if (child.NodeType != XmlNodeType.Element)
					continue;

				if (child.Name == "Section")
					ParseSections(child);
				else if (child.Name == "SelectedSection")
					ParseSections(child);
				else
				{
					//UnknownNode(child);
					//again, this is bad.  TODO.
				}
					//TODO something in this case.
					//UnknownNode(child);
			}
		}
		protected void ParseSections(XmlNode node)
		{
			// create new section
			Section section = new Section();
			section.Label = AttributeString(node.Attributes["Label"]);
			section.Title = AttributeString(node.Attributes["Title"]);
			section.ShortTitle = AttributeString(node.Attributes["ShortTitle"]);
			section.ScoringModel = AttributeString(node.Attributes["ScoringModel"], "YesNo");

			// add section to the given section list
			Model.Sections.Add(section);
			section.checklist = Model;

			// loop through child nodes
			for (int i = 0; i < node.ChildNodes.Count; i++)
			{
				XmlNode child = node.ChildNodes[i];

				if (child.NodeType != XmlNodeType.Element)
					continue;

				if (child.Name == "Part")
					ParseSectionParts(child, section);
				else if (child.Name == "Question")
				{
					ParseQuestions(child, section.Questions);
					foreach (Question question in section.Questions)
					{
						question.section = section;
					}
				}
				//else
					//UnknownNode(child);
			}
		}
		protected void ParseSectionParts(XmlNode node, Section section)
		{
			// create new part
			SectionPart part = new SectionPart();
			part.Label = AttributeString(node.Attributes["Label"]);
			part.Description = AttributeString(node.Attributes["Description"]);
			part.ScoringModel = AttributeString(node.Attributes["ScoringModel"], "YesNo");

			// add part to the given part list
			section.SectionParts.Add(part);
			part.section = section;

			// loop through child nodes
			for (int i = 0; i < node.ChildNodes.Count; i++)
			{
				XmlNode child = node.ChildNodes[i];

				if (child.NodeType != XmlNodeType.Element)
					continue;

				if (child.Name == "Question")
				{
					ParseQuestions(child, part.Questions);
					foreach (Question question in part.Questions)
					{
						question.part = part;
						question.section = section;
						section.Questions.Add(question); //I don't really like doing this, but the linkage really ought to be there.
						//Note, you usually aren't supposed to call "Section.Questions".
					}
				}
				//else
					//UnknownNode(child);
			}
		}
		protected void ParseQuestions(XmlNode node, List<Question> question_list)
		{
			// create new question
			Question question = new Question();
			question.Number = AttributeInt(node.Attributes["Number"]);
			question.Subqualifier = AttributeString(node.Attributes["Subqualifier"]);
			question.Critical = (AttributeString(node.Attributes["Critical"], "false").ToLower() == "true");
			question.CriticalApplication = AttributeString(node.Attributes["CriticalApplication"]);
			question.InvertScore = (AttributeString(node.Attributes["InvertScore"], "false").ToLower() == "true");
			question.IsLastQuestion = (AttributeString(node.Attributes["IsLastQuestion"], "false").ToLower() == "true");
			question.Text = AttributeString(node.Attributes["Text"]);
			question.PrintedText = AttributeString(node.Attributes["PrintedText"]);
			question.HasSubItems = (AttributeString(node.Attributes["HasSubItems"], "false").ToLower() == "true");
			try
			{
				question.Updated = (AttributeString(node.Attributes["Updated"]).ToLower() == "true");
			}
			catch (ArgumentException)
			{
				question.Updated = false;
			}

			// add question to the given question list
			question_list.Add(question);

			// loop through child nodes
			for (int i = 0; i < node.ChildNodes.Count; i++)
			{
				XmlNode child = node.ChildNodes[i];

				if (child.NodeType != XmlNodeType.Element)
					continue;

				if (child.Name == "Reference")
					ParseReference(child, question);
				//else
					//UnknownNode(child);
			}
		}
		protected void ParseReference(XmlNode node, Question question)
		{
			// create new reference
			Reference reference = new Reference();
			reference.Document = AttributeString(node.Attributes["Document"]);
			reference.DocumentName = AttributeString(node.Attributes["DocumentName"]);
			reference.Bookmark = AttributeString(node.Attributes["Bookmark"]);
			reference.Description = AttributeString(node.Attributes["Description"]);
			// add reference to the given question
			question.References.Add(reference);
		}

		protected string AttributeString(XmlAttribute attribute, string default_value)
		{
			return (attribute == null || attribute.Value.Length == 0) ? default_value : attribute.Value;
		}
		protected string AttributeString(XmlAttribute attribute)
		{
			return AttributeString(attribute, string.Empty);
		}
		protected int AttributeInt(XmlAttribute attribute, int default_value)
		{
			return (attribute == null) ? default_value : int.Parse(attribute.Value);
		}

		protected int AttributeInt(XmlAttribute attribute)
		{
			return AttributeInt(attribute, 0);
		}
	}
}

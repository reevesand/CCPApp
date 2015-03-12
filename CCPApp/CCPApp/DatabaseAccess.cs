using CCPApp.Models;
using SQLite.Net;
using SQLite.Net.Async;
using SQLiteNetExtensionsAsync.Extensions;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CCPApp
{
	public class DatabaseAccess
	{
		SQLiteConnection database;
		SQLiteAsyncConnection asyncDB;

		private object dbSync = new object();
		/*bool _executingAsyncTask = false;
		bool ExecutingAsyncTask { 
			get {
				lock (dbSync)
				{
					return _executingAsyncTask;
				}
			} 
			set {
				lock (dbSync)
				{
					_executingAsyncTask = value;
				}
			}
		}*/

		public DatabaseAccess()
		{
			database = DependencyService.Get<ISQLite>().GetConnection();
			asyncDB = DependencyService.Get<ISQLite>().GetAsyncConnection(database);
			database.CreateTable<ChecklistModel>();
			database.CreateTable<Inspection>();
			database.CreateTable<Question>();
			database.CreateTable<ScoredQuestion>();
			database.CreateTable<Section>();
			database.CreateTable<SectionPart>();
			database.CreateTable<Comment>();
		}
		/*private async Task waitUntilReady()
		{
			while (ExecutingAsyncTask)
			{
				await Task.Delay(100);
			}
		}*/
		public bool ChecklistExists(string id)
		{
			return database.Table<ChecklistModel>().Any(c => c.Id == id);
		}

		public async void SaveChecklists(List<ChecklistModel> checklists)
		{
			//ExecutingAsyncTask = true;
			Action action = new Action(() => {
				lock (dbSync)
				{
					database.InsertOrReplaceAllWithChildren(checklists, true);
				}
			});
			await Task.Run(action);
			//await asyncDB.InsertOrReplaceAllWithChildrenAsync(checklists, true);
			//ExecutingAsyncTask = false;
		}
		/*public async void SaveChecklist(ChecklistModel checklist)
		{
			ExecutingAsyncTask = true;
			await asyncDB.InsertOrReplaceWithChildrenAsync(checklist, true);
			ExecutingAsyncTask = false;
			//database.InsertOrReplaceWithChildren(checklist, true);
			//return database.InsertOrReplace(checklist, typeof(ChecklistModel));
		}*/
		/*public int SaveSection(Section section)
		{
			return database.InsertOrReplace(section, typeof(Section));
		}
		public int SavePart(SectionPart part)
		{
			return database.InsertOrReplace(part, typeof(SectionPart));
		}
		public int SaveQuestion(Question question)
		{
			return database.InsertOrReplace(question, typeof(Question));
		}*/

		public ChecklistModel LoadChecklist(string id)
		{
			ChecklistModel checklist = database.Table<ChecklistModel>().SingleOrDefault(c => c.Id == id);
			database.GetChildren(checklist, true);
			return checklist;
		}
		/*public IEnumerable<Inspection> LoadInspectionsForChecklist(string checklistId)
		{
			return database.Table<Inspection>().Where(i => i.ChecklistId == checklistId);
		}
		public IEnumerable<Section> LoadSectionsForChecklist(string checklistId)
		{
			return database.Table<Section>().Where(s => s.ChecklistId == checklistId);
		}
		public IEnumerable<SectionPart> LoadPartsForSection(int? sectionId)
		{
			return database.Table<SectionPart>().Where(p => p.SectionId == sectionId);
		}
		public IEnumerable<Question> LoadQuestionsForSection(int? sectionId)
		{
			return database.Table<Question>().Where(q => q.SectionId == sectionId);
		}
		public IEnumerable<Question> LoadQuestionsForPart(int? partId)
		{
			return database.Table<Question>().Where(q => q.SectionPartId == partId);
		}*/

		public async void SaveInspection(Inspection inspection)
		{
			//await waitUntilReady();
			//ExecutingAsyncTask = true;
			Action action = new Action(() =>
			{
				lock (dbSync)
				{
					database.Insert(inspection, typeof(Inspection));
					foreach (ScoredQuestion score in inspection.scores)
					{
						score.InspectionId = inspection.Id;
						SaveScore(score);
					}
					foreach (Comment comment in inspection.comments)
					{
						comment.InspectionId = inspection.Id;
						SaveComment(comment);
					}
				}
			});
			await Task.Run(action);
			
			//ExecutingAsyncTask = false;
		}
		public async void SaveScore(ScoredQuestion score)
		{
			//await waitUntilReady();
			//ExecutingAsyncTask = true;
			Action action = new Action(() =>
			{
				lock (dbSync)
				{
					score.InspectionId = score.inspection.Id;
					database.InsertOrReplace(score, typeof(ScoredQuestion));
				}
			});
			await Task.Run(action);
			
			//ExecutingAsyncTask = false;
		}
		public ScoredQuestion LoadScoreForQuestion(Inspection inspection, Question question)
		{
			return database.Table<ScoredQuestion>().SingleOrDefault(score => score.InspectionId == inspection.Id && score.QuestionId == question.Id);
		}
		public List<ScoredQuestion> LoadScoresForInspection(Inspection inspection)
		{
			return database.GetAllWithChildren<ScoredQuestion>(score => score.InspectionId == inspection.Id);
		}
		public async void DeleteScore(ScoredQuestion score)
		{
			//await waitUntilReady();
			Action action = new Action(() =>
			{
				lock (dbSync)
				{
					database.Delete(score);
				}
			});
			await Task.Run(action);
		}
		public async void SaveComment(Comment comment)
		{
			Action action = new Action(() =>
			{
				lock (dbSync)
				{
					comment.InspectionId = comment.inspection.Id;
					database.InsertOrReplace(comment);
				}
			});
			await Task.Run(action);
		}
		public List<Comment> LoadCommentsForInspection(Inspection inspection)
		{
			return database.GetAllWithChildren<Comment>(comment => comment.InspectionId == inspection.Id);
		}
	}
}

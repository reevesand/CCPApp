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
			database.CreateTable<SectionModel>();
			database.CreateTable<SectionPart>();
			database.CreateTable<Comment>();
			database.CreateTable<Reference>();
			database.CreateTable<Inspector>();
			database.CreateTable<InspectorInspections>();
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
		public async void SaveQuestion(Question question)
		{
			await Task.Run(() => {
				lock (dbSync)
				{
					database.Update(question);
				}
			});
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

		public IEnumerable<ChecklistModel> LoadAllChecklists()
		{
			return database.GetAllWithChildren<ChecklistModel>(null, true);
		}
		public ChecklistModel LoadChecklist(string id)
		{
			ChecklistModel checklist = database.Table<ChecklistModel>().SingleOrDefault(c => c.Id == id);
			database.GetChildren(checklist, true);
			return checklist;
		}

		public async void SaveInspection(Inspection inspection)
		{
			Action action = new Action(() =>
			{
				lock (dbSync)
				{
					database.InsertOrReplace(inspection, typeof(Inspection));
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
					foreach (Inspector inspector in inspection.inspectors)
					{
						InspectorInspections link = new InspectorInspections();
						link.InspectionId = inspection.Id;
						link.InspectorId = inspector.Id;
						database.InsertOrReplace(link);
					}
				}
			});
			await Task.Run(action);
		}
		public async void MinorUpdateInspection(Inspection inspection)
		{
			await Task.Run(() =>
			{
				lock (dbSync)
				{
					database.Update(inspection);
				}
			});
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

		public List<Inspector> LoadAllInspectors()
		{
			return database.GetAllWithChildren<Inspector>();
		}
		public void SaveInspector(Inspector inspector)
		{			
			database.InsertOrReplace(inspector);
			foreach (Inspection inspection in inspector.inspections)
			{
				InspectorInspections link = new InspectorInspections();
				link.InspectorId = inspector.Id;
				link.InspectionId = inspection.Id;
				database.InsertOrReplace(link);
			}
		}
		public void DeleteComment(Comment comment)
		{
			database.Delete(comment);
		}
		public void DeleteInspector(Inspector inspector)
		{
			database.Delete(inspector);
			List<InspectorInspections> linksToDelete = new List<InspectorInspections>();
			foreach (Inspection inspection in inspector.inspections)
			{
				linksToDelete.AddRange(database.Table<InspectorInspections>()
						.Where(link => link.InspectionId == inspection.Id && link.InspectorId == inspector.Id));
			}
			database.DeleteAll(linksToDelete);
		}
		public void DeleteInspection(Inspection inspection)
		{
			database.DeleteAll(inspection.scores);
			database.DeleteAll(inspection.comments);

			List<InspectorInspections> linksToDelete = new List<InspectorInspections>();
			foreach (Inspector inspector in inspection.inspectors)
			{
				linksToDelete.AddRange(database.Table<InspectorInspections>()
						.Where(link => link.InspectionId == inspection.Id && link.InspectorId == inspector.Id));
			}
			database.DeleteAll(linksToDelete);

			database.Delete(inspection);
		}
		public void DeleteChecklist(ChecklistModel checklist)
		{
			database.DeleteAll(checklist.GetAllQuestions());
			database.Delete(checklist);
		}
	}
}

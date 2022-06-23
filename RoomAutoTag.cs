using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomAutoTagPlugin
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class RoomAutoTag : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
 
            List<Level> levels = new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .OfType<Level>()
                .ToList();

            Level level1 = levels.Where(x => x.Name.Equals("Уровень 1")).FirstOrDefault();
            Level level2 = levels.Where(x => x.Name.Equals("Уровень 2")).FirstOrDefault();

            if (level1==null)
            {
                TaskDialog.Show("Ошибка", "Не найден уровень");
                return Result.Cancelled;
            }

            Wall wall = new FilteredElementCollector(doc)
                .OfClass(typeof(Wall))
                .OfType<Wall>()
                .FirstOrDefault();

            if (wall == null)
            {
                TaskDialog.Show("Ошибка", "Не найдены стены");
                return Result.Cancelled;
            }

            Phase phase = doc.GetElement(wall.CreatedPhaseId) as Phase;

            Transaction ts = new Transaction(doc, "Create rooms");
             ts.Start();

            foreach (var level in levels)
            {
                ICollection<ElementId> rooms = doc.Create.NewRooms2(level, phase);

                foreach (ElementId r in rooms)
                {
                    Room room = doc.GetElement(r) as Room;
                    room.Name = "Комната";
                }
            }

             ts.Commit();


            return Result.Succeeded;
        }

    }
}

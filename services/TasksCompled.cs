using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ListTaskApi.services
{
    public class TasksCompled
    {
        
        public int Id { get; set; }
        public int Id_Criador { get; set; }
        public string NameTask { get; set; }
        public DateTime DateTask { get; set; }

    }
}
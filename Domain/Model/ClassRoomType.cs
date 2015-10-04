using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class ClassRoomType
    {
        public ClassRoomType(string description)
        {
            Description = description;
        }

        public string Description { get; private set; }

    }
}

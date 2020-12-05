using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoLotDAL.Bulklmport
{
    interface IMyDataReader <T> :IDataReader
    {
        List <T> Records { get; set; }
    }
}

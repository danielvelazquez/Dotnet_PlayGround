using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayGround.EstructuraDeDatos.Algoritmos.Ordenamiento
{
    interface IInsertionSort
    {
        Task List<T>(List<T> list) where T : IComparable<T>;
    }
}

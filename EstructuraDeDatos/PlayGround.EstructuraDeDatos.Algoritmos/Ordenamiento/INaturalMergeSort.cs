﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayGround.EstructuraDeDatos.Algoritmos.Ordenamiento
{
    public interface INaturalMergeSort
    {
        Task List<T>(List<T> list) where T : IComparable<T>;
    }
}

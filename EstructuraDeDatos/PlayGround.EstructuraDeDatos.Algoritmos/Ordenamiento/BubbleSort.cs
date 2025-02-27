namespace PlayGround.EstructuraDeDatos.Algoritmos.Ordenamiento
{
    public class BubbleSort : IBubbleSort
    {
        public Task List<T>(List<T> list) where T : IComparable<T>
        {
            throw new NotImplementedException();
        }

        // TODO: Implementar el método Sort
        public void Sort(int[] array)
        {
            int n = array.Length;
            for(int i = 0; i < n-1; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    // Intercambiar array[j] y array[j+1]
                    if (array[j] > array[j + 1])
                    {
                        int temp = array[j];
                        array[j] = array[j + 1];
                        array[j + 1] = temp;
                    }
                }
            }
        }
    }
}

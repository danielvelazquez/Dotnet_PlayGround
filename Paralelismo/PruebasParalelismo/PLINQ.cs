using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebasParalelismo
{
    class PLINQ
    {
        public class Person
        {
            public string Name { get; set; }
            public string City { get; set; }
        }

        public Person[] CreacionPersona()
        {
            Person[] people = new Person[]
            { new Person { Name = "Alan", City = "Hull" },
                new Person { Name = "Beryl", City = "Seattle" },
                new Person { Name = "Charles", City = "London" },
                new Person { Name = "David", City = "Seattle" },
                new Person { Name = "Eddy", City = "Paris" },
                new Person { Name = "Fred", City = "Berlin" },
                new Person { Name = "Gordon", City = "Hull" },
                new Person { Name = "Henry", City = "Seattle" },
                new Person { Name = "Isaac", City = "Seattle" },
                new Person { Name = "James", City = "London" } };
            return people;
        }

        public void ProcesopLinq()
        {
            //The AsParallel method 
            // examines the query to determine if using a parallel version would speed it up.
            //    If it is decided that executing elements of the query in parallel would improve performance, 
            //    the query is broken down into a number of processes and each is run concurrently. 
            //If the AsParallel method can’t decide whether parallelization would improve performance the query is not executed in parallel.
            //    If you really want to use AsParallel you should design the behavior with this in mind, 
            //otherwise performance may not be improved and it is possible that you might get the wrong outputs.
            var people = this.CreacionPersona();
            var result = from person in people.AsParallel()
                         where person.City == "Seattle"
                         select person;
            foreach (var person in result)
                Console.WriteLine(person.Name);
            Console.WriteLine("Finished processing. Press a key to end.");
            Console.ReadKey();
        }


        // Programs can use other method calls to further inform the parallelization process,
        void InformaParalelismo()
        {
            var people = this.CreacionPersona();
            var result = from person in people.AsParallel().WithDegreeOfParallelism(4).WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                         where person.City == "Seattle"
                         select person;
        }

        void UsandoAsOrdered()
        {
            var people = this.CreacionPersona();
            var result = from person in people.AsParallel().AsOrdered() where person.City == "Seattle" select person;
        }
        // The AsSequential method can be used to identify parts of a query that must be sequentially executed.
        // AsSequential executes the query in order whereas AsOrdered returns a sorted result but does not necessarily run the query in order.
        void UsandoAsSequential()
        {
            var people = this.CreacionPersona();
            var result = (from person in people.AsParallel() where person.City == "Seattle" orderby (person.Name) select new { Name = person.Name }).AsSequential().Take(4);
        }

        // The parallel nature of the execution of ForAll means that the order of the printed 
        // output above will not reflect the ordering of the input data.
        void UsandoForAll()
        {
            var people = this.CreacionPersona();
            var result = from person in people.AsParallel() where person.City == "Seattle" select person;
            result.ForAll(person => Console.WriteLine(person.Name));
        }


        // Excepciones 
        public static bool CheckCity(string name)
        {
            if (name == "") throw new ArgumentException(name);
            return name == "Seattle";
        }
        void UsandoExcepciones()
        {
            try
            {
                var people = this.CreacionPersona();
                var result = from person in people.AsParallel() where CheckCity(person.City) select person; // el select utiliza la funcion CheckCity para validar si la string es "". Buena practica validar lo mas cercano al error. 
                result.ForAll(person => Console.WriteLine(person.Name));
            }
            catch (AggregateException e)
            {
                Console.WriteLine(e.InnerExceptions.Count + " exceptions.");
            }
        }
    }
}

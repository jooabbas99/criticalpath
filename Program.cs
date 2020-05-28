using System;
using System.Collections.Generic;
using System.Linq;

namespace Cirit_path
{
    class Program
    {
        static char[] COLON = { ':' };
        static char[] SEMI_COLON = { ';' };
        static char[] COMMA = { ',' };
        static char[] BRACE = { ']' };
        public static string GetInput()
        {
            Console.WriteLine("enter number of tasks");
            String input = "";
            int counter = int.Parse(Console.ReadLine());
            for (int i = 0; i < counter; i++)
            {
                Console.WriteLine("\t\t\t TASK {0}", (i + 1));
                Console.Write("Activity Name : ");
                input += Console.ReadLine();
                input += ':';
                Console.Write("Duration for this tack  : ");
                input += Console.ReadLine();

                Console.WriteLine("Preceding for this tack \n # NOTE : FOR MORE THAN ONE PRECEDING ADD ',' BETWEEN ACTIVITY NAME LIKE A,B , ");
                string t = Console.ReadLine();
                input += (t == "" ? "]" : (";" + t + "]"));
                Console.WriteLine("------------------------------------");
            }


            return input;
        }
        static void Main(string[] args)
        {
            int step = 0;

            List<Activity> i = new List<Activity>();
            Console.Write("Enter input: ");
            //string s = Console.ReadLine().Replace("[", null);
            string s = GetInput();
            foreach (string x in s.Split(BRACE, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] y = x.Split(SEMI_COLON);
                Activity n = new Activity(x.Split(COLON)[0], Int32.Parse(y[0].Split(COLON)[1]));
                n.Name = x.Split(COLON)[0];

                n.Dependencies = (y.Length > 1) ? y[1].Split(COMMA).ToList() : new List<string>();
                i.Add(n);
            }

            do
            {
                List<string> d = i.Where(x => x.Done()).Select(y => y.Name).ToList();
                foreach (Activity working in i.Where(x => x.Dependencies.Except(d).Count() == 0 && !x.Done())) working.Step(step);
                step++;
            } while (i.Count(x => !x.Done()) > 0);

            foreach (Activity a in i)
            {
                Activity next = i.OrderBy(x => x.EFinish).Where(x => x.Dependencies.Contains(a.Name)).FirstOrDefault();
                a.LFinish = (next == null) ? a.EFinish : next.EStart;
                a.LStart = a.LFinish - a.Duration;
            }

            Console.WriteLine(" TASK | DEPENDENCIES | DURATION |     EF     |     LF      | SLACK | critical");
            Console.WriteLine("------+--------------+----------+------------+-------------+-------+---------");
            foreach (Activity a in i)
            {
                Console.Write(a.Name.PadLeft(5) + " |");
                Console.Write("".PadLeft(14-a.Dependencies.Count*2));
                foreach (var dep in a.Dependencies)
                {
                    Console.Write(dep+',');
                }
                Console.Write("|");
                Console.Write(a.Duration.ToString().PadLeft(10) + " |");
                Console.Write(a.EFinish.ToString().PadLeft(10) + " |");
                Console.Write(a.LFinish.ToString().PadLeft(10) + " |");
                Console.Write((a.LFinish - a.EFinish).ToString().PadLeft(7) + " |");
                string critical = (a.LFinish - a.EFinish == 0 && a.LStart - a.EStart == 0) ? "Y" : "N";
                Console.WriteLine(critical.PadLeft(9));
            }
            Console.WriteLine("Hit any key to exit.");
            Console.ReadKey();
        }
    }

    class Activity
    {
        public string Name { get; set; }
        public int Duration { get; set; }
        public List<string> Dependencies { get; set; }
        public int EStart { get; set; }
        public int EFinish { get; set; }
        public int LStart { get; set; }
        public int LFinish { get; set; }
        private int steps = 0;

        public Activity(string name, int time)
        {
            EStart = -1;
            EFinish = 0;
            LStart = 0;
            LFinish = 0;
            Name = name;
            Duration = time;
            steps = time;
        }

        public void Step(int count)
        {
            EStart = (EStart == -1) ? count : EStart;
            if (--steps == 0) EFinish = count + 1;
        }

        public bool Done()
        {
            return steps == 0;
        }
    }
}

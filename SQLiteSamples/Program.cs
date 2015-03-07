// Author: Tigran Gasparian
// This sample is part Part One of the 'Getting Started with SQLite in C#' tutorial at http://www.blog.tigrangasparian.com/
// Autor2: Piotr Rabiniak 
// I took sample code from Tigran Gasparian and add some performance tests

using System;
using System.Data.SQLite;
using Simple.Data;
using System.IO;
using System.Collections.Generic;
using System.Timers;
using System.Diagnostics;
using System.Threading;
using System.Linq;

namespace SQLiteSamples
{
    class Program
    {
        // Holds our connection with the database
        SQLiteConnection m_dbConnection;

        static void Main(string[] args)
        {
            Program p = new Program();
        }

        public Program()
        {
            createNewDatabase();
            connectToDatabase();
            createTable();
            fillTable();
            printHighscores();
        }

        // Creates an empty database file
        void createNewDatabase()
        {
            SQLiteConnection.CreateFile("MyDatabase.sqlite");
        }
        // Creates a connection with our database file.
        void connectToDatabase()
        {
            m_dbConnection = new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
            m_dbConnection.Open();
        }
        // Creates a table named 'highscores' with two columns: name (a string of max 20 characters) and score (an int)
        void createTable()
        {
            string sql = "create table highscores (name varchar(30), score int)";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
        }
        // Inserts some values in the highscores table.
        // As you can see, there is quite some duplicate code here, we'll solve this in part two.
        void fillTable()
        {
            for(int i = 0; i < 10; i++)
            {
                //SimpleObj obj = new SimpleObj() { Name = i.ToString(), Score = i };
                string sql = string.Format("insert into highscores (name, score) values ('{0}', {1})", i.ToString(), i);
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                command.ExecuteNonQuery();


            }
        }
        // Writes the highscores to the console sorted on score in descending order.
        void printHighscores()
        {
            List<SimpleObj> dataReaderList = new List<SimpleObj>();
            Stopwatch dataReaderStopWarch = new Stopwatch();
            dataReaderStopWarch.Start();
            string sql = "select * from highscores order by score desc";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                SimpleObj obj = new SimpleObj() { Name = reader["name"].ToString(), Score = Convert.ToInt32(reader["score"].ToString()) };
                dataReaderList.Add(obj);
            }
            dataReaderStopWarch.Stop();
            Console.WriteLine(string.Format("Data reader pobrał {0} obiektów w czasie {1} milisekund",dataReaderList.Count, dataReaderStopWarch.ElapsedMilliseconds));

            var connectionBuilder = new SQLiteConnectionStringBuilder();
            var dir = System.IO.Directory.GetCurrentDirectory();
            connectionBuilder.DataSource = Path.Combine(dir, "MyDatabase.sqlite");
            connectionBuilder.Version = 3;

            Stopwatch simpleDataReaderStopWatch = new Stopwatch();
            List<SimpleObj> simpleDataReaderList = new List<SimpleObj>();
            simpleDataReaderStopWatch.Start();

            var cn = Database.OpenConnection(connectionBuilder.ConnectionString);
            var albums = cn.highscores.All();
            foreach (dynamic a in albums)
            {
                SimpleObj obj = new SimpleObj() { Name = a.name, Score = Convert.ToInt32(a.score) };
                simpleDataReaderList.Add(obj);
            }
            simpleDataReaderStopWatch.Stop();
            Console.WriteLine(string.Format("Simple.Data pobrał {0} obiektów w czasie {1} milisekund", simpleDataReaderList.Count, simpleDataReaderStopWatch.ElapsedMilliseconds));

            Stopwatch efStopWatch = new Stopwatch();
            efStopWatch.Start();
            List<EfSimpleObj> efList = new List<EfSimpleObj>();
            using (var context = new SqlLiteContext())
            {
                var artists = context.EfSimpleObj;
                foreach (var a in artists)
                {
                    EfSimpleObj obj = new EfSimpleObj() { Name = a.Name, Score = a.Score };
                    efList.Add(obj);
                }
            }
            efStopWatch.Stop();
            Console.WriteLine(string.Format("EF pobrał {0} obiektów w czasie {1} milisekund", efList.Count, efStopWatch.ElapsedMilliseconds));

            Console.ReadKey();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections;
using System.IO;

namespace A04
{
    static class Constants
    {
        public const float kEpsilon = 0.005f;
        public const int kArraySize = 50;
        public const float kMinSize = 1000;
        public const float kMaxSize = 20000000;
    }

    /*
     NAME      : MyThreads
     PURPOSE   : The class has been created to create methods that allow the initialization of the threads that
                 will write in the file and keep track of the size in the file to avoid duplicate code since the 
                 same block of code would have been presented in two parts of the main. As well, it creates the 
                 MyFile object to get the store the user input.
     */

    class MyThreads
    {
        /* 
        Name	: StartMyThreads
        Purpose : To initialize the threads that will write and keep track of the text file 
        Inputs	:	string path            Pathname of the text file
                    float newSize          Expected file of the file
        Outputs	:	NONE
        Returns	:	Nothing
        */

        public void StartMyThreads(string path, float size)
        {
            MyFile userFile = new MyFile(@path, size);

            ThreadStart threadDelegate = new ThreadStart(userFile.WriteInFile); // Delegate to the write in file method
            ThreadStart sizeDelegate = new ThreadStart(userFile.CurrentSize); // Delegate to current size method
            Thread[] threadsArray = new Thread[Constants.kArraySize]; // Array of 50 identical threads

            // Assignment of delegates to threads

            Thread checkSize = new Thread(sizeDelegate);

            for (int i = 0; i < Constants.kArraySize; i++)
            {
                Thread writingThread = new Thread(threadDelegate);
                threadsArray[i] = writingThread;
            }

            // Start of the threads

            checkSize.Start();

            for (int i = 0; i < Constants.kArraySize; i++)
            {
                threadsArray[i].Start();
            }

            for (int i = 0; i < Constants.kArraySize; i++)
            {
                threadsArray[i].Join();
            }

            checkSize.Join();

            long finalSize = userFile.GetFinalSize();

            // Final size of the file

            Console.WriteLine("The final size of the file is: {0} bytes.", finalSize.ToString());
        }

        /* 
        Name	: Usage
        Purpose : To show the user the usage of the program
        Inputs	:   NONE
        Outputs	:	Usage of the application
        Returns	:	Nothing
        */

        public void Usage()
        {
            Console.WriteLine("USAGE:");
            Console.WriteLine("A04.exe Pathname FileSize(between 1,000 and the other 20,000,000)");
        }
        
    }

    class Program
    {
        static void Main(string[] args)
        {
            MyThreads start = new MyThreads();

            if (args.Length == 0 || args.Length > 2 || args.Length == 1) // Wrong amount of command line arguments
            {
                start.Usage();
            }
            else if (args[0] == "/?") // User asks for the usage
            {
                start.Usage();
            }
            else
            {
                float size = float.Parse(args[1]); 

                if (size < Constants.kMinSize || size > Constants.kMaxSize) // Size is out range
                {
                    start.Usage();
                }
                else
                {
                    string checkPathName = @args[0];
                    
                    if (Directory.Exists(Path.GetDirectoryName(@args[0]))) // Directory exists?
                    {
                        MyFile userFile = new MyFile(@args[0], size);

                        if (!File.Exists(@args[0])) // Text file does not exist
                        {
                            StreamWriter newFile = File.CreateText(@args[0]); // New text file
                            newFile.Close();

                            start.StartMyThreads(args[0], size);

                        } else // Text File exists
                        {
                            long fileSize = new FileInfo(@args[0]).Length;
                            bool sameSize = (Math.Abs(fileSize - size) < Constants.kEpsilon);

                            if (sameSize) // Checks if the new file can receive random data
                            {
                                Console.WriteLine("The file exists and it is the same size. More random data cannot be added.\n");

                            } else
                            {
                                start.StartMyThreads(args[0], size); // Start of the threads
                            }
                        }
                    }
                    else // Directory does not exist
                    {
                        try // Creates directory and file
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(@args[0]));
                            StreamWriter newFile = File.CreateText(@args[0]);
                            newFile.Close();
                            start.StartMyThreads(args[0], size); // Starts the threads
                        }
                        catch (ArgumentException)
                        {
                            Console.WriteLine("Path cannot be empty string or all whitespace.\n");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("A exception was caught: {0}.\n", ex);
                        }
                    }
                }
            }
        }
    }
}

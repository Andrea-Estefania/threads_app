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
    class MyFile
    {
        private float fileSize;
        private bool okayToWrite = true;
        private string pathname;

        public float FileSize
        {
            get 
            {
                return fileSize;
            }
            set
            { 
                fileSize = value; 
            }
        }

        public string PathName
        {
            get
            {
                return pathname;
            }
            set
            {
                pathname = value;
            }
        }

        public bool OkayToWrite
        {
            get
            {
                return okayToWrite;
            }
            set
            {
                okayToWrite = value;
            }
        }

        /* 
        Name	: MyFile -- Constructor
        Purpose : to instantiate a new file object to keep track and store the information need for the program to write in the text file         
        Inputs	:	  string newFilePath            Pathname of the text file
                      float newSize                 Expected file of the file
        Outputs	:	NONE
        Returns	:	Nothing
        */

        public MyFile(string newFilePath, float newSize)
        {
            this.okayToWrite = true;
            this.fileSize = newSize;
            this.pathname = newFilePath;
        }

        /*
         TITLE        : Random String Generator Returning Same String
         AUTHOR       : RCIX
         DATE         : 2009 - 07 - 13
         VERSION      : N/A
         AVAILABILITY : https://stackoverflow.com/questions/1122483/random-string-generator-returning-same-string
         */

        /*  
       Name	    :   RandomData
       Purpose  :   It is used to generate the random data that will be appended to the text file
       Inputs	:	NONE
       Outputs	:	NONE
       Returns	:	string with the random data
        */

        public string RandomData()
        {
            StringBuilder theData = new StringBuilder();
            char ch;
            Random random = new Random((int)DateTime.Now.Ticks); // To help generate the random character

            for (int i = 0; i < Constants.kArraySize; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65))); // Generates the random character
                theData.Append(ch); // Appends the random character to the final string
            }

            return theData.ToString();
        }

        /*  
       Name	    :   WriteInFile
       Purpose  :   It is used to write in the file. It makes use of a lock so just one thread has access to the file at the same time.
       Inputs	:	NONE
       Outputs	:	NONE
       Returns	:	NONE
        */

        public void WriteInFile()
        { 
            string thePathName = this.@PathName;

                while (this.okayToWrite == true) // Is it okay to keep writing in the text file?
                {
                    lock (this)
                    {// Lock on the current instance of the class for current thread
                        StreamWriter appendData = new StreamWriter(thePathName, true);
                        string randomData = RandomData();
                    try
                        {
                            appendData.WriteLine(randomData); // Appends the random data to the file

                        }
                        catch (IOException ex) // An I/O was caught 
                        {
                            Console.WriteLine("An I/O error occurred while opening the file. Exception caught {0}\n", ex);
                        }
                        catch (Exception ex) // Regular exceptions
                        {
                            Console.WriteLine("Exception caught: {0}\n", ex);
                        }
                        finally
                        {
                            appendData.Close(); // Closes the file
                        }

                        long fileSize = new FileInfo(thePathName).Length; // Gets the current size of the file
                        float userSize = this.FileSize; // Size the user specified when the

                        if (fileSize >= userSize) // Have we reach the size the user wants?
                        {
                            this.OkayToWrite = false;
                        }
                    } // Lock released for the next thread here
                }
        }

        /*  
        Name	    :   CurrentSize
        Purpose     :   To print in the screen the current size of the file that's being written
        Inputs	    :	NONE
        Outputs	    :	Current size of the text file
        Returns	    :	NONE
        */

        public void CurrentSize()
        {
            string myPath = this.@PathName;
        
            while (this.okayToWrite == true) //Is okay to keep showing the message?
            {
                long fileSize = new FileInfo(myPath).Length; // Gets the current size of the file

                Console.WriteLine("The current file size is: {0} bytes.\n", fileSize.ToString());
                Thread.Sleep(1000); // Sleeps for one second

            }
        }

        /*  
        Name	    :   GetFinalSize
        Purpose     :   It is used to get the final size of the text file
        Inputs	    :	NONE
        Outputs	    :	NONE
        Returns	    :	The final size of the text file
        */

        public long GetFinalSize()
        {
            string myPath = this.@PathName;
            long finalSize = new FileInfo(myPath).Length;
            return finalSize;
        }
    }
}

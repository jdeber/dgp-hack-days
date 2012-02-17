/*********************************************************
 * The following class manages the logging functionality
 * for the DGP door bell project.
 *  
 * This class was initially created for another project, 
 * so includes functionality not used by the door bell 
 * project.
 * 
 * By: Rorik Henrikson
 * Date: Oct. 7th, 2011
 *********************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FreeFoodButton
{
    /// <summary>
    /// Class for storing log information.  
    /// 
    /// Used when buffering is turned on
    /// </summary>
    class LogInfo
    {
        private string m_Time;      //time of log message
        private string m_Message;   //message content

        //copnstructor
        public LogInfo(string time, string message)
        {
            Time = time;
            Message = message;
        }

        /// <summary>
        /// Time of log event
        /// </summary>
        public string Time
        { 
            set { m_Time = value; }
            get { return m_Time; }
        }

        /// <summary>
        /// Message to be logged
        /// </summary>
        public string Message
        {
            set { m_Message = value; }
            get { return m_Message; }
        }
    }

    /// <summary>
    /// Class that manages logging during execusion
    /// </summary>
    class Log
    {
        public enum Categories { Interaction, Email, System, Unknown }

        #region VARIABLES

        static string m_stLogFile = "Log.txt";                  //log file name
        static bool m_bBuffer = false;                          //flag indicating if buffering is being used
        static ArrayList m_LogEntries = new ArrayList();        //array list of messages stored while buffering
        static bool m_bAutoDate = true;                         //flag indicating if it should write the year/month/day value
                                                                //if it is different from last message
        static DateTime m_lastDate = new DateTime(1900, 01, 01);//default date

        #endregion

        /// <summary>
        /// Function writes the date to the log file
        ///  
        /// This function is provided as the formatting is different to regular logging
        /// 
        /// Format of message:
        /// [Date: dd/mm/yyyy]
        /// </summary>
        static public void WriteDate()
        {
            //if we're buffering information
            if (Buffer)
            {
                //add date stamp to buffer
                m_LogEntries.Add(new LogInfo(DateTime.Now.ToString("HH:mm:ss.fff"), "[Date: " + DateTime.Now.ToString("d") + "]"));
            }
            else
            {
                //open file for appending
                using (StreamWriter sw = File.AppendText(m_stLogFile))
                {
                    //write the date
                    sw.WriteLine("[Date: " + DateTime.Now.ToString("d") + "]");

                    //close the file
                    sw.Close();
                }

                //note the last date an entry was made
                m_lastDate = DateTime.Now;                
            }
        }

        static public void Write(string Info, Categories cat)
        {
            string pre = "";

            switch(cat)
            {
                case Categories.Email:
                    pre = "[E] ";
                    break;
                case Categories.Interaction:
                    pre = "[I] ";
                    break;
                case Categories.System:
                    pre = "[S] ";
                    break;
                case Categories.Unknown:
                    pre = "[U] ";
                    break;
            }

            Write(pre + Info);
        }

        /// <summary>
        /// Function to write log comment
        /// 
        /// Format of message being written:
        /// HH:mm:ss.fff > message
        /// </summary>
        /// <param name="Info">message to be logged</param>
        static public void Write(string Info)
        {
            //if we're buffering, store the entry
            if (Buffer)
            {
                //save entry in array
                m_LogEntries.Add(new LogInfo(DateTime.Now.ToString("HH:mm:ss.fff"), Info));
            }
            else
            {
                //if we're auto dating, and the day/month/year is different than last message
                if((m_bAutoDate) && ((DateTime.Now.Year != m_lastDate.Year) || (DateTime.Now.Month != m_lastDate.Month) ||
                    (DateTime.Now.Day != m_lastDate.Day)))
                {
                    //write the date
                    WriteDate();
                }

                //open the file for appending
                using (StreamWriter sw = File.AppendText(m_stLogFile))
                {                    
                    //write the message to file
                    sw.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff") + " > " + Info);

                    //note the current date
                    m_lastDate = DateTime.Now;

                    //close the file
                    sw.Close();
                }
            }
        }

        /// <summary>
        /// Gunction used while buffering to write any message that is stored in the buffer
        /// to file.
        /// </summary>
        static public void Commit()
        {
            //if we're buffering write the values - if not, there shouldn't be anything in the 
            //buffer
            if (Buffer)
            {
                //open file for append
                using (StreamWriter sw = File.AppendText(m_stLogFile))
                {
                    //for each log entry buffered
                    foreach (LogInfo i in m_LogEntries)
                    {
                        //if string is a special line
                        if (i.Message.IndexOf('[') == 0)
                            //write just the special line - no time stamp
                            sw.WriteLine(i.Message);
                        else
                            //write the message
                            sw.WriteLine(i.Time + " > " + i.Message);
                    } 

                    //close the file
                    sw.Close();
                }

                //clear the buffer
                m_LogEntries.Clear();
            }
        }


        #region PROPERTIES
        /// <summary>
        /// The file name of the log file
        /// </summary>
        static public string FileName
        {
            set
            {
                m_stLogFile = value;
            }
            get
            {
                return m_stLogFile;
            }
        }

        /// <summary>
        /// Flag to turn on/off the buffer
        /// </summary>
        static public bool Buffer
        {
            set 
            {
                //clear anything that may be remaining in the buffer
                m_LogEntries.Clear();

                //set the value
                m_bBuffer = value; 
            }
            get { return m_bBuffer; }
        }

        /// <summary>
        /// Flag to indicate if log should automatically write a date entry
        /// if it's different from last entry
        /// </summary>
        static public bool AutoDate
        {
            set{ m_bAutoDate = value; }
            get { return m_bAutoDate; }
        }

        /// <summary>
        /// The value of the last date a date entry was written
        /// </summary>
        static public DateTime LastDate
        {
            set { m_lastDate = value; }
            get { return m_lastDate; }
        }

        #endregion
    }
}

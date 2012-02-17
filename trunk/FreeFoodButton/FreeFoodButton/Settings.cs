/*********************************************************
 * The following class loads and provides the global 
 * settings for the DGP door bell project.
 * 
 * By: Rorik Henrikson
 * Date: Oct. 7th, 2011
 *********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace FreeFoodButton
{
    /// <summary>
    /// A static class to load and store global settings for project
    /// 
    /// Current variables available:
    ///     - Debug: flag to indicate debugging mode (true = debug; flase = not debug)
    ///     - LogNotSent: flag to indicate if "Not Sent" events should be logged (true = log; false = don't log)
    ///     - Timeout: number of seconds that must pass before an email can be sent to a specific user again
    /// </summary>
    class Settings
    {
        static private bool m_Debug = false;            //flag for debug mode
        static private bool m_LogNotSent = false;       //flag to indicate if we should log "not sent" events
        static private int m_Timeout = 60;              //number of seconds before an email can be sent again to that user
        static private bool m_SettingsLoaded = false;
        static private string m_stEmail = "";
        static private string m_stUsername = "";
        static private string m_stPassword = "";
        static private string m_stDomain = "";
        static private string m_stBcc = "";
        static private string m_stMailServer = "";


        /// <summary>
        /// Default function for loading settings from file
        /// </summary>
        static public void LoadSettings()
        {
            LoadSettings("Settings.txt");
        }

        /// <summary>
        /// Function to load settings from file
        /// </summary>
        static public void LoadSettings(string filename)
        {
            try
            {
                //open the settings file for read
                using (StreamReader sr = new StreamReader(filename))
                {
                    string line;        //variable for given line
                    string[] parts;     //string array for parts of line

                    //while there are lines in the file
                    while ((line = sr.ReadLine()) != null)
                    {
                        //split the line
                        parts = line.Split('=');

                        parts[0] = parts[0].Trim();
                        parts[1] = parts[1].Trim();

                        //check to see if the part is someting we're interested in
                        switch (parts[0].Trim())
                        {
                            case "Debug":  
                                //if the value is 1, make it true, else false
                                if (Int32.Parse(parts[1]) == 1)
                                    m_Debug = true;
                                else
                                    m_Debug = false;
                                break;
                            case "Timeout": 
                                //store number of seconds
                                m_Timeout = Int32.Parse(parts[1]);
                                break;
                            case "LogNotSent":  
                                //if the value is 1, make it true, else false
                                if (Int32.Parse(parts[1]) == 1)
                                    m_LogNotSent = true;
                                else
                                    m_LogNotSent = false;
                                break;
                            case "Email":
                                //stores the debug email address
                                m_stEmail = parts[1];
                                break;
                            case "username":
                                //stores email server user name
                                m_stUsername = parts[1];
                                break;
                            case "password":
                                //stores email server password
                                m_stPassword = parts[1];
                                break;
                            case "domain":
                                //stores email address domain
                                m_stDomain = parts[1];
                                break;
                            case "bcc":
                                //stores bcc names
                                m_stBcc = parts[1];
                                break;
                            case "mailserver":
                                //stores mail server address
                                m_stMailServer = parts[1];
                                break;
                        }
                    }
                }
            }
            catch (Exception e)  //error reading file
            {
                //show error
                System.Diagnostics.Debug.WriteLine("Error: " + e.Message);
            }
            finally
            {
                //regardless, we've tried to load the settings, so change flag
                m_SettingsLoaded = true;
            }
        }

        #region PROPERTIES
        /// <summary>
        /// flag indicating debug mode
        /// </summary>
        static public bool Debug
        {
            get 
            {
                //if the settings haven't been loaded, load them
                if (!m_SettingsLoaded) LoadSettings();

                //return value
                return m_Debug; 
            }
            set { m_Debug = value; }
        }

        /// <summary>
        /// number of seconds to timeout before next send
        /// </summary>
        static public int Timeout
        {
            get 
            {
                //if the settings haven't been loaded, load them
                if (!m_SettingsLoaded) LoadSettings();

                //return value
                return m_Timeout; 
            }
            set { m_Timeout = value; }
        }

        /// <summary>
        /// flag indicating if "Not Sent" messages being logged
        /// </summary>
        static public bool LogNotSent
        {
            get 
            {
                //if the settings haven't been loaded, load them
                if (!m_SettingsLoaded) LoadSettings();

                //return value
                return m_LogNotSent; 
            }
            set { m_LogNotSent = value; }
        }

        /// <summary>
        /// string containing email to use when in debug mode
        /// </summary>
        static public string Email
        {
            get
            {
                //if the settings haven't been loaded, load them
                if (!m_SettingsLoaded) LoadSettings();

                //return value
                return m_stEmail;
            }
            set { m_stEmail = value; }
        }

        /// <summary>
        /// String containing email server user name
        /// </summary>
        static public string UserName
        {
            get
            {                
                //return value
                return m_stUsername;
            }
            set{ m_stUsername = value;}            
        }

        /// <summary>
        /// String containing email server password
        /// </summary>
        static public string Password
        {
            get
            {
                //return value
                return m_stPassword;
            }
            set { m_stPassword = value; }
        }

        /// <summary>
        /// String containing email address domain
        /// </summary>
        static public string Domain
        {         
            get
            {                
                //return value
                return m_stDomain;
            }
            set{ m_stDomain = value;}            
        }

        /// <summary>
        /// String containing email addresses to BCC
        /// </summary>
        static public string Bcc
        {
            get
            {
                return m_stBcc;
            }
            set { m_stBcc = value; }
        }

        /// <summary>
        /// String containing email server address
        /// </summary>
        static public string Mailserver
        {
            get
            {
                return m_stMailServer;
            }
            set { m_stMailServer = value; }
        }
        #endregion
    }
}

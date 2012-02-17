 /*********************************************************
 * The following class manages the emailing functionality
 * for the DGP door bell project.
 * 
 * By: Rorik Henrikson
 * Date: Oct. 7th, 2011
 *********************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.Collections;
using System.Diagnostics;

namespace FreeFoodButton
{
    /// <summary>
    /// Class to handle email specific info
    /// </summary>
    class MessageInfo
    {
        private string m_ToAddress = "";            //local variable to store "to address"
        private DateTime m_Date = DateTime.Now;     //local variable to store time mail was sent

        //constructor
        public MessageInfo(string address, DateTime date)
        {
            m_ToAddress = address;
            m_Date = date;
        }

        /// <summary>
        /// Address stored by object
        /// </summary>
        public string Address
        {
            get { return m_ToAddress; }
        }
        /// <summary>
        /// Date stored by object
        /// </summary>
        public DateTime Date
        {
            get { return m_Date; }
        }
    }

    /// <summary>
    /// Class to send email and limit frequency of send
    /// </summary>
    static class Email
    {
        //array to store list of already sent
        static private ArrayList m_MessageList = new ArrayList();

        /// <summary>
        /// Function clears out "old" emails in sent list
        /// </summary>
        static private void clearOld()
        {
            DateTime cur = DateTime.Now;  //get current date time
            try
            {
                //fore each message that's in the sent list
                foreach (MessageInfo m in m_MessageList)
                {
                    //if the time that has passed is greater than the timeout setting
                    if ((cur - m.Date).TotalSeconds > Settings.Timeout)
                    {
                        //remove it from the list
                        m_MessageList.Remove(m);
                    }
                }
            }
            catch(Exception e)  //issue removing message
            {
                Debug.WriteLine("Error: " + e.Message);
            }
        }


        /// <summary>
        /// Function to check if person has already been emailed in the last "timeout" seconds
        /// </summary>
        /// <param name="ToAddress">string indicating which address we're looking for</param>
        /// <returns></returns>
        static private bool InList(string ToAddress)
        {
            bool exists = false;    //start of with address not found

            try
            {
                //fore each item in the list, see if email address exists
                foreach (MessageInfo m in m_MessageList)
                {
                    //if it exists flag this
                    if (m.Address == ToAddress) exists = true;

                    //break out of look if it's already been found
                    if (exists) break; 
                }
            }
            catch (Exception e) //issue in search
            {
                Debug.WriteLine("Error: " + e.Message);
            }

            //return if item exists or not
            return exists;
        }
        
        /// <summary>
        /// Function splits up multiple email addresses
        /// </summary>
        /// <param name="ToAddress">email address of person message should be sent to</param>
        /// <param name="Subject">subject of email</param>
        /// <param name="Body">text of body of email</param>
        static public int SendEmail(string ToAddress, string Subject, string Body)
        {
            int r = 0;
            string[] addresses;

            //split addresses into array
            addresses = ToAddress.Split(',');

            //for each email address, mail a copy
            foreach (string s in addresses)
            {
                //call actual send email code
                r = DoEmail(s.Trim(), Subject, Body);
            }

            return r;
        }

        /// <summary>
        /// Function send an email and picture
        /// </summary>
        /// <param name="ToAddress">email address of person message should be sent to</param>
        /// <param name="Subject">subject of email</param>
        /// <param name="Body">text of body of email</param>
        static private int DoEmail(string ToAddress, string Subject, string Body)
        {         
            //returns 0 if properly delivered.
            //returns -1 if other error.
            //returns 1 if already sent.

            //clear old "sent" door bell notificiations
            clearOld();

            //if this address has been sent to recently, don't send it
            if (InList(ToAddress))
            {
                if (Settings.LogNotSent)
                    Log.Write("Message already sent: " + ToAddress);

                Debug.WriteLine("Message already sent");
                
                return 1;
            }

            SmtpClient smtp = new SmtpClient();                                         //smtp server object
            MailMessage message = new MailMessage();                                    //message object
            MailAddress address = new MailAddress(ToAddress);                           //to address object
            NetworkCredential myCreds = new NetworkCredential(Settings.UserName, Settings.Password);  //default credentials

            //provide message info for email
            message.From = new MailAddress("FreeFoodButton@" + Settings.Domain, "Free Food");            
            message.Subject = Subject;
            message.Body = Body;
            message.IsBodyHtml = true;

            if (!Settings.Debug)
            {
                message.To.Add(address);
            }
            else
            {
                message.Body += " - intended for : " + address;
                message.To.Add(Settings.Email);
            }

            //split names in the bcc list
            if (Settings.Bcc.Length != 0)
            {
                string[] bcclist = Settings.Bcc.Split(',');

                //add all bcc names to the email
                foreach (string bcc in bcclist)
                    message.Bcc.Add(bcc.Trim());
            }

            //try sending the message
            try
            {
                //provide smtp info for server
                smtp.Host = Settings.Mailserver;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = myCreds;

                //send the message
                smtp.Send(message);

                //log results
                if (!Settings.Debug)
                    Log.Write("Message successfully sent to: " + ToAddress);
                else
                    Log.Write("Message successfully sent to: " + Settings.Email + " instead of " + ToAddress);

                //store that person has been emailed
                m_MessageList.Add(new MessageInfo(ToAddress, DateTime.Now));
            }
            catch (Exception e)
            {
                //log error
                Debug.WriteLine("Message: {0}", e.Message);
                Log.Write("Error sending message to: " + ToAddress + ".  Message: " + e.Message);
                return -1;
            }
            return 0;
        }
    }
}

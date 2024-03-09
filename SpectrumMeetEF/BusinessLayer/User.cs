using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectrumMeetEF
{
    public partial class User
    {
        public SpectrumMeetEntities db = new SpectrumMeetEntities();
        //Simple get statement to check if the user is marked as an administrator
        //Uses the users account ID stored in the database, to be used alongside
        //Session["AccountID"] when defining a user profile.
        public bool IsAdmin { 
            get
            { 
                if(db.Accounts.Find(AccountID).RoleID == 1)
                { 
                    return true; 
                } 
                else 
                { 
                    return false; 
                } 
            } 
        }
        //Gets every message a user has posted and returns them in a list
        //format
        public List<Message> postedMessages()
        {
            List<Message> messages = new List<Message>();
            foreach (var message in db.Messages.Where(m => m.AccountID == AccountID)) 
            { 
                messages.Add(message);
            }
            return messages;
        }
        //Gets every message from a user's postedMessages that has been *replied to*
        //TODO: Add "read" and "unread" status to replies
        public List<Message> getReplies() 
        {
            List<Message> messages = new List<Message>();
            foreach (var message in postedMessages())
            {
                foreach (var reply in db.Messages.Where(m=>m.ParentMessageID == message.MessageID))
                {
                    messages.Add(reply);
                }
            }
            return messages;
        }
        //Simple count function to get the amount of unread replies
        //TODO: Same as above, unread stuff only instead of every reply ever
        public int getReplyCount()
        {
            int count = getReplies().Count();
            return count;
        }
    }
}

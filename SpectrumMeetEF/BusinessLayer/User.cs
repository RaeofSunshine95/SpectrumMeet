using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        //dalia added this 

        public string ProfilePicturePath { get; set; }
        //Gets every message a user has posted and returns them in a list
        //format
        public List<Message> PostedMessages()
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
        //DONE: Modified the database to have a read status, this function only matters for reply count
        //Collapsed count and new reply functions since the replies page should show all replies regardless of read status
        public int GetNewReplyCount() 
        {
            List<Message> messages = new List<Message>();
            foreach (var message in PostedMessages())
            {
                foreach (var reply in db.Messages.Where(m=>m.ParentMessageID == message.MessageID))
                {
                    if (reply.MessageReadStatus == true)
                    {
                        messages.Add(reply);
                    }
                }
            }
            int count = messages.Count();
            return count;
        }
        //Copied above function but for returning a list of replies instead of the amount of new ones
        //The message status can be handled within the view itself (bold/unbolding certain message links)
        public List<Message> GetReplies()
        {
            List<Message> messages = new List<Message>();
            foreach (var message in PostedMessages())
            {
                foreach (var reply in db.Messages.Where(m => m.ParentMessageID == message.MessageID))
                {
                    messages.Add(reply);
                }
            }
            messages.OrderByDescending(m => m.PostedDate);
            return messages;
        }

        public bool ChildrenHaveCondition(int id)
        {
            List<ParentChild> parentchilds = db.ParentChilds.Where(p=>p.AccountID == AccountID).ToList();
            foreach (var pc in parentchilds)
            {
                foreach (var condition in pc.Child.ChildConditions)
                {
                    if (condition.ConditionID == id)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

    }
}

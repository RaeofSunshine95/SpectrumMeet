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
        public bool IsActive { get; set; }
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

        public List<Message> postedMessages()
        {
            List<Message> messages = new List<Message>();
            foreach (var message in db.Messages.Where(m => m.AccountID == AccountID)) 
            { 
                messages.Add(message);
            }
            return messages;
        }
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
        public int getReplyCount()
        {
            int count = getReplies().Count();
            return count;
        }
    }
}

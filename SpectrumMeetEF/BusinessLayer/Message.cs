using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Migrations;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectrumMeetEF
{
    public partial class Message
    {
        public SpectrumMeetEntities db = new SpectrumMeetEntities();
        
        //Function takes two parameters, the message it's modifying and the account (found via session id)
        //Controller action will call this function using the session's accountID
        public void MarkAsRead(Message m, Account a)
        {
            //first we check if this function even applies in the current context, for sanity
            //if the message in question doesn't have a parent message its read status does not matter
            if (m.ParentMessageID != null)
            {
                //we find the parent message so that we can find the accountID associated with it
                Message parentMessage = db.Messages.Find(m.ParentMessageID);
                //if it matches with our input account we continue through, otherwise nothing happens
                if (parentMessage.AccountID == a.AccountID)
                {
                    //then we check if the read status is even necessary to update
                    if (m.MessageReadStatus != true)
                    {
                        //if it is, we set it to true, marking the message as read, and update the database
                        m.MessageReadStatus = true;
                        db.Messages.AddOrUpdate(m);
                    }
                }  
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net.NetworkInformation;
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
        public List<Message> GetAllChildren()
        {
            //Okay let's try to break this down.
            //First I make an empty list for later
            List<Message> children = new List<Message>();
            //Then I create an sqlparameter for use in the upcoming database query
            SqlParameter anchorId = new SqlParameter("@anchorId", MessageID);
            //I make the query for the current message's ID and it returns to me all children in the hierarchy
            var rh = db.Database.SqlQuery<Message>("EXEC RecursiveHierarchy @anchorId", anchorId);
            //Unfortunately they aren't actually message objects in this form, so we have to iterate through the
            //result and make all the properties ourselves. Hacky but functional.
            foreach (var child in rh)
            {
                Message m = new Message();
                m.GroupID = child.GroupID;
                m.AccountID = child.AccountID;
                m.MessageID = child.MessageID;
                m.Title = child.Title;
                m.Content = child.Content;
                m.ParentMessageID = child.ParentMessageID;
                m.PostedDate = child.PostedDate;
                m.MessageReadStatus = child.MessageReadStatus;
                m.User = db.Users.Find(child.AccountID);
                children.Add(m);
            }
            //I then group the children by parent message ID, probably not useful so I might remove it.
            children.GroupBy(m => m.ParentMessageID);
            //I make a new list of the IDs of all messages that should be parents
            List<int> ParentIDs = new List<int>();
            //Populate the list with everything that isn't a direct descendant or already on the list
            foreach (var child in children)
            {
                if (child.ParentMessageID != MessageID && !ParentIDs.Contains((int)child.ParentMessageID))
                {
                    ParentIDs.Add((int)child.ParentMessageID);
                }
            }
            //Make a new list of messages that are supposed to be parents
            List<Message> ParentMessages = new List<Message>();
            //Populate that list by crossreferencing the IDs from earlier.
            foreach(var child in children)
            {
                foreach(var id in ParentIDs)
                {
                    if (child.MessageID == id)
                    {
                        ParentMessages.Add(child);
                    }
                }
            }
            //Make a copy of the children list so I can modify it as I choose, will be important later.
            List<Message> modifiedChildren = new List<Message>(children);
            //Make a list of messages that should be replies
            List<Message> replies = new List<Message>();
            //Populate that list with replies by parent message
            foreach (var parent in ParentMessages)
            {
                foreach (var child in children)
                {
                    if (child.ParentMessageID == parent.MessageID)
                    {
                        modifiedChildren.Remove(child);
                        replies.Add(child);
                    }
                }
            }
            //iterate through children one more time (this is probably wrong and I should use the length of modified children
            //so that the data is consistent
            //but it populates modified-children with the replies one ahead of its parent
            foreach (var child in children)
            {
                foreach(var reply in replies)
                {
                    if(reply.ParentMessageID == child.MessageID && !modifiedChildren.Contains(child))
                    {
                        modifiedChildren.Insert(modifiedChildren.IndexOf(child)+1, reply);
                    }
                }
            }
            //had to add this to filter out the duplicate messages appearing from the recursive query
            //this function sucks but I'm not changing it since it works now.
            //Hope this made even a slight amount of sense.
            //- Rachel
            for(int i=0;i<modifiedChildren.Count();i++)
            {
                if (modifiedChildren[i].ParentMessageID != MessageID)
                {
                    modifiedChildren.Remove(modifiedChildren[i]);
                }
            }
            return modifiedChildren;
        }          
    }
}

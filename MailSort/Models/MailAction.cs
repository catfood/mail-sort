
using System;
using System.Collections.Generic;
using System.Text;

namespace MailSort.Models
{
    public interface IMailAction
    {

    }

    public class NullMailAction: IMailAction
    {
        public void Null()
        {

        }
    }
}

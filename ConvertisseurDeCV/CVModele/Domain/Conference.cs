using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVModel.Domain
{
    [Serializable]
    public class Conference
    {
        public string Description { get; set; }

        private Conference()
        { }

        public static Conference CreateConference(string conference)
        {
            Conference conf = new Conference();
            conf.Description = conference;

            return conf;
        }
    }
}

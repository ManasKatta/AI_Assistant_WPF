using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManasKattaFinal
{
    class Message
    {
        public string Username { get; set; }
        public string UsernameColor { get; set; }
        public string ImageSource { get; set; }

        public string Content { get; set; }
        public Message(string name, string color, string img_path,string content )
        {
            this.Username = name;
            this.UsernameColor = color;
            this.ImageSource = img_path;
            this.Content = content;
        }
    }
}

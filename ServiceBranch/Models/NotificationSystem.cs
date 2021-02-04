using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceBranch.Models
{
    public class NotificationSystem
    {
        public string Message { set; get; }
        public string NotificationType { get; set; }
        //Creates the HTML string.
        //This outputs the div in HTML with the current message formatted. 
        public static string GenerateNotification(string Message = "", string Type = "success")
        {
            //Div Tag
            var divTag = new TagBuilder("div");
            divTag.AddCssClass("alert alert-dismissible show text-center alert-" + Type.ToString());
            divTag.Attributes.Add("role", "alert");
            divTag.InnerHtml.AppendHtml(Message + "<button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden ='true'> &times;</span></button>");
            var htmloutput = "";
            using (var writer = new StringWriter())
            {
                divTag.WriteTo(writer, System.Text.Encodings.Web.HtmlEncoder.Default);
                htmloutput = writer.ToString();
            }
            return htmloutput;
        }
        public static string AddMessage(string message, string type)
        {
            NotificationSystem m = new NotificationSystem();
            m.Message = message;
            m.NotificationType = type;
            return JsonConvert.SerializeObject(m);
        }
    }
    public class NotificationType
    {
        public string Value { get; set; }
        private NotificationType(string value) { Value = value; }

        public static NotificationType Success { get { return new NotificationType("success"); } }
        public static NotificationType Danger { get { return new NotificationType("danger"); } }
        public static NotificationType Warning { get { return new NotificationType("warning"); } }
        public static NotificationType Info { get { return new NotificationType("info"); } }
    }
}

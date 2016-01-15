using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace BearTT.Entity
{
    [Serializable]
    public class TTarget
    {
        public string Name { get; set; }
        public string Target { get; set; }
        public string Param { get; set; }
        public string Result { get; set; }
        public string HttpMethod { get; set; }
        public int Order { get; set; }

        public bool HoldFlag = true;

        //ILog log = LogManager.GetLogger("loginfo");

        public void ToTarget(object o)
        {
            while (HoldFlag)
            {
                DateTime startDateTime = DateTime.Now;
                string strURL = this.Target;
                System.Net.HttpWebRequest request;
                request = (System.Net.HttpWebRequest)WebRequest.Create(strURL);
                request.Method = this.HttpMethod.ToUpper();
                request.ContentType = "application/json;charset=UTF-8";
                string paraUrlCoded = this.Param;
                byte[] payload;
                payload = System.Text.Encoding.UTF8.GetBytes(paraUrlCoded);
                request.ContentLength = payload.Length;
                Stream writer = request.GetRequestStream();
                writer.Write(payload, 0, payload.Length);
                writer.Close();
                System.Net.HttpWebResponse response;
                response = (System.Net.HttpWebResponse)request.GetResponse();
                System.IO.Stream s;
                s = response.GetResponseStream();
                string StrDate = "";
                string strValue = "";
                StreamReader Reader = new StreamReader(s, Encoding.UTF8);
                while ((StrDate = Reader.ReadLine()) != null)
                {
                    strValue += StrDate + "\r\n";
                }
                //Console.WriteLine(strValue);


                //string dateDiff = null;
                //TimeSpan ts1 = new TimeSpan(DateTime.Now.Ticks);
                //TimeSpan ts2 = new TimeSpan(startDateTime.Ticks);
                //TimeSpan ts = ts1.Subtract(ts2).Duration();
                //dateDiff = ts.Hours.ToString() + "时" + ts.Minutes.ToString() + "分" + ts.Seconds.ToString() + "秒" + ts.Milliseconds.ToString() + "MS";

                //log.Info(dateDiff+"    Name: "+this.Name+"    result: "+strValue);
            }
        }
    }
}

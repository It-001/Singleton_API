using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace ClinicalTrialAPI.Custom
{
    public class GenerateXML
    {
        #region Singleton object
        private GenerateXML() { }
        private static readonly Lazy<GenerateXML> lazy = new Lazy<GenerateXML>(() => new GenerateXML());
        public static GenerateXML Instance
        {
            get { return lazy.Value; }
        }
        #endregion

        #region Function for convert Object to XML
        /// <summary>
        /// ToXmlString function for convert Object to XML
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>string</returns>
        public string ToXmlString(Object obj)
        {

            var stringwriter = new System.IO.StringWriter();
            var serializer = new XmlSerializer(obj.GetType());
            serializer.Serialize(stringwriter, obj);
            return stringwriter.ToString();
        }
        #endregion

       
    }
}
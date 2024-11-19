using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ClinicalTrialAPI.Custom
{
    public class GetSqlParameterFromClassProperty
    {
        public SqlParameter[] GenerateParameterfromProperty(object _obj, List<SetXmlwithClassProperty> _SetXmlwithClassProperty, List<string> SkipParameter)
        {
            int lastcount = 0;
            var _properties = _obj.GetType().GetProperties();
            List<SqlParameter> _sqlParaList = new List<SqlParameter>();
            for (int i = 0; i < _properties.Length; i++)
            {
                var _prop = _obj.GetType().GetProperty(_properties[i].Name);
                var _value = _prop.GetValue(_obj, null);
                var _type = _prop.PropertyType.Name;
                SqlParameter sqlPara;
                if (_type.Contains("List"))
                {
                    if (_SetXmlwithClassProperty != null)
                    {
                        string _xmlstring = _SetXmlwithClassProperty.SingleOrDefault(s => s.PropName == Convert.ToString(_prop.Name)).XmlString;
                        _SetXmlwithClassProperty.RemoveAll(item => item.PropName == _prop.Name);
                        if (!SkipParameter.Contains(_prop.Name))
                        {
                            sqlPara = new SqlParameter("@" + _prop.Name, _xmlstring);
                            _sqlParaList.Add(sqlPara);
                        }
                    }
                }
                else
                {
                    if (!SkipParameter.Contains(_prop.Name))
                    {
                        sqlPara = new SqlParameter("@" + _prop.Name, _value);
                        _sqlParaList.Add(sqlPara);
                    }
                }
                lastcount = i;
            }
            lastcount++;
            if (_SetXmlwithClassProperty != null)
            {
                for (int j = 0; j < _SetXmlwithClassProperty.Count; j++)
                {
                    SqlParameter _sqlPara;
                    _sqlPara = new SqlParameter("@" + _SetXmlwithClassProperty[j].PropName, _SetXmlwithClassProperty[j].XmlString);
                    _sqlParaList.Add(_sqlPara);
                    lastcount++;
                }
            }
            return _sqlParaList.ToArray();
        }

        public SqlParameter[] GenerateParameterfromProperty(object _obj, List<SetXmlwithClassProperty> _SetXmlwithClassProperty)
        {
            int lastcount = 0;
            var _properties = _obj.GetType().GetProperties();
            List<SqlParameter> _sqlParaList = new List<SqlParameter>();

            for (int i = 0; i < _properties.Length; i++)
            {
                var _prop = _obj.GetType().GetProperty(_properties[i].Name);
                var _value = _prop.GetValue(_obj, null);
                var _type = _prop.PropertyType.Name;
                SqlParameter sqlPara;
                if (_type.Contains("List"))
                {
                    if (_SetXmlwithClassProperty != null)
                    {
                        string _xmlstring = _SetXmlwithClassProperty.SingleOrDefault(s => s.PropName == Convert.ToString(_prop.Name)).XmlString;
                        _SetXmlwithClassProperty.RemoveAll(item => item.PropName == _prop.Name);
                        sqlPara = new SqlParameter("@" + _prop.Name, _xmlstring);
                        _sqlParaList.Add(sqlPara);
                    }

                }
                else
                {
                    sqlPara = new SqlParameter("@" + _prop.Name, _value);
                    _sqlParaList.Add(sqlPara);
                }
                lastcount = i;
            }
            lastcount++;
            if (_SetXmlwithClassProperty != null)
            {
                for (int j = 0; j < _SetXmlwithClassProperty.Count; j++)
                {
                    SqlParameter _sqlPara;
                    _sqlPara = new SqlParameter("@" + _SetXmlwithClassProperty[j].PropName, _SetXmlwithClassProperty[j].XmlString);
                    _sqlParaList.Add(_sqlPara);
                    lastcount++;
                }
            }
            return _sqlParaList.ToArray();
        }

        public SqlParameter[] GenerateParameterfromProperty(object _obj)
        {
            var properties = _obj.GetType().GetProperties();
            SqlParameter[] _sqlPara = new SqlParameter[properties.Length];
            for (int i = 0; i < properties.Length; i++)
            {
                var _prop = _obj.GetType().GetProperty(properties[i].Name);
                var _value = _prop.GetValue(_obj, null);
                var _type = _prop.PropertyType.Name;
                if (!_type.Contains("List"))
                {
                    _sqlPara[i] = new SqlParameter("@" + _prop.Name, _value);
                }
            
            }
            return _sqlPara;
        }
    }
}
﻿using Javor.SipSerializer.Attributes;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Javor.SipSerializer.HeaderFields
{
    /// <summary>
    ///     Default sip headers valid for all types of sip messages.
    /// </summary>
    public class StandardHeaders
    {
        // TODO compact forms

        [HeaderName(HeaderFieldsNames.From)]
        //public  From { get; set; }
        public string From { get; set; }
         
        [HeaderName(HeaderFieldsNames.To)]
        //public To To { get; set; }
        public string To { get; set; }
        
        [HeaderName(HeaderFieldsNames.CallId)]
        //public Guid CallId { get; set; }    // TODO call id muze vypadat nasledovne, callid@host, umoznit uzivateli zadat host
        public string CallId { get; set; }
        
        [HeaderName(HeaderFieldsNames.MaxForwards)]
        //public int MaxForwards { get; set; }
        public string MaxForwards { get; set; }

        [HeaderName(HeaderFieldsNames.Via)]
        public ICollection<Via> Via {
            get
            {
                return _via;
            }
            set
            {
                if (_via == null)
                {
                    _via = value;
                }
                else
                {
                    foreach (Via via in value)
                    {
                        _via.Add(via);
                    }
                }
            }
        }
        private ICollection<Via> _via;
        
        [HeaderName(HeaderFieldsNames.Cseq)]
        //public CSeq Cseq { get; set; }
        public CSeq CSeq { get; set; }

        [HeaderName(HeaderFieldsNames.Contact, HeaderFieldsNames.ContactCompactForm)]
        //public IEnumerable<Contact> Contact { get; set; }
        public string Contact { get; set; }
        
        [HeaderName(HeaderFieldsNames.Require)]
        public IEnumerable<string> Require { get; set; }

        [HeaderName(HeaderFieldsNames.Supported)]
        //public IEnumerable<string> Supported { get; set; }
        public string Supported { get; set; }
        
        [HeaderName(HeaderFieldsNames.Unsupported)]
        //public IEnumerable<string> Unsupported { get; set; }
        public string Unsupported { get; set; }

        [HeaderName(HeaderFieldsNames.ContentLength)]
        public int ContentLength { get; set; }

        //public IEnumerable<string> UnknownHeaders { get; set; }

        /// <summary>
        ///     Convert SIP headers into the string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            PropertyInfo[] properties = typeof(StandardHeaders)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(this);
                if (value != null)
                {
                    HeaderNameAttribute headerName =
                        (HeaderNameAttribute)property.GetCustomAttribute(typeof(HeaderNameAttribute));

                    // TODO I dont like this implementation :(
                    if (value.GetType() == typeof(string))
                    {
                        // deal with classic strings
                        sb.Append(headerName.GetHeaderFullName());
                        sb.Append(':'); // TODO to ABRNF?
                        sb.Append(value);
                        sb.AppendLine();
                    }
                    else if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                    {
                        // deal with other enums (string inherits from enum, for that reason is 
                        //  string type control first and other enum type control 
                        //  (which using foreach loop) second)
                        foreach (var item in (IEnumerable)value)
                        {
                            sb.Append(headerName.GetHeaderFullName());
                            sb.Append(':'); // TODO to ABRNF?
                            sb.Append(item.ToString());
                            sb.AppendLine();
                        }
                    }
                    else if (value.GetType() == typeof(int))
                    {
                        string converted = value.ToString(); // TODO optimalizace!!!! zbytecne opakuju kod

                        sb.Append(headerName.GetHeaderFullName());
                        sb.Append(':'); // TODO to ABRNF?
                        sb.Append(value);
                        sb.AppendLine();
                    }
                }
            }

            return sb.ToString();
        }
    }
}
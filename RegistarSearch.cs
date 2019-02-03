using System;
using System.IO;
using System.Net;
using HtmlAgilityPack;
using System.Collections.Generic;

namespace oibregistarhack
{
    class RegistarSearch
    {

        private const string TARGET_URL = "https://sudreg.pravosudje.hr/registar/";
        private const string SEARCH_FIELD_NAME = "p_t05";
        private const string REQUEST_KEYWORD = "p_request";
        private const string REQUEST_VALUE = "PRETRAZI";

        private struct response {
            public int Status;
            public string Body;
            public WebHeaderCollection Headers;
        }

        private struct item {
            public item(string key, string value){
                this.Key = key;
                this.Value = value;
            }
            public string Key;
            public string Value;
        }

        public RegistarSearch()
        {
        }

        public string Results(string term)
        {
            response res = this.say_hello();

            if( res.Status > 0 ){

                // magic is here, parse content of original page to extract details!
                var doc = new HtmlDocument();
                doc.LoadHtml( res.Body );

                string[] fields = new string[]{ "input", "select" };
                List<item> vals = new List<item>();
                // first find all input fields
                foreach( string target in fields ){
                    foreach( var node in doc.DocumentNode.SelectNodes("//" + target ) ){
                        try {
                            Console.WriteLine( node.Attributes["name"].Value + " -> " + node.Attributes["value"].Value );
                        }
                        catch(Exception){
                            Console.WriteLine( "Have an issue with node, inspect docs, maybe there exists contains" );
                            continue;
                        }

                        string val;
                        switch( node.Attributes["name"].Value ){
                            case RegistarSearch.SEARCH_FIELD_NAME:
                                val = term;
                                break;
                            case RegistarSearch.REQUEST_KEYWORD:
                                val = RegistarSearch.REQUEST_VALUE;
                                break;
                            default:
                                val = node.Attributes["value"].Value;
                                break;
                        }
                        vals.Add( new item( node.Attributes["name"].Value, val ) );
                    }
                }

                Console.WriteLine( vals.Count );

            }

            return "-";
        }


        private response say_hello()
        {
            response r = new response();
            HttpWebRequest req = (HttpWebRequest) WebRequest.Create( RegistarSearch.TARGET_URL + "f?p=150:1" );
            
            try{
                HttpWebResponse res = (HttpWebResponse) req.GetResponse();
                Stream data = res.GetResponseStream();
                using( StreamReader sr = new StreamReader(data) ){
                    r.Body = sr.ReadToEnd();
                }
                r.Headers = res.Headers;
                r.Status  = (int) res.StatusCode;
            }
            catch(Exception ex){
                Console.WriteLine( "Exception during server hello method" );
                Console.WriteLine( ex );
                r.Status = -1;
            }
            return r;
        }

    }
}
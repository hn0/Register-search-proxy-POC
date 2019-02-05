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
        private const string SELECT1_KEYWORD = "p_t04";
        private const string SELECT1_VALUE = "app_begins";
        private const string SELECT2_KEYWORD = "p_t07";
        private const string SELECT2_VALUE = "0";

        private struct response {
            public int Status;
            public string Body;
            public string Cookie;
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
                List<item> vals = new List<item>();
                var doc = new HtmlDocument();
                doc.LoadHtml( res.Body );
                 foreach( var node in doc.DocumentNode.SelectNodes("//*[self::input or self::select]" ) ){
                    try {
                        string a = node.Attributes["name"].Value;
                    }
                    catch(Exception){
                        // Console.WriteLine( "Have an issue with node, inspect docs, maybe there exists contains" );
                        continue;
                    }

                    string val;
                    try{
                        // Console.WriteLine( node.Attributes["name"].Value + " -> " + node.Attributes["value"].Value );
                        val = node.Attributes["value"].Value;
                    }
                    catch(Exception){
                        // Console.WriteLine( node.Attributes["name"].Value + "=>" + node.InnerText );
                        val = "";
                    }

                    switch( node.Attributes["name"].Value ){
                        case RegistarSearch.SEARCH_FIELD_NAME:
                            val = term.ToLower();
                            break;
                        case RegistarSearch.REQUEST_KEYWORD:
                            val = RegistarSearch.REQUEST_VALUE;
                            break;
                        case RegistarSearch.SELECT1_KEYWORD:
                            val = RegistarSearch.SELECT1_VALUE;
                            break;
                        case RegistarSearch.SELECT2_KEYWORD:
                            val = RegistarSearch.SELECT2_VALUE;
                            break;    
                        default:
                            break;
                    }
                    vals.Add( new item( node.Attributes["name"].Value, val ) );
                }

                // Console.WriteLine( vals.Count );
                // foreach( item itm in vals ){
                //     Console.WriteLine( itm.Key + " -> " + itm.Value );
                // }

                response redirected = this.query(vals, res.Cookie);
                if( redirected.Status == 200 ){
                    return parse_results( redirected.Body );
                }

            }

            return "-";
        }


        private string parse_results(string htmlBody){

            List<string> results = new List<string>();
            var doc = new HtmlDocument();
            doc.LoadHtml( htmlBody );
            // foreach( var node in doc.DocumentNode.SelectNodes("//*[self::input or self::select]" ) ){
            foreach( var node in doc.DocumentNode.SelectNodes("//table" ) ){

                try{
                    string _ = node.Attributes["id"].Value;
                }
                catch(Exception){
                    continue;
                }

                if( node.Attributes["id"].Value.StartsWith( "report" ) ){
                    foreach( var row in node.SelectNodes( "//tr" ) ){

                        // agility pack is missing any kind of documentation so, try catch is the easiest way to go
                        try{
                            if( row.Attributes["class"].Value == "highlight-row" ){
                                var tds = row.ChildNodes;                                
                                results.Add( String.Format( "{0}: {1}({2})", tds[0].InnerText, tds[4].InnerText, tds[5].InnerText ) );
                            }
                        }
                        catch(Exception){ 
                            // Console.WriteLine( ex );
                        }
                    }
                }
            }

            if( results.Count > 0 ){
                results.Insert(0, "MBS:  Title(status)");
                return String.Join( "\n", results.ToArray() );
            }

            return "No results found";
        }

        private response query(List<item> vals, string cookie) {
            response r = new response();

            string data = "";
            foreach( item itm in vals ){
                data += "&" + itm.Key + "=" + WebUtility.UrlEncode( itm.Value );
            }
            HttpWebRequest req = (HttpWebRequest) WebRequest.Create( RegistarSearch.TARGET_URL + "wwv_flow.accept" );
            req.Method = "POST";
            req.Headers["Connection"] = "keep-alive";
            req.Headers["Pragma"] = "no-cache";
            req.Headers["Cache-Control"] = "no-cache";
            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Headers["Origin"] = "https://sudreg.pravosudje.hr";
            req.Headers["Referer"] = "https://sudreg.pravosudje.hr/registar/f?p=150:1";
            req.Headers["Cookie"] = cookie;
            using( var sw = new StreamWriter( req.GetRequestStream() ) ) {
                sw.Write( data.Substring(1) );
                sw.Flush();
                sw.Close();
            }

            // Console.WriteLine( "second request!" );
            // Console.WriteLine( data );
            try{
                HttpWebResponse res = (HttpWebResponse) req.GetResponse();
                Console.WriteLine( res.StatusCode );
                Stream resp_data = res.GetResponseStream();
                using( StreamReader sr = new StreamReader(resp_data) ){
                    r.Body = sr.ReadToEnd();
                    // Console.WriteLine( sr.ReadToEnd() );
                }
                r.Status  = (int) res.StatusCode;
            }
            catch(Exception ex){
                Console.WriteLine( "Exception during server hello method" );
                Console.WriteLine( ex );
                r.Status = -1;
            }

            return r;
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
                r.Cookie = res.Headers["Set-Cookie"];
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
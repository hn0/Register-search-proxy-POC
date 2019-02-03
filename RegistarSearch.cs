using System;

namespace oibregistarhack
{
    class RegistarSearch
    {

        private const string TARGET_URL = "https://sudreg.pravosudje.hr/registar/";
        private const string SEARCH_FIELD_NAME = "p_t05";
        private const string REQUEST_KEYWORD = "p_request";
        private const string REQUEST_VALUE = "PRETRAZI";

        public RegistarSearch()
        {
        }

        public string Results(string term){


            // is there anything outthere that might help!?
            // https://html-agility-pack.net/third-party-library

            return term;
        }
    }
}
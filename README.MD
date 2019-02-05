Register search proxy POC
=========================

Its a quick POC of searching, by name, records available at https://sudreg.pravosudje.hr/registar/ targeted for implementation in dotnet framework.

Project acts as a proxy for searching available records over cli. Instead of directly using web interface it uses the same for fetching results from web pages and displaying them in different interfaces (currently as a text in console).

Same approach can be used for implementing background service that could then provide data to other interfaces such as desktop applications, or even other web pages that do not have direct access to the database in question.


Dependencies
------------

Project is written using dotnet core framework with additional help of HtmlAgilityPack library (used for parsing html).
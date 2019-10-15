# DualSearch
Search through two search engines with a common output: Developer Technical Test for BlackDot Solution

## Presets:
* It is not specified where the search should be performed. It’s accepted that the search on the Internet.
* It is not specified which search engines should be used. Two well-known search engines are selected.
* Since complete freedom in the choice of language and technology was provided, a WinForm C# application using a WebBrowser control was chosen.
* Due to the purely technical reasons, the Vusual Studio 2010 was used.

## Brief comments:
* Since the task is demonstration, and not practical, the WebBrowser is used in simplified mode. Non-text items in the output are not filtered.
* To parse the search engines output, it is more correct to use specialized tools, such as [Lucene](http://lucenenet.apache.org/), [Solr](http://lucene.apache.org/solr/) etc. Howevere, in this case, due to minimal reformatting, it was decided to use my own simplified parsing.

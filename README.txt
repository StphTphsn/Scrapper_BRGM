# Scrapper_BRGM
Scrapper in C# adapted to BRGM website.

input: list of sample identifiers in a csv file

output: csv files containing all information about the sample

The scrapper is multi-threaded and uses proxies in order to be discrete.

The code can be run from the command line.

# Proxy Grabber

Prior to running the proxy grabber for the first time:
- go on the Package Manager Console of Visual Studio and type: 
$ Install-Package Selenium.WebDriver
$ Install-Package HtmlAgilityPack

Avec 50 crawler, temps de cycle minimium 10 sec, max 30 sec, On est a 3 requetes par secondes.
Au bout de 2h30 185 proxy atteints
31,0000




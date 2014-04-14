WkHtmlToPDF-ServiceStack-Example
================================

Example of a [ServiceStack v4][1] Web Service which uses WkHtmlToPDF to convert html to pdfs.

Due to the dependency on [WkHtmlToXSharp][2], the applicaiton must be configured for a 32-bit build.

Specs are written in a BDD style using [nspec][3].

---

To use the release, create a new web applicaiton in IIS, and configure accordingly.

Uses .net 4.5.1

---

[WkHtmlToXSharp][2] is supposed to provide its own copy of wkhtmltox.dll however,
I've noticed that it does not.

I've provided a copy of the binary file, with the correct name in the lib folder.

Copy this file into the bin output folder, if the program has issues finding the dll.


License
-------
The example is licenced under the MIT License. 

All other libraries used, are licensed under their repsective licenses. 

[1]:(https://github.com/ServiceStack/ServiceStack)
[2]:(https://github.com/pruiz/WkHtmlToXSharp) 
[3]:(http://nspec.org/)

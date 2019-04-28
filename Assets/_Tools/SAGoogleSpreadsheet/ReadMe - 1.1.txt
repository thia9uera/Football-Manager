----------------------------------------------
 SA: Google Spreadsheet Loader
 Copyright © 2014 SuperAshley Entertainment
 Version 1.0
 superashley1020@gmail.com
----------------------------------------------

Thank you for buying SAGoogleSpreadsheet const data generator!
Feature request, bug report, help support, please email me at superashley1020@gmail.com
I will respond as soon as possible within 24 hours.

------------------
 Features
------------------

This tool can create and generate your game data from Google Spreadsheet into ScriptableObject assets.

Google Spreadsheet is great for game designers to create and store their game data, because it allows
different people working on the same spreadsheet at the same time, and they don't have to worry about
how to use Unity.

It is also great for creating string tables which makes it really easy for localization.

------------------
 JsonFx Library
------------------

SAGoogleSpreadsheet includes JsonFx library for serialize/deserialize Json strings returned by Google.
JsonFx is an open source project (https://github.com/jsonfx/jsonfx)
JsonFx license: https://github.com/jsonfx/jsonfx/blob/master/LICENSE.txt

-----------------
 Version History
-----------------
1.2
- FIX: Fixed errors when used with Unity2017+
- FIX: Updated the manual
- NEW: Added support for bool type (column name start with b_

1.1
- NEW: Added one button to generate class for all worksheets
- NEW: Added one button to generate asset for all worksheets
- FIX: Fixed an annoying warning "Unable to find style...."

1.0
- NEW: Added support to load published google spreadsheet using spreadsheet ID (No authentication needed)
- NEW: Generate C# classes for each of the worksheets within the spreadsheet
- NEW: Generate ScriptableObject classes & Assets for storing the data from the spreadsheet 

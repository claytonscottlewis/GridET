v2014.10.10

Fixes/Code Changes:
-Output Folder on menubar is now updated when the selected
 ini settings is changed.
-Fixed false reporting of linked images not found. This
 would occur for filenames having characters that are URL
 escaped, e.g. space is %20
-Improved logic to find linked image location.


v2014.10.09

Features Added:
-Improved the layout of the 'Advanced' options panel.
-Added a 'Preview' button to preview the Output Tokens File.
-Added two new quoting options for the token file.
 1) Never quote, quotations are never used.
 2) Never quote literals, literals are never quoted.
-Warning messages added for the UI if a linked image
 cannot be found.

Fixes/Code Changes:
-Replaced character \n with \r\n when outputting the
 'HHP Append Text' to the hhp file, otherwise the
 newlines are not recognized.
-Fixed quoting of values when the quote char is
 blank. A blank value would accidentally quote because
 all values contain the empty string.
-Fixed image linking problem when the image is located
 relative to the desktop. For whatever reason, Word saves
 the path as ../../../Desktop/.../Image.png.  NuHelp
 detects if Desktop is the first folder and translates
 to the correct path to the desktop.
-The above goes true for all special folders, Documents,
 Pictures, Videos, etc.


v2014.09.30

Features Added:
-When using the command line, it's possible to specify the
 number of milliseconds the message box is displayed for a
 success message and error message.
 Examples: NuHelp.exe -ErrorBox:15000 ...
           Will display for 15 seconds if the result is an error.
           NuHelp.exe -SuccessBox:-1 ...
           A negative value means don't display.
           A zero value means display and don't timeout.
-New tokens added to the output tokens file in Advanced Options.
 1) 'Local' : outputs the filename.
 2) 'Name' : was renamed to 'Title'.
 3) 'Anchors' : outputs the bookmarks for each node.
 4) 'IndexEntries' : outputs the index entries for each node.
 5) 'IndexEntries.<prop>' : same as above but specific index
    property value, e.g. SeeAlso or Bookmark or custom.
-Added two new special token delimiters <space> and <none>.

Fixes/Code Changes:
-Fixed the location where the tokens file is saved when
 no path is specified. Before, the tokens file would be
 saved in the NuHelp.exe folder. Now, the tokens file is
 saved in the specified output folder (in settings / on menubar).
-Fixed the DefaultPage and HomePage paths in the hhp file.
 Before, only the filename would be specified.  Now, if
 those html pages exist in a subfolder, then the relative
 path and filename are outputted to the hhp file.
-When running through the command line, the default message
 box timeout has been changed to 10 seconds when there
 is an error.
-Renamed 'Name' special token to 'Title' and updated the
 tooltip to reflect the correct description.


v2014.09.20

Features Added:
-Added Advanced Options tab which allows for:
 1) Ability to append custom text to the HHP file.
 2) Ability to call a process or batch file just before the
    actual compile (hhc.exe) is called.
 The intent of these options is to allow the user to access
 features in the hhc.exe such as [MAP] and [ALIAS] that
 NuHelp doesn't support directly.
 3) Added Token File option which outputs a list of [CHM ...]
 tokens to the specified file, one for each node. See the
 "Advanced Options" topic in the help documentation for more
 information.
-Better support for relative paths.  Html files can now be
 added in a sub-folder (relative to the output folder).

Fixes/Code Changes:
-Renamed variable "String comment" to "String Comment" in CHMAliasMap.
-Fixed C# contant file so that variables only contain a-z, A-Z, 0-9 or
 underscore (_).
-Added "Tokens" hashtable to the CHMNode object.  The tokens come from
 the [CHM ... ] definition in the Word file.
-OutputFolder (on the menubar) is now updated in the settings when the
 compile button is clicked. E.g.
 settings.OutputFolder = tbOutputFolder.Text.Trim();


v2014.08.26

Features Added:
-New feature allows the user to specify the delimiter
 character used between keywords.  The delimiter can
 also be ignored by using it twice in a row.  E.g.
 { Keywords="a,,a,b,c" } =>  a,a   b    c
-Scroll bars added to the Options tab.  They will
 appear when needed to better support smaller screens.
-Linked local images now supported.  This works by
 copying the images to the output folder relative to
 the individual html files.

Fixes/Code Changes:
-Default Keywords delimiter changed to ';'.
-Added '#' character as a special character.  Cannot
 appear in filenames because it's used as a #bookmark
 character.


v2014.08.22

Fixes/Code Changes:
-Compiling the default Help Welcome resulting in an error
 has been fixed.  This was a result of not setting some
 new variables required by the updated features.


v2014.08.22

Features Added:
-New {Keywords="a,b,c"} index option that is independent of
 IndexingOn/IndexingOff.  This makes it easy to define a
 quick list of index keywords separated by commas.
-Added two warning messages for invariant names.  In both
 cases, the invariant name is modified to make it useable.
 1) Invariant name cannot be used as-is because it uses
    spaces or reserved characters.
 2) A file already exists with the same invariant name.


Fixes/Code Changes:
-Improved parsing of page text for index entries { }.
 More flexible to allow for new "Keywords" regardless
 of IndexingOn/IndexingOff.  Also, better handling of
 index entries that contain formatting tags, like
 <u>,</u>,<b>,</b>,etc.
-Fixed Word doc filename problem mentioned in v2014.08.20,
 but occurred using the command line.  Now both UI and
 command line use the same method to determine the name
 to use.
-Fixed embedded <span> tags that contain the regular
 quotation mark character.
 E.g.
 Raw html: &lt;span Title=&quot;Tooltip&quot;&gt; ...
 Before: <span Title=&quot;Tooltip&quot;> ...
 Fixed:  <span Title="Tooltip">...


v2014.08.20

Features Added:
-The warning message box has been replaced with a scrollable
 text box, otherwise lots of warning messages result in a
 very large message box.
-Added warning message for improperly formatted [CHM tag
 that is missing a closing quotation mark or close bracket ].
-Added warning for filenames that are more than 160 characters
 long.  Total length including directory cannot exceed 260.
 Filenames are truncated at 160.
-Added new option to *Copy path* when right clicking on the
 Add button file menu.

Fixes/Code Changes:
-Fixed problem where a Word doc is added, but it has the same
 same filename as a an existing header.  For example, Doc1.docx
 has topic <h1>Header1</h1>.  Then Header1.docx is added to
 the same project tree.  Header1.docx would be saved as
 Header1.html and overwrite the Header1.html from Doc1.docx.
 Now, Header1.docx is saved as Header1-1.docx.  An incremental
 value is appended until a filename is found that does not
 currently exist.
-Headers with two spaces in a row not showing up in the TOC.
 Word uses character U+00A0 (aka 160), which is a non-breaking
 whitespace, as well as the regular space (32) in the header.
 Before, only (32) was replaced with dash.  Now, both (32) and
 (160) are replaced, as well as any other characters that are
 classified as whitespace.
-Fixed parsing of [CHM InvariantName when the standard ascii
 quotation mark is used.  The problem was that these were
 turned into &quot; and not recognized by the parser.


v2014.08.19

Features Added:
-When combining multiple docs into a single chm, added feature
 to detect hyperlinks from one document to another.
-Warning message displayed when hyperlinks cannot be resolved.
-Warning message displayed for the same bookmark used more than
 once.  E.g. <a name="book1">...</a> is used multiple times.
-Added new topic in NuHelp.chm called 'Linking Across Documents'

Fixes/Code Changes:
-Topics that only had blank headers as children now correctly
 display the page icon if blank headers are hidden from the TOC.
 Before, a book icon was displayed.
-Prevent adding duplicate constants to the C# file.


v2014.08.15

Features Added:
-Command line blank topic uses the filename without extension.  E.g.
 "-File:Doc1.doc|Doc2.doc*", Doc2.doc is appended to "Doc2" section.
-New Warning added if blank headers are detected.
-New options on how to treat blank headers:
 1) Show them in TOC and link to them via [next][prev]
 2) Hide them in TOC and link to them via [next][prev]
 3) Hide them in TOC but don't link to them.
 More detailed information is available in the included help file.


v2014.08.12

Features Added:
-Added message box at the end of the command line that shows either Success
 or Error with the message.  The message box automatically closes after
 5 seconds.
-Added command line option "-Path:" to avoid having to type the same
 directory name over and over.  This is described in more detail in the
 help documentation included in the download in the Command Line topic.

Fixes/Code Changes:
-Fixed the command line arguments.  The wrong variable was used which was
 causing filenames not to be found.
-Fixed append node not finding the correct node to append to.
-Fixed the [next] and [prev] links when adding multiple documents to the
 same project tree.  Before, there would be no links from the last page
 of the first document to the first page of the second document.


v2014.07.29

Fixes/Code Changes:
-Parsing of the font sizes now specifies to use invariant culture so that
 the period is always treated as the decimal separator and not a grouping
 separator in other cultures.


v2014.07.28

Features Added:
-New warning message if index entry, e.g. {apple Text="Apple"} isn't formatted
 correctly.
-New warning message if <div> tags are not closed properly.
-New option to replace font sizes with font size names or not, e.g.
 font-size:10pt; -> font-size:small;

Fixes/Code Changes:
-Improved parsing of <div class=WordSection tags.  The code now looks for
 all of these types of div tags and only takes the inner html from each
 section.
-Fixed problem where a badly formatted index entry was causing a sort error.
 Badly formatted index entries are now skipped.
-Fixed formatting of index entries that contain escaped html, e.g. {Text="&gt;"}
 The text is now unescaped so it will appear correctly in the index list.
-Fixed problem where index entries have two spaces in a row.


v2014.07.02

Features Added:
Two new options added to the CHM Options screen:
  1) An option to allow the user to decide to override the h-tag size or keep the
     h-tag size as it is defined in the Word document.
  2) An option to specify the maximum depth to allow nested topics.  The default
     has been set at h3.
Both options are described in more detail in the Quick Guide help.

Fixes/Code Changes:
-The ellipsis character is now treated as a special character and is automatically
 filtered out of the filename used to save the individual html files.
-Fixed &quot; from appearing in the headers displayed in the TOC.  Now displays
 as the double quotation mark character.
-Fixed parsing of header tags that are surrounded in a div tag which provides
 additional style elements.  For example, a header with bars above and below.


v2014.07.01

Features Added:
-Images can now be clicked on to enlarge the image in the help file.
 See the Image Quality topic in the Quick Guide for more information.
-When adding a file, a check is done to detect if another process has
 the file locked and the user is alerted to which process is using the
 file.  The user is presented with an option to wait or cancel.
-New Troubleshooting topic has been added to the Quick Guide.
-For the error: IDN labels must be between 1 and 63 characters long.
 The header causing the problem is identified in a warning message
 dialog box.

Fixes/Code Changes:
-Fonts & Styles panel wasn't initializing the Charset properly.


v2014.06.06

Major release of new features!  All new features are explained in QuickGuide.chm.

Features Added:
-Added ability to save compile settings into different files.
-Added ability to run through the command line.
-Added option to change the body open tag.
-Added option to change the First/Last page header/footer html fields.
-Added options to include extra files.
-Added options to change fonts, colors and icon images.
-Added Language option and more compile options.
-Added option to hide or show Word when it saves a document to HTML.

Fixes/Code Changes:
-Fixed font in TOC and index.  Unneeded @ sign was causing the font family
 to have no effect.
-Font dialog now initialized with current values.
-Changed the FileInfo references to String for XML encoding purposes.
-Included RCLayout layout manager / layout engine to make the Option screens
 look better.
-Changed Executable name to NuHelp.exe to make it easier to call from the
 command line.
-Fixed charset in ini file.  Charset said utf-16, but ini file is actually
 saved in utf-8.  The charset attribute now reads utf-8.
-Merged CHMHhp and CHMOptions into the same class CHMOptions and deleted CHMHhp.

v2014.06.01

Features Added:
-Right click on recent file menu has two new options, Open folder and Open file.
-Can now specify the following encodings in Options:
	The HTML file read encoding.
	The HTML file write encoding.
	The encodings of the files (hhc, hhk, hhp) used for compiling
-Can now specify the font family and font size of the TOC and Index in Options.
-Filenames that contain non-English characters are now converted to the English
 letter equivalents so they are URL friendly.  CHM cannot handle filenames that
 contain international characters.  For more information, the Filenames topic
 in the help file has been updated.
-Header and Footer can contain "[next]" and "[prev]" tags which will link to
 the next or previous file.

Fixes/Code Changes:
-Fixed the problem where where MS Word uses a bunch of &nbsp; in the header name.
 &nbsp; is replaced with the space character and multiple spaces in a row are
 condensed into a single space.
-Re-organized the code so that adding a Word doc or HTML file both go through
 the process that replaces the fancy quotation marks with the simple quotation marks.
-Fixed problem where MS Word returns an TextEncoding value out of range 0 to 65535.
 In this case, the code will try to auto detect the encoding of the html file.
-Fixed problem where {<b>index></b>} entries had the style tags stripped out in the
 page content.  An UncleanName was added to the CHMIndexEntry definition.
-Changed using underscore _ to dash - for filenames which is more consistent with
 common web practises.
-Output folder now remembered in ini file.
-Fixed find focused control to select the SelectedTab when the control found was
 a TabControl.

v2014.05.30

-NuHelp should work with Word 2007 and earlier installed now:
-When converting a doc to HTML, the code first tries SaveAs2 method (for Word 2010/2013) first,
 followed by SaveAs (for Word 2007 and earlier).
-An Opulos.Core.Interop.Word namespace was created to keep the code cleaner looking.  The
 current functionality includes opening and saving Word documents.  The functionality will be
 expanded with time as needed.
-Added logic to pass in 17 parameters to the SaveAs2 method, and 16 parameters to the SaveAs
 method.
-Changed target build to .Net 3.5 Client Profile.

v2014.05.29

-Switched back to .Net 3.5 and use reflection to invoke the Microsoft COM libraries.
 See stackoverflow: http://stackoverflow.com/questions/21015374/could-not-load-microsoft-office-interop-word

v2014.05.28

Features Added:
-Hyperlinks in the headers are now supported.
-Add button now changed to a drop down and also remembers recent files.
-Right click on a recent file menu item gives an option to remove it from the menu.
-Add file button now opens to the folder of the last added file.
-Output compiler message has \r\r replaced with a single \r (possibly an hhc.exe bug?)
-Output folder textbox now fills the available width.
-Preferred window size and position are now saved in the ini file.

Fixes/Code Changes:
-All special characters in headers, / \ : * ? " < > | are now replaced with _ when saving
 to file. Otherwise a filename would contain an invalid character and could not be saved.
-Removed IsTempFile property from CHMNode.  All nodes are temp files generated from the headers
 in the html file.
-CHMFile.Name changed to just CHMFile when passed to the Help.ShowHelp(...) method.
 Originally CHMFile was a FileInfo object, which was changed to a String for a possible
 future enhancement to support more flexible paths.
-Fixed parsing of hyperlinks when Word does not use quotation marks.  E.g. <a name=Features>
 vs. <a name="Features">
-Fixed problem when Word incorrectly outputs an internal bookmark as an external file hyperlink.
 NuHelp will automatically detect the bad reference and output the correct html for it.
-Fixed problem when two headers have the same name.  The filename of subsequent equal names
 will have an _k appended, e.g. e.g. Filename_2, Filename_3, Filename_4, etc.  InvariantName
 still must be unique though.
-Fixed problem of adding html/htm file outputing to the same folder as the original file.  Now
 outputs to the menubar output folder.


v2014.05.24

-The .hhk, .hhc, and .hhp are now saved using 1252 encoding.
-Increased font size and changed font family of 'This is the welcome page.' to Arial.
-Html tags, like <b> and <i>, are now stripped out of the index name.

v2014.05.23

-Initial release under the BSD license.
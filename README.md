.NET SDK for YellowDrawer.
==========================

.NET library to access GridFS, Azure or Amazon file storage. Thank you for
choosing YellowDrawer.

 

DIRECTORY STRUCTURE -------------------

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
YellowDrawer.Common                core wrapper code
YellowDrawer.GridFS
YellowDrawer.Azure
YellowDrawer.Amazon
YellowDrawer.Tests       tests of the core wrapper code
YellowDrawer.Simple      example project
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

REQUIREMENTS
------------

The minimum requirement by YellowDrawer wrapper is that your Visual Studio
supported .Net Framework 4.5.

 

INSTALLATION
------------

Add reference "YellowDrawer" to your project.

USAGE
=====

Actions
-------

###  

GetPublicUrl(string path);

IStorageFile GetFile(string path);

IEnumerable\<IStorageFile\> ListFiles(string path);

IEnumerable\<IStorageFolder\> ListFolders(string path);

void CreateFolder(string path);

void DeleteFolder(string path);

void RenameFolder(string path, string newPath);

void DeleteFile(string path);

void RenameFile(string path, string newPath);

IStorageFile CreateFile(string path, byte[] arr = null);

bool IsFileExists(string path);

bool IsFolderExits(string path);

bool TryCreateFolder(string path);

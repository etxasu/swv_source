var OpenWebPagePlugin = 
{
    OpenWebPage: function (link, name, sizeString) 
	{
        var url = Pointer_stringify(link);
		var _size = Pointer_stringify(sizeString);
        document.onmouseup = function () 
		{
            window.open(url, name, _size);
            document.onmouseup = null;
			console.log("OPEN ATTEMPTED");
        }
    }
};

mergeInto(LibraryManager.library, OpenWebPagePlugin);
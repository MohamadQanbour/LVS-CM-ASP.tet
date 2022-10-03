function loadTxt()
    {
    var txtLang = document.getElementsByName("txtLang");
    txtLang[0].innerHTML = "Paste youtube video code here (editor insert the code as is):";
    txtLang[1].innerHTML = "Or paste youtube url (http://...) here:";
    txtLang[2].innerHTML = "Url";
    txtLang[3].innerHTML = "Width";
    txtLang[4].innerHTML = "Height";
            
    document.getElementById("btnCancel").value = "cancel";
    document.getElementById("btnOk").value = " ok ";
    }
function getTxt(s)
    {
    }    
function writeTitle()
    {
    document.write("<title>Insert Youtube Video</title>")
    }
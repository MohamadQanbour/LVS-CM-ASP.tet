function loadTxt()
    {
    var txtLang = document.getElementsByName("txtLang");
    txtLang[0].innerHTML = "Autopassa";
    txtLang[1].innerHTML = "Egenskaper";
    txtLang[2].innerHTML = "Formatering";
    txtLang[3].innerHTML = "Bredd";
    txtLang[4].innerHTML = "Autopassa till inneh\u00E5llet";
    txtLang[5].innerHTML = "Fast tabellbredd";
    txtLang[6].innerHTML = "Autopassa till f\u00F6nstret";
    txtLang[7].innerHTML = "H\u00F6jd";
    txtLang[8].innerHTML = "Autopassa till inneh\u00E5llet";
    txtLang[9].innerHTML = "Fast tabellh\u00F6jd";
    txtLang[10].innerHTML = "Autopassa till f\u00F6nstret";

    txtLang[11].innerHTML = "Cellutfyllnad";
    txtLang[12].innerHTML = "Cellmellanrum";
    txtLang[13].innerHTML = "Kantlinjer";
    txtLang[14].innerHTML = "Sammanfoga";
    txtLang[15].innerHTML = "Bakgrund";

    txtLang[16].innerHTML = "Placering";
    txtLang[17].innerHTML = "Marginal";
    txtLang[18].innerHTML = "V\u00E4nster";
    txtLang[19].innerHTML = "H\u00F6ger";
    txtLang[20].innerHTML = "\u00D6verst";
    txtLang[21].innerHTML = "Nederst";  
    txtLang[22].innerHTML = "Caption";
    txtLang[23].innerHTML = "Summary";
    txtLang[24].innerHTML = "CSS Text";

    var optLang = document.getElementsByName("optLang");
    optLang[0].text = "pixlar"
    optLang[1].text = "procent"
    optLang[2].text = "pixlar"
    optLang[3].text = "procent"
    optLang[4].text = "Ingen"
    optLang[5].text = "Ja"
    optLang[6].text = "Nej"
    optLang[7].text = ""
    optLang[8].text = "V\u00E4nster"
    optLang[9].text = "Centrerad"
    optLang[10].text = "H\u00F6ger"

    document.getElementById("btnPick").value="V\u00E4lj";
    document.getElementById("btnImage").value="Bild";

    document.getElementById("btnCancel").value = "Avbryt";
    document.getElementById("btnApply").value = "Verkst\u00E4ll";
    document.getElementById("btnOk").value = " OK ";
    }
function getTxt(s)
    {
    switch(s)
        {
        case "Custom Colors": return "Custom Colors";
        case "More Colors...": return "More Colors...";
        default:return "";
        }
    }
function writeTitle()
    {
    document.write("<title>Tabellegenskaper</title>")
    }
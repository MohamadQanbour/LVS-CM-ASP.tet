function loadTxt()
    {
    var txtLang = document.getElementsByName("txtLang");
    txtLang[0].innerHTML = "St\u00F8rrelse";
    txtLang[1].innerHTML = "Egenskaber";
    txtLang[2].innerHTML = "Typografi";
    txtLang[3].innerHTML = "Bredde";
    txtLang[4].innerHTML = "Bredde styret af indhold";
    txtLang[5].innerHTML = "Tabelbredde";
    txtLang[6].innerHTML = "Tilpas til vindue";
    txtLang[7].innerHTML = "H\u00F8jde";
    txtLang[8].innerHTML = "Bredde styret af indhold";
    txtLang[9].innerHTML = "Tabelbredde";
    txtLang[10].innerHTML = "Tilpas til vindue";

    txtLang[11].innerHTML = "Celle margen";
    txtLang[12].innerHTML = "Celle afstand";

    txtLang[13].innerHTML = "Ramme";
    txtLang[14].innerHTML = "Collapse";
    txtLang[15].innerHTML = "Baggrund";

    txtLang[16].innerHTML = "Justering";
    txtLang[17].innerHTML = "Margen";
    txtLang[18].innerHTML = "Venstre";
    txtLang[19].innerHTML = "H\u00F8jre";
    txtLang[20].innerHTML = "Top";
    txtLang[21].innerHTML = "Nederst";  

    txtLang[22].innerHTML = "Caption";
    txtLang[23].innerHTML = "Summary";
    txtLang[24].innerHTML = "Typografi";

    var optLang = document.getElementsByName("optLang");
    optLang[0].text = "pixels"
    optLang[1].text = "procent"
    optLang[2].text = "pixels"
    optLang[3].text = "procent"
    optLang[4].text = "Ingen"
    optLang[5].text = "Ja"
    optLang[6].text = "Nej"
    optLang[7].text = ""
    optLang[8].text = "Venstre"
    optLang[9].text = "Centrer"
    optLang[10].text = "H\u00F8jre"

    document.getElementById("btnPick").value="V\u00E6lg";
    document.getElementById("btnImage").value="Billede";

    document.getElementById("btnCancel").value = "Annuller";
    document.getElementById("btnApply").value = "Opdater";
    document.getElementById("btnOk").value = " Ok ";
    }
function getTxt(s)
    {
    switch(s)
        {
        case "Custom Colors": return "Egne farver";
        case "More Colors...": return "Flere farver...";
        default:return "";
        }
    }
function writeTitle()
    {
    document.write("<title>Tabel egenskaber</title>")
    }
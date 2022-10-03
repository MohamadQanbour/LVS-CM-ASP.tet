function loadTxt()
    {
    var txtLang =  document.getElementsByName("txtLang");
    txtLang[0].innerHTML = "AutoAnpassen";
    txtLang[1].innerHTML = "Eigenschaften";
    txtLang[2].innerHTML = "Stil";
    txtLang[3].innerHTML = "Breite";
    txtLang[4].innerHTML = "AutoAnpassen an Inhalt";
    txtLang[5].innerHTML = "Feste Tabellenbreite";
    txtLang[6].innerHTML = "AutoAnpassen an Fenster";
    txtLang[7].innerHTML = "H&ouml;he";
    txtLang[8].innerHTML = "AutoAnpassen an Inhalt";
    txtLang[9].innerHTML = "Feste Tabllenh&ouml;he";
    txtLang[10].innerHTML = "AutoAnpassen an Fenster";

    txtLang[11].innerHTML = "Abstand vom Text";
    txtLang[12].innerHTML = "Zellenabstand";
    
    txtLang[13].innerHTML = "Rahmen";
    txtLang[14].innerHTML = "kollabieren";
    txtLang[15].innerHTML = "Hintergrund";

    txtLang[16].innerHTML = "Ausrichtung";
    txtLang[17].innerHTML = "Abstand";
    txtLang[18].innerHTML = "links";
    txtLang[19].innerHTML = "rechts";
    txtLang[20].innerHTML = "oben";
    txtLang[21].innerHTML = "unten";    

    txtLang[22].innerHTML = "Caption";
    txtLang[23].innerHTML = "Summary";
    txtLang[24].innerHTML = "CSS Text";

    var optLang = document.getElementsByName("optLang");
    optLang[0].text = "Pixel"
    optLang[1].text = "Prozent"
    optLang[2].text = "Pixel"
    optLang[3].text = "Prozent"
    optLang[4].text = "kein Rahmen"
    optLang[5].text = "Ja"
    optLang[6].text = "Nein"
    optLang[7].text = ""
    optLang[8].text = "links"
    optLang[9].text = "zentriert"
    optLang[10].text = "rechts"

    document.getElementById("btnPick").value = "w\u00E4hlen";
    document.getElementById("btnImage").value = "Bild";

    document.getElementById("btnCancel").value = "Abbrechen";
    document.getElementById("btnApply").value = "\u00DCbernehmen"; //"apply";
    document.getElementById("btnOk").value = " OK ";
    }
function getTxt(s)
    {
    switch(s)
        {
        case "Custom Colors": return "Benutzerfarben";
        case "More Colors...": return "weitere Farben...";
        default:return "";
        }
    }
function writeTitle()
    {
    document.write("<title>Tabelle Eigenschaften</title>")
    }
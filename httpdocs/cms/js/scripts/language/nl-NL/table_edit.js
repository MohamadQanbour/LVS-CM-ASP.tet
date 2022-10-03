function loadTxt()
    {
    var txtLang = document.getElementsByName("txtLang");
    txtLang[0].innerHTML = "Passend";
    txtLang[1].innerHTML = "Eigenschappen";
    txtLang[2].innerHTML = "Stijl";
    txtLang[3].innerHTML = "Breedte";
    txtLang[4].innerHTML = "Passend aan inhoud";
    txtLang[5].innerHTML = "Vaste tabel breedte";
    txtLang[6].innerHTML = "Passend aan venster";
    txtLang[7].innerHTML = "Hoogte";
    txtLang[8].innerHTML = "Passend aan inhoud";
    txtLang[9].innerHTML = "Vaste tabel hoogte";
    txtLang[10].innerHTML = "Passend aan venster";

    txtLang[11].innerHTML = "Opvulling";//Cell Padding
    txtLang[12].innerHTML = "Cel Tussenruimte";//Cell Spacing
    txtLang[13].innerHTML = "Randen";
    txtLang[14].innerHTML = "Samenvouwen";
    txtLang[15].innerHTML = "Achtergrond";

    txtLang[16].innerHTML = "Uitlijning";
    txtLang[17].innerHTML = "Marge";
    txtLang[18].innerHTML = "Links";
    txtLang[19].innerHTML = "Rechts";
    txtLang[20].innerHTML = "Boven";
    txtLang[21].innerHTML = "Onder";    
    txtLang[22].innerHTML = "Caption";
    txtLang[23].innerHTML = "Summary";
    txtLang[24].innerHTML = "CSS Tekst";

    var optLang = document.getElementsByName("optLang");
    optLang[0].text = "pixels"
    optLang[1].text = "procent"
    optLang[2].text = "pixels"
    optLang[3].text = "procent"
    optLang[4].text = "Geen Rand"
    optLang[5].text = "Ja"
    optLang[6].text = "Nee"
    optLang[7].text = ""
    optLang[8].text = "links"
    optLang[9].text = "midden"
    optLang[10].text = "rechts"

    document.getElementById("btnPick").value="Kiezen";
    document.getElementById("btnImage").value="Afbeelding";

    document.getElementById("btnCancel").value = "annuleren";
    document.getElementById("btnApply").value = "toepassen";
    document.getElementById("btnOk").value = " ok ";
    }
function getTxt(s)
    {
    switch(s)
        {
        case "Custom Colors": return "Eigen kleuren";
        case "More Colors...": return "Meer kleuren...";
        default:return "";
        }
    }
function writeTitle()
    {
    document.write("<title>Tabel Eigenschappen</title>")
    }
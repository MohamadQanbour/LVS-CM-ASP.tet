function loadTxt()
    {
    var txtLang =  document.getElementsByName("txtLang");
    txtLang[0].innerHTML = "Automaattinen sovitus";
    txtLang[1].innerHTML = "Ominaisuudet";
    txtLang[2].innerHTML = "Tyyli";
    txtLang[3].innerHTML = "Leveys";
    txtLang[4].innerHTML = "Sis\u00E4ll\u00F6n automaattinen sovitus";
    txtLang[5].innerHTML = "Muokattu taulukon leveys";
    txtLang[6].innerHTML = "Automaattinen sovitus ikkunaan";
    txtLang[7].innerHTML = "Korkeus";
    txtLang[8].innerHTML = "Sis\u00E4ll\u00F6n automaattinen sovitus";
    txtLang[9].innerHTML = "Muokattu taulukon korkeus";
    txtLang[10].innerHTML = "Automaattinen sovitus ikkunaan";

    txtLang[11].innerHTML = "Solun sis\u00E4lt\u00F6";
    txtLang[12].innerHTML = "Solun v\u00E4li";

    txtLang[13].innerHTML = "Kehys";
    txtLang[14].innerHTML = "Pura rakenne";
    txtLang[15].innerHTML = "Tausta";

    txtLang[16].innerHTML = "Keskitys";
    txtLang[17].innerHTML = "Marginaali";
    txtLang[18].innerHTML = "Vasen";
    txtLang[19].innerHTML = "Oikea";
    txtLang[20].innerHTML = "Yl\u00F6s";
    txtLang[21].innerHTML = "Alas"; 

    txtLang[22].innerHTML = "Caption";
    txtLang[23].innerHTML = "Summary";
    txtLang[24].innerHTML = "CSS teksti";
    
    var optLang = document.getElementsByName("optLang");
    optLang[0].text = "Pikselit";
    optLang[1].text = "Prosentti";
    optLang[2].text = "Pikselit";
    optLang[3].text = "Prosentti";
    optLang[4].text = "Ei kehyksi\u00E4";
    optLang[5].text = "Kyll\u00E4";
    optLang[6].text = "Ei";
    optLang[7].text = ""
    optLang[8].text = "Vasen";
    optLang[9].text = "Keskitetty";
    optLang[10].text = "Oikea";

    document.getElementById("btnPick").value="Poimi";
    document.getElementById("btnImage").value="Kuva";

    document.getElementById("btnCancel").value = "Peruuta";
    document.getElementById("btnApply").value = "K\u00E4yt\u00E4";
    document.getElementById("btnOk").value = " OK ";;
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
    document.write("<title>Taulukon ominaisuudet</title>")
    }
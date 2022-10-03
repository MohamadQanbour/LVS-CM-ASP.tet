function loadTxt()
    {
    var txtLang =  document.getElementsByName("txtLang");
    //txtLang[0].innerHTML = "Size";
    txtLang[0].innerHTML = "AutoDimensiona";
    txtLang[1].innerHTML = "Propriet&agrave;";
    txtLang[2].innerHTML = "Stile";
    //txtLang[4].innerHTML = "Inserisci riga";
    //txtLang[5].innerHTML = "Inserisci colonna";
    //txtLang[6].innerHTML = "Unisci/dividi riga";
    //txtLang[7].innerHTML = "Unisci/dividi colonna";
    //txtLang[8].innerHTML = "Rimuovi Riga";
    //txtLang[9].innerHTML = "Rimuovi Colonna";    
    txtLang[3].innerHTML = "Larghezza";
    txtLang[4].innerHTML = "Auto-dimensiona in base Contenuto";
    txtLang[5].innerHTML = "Largezza Fissa";
    txtLang[6].innerHTML = "Auto-dimensiona alla videata";
    txtLang[7].innerHTML = "Altezza";
    txtLang[8].innerHTML = "Auto-dimensiona al contenuto";
    txtLang[9].innerHTML = "Altezza Fissa";
    txtLang[10].innerHTML = "Auto-dimensiona alla videata";

    txtLang[11].innerHTML = "Spessore Cornice";
    txtLang[12].innerHTML = "Spazio Cella";

    txtLang[13].innerHTML = "Bordi";
    txtLang[14].innerHTML = "Crollo";
    txtLang[15].innerHTML = "Sfondo";

    txtLang[16].innerHTML = "Allineamento";
    txtLang[17].innerHTML = "Margini";
    txtLang[18].innerHTML = "Sinistra";
    txtLang[19].innerHTML = "Destra";
    txtLang[20].innerHTML = "Sopra";
    txtLang[21].innerHTML = "Sotto";
    txtLang[22].innerHTML = "Caption";
    txtLang[23].innerHTML = "Summary";
    txtLang[24].innerHTML = "Testo CSS ";
        
    var optLang = document.getElementsByName("optLang");
    optLang[0].text = "pixels"
    optLang[1].text = "percentuale"
    optLang[2].text = "pixels"
    optLang[3].text = "percentuale"
    optLang[4].text = "Senza bordi"
    optLang[5].text = "Si"
    optLang[6].text = "No"
    optLang[7].text = ""
    optLang[8].text = "Sinistra"
    optLang[9].text = "centro"
    optLang[10].text = "destra"

    document.getElementById("btnPick").value="Scegli";
    document.getElementById("btnImage").value="Immagine";

    document.getElementById("btnCancel").value = "cancella";
    document.getElementById("btnApply").value = "applica";
    document.getElementById("btnOk").value = " ok ";
    }
function getTxt(s)
    {
    switch(s)
        {
        case "Cannot delete column.":
            return "Colonna non rimuovibile. La colonna contiene celle unite ad altre colonne. Prima bisogna rimuovere l\'unione.";
        case "Cannot delete row.":
            return "Riga non rimuovibile. La riga contiene celle unite ad altre righe. Prima bisogna rimuovere l\'unione.";
        case "Custom Colors": return "Colori Personalizzati";
        case "More Colors...": return "Altri Colori...";
        default:return "";
        }
    }
function writeTitle()
    {
    document.write("<title>Proprietà Tabella</title>")
    }
const kartes = 22;
let Trapoula = [];
let paixtes = new Set();
let afigitis = 0;
let net = require('net');
let skarta = kartes;    // meiono kathe giro, 
let players = [];
let palio_length = 0;
let count_arxhs = 0;  // posoi paixtes etoimoi
let min_players = 2;
let In_game = [];
let count_epilogh = 0;

class Paixths
{
    constructor(socket, xroma)
    {
        this.socket = socket;
        this.vathmos = 0;
        //this.arxh = false;      // an eimai ok ksekiname        // isos de xreiazetai
        this.xroma;
        this.karta;
        this.nikitis = false;
        this.onoma;
        this.ID;
    }
    set_Vathmo(x)                       // Pote den ta xrhsimpoihsa
    {
        this.vathmos += x;
    }
    get_Vathmo()
    {
        return this.vathmos;
    }
    set_Karta(x)
    {
        this.karta = x;
    }
    set_name(onoma)
    {
        this.onoma = onoma;
    }
};
//-------------------------------------
//              INIT GAME
//------------------------------------
function ekkinisi(size)
{
    console.log(Trapoula.length);
    let temp = [];
    for (i=0; i<players.length; i++)
    {
        temp.push(0);
    }
    init_anakatepse();
    console.log(Trapoula);

    // afigitis random
    afigitis = Math.floor(Math.random()*players.length);
    moirase(6);
    console.log(In_game);
    
    ///---------------------------ARXH----------
    let message = 'A'+players.length.toString();
    for (i=0; i<players.length; i++)
    {
        message = message + players[i].onoma + "." + players[i].xroma + ".";      // onoma + xroma
    }
    message = message + afigitis.toString() + ".";                              // afigitis
    for (i=0; i<players.length; i++)
    {
        let message1 = message;
        for (j=0; j<6; j++)
        {
            let message1 = message1 + In_game[j+6*i].toString() + ".";                        // 6 kartes
        }
        send(message1, players[i].socket);
    }
}

function init_anakatepse()
{
    for (i=0; i<kartes; i++)
    {
        // an einai sto xeri kapoiou asto, i an teleiosoun de sikonoume alli
        Trapoula.push(i);
    }
    for (i=0; i<kartes-1; i++)
    {
        let x = Math.floor(Math.random()*(kartes-i));
        console.log(x);
        
        let j = Trapoula[x];
        Trapoula[x] = Trapoula[kartes-i-1];
        Trapoula[kartes-i-1] = j;
    }
}

function moirase(poses)
{
    for (i=0; i<paixtes.size; i++)
    {
        for (j=0; j<poses; j++)
        {
            if (Trapoula.length!=0)
            {
                //pop
                In_game.push(Trapoula[(Trapoula.length)-1]);
                Trapoula.pop();
            }
            else
            {
                telos();
                break;
            }
           
        }
    }
}

function neos_giros()
{
    //oloi nikitis false
    afigitis++;
    if (afigitis > players.length-1)
    {
        afigitis = 0;
    }
    //elegxo an teleiosan oi kartes
    moirase(1);
    for(i=0; i<players.length; i++)
    {
        send_karta();       //
    }
}


function vathmologia()
{
    let exasan = false;
    let nikisan = false;
    // to exoun vrei oloi i den to exei vrei kaneis ?
    for (i=0; i<players.length; i++)
    {
        if (i == afigitis)
        {
            continue;
        }
        if (players[i].karta == players[afigitis].karta)
        {
            players[i].nikitis == true;
            nikisan = true;
        }
        else{
            players[i].nikitis == false;
            exasan = true;
        }
    }
    if (exasan && nikisan)
    {
        players[afigitis].vathmos += 2; // tha tou dosei kai ena o eaftos tou
        for (i=0; i<players.length-1; i++)
        {
            if(players[i].nikitis)
            {
                players[i].vathmos+=3;
            }
            else
            {
                for (j=0; i<players.length-1; i++)
                {
                    if (players[j].karta == players[i].karta)
                    {
                        players[j].vathmos++;
                        break;
                    }
                }
            }
        }
    }
    else
    {
        for (i=0; i<players.length; i++)
        {
            if (i!=afigitis)
            {
                players[i].vathmos += 2;
            }
            
        }
    }
    send_vathmologia();
    //elegxos an nikise kapoios
    for (i=0; i<players.length; i++)
    {
        if (players[i].vathmos>30)
        {
            telos();
        }
    }
    
}
//-------------------------
//          SEND
//------------------------
function send_karta()
{
    let message = "K";
    let message1 = message;
    for (i=0; i<players.length; i++)
    {
        message1 = message + In_game[In_game.length-1-i].toString(); 
        send(message1, players[i].socket);
    }
}

function send_vathmologia()
{
    let message = "P";
    for (i=0; i<players.length; i++)
    {
        message = message + players[i].vathmos.toString(); + '.';  //////////
    }
    send_Olous(message);
}

function send_perigrafh()
{
    let message2 = "G" + players[afigitis].karta.toString();
    for (i=0; i<players.length; i++)
    {
        if (i!=afigitis)
        {
            send(message2, players[i].socket);
        }
    }
}

function swap(a,b)
{
    let temp =a;
    a = b;
    b = temp;
}

function telos()
{
    send_Olous("N")
}
//----------------------------------------------------------
//                       SERVER
//----------------------------------------------------------
net.createServer(function (socket)                // to callback. Socket apo to event
{
    // kratao reference
    paixtes.add(socket.remoteAddress);
    if (paixtes.size > palio_length)
    {
        players.push(new Paixths(socket, paixtes.size-1));  // edW exei provlima
        palio_length = paixtes.size;
    }
    socket.on('data', elegxos);
   // EDWW send()
   send(("I"+ paixtes.size), socket);
    console.log(socket.remoteAddress + " has connected");
}).listen(8080, "192.168.2.2");

// ------ Parse -- paketa       // isos thelei kai kana FLUSH
function send_Olous(message)
{
    for (i=0; i<players.length; i++)
    {
        send(message, players[i].socket);
    }
}

function send(mymessage, paraliptis)
{
    paraliptis.write(mymessage);
    console.log(mymessage);
}

function elegxos(data)              // an de ksero poios, na to exei mesa to minima
{
    console.log(players);
    console.log(data);
    switch(data[0])
    {
        case 1:             // name
            players[data[1]].onoma = data.slice(2,data.length);               // ta afino bytes kai ta metafrazoun oi clients
            break;
        case 2:             // xroma
            players[data[1]].xroma = data.slice(2, data.length);
            count_arxhs++;
            if(count_arxhs >= min_players)
            {
                ekkinisi(count_arxhs);
            }
            break;
        case 3:             // epilogh gia sosto
            players[data[1]].karta = data.slice(2, data.length);
            count_epilogh++;
            if(count_epilogh >= players.size)
            {
                count_epilogh = 0;
                vathmologia();
            }
            break;
        case 4:             // epilogh gia berdema
            players[data[1]].karta = data.slice(2, data.length);
            count_epilogh++;
            if (count_epilogh == 1)
            {
                send_perigrafh();
            }
            if(count_epilogh >= players.size)
            {
                count_epilogh = 0;
                vathmologia();
            }
            break;
        default:
           console.log("apo edo");
            break;
    }
}